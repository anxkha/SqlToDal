using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SqlToDal;

public class SelectLogs(IDatabaseConnectionFactory _connectionFactory)
{
	public virtual async Task<Output[]> ExecuteAsync(CancellationToken? cancellationToken = null)
	{
		cancellationToken ??= CancellationToken.None;

		await using var connection = await _connectionFactory.ConnectAsync(cancellationToken.Value).ConfigureAwait(false);

		var command = new SqlCommand("[Platform].[usp_SelectLogs]", connection)
		{
			CommandType = CommandType.StoredProcedure
		};



		using SqlDataReader dataReader = await command.ExecuteReaderAsync(cancellationToken.Value).ConfigureAwait(false);
		if (dataReader.HasRows)
		{
			var o0 = dataReader.GetOrdinal("ObjectID");
			var o1 = dataReader.GetOrdinal("Message");
			var o2 = dataReader.GetOrdinal("CreatedTimestamp");

			var outputs = new List<Output>();
			while (await dataReader.ReadAsync(cancellationToken.Value).ConfigureAwait(false))
			{
				var output = new Output
				{
					ObjectID = await dataReader.GetFieldValueAsync<Guid>(o0, cancellationToken.Value).ConfigureAwait(false),
					Message = await dataReader.GetFieldValueAsync<string>(o1, cancellationToken.Value).ConfigureAwait(false),
					CreatedTimestamp = await dataReader.GetFieldValueAsync<DateTimeOffset>(o2, cancellationToken.Value).ConfigureAwait(false)
				};
			}

			return [.. outputs];
		}
		else
		{
			return [];
		}
	}

	public record Output
	{
		public Guid ObjectID { get; init; }
		public string Message { get; init; }
		public DateTimeOffset CreatedTimestamp { get; init; }
	}
}
