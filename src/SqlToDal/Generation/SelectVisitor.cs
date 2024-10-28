using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlToDal.Generation;

internal class SelectVisitor : TSqlFragmentVisitor
{
	public SelectVisitor()
	{
		Nodes = [];
	}

	public List<QueryExpression> Nodes { get; private set; }

	public override void Visit(SelectStatement node)
	{
		base.Visit(node);
		Nodes.Add(node.QueryExpression);
	}
}
