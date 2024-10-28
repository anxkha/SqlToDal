using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Dac.Model;

namespace SqlToDal.Generation.Model;

public class ParsedColumn
{
	public string Name { get; private set; }
	public IDictionary<TypeFormat, string> DataTypes { get; private set; }
	public bool IsIdentity { get; private set; }
	public bool IsNullable { get; private set; }
	public int Precision { get; private set; }
	public int Scale { get; private set; }
	public int Length { get; private set; }
	public bool IsPrimaryKey { get; private set; }
	public bool IsForeignKey { get; private set; }
	public IEnumerable<RelationshipIdentifier> ParentRelationships { get; private set; }
	public IEnumerable<RelationshipIdentifier> ChildRelationships { get; private set; }

	public ParsedColumn(
		TSqlObject tSqlObject,
		TSqlObject tSqlTable,
		IEnumerable<TSqlObject> primaryKeys,
		IDictionary<TSqlObject, IEnumerable<Microsoft.SqlServer.TransactSql.ScriptDom.ForeignKeyConstraintDefinition>> foreignKeys)
	{
		Name = tSqlObject.Name.Parts.Last();
		var fullName = string.Join(".", tSqlObject.Name.Parts);

		IsPrimaryKey = primaryKeys.Any(p => string.Join(".", p.Name.Parts) == fullName);

		// Get relationships where this column is the child.
		foreignKeys.TryGetValue(tSqlTable, out IEnumerable<Microsoft.SqlServer.TransactSql.ScriptDom.ForeignKeyConstraintDefinition> myForeignKeys);
		myForeignKeys ??= [];
		ParentRelationships = from f in myForeignKeys
							  where f.Columns.Any(c => c.Value ==Name)
							  select new RelationshipIdentifier
							  {
								  TableOrView = f.ReferenceTableName.BaseIdentifier?.Value,
								  Schema = f.ReferenceTableName.SchemaIdentifier?.Value,
								  Database = f.ReferenceTableName.DatabaseIdentifier?.Value,
								  Columns = f.ReferencedTableColumns.Select(c => c.Value)
							  };
		IsForeignKey = ParentRelationships.Any();

		// Get relationships where this column is the parent.
		var childTables = foreignKeys.Where(f => f.Value.Any(v =>
			v.ReferenceTableName.BaseIdentifier.Value == tSqlTable.Name.Parts.Last()
			&& v.ReferencedTableColumns.Any(c => c.Value == Name)));
		ChildRelationships = from t in childTables
							 from r in t.Value
							 let tableParts = t.Key.Name.Parts.Count
							 select new RelationshipIdentifier
							 {
								 TableOrView = t.Key.Name.Parts.Last(),
								 Schema = tableParts > 1 ? t.Key.Name.Parts.ElementAt(tableParts - 2) : null,
								 Database = tableParts > 2 ? t.Key.Name.Parts.ElementAt(tableParts - 3) : null,
								 Columns = r.Columns.Select(c => c.Value)
							 };

		if (tSqlObject.ObjectType.Name == "TableTypeColumn")
		{
			var sqlDataTypeName = tSqlObject.GetReferenced(TableTypeColumn.DataType).ToList().First().Name.Parts.Last();
			DataTypes = DataTypeHelper.Instance.GetMap(TypeFormat.SqlServerDbType, sqlDataTypeName);
			IsIdentity = TableTypeColumn.IsIdentity.GetValue<bool>(tSqlObject);
			IsNullable = TableTypeColumn.Nullable.GetValue<bool>(tSqlObject);
			Precision = TableTypeColumn.Precision.GetValue<int>(tSqlObject);
			Scale = TableTypeColumn.Scale.GetValue<int>(tSqlObject);
			Length = TableTypeColumn.Length.GetValue<int>(tSqlObject);
		}
		else
		{
			ColumnType metaType = tSqlObject.GetMetadata<ColumnType>(Column.ColumnType);

			switch (metaType)
			{
				case ColumnType.Column:
				case ColumnType.ColumnSet:
					SetProperties(tSqlObject);
					break;
				case ColumnType.ComputedColumn:
					// use the referenced column - this works for simple view referenced
					// column but not for a computed expression like [Name] = [FirstName] + ' ' + [LastName]
					var referenced = tSqlObject.GetReferenced().ToArray();
					if (referenced.Length == 1)
					{
						var tSqlObjectReferenced = referenced[0];
						SetProperties(tSqlObjectReferenced);
					}
					else
					{
						// TODO: how to get and evaluate the expression?
					}
					break;
			}
		}
	}

	private void SetProperties(TSqlObject tSqlObject)
	{
		var sqlDataTypeName = tSqlObject.GetReferenced(Column.DataType).ToList().First().Name.Parts.Last();
		DataTypes = DataTypeHelper.Instance.GetMap(TypeFormat.SqlServerDbType, sqlDataTypeName);
		IsIdentity = Column.IsIdentity.GetValue<bool>(tSqlObject);
		IsNullable = Column.Nullable.GetValue<bool>(tSqlObject);
		Precision = Column.Precision.GetValue<int>(tSqlObject);
		Scale = Column.Scale.GetValue<int>(tSqlObject);
		Length = Column.Length.GetValue<int>(tSqlObject);
	}
}
