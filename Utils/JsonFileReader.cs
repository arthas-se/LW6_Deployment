using System.Text.Json;
using System.Text.Json.Serialization;

namespace Task2.Utils;

public class JsonFileReader
{
    private readonly string _dataFolderPath;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonFileReader()
    {
        _dataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<T>> ReadDataAsync<T>(string fileName)
    {
        var filePath = Path.Combine(_dataFolderPath, fileName);

        if (!File.Exists(filePath))
        {
            return new List<T>();
        }

        var json = await File.ReadAllTextAsync(filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<T>();
        }

        return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new List<T>();
    }
}
