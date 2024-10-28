using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SqlToDal;

public interface IDatabaseConnectionFactory
{
	Task<SqlConnection> ConnectAsync(CancellationToken cancellationToken);
}
