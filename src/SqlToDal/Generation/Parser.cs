using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlToDal.Generation.Model;

namespace SqlToDal.Generation;

public class Parser
{
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
		var procedures = model
			.GetObjects(DacQueryScopes.UserDefined, Procedure.TypeClass)
			.Select(sqlProc => new ParsedProcedure(sqlProc, ProcedurePrefix, primaryKeys, foreignKeys))
			.ToList();

		return [.. procedures];
	}

	public static IDictionary<TSqlObject, IEnumerable<ForeignKeyConstraintDefinition>> GetForeignKeys(IEnumerable<TSqlObject> objects)
	{
		return objects.Select(obj =>
		{
			TSqlModelUtils.TryGetFragmentForAnalysis(obj, out TSqlFragment fragment);
			var foreignKeyConstraintVisitor = new ForeignKeyConstraintVisitor();
			fragment.Accept(foreignKeyConstraintVisitor);
			return new { obj, foreignKeyConstraintVisitor.Nodes };
		}).ToDictionary(key => key.obj, val => val.Nodes.AsEnumerable());
	}
}
