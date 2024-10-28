using System;
using System.Collections.Generic;
using System.Linq;
using SqlToDal.Generation.Model;

namespace SqlToDal.Generation;

public class DataTypeHelper
{
	private DataTypeHelper()
	{
		LoadDataTypes();
	}

	private IDictionary<DataTypes, IDictionary<TypeFormat, string>> dataTypes;

	private static DataTypeHelper instance;

	public static DataTypeHelper Instance
	{
		get
		{
			instance ??= new DataTypeHelper();
			return instance;
		}
	}

	public string ToDataType(string sourceDataType, TypeFormat sourceFormat, TypeFormat destinationFormat)
	{
		var map = GetMap(sourceFormat, sourceDataType)
			?? throw new NotSupportedException(string.Format("Could not find a {0} data type of {1}. Ensure the sourceFormat is appropriate for the specified sourceDataType.", Enum.GetName(typeof(TypeFormat), sourceFormat), sourceDataType));
		return map[destinationFormat];
	}

	/// <summary>
	/// Gets a single data type dictionary with values for each TypeFormat by finding the specified entry in the list of data type dictionaries.
	/// </summary>
	/// <param name="lookupFormat">The TypeFormat of the lookupValue parameter.</param>
	/// <param name="lookupValue">The value to identify the dictionary to return.</param>
	/// <returns>
	/// The first data type dictionary that contains the specified TypeFormat and value.
	/// </returns>
	public IDictionary<TypeFormat, string> GetMap(TypeFormat lookupFormat, string lookupValue)
	{
		return dataTypes.Values.FirstOrDefault(dic => dic.Any(entry => entry.Key == lookupFormat && entry.Value == lookupValue));
	}

	/// <summary>
	/// Gets a single data type dictionary with values for each TypeFormat 
	/// </summary>
	public IDictionary<TypeFormat, string> GetMap(DataTypes dataType)
	{
		return dataTypes[dataType];
	}

