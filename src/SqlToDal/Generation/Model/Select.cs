using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlToDal.Generation.Model;

public class Select
{
	public Select(QuerySpecification querySpecification, IDictionary<string, DataType> bodyColumnTypes)
	{
		// Get any table aliases.
		var aliasResolutionVisitor = new AliasResolutionVisitor();
		querySpecification.Accept(aliasResolutionVisitor);
		TableAliases = aliasResolutionVisitor.Aliases;

		var outerJoinedTables = new List<string>();
		if (querySpecification.FromClause != null)
		{
			foreach (var join in querySpecification.FromClause.TableReferences.OfType<QualifiedJoin>())
			{
				FillOuterJoins(outerJoinedTables, join, false);
			}
		}

		var topInt = querySpecification.TopRowFilter != null ? querySpecification.TopRowFilter.Expression as IntegerLiteral : null;
		IsSingleRow = topInt != null && topInt.Value == "1" && querySpecification.TopRowFilter.Percent == false;
		Columns = querySpecification.SelectElements.OfType<SelectScalarExpression>().Select(x => new SelectColumn(x, bodyColumnTypes, TableAliases, outerJoinedTables)).ToList();
	}

	private static void FillOuterJoins(List<string> outerJoinedTables, QualifiedJoin qualifiedJoin, bool isParentOuterJoined)
	{
		var tableReferences = new List<TableReference>();
		if (qualifiedJoin.QualifiedJoinType == QualifiedJoinType.LeftOuter)
		{
			if (isParentOuterJoined) tableReferences.Add(qualifiedJoin.FirstTableReference);
			tableReferences.Add(qualifiedJoin.SecondTableReference);
		}
		else if (qualifiedJoin.QualifiedJoinType == QualifiedJoinType.RightOuter)
		{
			if (isParentOuterJoined) tableReferences.Add(qualifiedJoin.SecondTableReference);
			tableReferences.Add(qualifiedJoin.FirstTableReference);
		}
		else if (qualifiedJoin.QualifiedJoinType == QualifiedJoinType.FullOuter || qualifiedJoin.QualifiedJoinType == QualifiedJoinType.Inner)
		{
			if (isParentOuterJoined)
			{
				tableReferences.Add(qualifiedJoin.FirstTableReference);
				tableReferences.Add(qualifiedJoin.SecondTableReference);
			}
		}

		foreach (var tableReference in tableReferences)
		{
			if (tableReference is QualifiedJoin nestedQualifiedJoin)
			{
				FillOuterJoins(outerJoinedTables, nestedQualifiedJoin, true);
			}
			else if (tableReference is NamedTableReference namedTableReference)
			{
				var aliasOrName = namedTableReference.Alias != null && !string.IsNullOrEmpty(namedTableReference.Alias.Value)
					? namedTableReference.Alias.Value
					: namedTableReference.SchemaObject.BaseIdentifier.Value;
				outerJoinedTables.Add(aliasOrName);
			}
		}
	}

	public IEnumerable<SelectColumn> Columns { get; private set; }
	public bool IsSingleRow { get; private set; }
	public IDictionary<string, string> TableAliases { get; private set; }
}
