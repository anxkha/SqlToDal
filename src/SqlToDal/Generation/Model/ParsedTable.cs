using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlToDal.Generation.Model;

public class ParsedTable
{
	public string Schema { get; private set; }
	public string Name { get; private set; }
	public IEnumerable<ParsedColumn> Columns { get; private set; }

	public ParsedTable(
		TSqlObject tSqlObject,
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

		// Get the name.
		Name = tSqlObject.Name.Parts.Last();

		// Get the columns
		var columns = new List<ParsedColumn>();
		var sqlColumns = tSqlObject.ObjectType.Name == "TableType" ? tSqlObject.GetReferenced(TableType.Columns) : tSqlObject.GetReferenced(Table.Columns);
		foreach (var sqlColumn in sqlColumns)
		{
			var column = new ParsedColumn(sqlColumn, tSqlObject, primaryKeys, foreignKeys);
			columns.Add(column);
		}
		Columns = columns;
	}
}
