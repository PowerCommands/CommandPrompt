using PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Attributes;
using System.Reflection;
using System.Text.Json;

namespace PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Utilities;
public static class JsonColumnHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public static void DeserializeJsonColumns<T>(T obj)
    {
        if (obj == null) return;

        var type = typeof(T);
        foreach (var prop in type.GetProperties())
        {
            if (prop.GetCustomAttribute<JsonColumnAttribute>() is null)
                continue;

            if (prop.PropertyType == typeof(string)) continue; // skip JSON-as-string

            var value = prop.GetValue(obj);
            if (value is not string json) continue;

            var deserialized = JsonSerializer.Deserialize(json, prop.PropertyType, JsonOptions);
            prop.SetValue(obj, deserialized);
        }
    }
    public static void DeserializeJsonColumns<T>(IEnumerable<T> objects)
    {
        foreach (var obj in objects)
        {
            DeserializeJsonColumns(obj);
        }
    }
}