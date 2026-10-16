using System.Text.Json;
using System.Text.Json.Serialization;

namespace Task2.Utils;

public class JsonFileWriter
{
    private readonly string _dataFolderPath;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonFileWriter()
    {
        _dataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");

        if (!Directory.Exists(_dataFolderPath))
        {
            Directory.CreateDirectory(_dataFolderPath);
        }

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task WriteDataAsync<T>(string fileName, List<T> data)
    {
        var filePath = Path.Combine(_dataFolderPath, fileName);

        var json = JsonSerializer.Serialize(data, _jsonOptions);
        await File.WriteAllTextAsync(filePath, json);
    }
}
