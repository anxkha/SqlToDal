using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SqlToDal.Generated;

public class InsertLogs(IDatabaseConnectionFactory _connectionFactory)
{
	public virtual async Task ExecuteAsync(Input input, CancellationToken? cancellationToken = null)
	{
		cancellationToken ??= CancellationToken.None;

		await using var connection = await _connectionFactory.ConnectAsync(cancellationToken.Value).ConfigureAwait(false);

		var command = new SqlCommand("[Platform].[usp_InsertLogs]", connection)
		{
			CommandType = CommandType.StoredProcedure
		};

		var udtLogs = new DataTable();
		udtLogs.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "ObjectID", DataType = typeof(Guid) });
		udtLogs.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "Message", DataType = typeof(string) });

		foreach (var item in input.Logs)
		{
			udtLogs.Rows.Add(item.ObjectID, item.Message);
		}

		var p0 = command.Parameters.AddWithValue("@Logs", udtLogs);
		p0.SqlDbType = SqlDbType.Structured;
		p0.TypeName = "[Platform].[LogType]";
	}

	public record Input
	{
		public LogsInput[] Logs { get; init; }
	}

	public record LogsInput
	{
		public Guid ObjectID { get; init; }
		public string Message { get; init; }
	}
}
