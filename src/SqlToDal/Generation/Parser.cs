using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlToDal.Generation.Model;

namespace SqlToDal.Generation;

public class Parser
{
	private static readonly TSqlParser _parser = new TSql160Parser(true, SqlEngineType.All);

	public string ProcedurePrefix { get; set; } = "";

	public ParsedProcedure[] Parse(string dacpacPath)
	{
		var model = new TSqlModel(dacpacPath);

		var sqlTables = model.GetObjects(DacQueryScopes.UserDefined, Table.TypeClass);
		var sqlViews = model.GetObjects(DacQueryScopes.UserDefined, View.TypeClass);
		var foreignKeyDictionaries = new[] { GetForeignKeys(sqlTables), GetForeignKeys(sqlViews) };
		var foreignKeys = foreignKeyDictionaries
			.SelectMany(x => x)
			.ToDictionary(pair => pair.Key, pair => pair.Value);

		var primaryKeys = model
			.GetObjects(DacQueryScopes.UserDefined, PrimaryKeyConstraint.TypeClass)
			.Select(o => o.GetReferenced().Where(r => r.ObjectType.Name == "Column")).SelectMany(c => c);

		var parsedViews = sqlViews.Select(_ => new ParsedView(_, primaryKeys, foreignKeys));
		var procedures = model
			.GetObjects(DacQueryScopes.UserDefined, Procedure.TypeClass)
			.Select(sqlProc => new ParsedProcedure(sqlProc, ProcedurePrefix, parsedViews, primaryKeys, foreignKeys))
			.ToList();

		return [.. procedures];
	}

	public static IDictionary<TSqlObject, IEnumerable<ForeignKeyConstraintDefinition>> GetForeignKeys(IEnumerable<TSqlObject> objects)
	{
		return objects.Select(obj =>
		{
			_ = obj.TryGetAst(out var ast);
			var foreignKeyConstraintVisitor = new ForeignKeyConstraintVisitor();
			if (ast.Batches[0].Statements[0] is TSqlStatementSnippet statementSnippet)
			{
				using var reader = new StringReader(statementSnippet.Script);
				var fragment = _parser.Parse(reader, out var errors);
				fragment.Accept(foreignKeyConstraintVisitor);
			}
			else
				ast.AcceptChildren(foreignKeyConstraintVisitor);

			return new { obj, foreignKeyConstraintVisitor.Nodes };
		}).ToDictionary(key => key.obj, val => val.Nodes.AsEnumerable());
	}
}
