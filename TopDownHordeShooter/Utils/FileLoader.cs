using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace TopDownHordeShooter.Utils
{
    // Loader class from course content with modifications to make it more reusable and less memory-expensive
    public static class FileLoader
    {
        public static string ReadTextFileComplete(string filePath)
        {
            var result = new StringBuilder();
            var fileStream = File.OpenRead(filePath);
            try
            {
                using var reader = new StreamReader(fileStream);
                result.Append(reader.ReadToEnd());
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: File could not be read!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
            
            return result.ToString();
        }
        
        public static List<string> ReadLinesFromTextFile(string filePath)
        {
            var lines = new List<string>();
            var fileStream = File.OpenRead(filePath);
            try
            {
                using (var reader = new StreamReader(fileStream))
                {
                    var line = "";
                    while ((line = reader.ReadLine()) != null)
                        lines.Add(line);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: File could not be read!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
            return lines;
        }
        
        // Object to JSON
        public static string ObjectToJSON<T>(T obj) => JsonSerializer.Serialize(obj);
        // Convert JSON string to any class object
        public static object JSONToObject(string jsonBody, Type classType) => JsonSerializer.Deserialize(jsonBody, classType);

    }
}