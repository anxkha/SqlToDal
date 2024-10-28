using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlToDal.Generation.Model;

public class ParsedProcedure
{
	private static TSqlParser _parser = new TSql160Parser(true, SqlEngineType.All);

	public string Schema { get; private set; }
	public string Name { get; private set; }
	public string RawName { get; private set; }
	public string Prefix { get; private set; }
	public IEnumerable<ParsedParameter> Parameters { get; private set; }
	public IEnumerable<Select> Selects { get; private set; }

	public ParsedProcedure(
		TSqlObject tSqlObject,
		string prefix,
		IEnumerable<TSqlObject> primaryKeys,
		IDictionary<TSqlObject,
			IEnumerable<ForeignKeyConstraintDefinition>> foreignKeys)
	{
		if (tSqlObject.Name.Parts.Count == 2)
			Schema = tSqlObject.Name.Parts[0];
		else if (tSqlObject.Name.Parts.Count == 3)
			Schema = tSqlObject.Name.Parts[1];
		else
			Schema = "dbo";

        Prefix = prefix ?? "";
		RawName = tSqlObject.Name.Parts.Last();
		Name = RawName[Prefix.Length..];
		Parameters = tSqlObject.GetReferenced(Procedure.Parameters).Select(x => new ParsedParameter(x, primaryKeys, foreignKeys));

		_ = tSqlObject.TryGetAst(out var ast);
		var selectVisitor = new SelectVisitor();
		if (ast.Batches[0].Statements[0] is TSqlStatementSnippet statementSnippet)
		{
			using var reader = new StringReader(statementSnippet.Script);
			var fragment = _parser.Parse(reader, out var errors);
			fragment.Accept(selectVisitor);
		}
		else
			ast.AcceptChildren(selectVisitor);

		var bodyColumnTypes = tSqlObject.GetReferenced(Procedure.BodyDependencies)
			.Where(x => x.ObjectType.Name == "Column")
			.GroupBy(bd => string.Join(".", bd.Name.Parts))
			.Select(grp => grp.First())
			.ToDictionary(
				key => string.Join(".", key.Name.Parts),
				val => new DataType
				{
					Map = DataTypeHelper.Instance.GetMap(TypeFormat.SqlServerDbType, val.GetReferenced(Column.DataType).First().Name.Parts.Last()),
					Nullable = Column.Nullable.GetValue<bool>(val)
				},
				StringComparer.InvariantCultureIgnoreCase);

		var unions = selectVisitor.Nodes.OfType<BinaryQueryExpression>().Select(GetQueryFromUnion).Where(x => x != null);
		var selects = selectVisitor.Nodes.OfType<QuerySpecification>().Concat(unions);

		Selects = selects.Select(s => new Select(s, bodyColumnTypes)).ToList();
	}

	private static QuerySpecification GetQueryFromUnion(BinaryQueryExpression binaryQueryExpression)
	{
		while (binaryQueryExpression.FirstQueryExpression as BinaryQueryExpression != null)
		{
			binaryQueryExpression = binaryQueryExpression.FirstQueryExpression as BinaryQueryExpression;
		}
		return binaryQueryExpression.FirstQueryExpression as QuerySpecification;
	}
}
