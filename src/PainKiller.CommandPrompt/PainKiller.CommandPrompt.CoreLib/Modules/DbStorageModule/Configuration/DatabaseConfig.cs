using PainKiller.CommandPrompt.CoreLib.Core.Extensions;

namespace PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Configuration;
public class DatabaseConfig
{
    private readonly string _connectionString = string.Empty;

    public string ConnectionString
    {
        get => _connectionString.GetReplacedPlaceHolderPath();
        init => _connectionString = value;
    }
    public string Provider { get; init; } = "sqlserver";
    public Dictionary<string, string> TypeMappingsRaw { get; init; } = new();
    public Dictionary<Type, string> TypeMappings => TypeMappingsRaw.ToDictionary(kvp => ResolveType(kvp.Key), kvp => kvp.Value);
    private static Type ResolveType(string typeName) => typeName.ToLowerInvariant() switch
    {
        "string"    => typeof(string),
        "int"       => typeof(int),
        "guid"      => typeof(Guid),
        "datetime"  => typeof(DateTime),
        "bool"      => typeof(bool),
        "double"    => typeof(double),
        "float"     => typeof(float),
        "decimal"   => typeof(decimal),
        "long"      => typeof(long),
        "short"     => typeof(short),
        "byte"      => typeof(byte),
        "char"      => typeof(char),
        "byte[]"    => typeof(byte[]),
        _ => throw new NotSupportedException($"Unsupported type mapping: {typeName}")
    };
}