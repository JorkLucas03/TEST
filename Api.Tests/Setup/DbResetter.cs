using Microsoft.Data.SqlClient;
using Respawn;

namespace Api.Tests.Setup;

public class DbResetter
{
    private readonly string _connectionString;
    private Respawner? _respawner;

    public DbResetter(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task ResetAsync()
    {
        if (_respawner == null)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            
            _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer
            });
        }

        await using var conn2 = new SqlConnection(_connectionString);
        await conn2.OpenAsync();
        await _respawner.ResetAsync(conn2);
    }
}
