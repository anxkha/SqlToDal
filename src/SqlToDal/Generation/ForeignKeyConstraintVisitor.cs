using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlToDal.Generation;

internal class ForeignKeyConstraintVisitor : TSqlFragmentVisitor
{
	public ForeignKeyConstraintVisitor()
	{
		Nodes = [];
	}

	public List<ForeignKeyConstraintDefinition> Nodes { get; private set; }

	public override void Visit(TSqlFragment node)
	{
		base.Visit(node);
	}

	public override void Visit(ForeignKeyConstraintDefinition node)
	{
		base.Visit(node);
		Nodes.Add(node);
	}
}
