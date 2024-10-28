using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlToDal.Generation.Model;

public class SelectColumn
{
	public SelectColumn(
		SelectScalarExpression selectScalarExpression,
		IDictionary<string, DataType> bodyColumnTypes,
		IDictionary<string, string> tableAliases,
		IEnumerable<string> outerJoinedTables)
	{
		if (selectScalarExpression.Expression is ColumnReferenceExpression columnReferenceExpression)
		{
			var identifiers = columnReferenceExpression.MultiPartIdentifier.Identifiers;
			var fullColName = GetFullColumnName(tableAliases, identifiers);

			Name = selectScalarExpression.ColumnName != null && selectScalarExpression.ColumnName.Value != null
				? selectScalarExpression.ColumnName.Value
				: identifiers.Last().Value;

			var key = bodyColumnTypes.Keys.FirstOrDefault(x => x.EndsWith(fullColName, StringComparison.InvariantCultureIgnoreCase))
				?? throw new InvalidOperationException("Could not find column within BodyDependencies: " + fullColName);
			bool outerJoined = false;
			// If the column was defined in the SELECT with the table alias or name, check if the table was outer joined.
			if (identifiers.Count > 1)
			{
				var tableAliasOrName = identifiers.ElementAt(identifiers.Count - 2).Value;
				outerJoined = outerJoinedTables.Contains(tableAliasOrName);
			}
			else // If the column was defined in the SELECT without any qualification, then there must
				 // be only one column in the list of tables with that name. Look it up in the bodyColumnTypes.
			{
				var keyParts = key.Split('.');
				if (keyParts.Length > 1)
				{
					var tableAliasOrName = keyParts.ElementAt(keyParts.Length - 2);
					outerJoined = outerJoinedTables.Contains(tableAliasOrName);
				}
			}

			var bodyColumnType = bodyColumnTypes[key];
			DataTypes = bodyColumnType.Map;
			IsNullable = bodyColumnType.Nullable || outerJoined;
		}
		else if (selectScalarExpression.Expression is ConvertCall convertCall)
		{
			Name = selectScalarExpression.ColumnName != null && selectScalarExpression.ColumnName.Value != null
				? selectScalarExpression.ColumnName.Value
				: "Value";
			DataTypes = DataTypeHelper.Instance.GetMap(TypeFormat.SqlServerDbType, convertCall.DataType.Name.BaseIdentifier.Value);
			IsNullable = true;
		}
		else if (selectScalarExpression.Expression is CastCall castCall)
		{
			Name = selectScalarExpression.ColumnName != null && selectScalarExpression.ColumnName.Value != null
				? selectScalarExpression.ColumnName.Value
				: "Value";
			DataTypes = DataTypeHelper.Instance.GetMap(TypeFormat.SqlServerDbType, castCall.DataType.Name.BaseIdentifier.Value);
			IsNullable = true;
		}
		else if (selectScalarExpression.Expression is IntegerLiteral integerLiteral)
		{
			Name = selectScalarExpression.ColumnName != null && selectScalarExpression.ColumnName.Value != null
				? selectScalarExpression.ColumnName.Value
				: "Value";
			DataTypes = DataTypeHelper.Instance.GetMap(TypeFormat.SqlServerDbType, "int");
			IsNullable = true;
		}
		else if (selectScalarExpression.Expression is VariableReference variableReference)
		{
			Name = variableReference.Name.TrimStart('@');
			DataTypes = DataTypeHelper.Instance.GetMap(TypeFormat.DotNetFrameworkType, "Object");
			IsNullable = true;
		}
		else
		{
			Name = selectScalarExpression.ColumnName != null && selectScalarExpression.ColumnName.Value != null
				? selectScalarExpression.ColumnName.Value
				: "Value";
			DataTypes = DataTypeHelper.Instance.GetMap(TypeFormat.DotNetFrameworkType, "Object");
			IsNullable = true;
		}
	}

	public string Name { get; private set; }
	public IDictionary<TypeFormat, string> DataTypes { get; private set; }
	public bool IsNullable { get; set; }

	private static string GetFullColumnName(IDictionary<string, string> tableAliases, IList<Identifier> identifiers)
	{
		var list = identifiers.Select(x => x.Value).ToArray();
		if (list.Length > 1)
		{
			var tableIdentifier = list.ElementAt(list.Length - 2);
			if (tableAliases.Keys.Any(x => x == tableIdentifier))
			{
				list[^2] = tableAliases[tableIdentifier];
			}
		}
		return string.Join(".", list);
	}
}
