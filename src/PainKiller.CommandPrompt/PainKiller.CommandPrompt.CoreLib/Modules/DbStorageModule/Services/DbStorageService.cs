using System.Data;
using Dapper;
using System.Reflection;
using PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Configuration;
using PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Extensions;

namespace PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Services;

public class DbStorageService<T> where T : new()
{
    private readonly IDbConnection _dbConnection;
    private readonly string _tableName;
    private readonly DatabaseConfig _config;

    public DbStorageService(DatabaseConfig config)
    {
        _config = config;
        _tableName = typeof(T).Name;
        _dbConnection = config.ConnectionString.CreateDbConnection(config.Provider);
        EnsureTableExists();
    }

    public TIdentity InsertObject<TIdentity>(T storeObject)
    {
        var insertQuery = BuildInsertQuery(storeObject, out var parameters);
        var id = _dbConnection.ExecuteScalar(insertQuery, parameters);
        SetIdentityValue(storeObject, id!);
        return (TIdentity)Convert.ChangeType(id, typeof(TIdentity));
    }

    public TIdentity UpdateObject<TIdentity>(T storeObject, Func<T, bool> match)
    {
        var existing = GetAll().FirstOrDefault(match);
        if (existing == null) return default!;

        var updateQuery = BuildUpdateQuery(storeObject, out var parameters);
        _dbConnection.Execute(updateQuery, parameters);
        return (TIdentity)GetIdentityValue(storeObject)!;
    }

    public bool DeleteObject<TIdentity>(Func<T, bool> match)
    {
        var obj = GetAll().FirstOrDefault(match);
        if (obj == null) return false;

        var id = GetIdentityValue(obj);
        var sql = $"DELETE FROM {_tableName} WHERE {GetIdentityProperty()?.Name} = @Id";
        return _dbConnection.Execute(sql, new { Id = id }) > 0;
    }

    public T GetObject(Func<T, bool> match) => GetAll().FirstOrDefault(match)!;

    public List<T> GetAll() => _dbConnection.Query<T>($"SELECT * FROM {_tableName}").ToList();

    // === Internal ===
    private string BuildInsertQuery(T storeObject, out DynamicParameters parameters)
    {
        var props = typeof(T).GetProperties();
        var identity = GetIdentityProperty();
        var columns = props.Where(p => identity == null || p.Name != identity.Name).ToList();

        var colNames = string.Join(", ", columns.Select(p => p.Name));
        var colParams = string.Join(", ", columns.Select(p => $"@{p.Name}"));

        parameters = new DynamicParameters();
        foreach (var prop in columns)
            parameters.Add($"@{prop.Name}", prop.GetValue(storeObject));

        return $"INSERT INTO {_tableName} ({colNames}) VALUES ({colParams}); SELECT SCOPE_IDENTITY();";
    }

    private string BuildUpdateQuery(T storeObject, out DynamicParameters parameters)
    {
        var props = typeof(T).GetProperties();
        var identity = GetIdentityProperty();

        if (identity == null) throw new InvalidOperationException("No identity property found.");

        var setClause = string.Join(", ", props.Where(p => p.Name != identity.Name).Select(p => $"{p.Name} = @{p.Name}"));

        parameters = new DynamicParameters(storeObject);
        parameters.Add("@Id", GetIdentityValue(storeObject));

        return $"UPDATE {_tableName} SET {setClause} WHERE {identity.Name} = @Id";
    }

    private void EnsureTableExists()
    {
        var identity = GetIdentityProperty();
        if (identity == null) throw new Exception("No identity property found for table creation.");

        var props = typeof(T).GetProperties();
        var columnDefinitions = new List<string>();

        foreach (var prop in props)
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            var isNullable = IsNullableProperty(prop);

            if (!_config.TypeMappings.TryGetValue(type, out var sqlBaseType))
                throw new NotSupportedException($"No SQL mapping defined for {type.Name}");

            var nullability = isNullable ? "NULL" : "NOT NULL";

            var column = prop.Name == identity.Name
                ? $"{prop.Name} {sqlBaseType} PRIMARY KEY {(type == typeof(Guid) ? "DEFAULT NEWID()" : "IDENTITY(1,1)")}"
                : $"{prop.Name} {sqlBaseType} {nullability}";

            columnDefinitions.Add(column);
        }

        var createSql = $@"
IF OBJECT_ID(N'{_tableName}', N'U') IS NULL
BEGIN
    CREATE TABLE {_tableName} (
        {string.Join(",\n        ", columnDefinitions)}
    )
END";

        _dbConnection.Execute(createSql);
    }

    private static bool IsNullableProperty(PropertyInfo propertyInfo)
    {
        if (!propertyInfo.PropertyType.IsValueType)
        {
            var nullableAttr = propertyInfo.CustomAttributes
                .FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");

            if (nullableAttr != null && nullableAttr.ConstructorArguments.Count > 0)
            {
                var arg = nullableAttr.ConstructorArguments[0];
                if (arg.ArgumentType == typeof(byte) && (byte)arg.Value! == 2) return true;
            }
            return false; // assume non-nullable
        }

        return Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;
    }

    private PropertyInfo? GetIdentityProperty()
    {
        var type = typeof(T);
        return type.GetProperty("Id") ?? type.GetProperty($"{type.Name}Id");
    }

    private object? GetIdentityValue(T obj)
    {
        var prop = GetIdentityProperty();
        return prop?.GetValue(obj);
    }

    private void SetIdentityValue(T obj, object value)
    {
        var prop = GetIdentityProperty();
        if (prop == null || value == null) return;

        if (prop.PropertyType == typeof(Guid))
        {
            if (value is Guid g) prop.SetValue(obj, g);
            else if (value is string s) prop.SetValue(obj, Guid.Parse(s));
            else if (value is byte[] bytes) prop.SetValue(obj, new Guid(bytes));
        }
        else
        {
            prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
        }
    }
}