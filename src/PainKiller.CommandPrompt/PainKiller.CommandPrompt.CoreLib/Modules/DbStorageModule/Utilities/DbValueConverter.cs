using PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Attributes;
using System.Reflection;
using System.Text.Json;

namespace PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Utilities;

public static class DbValueConverter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static object? ConvertToDbValue(object? value, PropertyInfo property)
    {
        if (value == null) return null;

        if (property.GetCustomAttribute<JsonColumnAttribute>() != null)
            return JsonSerializer.Serialize(value, JsonOptions);

        var type = value.GetType();

        if (type.IsEnum)
            return value.ToString();

        return IsSimpleType(type) ? value : JsonSerializer.Serialize(value, JsonOptions);
    }

    public static object? ConvertFromDbValue(object? dbValue, Type targetType)
    {
        if (dbValue == null) return null;
        if (IsSimpleType(targetType)) return Convert.ChangeType(dbValue, targetType);

        return JsonSerializer.Deserialize(dbValue.ToString()!, targetType, JsonOptions);
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
               || type == typeof(string)
               || type == typeof(DateTime)
               || type == typeof(decimal)
               || type == typeof(Guid)
               || type == typeof(DateTimeOffset)
               || type == typeof(TimeSpan)
               || type.IsEnum;
    }
}