namespace SqlToDal.Generation.Model;

public enum TypeFormat
{
	/// <summary>
	/// The SQL server database type
	/// </summary>
	/// <example>"bigint"</example>
	SqlServerDbType,

	/// <summary>
	/// The dot net framework type
	/// </summary>
	/// <example>"Int64?"</example>
	DotNetFrameworkType,

	/// <summary>
	/// The SqlDbTypeEnum type
	/// </summary>
	/// <example>"BigInt"</example>
	SqlDbTypeEnum,

	/// <summary>
	/// The SQL data reader SQL type
	/// </summary>
	/// <example>"GetSqlInt64"</example>
	SqlDataReaderSqlType,

	/// <summary>
	/// The database type enum
	/// </summary>
	/// <example>"Int64"</example>
	DbTypeEnum,

	/// <summary>
	/// The SQL data reader database type
	/// </summary>
	/// <example>"GetInt64"</example>
	SqlDataReaderDbType
}
