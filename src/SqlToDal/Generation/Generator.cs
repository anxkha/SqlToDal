using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlToDal.Generation.Model;

namespace SqlToDal.Generation;

public class Generator
{
	public string Namespace { get; set; }

	public string GenerateConnectionFactoryInterface()
	{
		return $@"using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace {Namespace};

public interface IDatabaseConnectionFactory
{{
	Task<SqlConnection> ConnectAsync(CancellationToken cancellationToken);
}}
";
	}

	public string GenerateProcedure(ParsedProcedure procedure)
	{
		var builder = new StringBuilder();
		BuildUsings(builder);
		BuildNamespace(builder);
		BuildClassStart(procedure, builder);

		var hasInput = procedure.Parameters.Any();
		var hasOutput = procedure.Selects.Any();

		BuildExecuteFunction(procedure, builder, hasInput, hasOutput);

		if (hasInput)
			GenerateMainInputRecord(procedure, builder);

		ParsedParameter[] tableValueParameters = procedure.Parameters.Where(_ => _.IsTableValue).ToArray();
		if (tableValueParameters.Length > 0)
			GenerateTableValueInputRecords(tableValueParameters, builder);

		if (hasOutput)
			GenerateOutputRecord(procedure, builder);

		builder.AppendLine("}");

		string classFile = builder.ToString();
		return classFile;
	}

	private static void BuildUsings(StringBuilder builder)
	{
		builder.AppendLine("using System;");
		builder.AppendLine("using System.Data;");
		builder.AppendLine("using System.Threading;");
		builder.AppendLine("using System.Threading.Tasks;");
		builder.AppendLine("using Microsoft.Data.SqlClient;");
		builder.AppendLine();
	}

	private void BuildNamespace(StringBuilder builder)
	{
		builder.Append("namespace ");
		builder.Append(Namespace);
		builder.AppendLine(";");
		builder.AppendLine();
	}

	private static void BuildClassStart(ParsedProcedure procedure, StringBuilder builder)
	{
		builder.Append("public class ");
		builder.Append(procedure.Name);
		builder.AppendLine("(IDatabaseConnectionFactory _connectionFactory)");
		builder.AppendLine("{");
	}

