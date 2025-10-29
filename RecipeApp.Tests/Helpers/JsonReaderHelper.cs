using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RecipeApp.Tests.Helpers;

public static class JsonReaderHelper<T>
{
    public static List<T> GetJsonData(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        string resourceName = $"RecipeApp.Tests.TestData.{fileName}.json";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
        using var reader = new StreamReader(stream);

        string jsonData = reader.ReadToEnd();
        var deseralizerOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        List<T>? jsonDataAsList = JsonSerializer.Deserialize<List<T>>(jsonData, deseralizerOptions);
        if (jsonDataAsList == null)
            return new List<T>() { };

        return jsonDataAsList;
    }
}
