﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using TopDownHordeShooter.Entities.Misc;

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
    
    public class ScoreManager
    {
        private static string _fileName = "scores.xml"; // Since we don't give a path, this'll be saved in the "bin" folder

        public List<Score> Highscores { get; private set; }

        public List<Score> Scores { get; private set; }

        public ScoreManager()
            : this(new List<Score>())
        {

        }

        public ScoreManager(List<Score> scores)
        {
            Scores = scores;

            UpdateHighScores();
        }

        public void Add(Score score)
        {
            Scores.Add(score);

            Scores = Scores.OrderByDescending(c => c.Value).ToList(); // Orders the list so that the higher scores are first

            UpdateHighScores();
        }

        public static ScoreManager Load()
        {
            // If there isn't a file to load - create a new instance of "ScoreManager"
            if (!File.Exists(_fileName))
                return new ScoreManager();

            // Otherwise we load the file

            using (var reader = new StreamReader(new FileStream(_fileName, FileMode.Open)))
            {
                var serializer = new XmlSerializer(typeof(List<Score>));

                var scores = (List<Score>)serializer.Deserialize(reader);

                return new ScoreManager(scores);
            }
        }

        private void UpdateHighScores()
        {
            Highscores = Scores.Take(5).ToList(); // Takes the first 5 elements
        }

        public static void Save(ScoreManager scoreManager)
        {
            // Overrides the file if it already exists
            using var writer = new StreamWriter(new FileStream(_fileName, FileMode.Create));
            var serializer = new XmlSerializer(typeof(List<Score>));

            serializer.Serialize(writer, scoreManager.Scores);
        }
    }
}