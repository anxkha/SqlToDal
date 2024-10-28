using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SqlToDal;

public class InsertLog(IDatabaseConnectionFactory _connectionFactory)
{
	public virtual async Task ExecuteAsync(Input input, CancellationToken? cancellationToken = null)
	{
		cancellationToken ??= CancellationToken.None;

		await using var connection = await _connectionFactory.ConnectAsync(cancellationToken.Value).ConfigureAwait(false);

		var command = new SqlCommand("[Platform].[usp_InsertLog]", connection)
		{
			CommandType = CommandType.StoredProcedure
		};

		var p0 = command.Parameters.AddWithValue("@ObjectID", input.ObjectID);
		p0.SqlDbType = SqlDbType.UniqueIdentifier;

		var p1 = command.Parameters.AddWithValue("@Message", input.Message);
		p1.SqlDbType = SqlDbType.NVarChar;
		p1.Size = 200;

		_ = await command.ExecuteNonQueryAsync(cancellationToken.Value).ConfigureAwait(false);
	}

	public record Input
	{
		public Guid ObjectID { get; init; }
		public string? Message { get; init; }
	}
}
