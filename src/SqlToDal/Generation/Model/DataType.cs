using System.Collections.Generic;

namespace SqlToDal.Generation.Model;

public class DataType
{
	public IDictionary<TypeFormat, string> Map { get; set; }
	public bool Nullable { get; set; }
}