	private void LoadDataTypes()
	{
		dataTypes = new Dictionary<DataTypes, IDictionary<TypeFormat, string>>
		{
			{ DataTypes.@bigint, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "bigint" }, { TypeFormat.DotNetFrameworkType, "Int64?" }, { TypeFormat.SqlDbTypeEnum, "BigInt" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlInt64" }, { TypeFormat.DbTypeEnum, "Int64" }, { TypeFormat.SqlDataReaderDbType, "GetInt64" } } },
			{ DataTypes.@binary, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "binary" }, { TypeFormat.DotNetFrameworkType, "byte[]" }, { TypeFormat.SqlDbTypeEnum, "VarBinary" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlBinary" }, { TypeFormat.DbTypeEnum, "Binary" }, { TypeFormat.SqlDataReaderDbType, "GetBytes" } } },
			{ DataTypes.@bit, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "bit" }, { TypeFormat.DotNetFrameworkType, "boolean?" }, { TypeFormat.SqlDbTypeEnum, "Bit" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlBoolean" }, { TypeFormat.DbTypeEnum, "Boolean" }, { TypeFormat.SqlDataReaderDbType, "GetBoolean" } } },
			{ DataTypes.@char, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "char" }, { TypeFormat.DotNetFrameworkType, "string?" }, { TypeFormat.SqlDbTypeEnum, "Char" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlString" }, { TypeFormat.DbTypeEnum, "String" }, { TypeFormat.SqlDataReaderDbType, "GetString" } } },
			{ DataTypes.@date, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "date" }, { TypeFormat.DotNetFrameworkType, "DateTime?" }, { TypeFormat.SqlDbTypeEnum, "Date" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlDateTime" }, { TypeFormat.DbTypeEnum, "Date" }, { TypeFormat.SqlDataReaderDbType, "GetDateTime" } } },
			{ DataTypes.@datetime, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "datetime" }, { TypeFormat.DotNetFrameworkType, "DateTime?" }, { TypeFormat.SqlDbTypeEnum, "DateTime" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlDateTime" }, { TypeFormat.DbTypeEnum, "DateTime" }, { TypeFormat.SqlDataReaderDbType, "GetDateTime" } } },
			{ DataTypes.@datetime2, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "datetime2" }, { TypeFormat.DotNetFrameworkType, "DateTime?" }, { TypeFormat.SqlDbTypeEnum, "DateTime2" }, { TypeFormat.SqlDataReaderSqlType, null }, { TypeFormat.DbTypeEnum, "DateTime2" }, { TypeFormat.SqlDataReaderDbType, "GetDateTime" } } },
			{ DataTypes.@datetimeoffset, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "datetimeoffset" }, { TypeFormat.DotNetFrameworkType, "DateTimeOffset?" }, { TypeFormat.SqlDbTypeEnum, "DateTimeOffset" }, { TypeFormat.SqlDataReaderSqlType, null }, { TypeFormat.DbTypeEnum, "DateTimeOffset" }, { TypeFormat.SqlDataReaderDbType, "GetDateTimeOffset" } } },
			{ DataTypes.@decimal, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "decimal" }, { TypeFormat.DotNetFrameworkType, "decimal?" }, { TypeFormat.SqlDbTypeEnum, "Decimal" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlDecimal" }, { TypeFormat.DbTypeEnum, "Decimal" }, { TypeFormat.SqlDataReaderDbType, "GetDecimal" } } },
			{ DataTypes.@float, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "float" }, { TypeFormat.DotNetFrameworkType, "double?" }, { TypeFormat.SqlDbTypeEnum, "Float" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlDouble" }, { TypeFormat.DbTypeEnum, "Double" }, { TypeFormat.SqlDataReaderDbType, "GetDouble" } } },
			{ DataTypes.@image, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "image" }, { TypeFormat.DotNetFrameworkType, "byte[]" }, { TypeFormat.SqlDbTypeEnum, "Binary" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlBinary" }, { TypeFormat.DbTypeEnum, "Binary" }, { TypeFormat.SqlDataReaderDbType, "GetBytes" } } },
			{ DataTypes.@int, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "int" }, { TypeFormat.DotNetFrameworkType, "int?" }, { TypeFormat.SqlDbTypeEnum, "Int" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlInt32" }, { TypeFormat.DbTypeEnum, "Int32" }, { TypeFormat.SqlDataReaderDbType, "GetInt32" } } },
			{ DataTypes.@money, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "money" }, { TypeFormat.DotNetFrameworkType, "decimal?" }, { TypeFormat.SqlDbTypeEnum, "Money" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlMoney" }, { TypeFormat.DbTypeEnum, "Decimal" }, { TypeFormat.SqlDataReaderDbType, "GetDecimal" } } },
			{ DataTypes.@nchar, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "nchar" }, { TypeFormat.DotNetFrameworkType, "string?" }, { TypeFormat.SqlDbTypeEnum, "NChar" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlString" }, { TypeFormat.DbTypeEnum, "StringFixedLength" }, { TypeFormat.SqlDataReaderDbType, "GetString" } } },
			{ DataTypes.@ntext, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "ntext" }, { TypeFormat.DotNetFrameworkType, "string?" }, { TypeFormat.SqlDbTypeEnum, "NText" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlString" }, { TypeFormat.DbTypeEnum, "String" }, { TypeFormat.SqlDataReaderDbType, "GetString" } } },
			{ DataTypes.@numeric, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "numeric" }, { TypeFormat.DotNetFrameworkType, "decimal?" }, { TypeFormat.SqlDbTypeEnum, "Decimal" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlDecimal" }, { TypeFormat.DbTypeEnum, "Decimal" }, { TypeFormat.SqlDataReaderDbType, "GetDecimal" } } },
			{ DataTypes.@nvarchar, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "nvarchar" }, { TypeFormat.DotNetFrameworkType, "string?" }, { TypeFormat.SqlDbTypeEnum, "NVarChar" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlString" }, { TypeFormat.DbTypeEnum, "String" }, { TypeFormat.SqlDataReaderDbType, "GetString" } } },
			{ DataTypes.@real, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "real" }, { TypeFormat.DotNetFrameworkType, "float?" }, { TypeFormat.SqlDbTypeEnum, "Real" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlSingle" }, { TypeFormat.DbTypeEnum, "Single" }, { TypeFormat.SqlDataReaderDbType, "GetFloat" } } },
			{ DataTypes.@rowversion, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "rowversion" }, { TypeFormat.DotNetFrameworkType, "byte[]" }, { TypeFormat.SqlDbTypeEnum, "Timestamp" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlBinary" }, { TypeFormat.DbTypeEnum, "Binary" }, { TypeFormat.SqlDataReaderDbType, "GetBytes" } } },
			{ DataTypes.@smalldatetime, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "smalldatetime" }, { TypeFormat.DotNetFrameworkType, "DateTime?" }, { TypeFormat.SqlDbTypeEnum, "DateTime" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlDateTime" }, { TypeFormat.DbTypeEnum, "DateTime" }, { TypeFormat.SqlDataReaderDbType, "GetDateTime" } } },
			{ DataTypes.@smallint, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "smallint" }, { TypeFormat.DotNetFrameworkType, "short?" }, { TypeFormat.SqlDbTypeEnum, "SmallInt" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlInt16" }, { TypeFormat.DbTypeEnum, "Int16" }, { TypeFormat.SqlDataReaderDbType, "GetInt16" } } },
			{ DataTypes.@smallmoney, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "smallmoney" }, { TypeFormat.DotNetFrameworkType, "decimal?" }, { TypeFormat.SqlDbTypeEnum, "SmallMoney" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlMoney" }, { TypeFormat.DbTypeEnum, "Decimal" }, { TypeFormat.SqlDataReaderDbType, "GetDecimal" } } },
			{ DataTypes.@sql_variant, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "sql_variant" }, { TypeFormat.DotNetFrameworkType, "object" }, { TypeFormat.SqlDbTypeEnum, "Variant" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlValue *" }, { TypeFormat.DbTypeEnum, "Object" }, { TypeFormat.SqlDataReaderDbType, "GetValue" } } },
			{ DataTypes.@text, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "text" }, { TypeFormat.DotNetFrameworkType, "string?" }, { TypeFormat.SqlDbTypeEnum, "Text" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlString" }, { TypeFormat.DbTypeEnum, "String" }, { TypeFormat.SqlDataReaderDbType, "GetString" } } },
			{ DataTypes.@time, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "time" }, { TypeFormat.DotNetFrameworkType, "TimeSpan?" }, { TypeFormat.SqlDbTypeEnum, "Time" }, { TypeFormat.SqlDataReaderSqlType, "none" }, { TypeFormat.DbTypeEnum, "Time" }, { TypeFormat.SqlDataReaderDbType, "GetDateTime" } } },
			{ DataTypes.@timestamp, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "timestamp" }, { TypeFormat.DotNetFrameworkType, "byte[]" }, { TypeFormat.SqlDbTypeEnum, "Timestamp" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlBinary" }, { TypeFormat.DbTypeEnum, "Binary" }, { TypeFormat.SqlDataReaderDbType, "GetBytes" } } },
			{ DataTypes.@tinyint, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "tinyint" }, { TypeFormat.DotNetFrameworkType, "byte?" }, { TypeFormat.SqlDbTypeEnum, "TinyInt" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlByte" }, { TypeFormat.DbTypeEnum, "Byte" }, { TypeFormat.SqlDataReaderDbType, "GetByte" } } },
			{ DataTypes.@uniqueidentifier, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "uniqueidentifier" }, { TypeFormat.DotNetFrameworkType, "Guid" }, { TypeFormat.SqlDbTypeEnum, "UniqueIdentifier" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlGuid" }, { TypeFormat.DbTypeEnum, "Guid" }, { TypeFormat.SqlDataReaderDbType, "GetGuid" } } },
			{ DataTypes.@varbinary, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "varbinary" }, { TypeFormat.DotNetFrameworkType, "byte[]" }, { TypeFormat.SqlDbTypeEnum, "VarBinary" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlBinary" }, { TypeFormat.DbTypeEnum, "Binary" }, { TypeFormat.SqlDataReaderDbType, "GetBytes" } } },
			{ DataTypes.@varchar, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "varchar" }, { TypeFormat.DotNetFrameworkType, "string?" }, { TypeFormat.SqlDbTypeEnum, "VarChar" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlString" }, { TypeFormat.DbTypeEnum, "String" }, { TypeFormat.SqlDataReaderDbType, "GetString" } } },
			{ DataTypes.@xml, new Dictionary<TypeFormat, string> { { TypeFormat.SqlServerDbType, "xml" }, { TypeFormat.DotNetFrameworkType, "Xml" }, { TypeFormat.SqlDbTypeEnum, "Xml" }, { TypeFormat.SqlDataReaderSqlType, "GetSqlXml" }, { TypeFormat.DbTypeEnum, "Xml" }, { TypeFormat.SqlDataReaderDbType, null } } }
		};
	}
}
