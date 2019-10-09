using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArrayToDictionaryTool
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var jsonInFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.json");
            var jsonOutFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.json");
            var keyFieldName = "verb";

            await using var fileStream = File.OpenRead(jsonInFile);
            var jsonDocument = await JsonDocument.ParseAsync(fileStream);
            
            var dict = new Dictionary<string, Dictionary<string, object>>();
            foreach (var jsonElement in jsonDocument.RootElement.EnumerateArray())
            {
                string key = null;
                var value = new Dictionary<string, object>();
                foreach (var jsonProperty in jsonElement.EnumerateObject())
                    if (jsonProperty.Name == keyFieldName)
                        key = jsonProperty.Value.GetString();
                    else
                        value.TryAdd(jsonProperty.Name, jsonProperty.Value);

                if (key != null) dict.TryAdd(key, value);
            }

            var transformedJson = JsonSerializer.Serialize(dict,
                new JsonSerializerOptions
                    {WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping});
            File.WriteAllText(jsonOutFile, transformedJson);
        }
    }
}