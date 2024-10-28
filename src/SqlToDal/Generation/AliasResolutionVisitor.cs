using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlToDal.Generation;

internal class AliasResolutionVisitor : TSqlFragmentVisitor
{
	public Dictionary<string, string> Aliases { get; } = [];

	public override void Visit(NamedTableReference namedTableReference)
	{
		var alias = namedTableReference.Alias;
		if (alias != null)
		{
			var baseObjectName = string.Join(".", namedTableReference.SchemaObject.Identifiers.Select(x => x.Value));
			Aliases.Add(alias.Value, baseObjectName);
		}
	}
}
