using System.Collections.Generic;

namespace SqlToDal.Generation.Model;

public record RelationshipIdentifier
{
	public required string Database { get; init; }
	public required string Schema { get; init; }
	public required string TableOrView { get; init; }
	public required IEnumerable<string> Columns { get; init; }
}