	private static void BuildExecuteFunction(
		ParsedProcedure procedure,
		StringBuilder builder,
		bool hasInput,
		bool hasOutput)
	{
		builder.Append("\tpublic virtual async Task");
		if (hasOutput)
		{
			builder.Append("<Output");
			Select select = procedure.Selects.First();
			if (!select.IsSingleRow)
				builder.Append("[]");

			builder.Append('>');
		}

		builder.Append(" ExecuteAsync(");
		if (hasInput)
			builder.Append("Input input, ");

		builder.AppendLine("CancellationToken? cancellationToken = null)");

		builder.AppendLine("\t{");
		builder.AppendLine("\t\tcancellationToken ??= CancellationToken.None;");
		builder.AppendLine();
		builder.AppendLine("\t\tawait using var connection = await _connectionFactory.ConnectAsync(cancellationToken.Value).ConfigureAwait(false);");
		builder.AppendLine();

		builder.Append("\t\tvar command = new SqlCommand(\"[");
		builder.Append(procedure.Schema);
		builder.Append("].[");
		builder.Append(procedure.RawName);
		builder.AppendLine("]\", connection)");
		builder.AppendLine("\t\t{");
		builder.AppendLine("\t\t\tCommandType = CommandType.StoredProcedure");
		builder.AppendLine("\t\t};");
		builder.AppendLine();

		var parameterIndex = 0;

		foreach (ParsedParameter parameter in procedure.Parameters.Where(_ => !_.IsOutput))
		{
			if (parameter.IsTableValue)
			{
				var udtName = $"udt{parameter.Name}";
				builder.Append("\t\tvar ");
				builder.Append(udtName);
				builder.AppendLine(" = new DataTable();");

				foreach (var column in parameter.TableValue.Columns)
				{
					builder.Append("\t\t");
					builder.Append(udtName);
					builder.Append(".Columns.Add(new DataColumn { AllowDBNull = ");

					if (column.IsNullable)
						builder.Append("true");
					else
						builder.Append("false");

					builder.Append(", ColumnName = \"");
					builder.Append(column.Name);
					builder.Append("\", DataType = typeof(");

					var typeName = column.DataTypes[TypeFormat.DotNetFrameworkType];
					builder.Append(typeName.Replace("?", ""));
					builder.AppendLine(") });");
				}
				builder.AppendLine();

				builder.Append("\t\tforeach (var item in input.");
				builder.Append(parameter.Name);
				builder.AppendLine(")");
				builder.AppendLine("\t\t{");

				builder.Append("\t\t\t");
				builder.Append(udtName);
				builder.Append(".Rows.Add(");

				bool isFirst = true;
				foreach (var column in parameter.TableValue.Columns)
				{
					if (!isFirst)
						builder.Append(", ");

					builder.Append("item.");
					builder.Append(column.Name);

					isFirst = false;
				}
				builder.AppendLine(");");
				builder.AppendLine("\t\t}");
				builder.AppendLine();

				var paramVarName = $"p{parameterIndex}";
				builder.Append("\t\tvar ");
				builder.Append(paramVarName);
				builder.Append(" = command.Parameters.AddWithValue(\"@");
				builder.Append(parameter.Name);
				builder.Append("\", ");
				builder.Append(udtName);
				builder.AppendLine(");");

				builder.Append("\t\t");
				builder.Append(paramVarName);
				builder.AppendLine(".SqlDbType = SqlDbType.Structured;");

				builder.Append("\t\t");
				builder.Append(paramVarName);
				builder.Append(".TypeName = \"[");
				builder.Append(parameter.TableValue.Schema);
				builder.Append("].[");
				builder.Append(parameter.TableValue.Name);
				builder.AppendLine("]\";");
				builder.AppendLine();
			}
			else
			{
				var paramVarName = $"p{parameterIndex}";
				builder.Append("\t\tvar ");
				builder.Append(paramVarName);
				builder.Append(" = command.Parameters.AddWithValue(\"@");
				builder.Append(parameter.Name);
				builder.Append("\", input.");
				builder.Append(parameter.Name);
				builder.AppendLine(");");

				builder.Append("\t\t");
				builder.Append(paramVarName);
				builder.Append(".SqlDbType = SqlDbType.");
				builder.Append(parameter.DataTypes[TypeFormat.SqlDbTypeEnum]);
				builder.AppendLine(";");

				if (parameter.DataTypes[TypeFormat.DotNetFrameworkType] == "string?")
				{
					builder.Append("\t\t");
					builder.Append(paramVarName);
					builder.Append(".Size = ");
					builder.Append(parameter.Size);
					builder.AppendLine(";");
				}

				builder.AppendLine();
			}

			parameterIndex++;
		}

		// TODO

		if (!hasOutput)
			builder.AppendLine("\t\t_ = await command.ExecuteNonQueryAsync(cancellationToken.Value).ConfigureAwait(false);");
		else
		{
			Select select = procedure.Selects.First();

			builder.AppendLine("\t\tusing SqlDataReader dataReader = await command.ExecuteReaderAsync(cancellationToken.Value).ConfigureAwait(false);");
			builder.AppendLine("\t\tif (dataReader.HasRows)");
			builder.AppendLine("\t\t{");

			var ordinalVariableNames = new Dictionary<string, string>();
			var ordinalIndex = 0;
			foreach (SelectColumn column in select.Columns)
			{
				ordinalVariableNames[column.Name] = $"o{ordinalIndex}";

				builder.Append("\t\t\tint o");
				builder.Append(ordinalIndex);
				builder.Append(" = dataReader.GetOrdinal(\"");
				builder.Append(column.Name);
				builder.AppendLine("\");");

				ordinalIndex++;
			}
			builder.AppendLine();

			builder.AppendLine("\t\t\tvar outputs = new List<Output>();");
			builder.AppendLine("\t\t\twhile (await dataReader.ReadAsync(cancellationToken.Value).ConfigureAwait(false))");
			builder.AppendLine("\t\t\t{");
			builder.AppendLine("\t\t\t\tvar output = new Output");
			builder.AppendLine("\t\t\t\t{");

			bool isFirst = true;
			foreach (SelectColumn column in select.Columns)
			{
				if (!isFirst)
					builder.AppendLine(",");

				builder.Append("\t\t\t\t\t");
				builder.Append(column.Name);
				builder.Append(" = await dataReader.GetFieldValueAsync<");

				var columnTypeName = GetColumnTypeName(column);
				builder.Append(columnTypeName);

				builder.Append(">(");
				builder.Append(ordinalVariableNames[column.Name]);
				builder.Append(", cancellationToken.Value)");

				isFirst = false;
			}
			builder.AppendLine();

			builder.AppendLine("\t\t\t\t}");
			builder.AppendLine("\t\t\t\toutputs.Add(output);");
			builder.AppendLine("\t\t\t}");

			builder.AppendLine();
			builder.AppendLine("\t\t\treturn [.. outputs];");

			builder.AppendLine("\t\t}");
			builder.AppendLine("\t\telse");
			builder.AppendLine("\t\t{");
			builder.AppendLine("\t\t\treturn [];");
			builder.AppendLine("\t\t}");
		}

		builder.AppendLine("\t}");
		builder.AppendLine();
	}

