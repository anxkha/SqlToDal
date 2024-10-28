using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlToDal.Generation.Model;

public class ParsedParameter
{
	public string Name { get; private set; }
	public IDictionary<TypeFormat, string> DataTypes { get; private set; }
	public int Size { get; private set; }
	public ParsedTable TableValue { get; private set; }
	public bool IsTableValue => TableValue != null;
	public bool IsOutput { get; private set; }

	public ParsedParameter(
		TSqlObject tSqlObject,
		IEnumerable<TSqlObject> primaryKeys,
		IDictionary<TSqlObject,
		IEnumerable<ForeignKeyConstraintDefinition>> foreignKeys)
	{
		Name = tSqlObject.Name.Parts.Last().Trim('@');
		IsOutput = Parameter.IsOutput.GetValue<bool>(tSqlObject);

		var dataType = tSqlObject.GetReferenced(Parameter.DataType).ToList().FirstOrDefault();
		if (dataType is null)
		{
			DataTypes = DataTypeHelper.Instance.GetMap(TypeFormat.DotNetFrameworkType, "Object");
		}
		else if (dataType.ObjectType.Name == "TableType")
		{
			TableValue = new ParsedTable(dataType, primaryKeys, foreignKeys);
		}
		else
		{
			var sqlDataTypeName = dataType.Name.Parts.Last();
			DataTypes = DataTypeHelper.Instance.GetMap(TypeFormat.SqlServerDbType, sqlDataTypeName);
			Size = Parameter.Length.GetValue<int>(tSqlObject);
		}
	}
}
