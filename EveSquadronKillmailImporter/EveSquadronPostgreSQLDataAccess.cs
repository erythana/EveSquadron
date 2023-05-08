using System.Data;
using EveSquadronKillmailImporter.Interfaces;
using Microsoft.Extensions.Configuration;
using Dapper;
using Npgsql;

namespace EveSquadronKillmailImporter;

internal class EveSquadronPostgreSqlDataAccess : IEveSquadronDataAccess
{
    private readonly int _maxQueryParameters;

    private readonly Dictionary<Type, NpgsqlTypes.NpgsqlDbType> _postgreSqlTypeMapping = new()
    {
        [typeof(int)] = NpgsqlTypes.NpgsqlDbType.Integer,
        [typeof(double)] = NpgsqlTypes.NpgsqlDbType.Double,
        [typeof(decimal)] = NpgsqlTypes.NpgsqlDbType.Numeric,
        [typeof(string)] = NpgsqlTypes.NpgsqlDbType.Varchar,
        [typeof(DateTime)] = NpgsqlTypes.NpgsqlDbType.Timestamp,
        [typeof(char[])] = NpgsqlTypes.NpgsqlDbType.Varchar,
        [typeof(Guid)] = NpgsqlTypes.NpgsqlDbType.Uuid
    };

    private readonly NpgsqlDataSource _postgreSqlDataSource;

    public EveSquadronPostgreSqlDataAccess(IConfiguration configuration, string connectionID = "Default")
    {
        _maxQueryParameters = configuration.GetValue<int>("MaximumQueryParameterSize");
        if (_maxQueryParameters == 0)
            _maxQueryParameters = 2000;

        _postgreSqlDataSource = new NpgsqlDataSourceBuilder(configuration.GetConnectionString(connectionID)).Build();
    }

    public async Task<IEnumerable<int>> GetAlreadyCreatedKillmailIDs(IEnumerable<int> checkableIDs)
    {
        var results = new List<int>();
        var idList = checkableIDs.ToList();

        await using var connection = await _postgreSqlDataSource.OpenConnectionAsync();

        for (var i = 0; i < idList.Count; i += _maxQueryParameters)
        {
            results.AddRange(connection.Query<int>("select * from killmail where ID = ANY(@IDs)",
                new
                {
                    IDs = idList.Skip(i).Take(_maxQueryParameters).ToList()
                }));
        }

        return results;
    }

    public async Task SaveKillmails(DataTable killmailsDataTable)
    {
        var sql = killmailsDataTable.Columns.Cast<DataColumn>().Aggregate("COPY " + "Killmail" + " ( ", (current, col) => current + col.ColumnName.ToLower() + ",").TrimEnd(',') + ") FROM STDIN (FORMAT BINARY)";

        await using var connection = await _postgreSqlDataSource.OpenConnectionAsync();
        await using var bulkWrite = await connection.BeginBinaryImportAsync(sql);

        for (var i = 0; i < killmailsDataTable.Rows.Count; i++)
        {
            await bulkWrite.StartRowAsync();
            foreach (DataColumn col in killmailsDataTable.Columns)
            {
                if (killmailsDataTable.Rows[i].IsNull(col) || col.DataType == typeof(string) && string.IsNullOrEmpty(killmailsDataTable.Rows[i].Field<string>(col)))
                    await bulkWrite.WriteNullAsync();
                else
                    await bulkWrite.WriteAsync(killmailsDataTable.Rows[i][col.Ordinal], _postgreSqlTypeMapping[col.DataType]);
            }

        }

        await bulkWrite.CompleteAsync();
    }
}