	private static void GenerateMainInputRecord(ParsedProcedure procedure, StringBuilder builder)
	{
		builder.AppendLine("\tpublic record Input");
		builder.AppendLine("\t{");

		foreach (ParsedParameter parameter in procedure.Parameters.Where(_ => !_.IsOutput))
		{
			GenerateParameter(builder, parameter);
		}

		builder.AppendLine("\t}");
	}

	private static void GenerateParameter(StringBuilder builder, ParsedParameter parameter)
	{
		builder.Append("\t\tpublic ");

		if (parameter.IsTableValue)
		{
			BuildTableValueInputRecordName(parameter, builder);
			builder.Append("[]");
		}
		else
		{
			builder.Append(parameter.DataTypes[TypeFormat.DotNetFrameworkType]);
		}

		builder.Append(' ');
		builder.Append(parameter.Name);
		builder.AppendLine(" { get; init; }");
	}

	private static void GenerateTableValueInputRecords(ParsedParameter[] tableValueParameters, StringBuilder builder)
	{
		foreach (ParsedParameter parameter in tableValueParameters)
		{
			builder.AppendLine();
			GenerateTableValueInputRecord(parameter, builder);
		}
	}

	private static void GenerateTableValueInputRecord(ParsedParameter parameter, StringBuilder builder)
	{
		builder.Append("\tpublic record ");
		BuildTableValueInputRecordName(parameter, builder);
		builder.AppendLine();
		builder.AppendLine("\t{");

		foreach (ParsedColumn column in parameter.TableValue.Columns)
		{
			builder.Append("\t\tpublic ");
			builder.Append(column.DataTypes[TypeFormat.DotNetFrameworkType]);
			builder.Append(' ');
			builder.Append(column.Name);
			builder.AppendLine(" { get; init; }");
		}

		builder.AppendLine("\t}");
	}

	private static void BuildTableValueInputRecordName(ParsedParameter parameter, StringBuilder builder)
	{
		builder.Append(parameter.Name);
		builder.Append("Input");
	}

	private static void GenerateOutputRecord(ParsedProcedure procedure, StringBuilder builder)
	{
		Select select = procedure.Selects.First();

		builder.AppendLine("\tpublic record Output");
		builder.AppendLine("\t{");

		foreach (SelectColumn column in select.Columns)
		{
			builder.Append("\t\tpublic ");
			builder.Append(GetColumnTypeName(column));
			builder.Append(' ');
			builder.Append(column.Name);
			builder.AppendLine(" { get; init; }");
		}

		builder.AppendLine("\t}");
	}

	private static string GetColumnTypeName(SelectColumn column)
	{
		var typeName = column.DataTypes[TypeFormat.DotNetFrameworkType];
		if (!column.IsNullable)
			typeName = typeName.Replace("?", "");
		return typeName;
	}
}
