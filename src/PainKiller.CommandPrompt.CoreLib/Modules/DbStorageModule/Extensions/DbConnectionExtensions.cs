using System.Data;
using System.Data.SqlClient;

namespace PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Extensions;

public static class DbConnectionExtensions
{
    public static IDbConnection CreateDbConnection(this string connectionString, string provider = "sqlserver")
    {
        return provider.ToLower() switch
        {
            "sqlserver" => new SqlConnection(connectionString),
            //"sqlite"    => new SqliteConnection(connectionString),
            //"postgres"  => new NpgsqlConnection(connectionString),
            _           => throw new NotSupportedException($"Provider '{provider}' is not supported.")
        };
    }
}