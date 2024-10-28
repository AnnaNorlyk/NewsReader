using System;
using System.Collections.Generic;
using System.IO;

namespace NewsReader
{
    public class IniFile
    {
        private readonly string _path;

        // Constructor to set the INI file path, defaults to "config.ini" 
        public IniFile(string iniPath = null)
        {
            _path = iniPath ?? "config.ini";
        }



        // Writes a key-value pair to the INI file, updating if the key exists or adding it if not
        public void Write(string key, string value)
        {

            // Read all lines if the file exists, or start with an empty array
            var lines = File.Exists(_path) ? File.ReadAllLines(_path) : Array.Empty<string>();
            bool keyExists = UpdateKeyIfExists(lines, key, value);

            if (!keyExists)
            {
                AddNewKey(key, value);
            }
            else
            {
                File.WriteAllLines(_path, lines);
            }
        }



        // Reads the value of a specified key from the INI file
        public string Read(string key)
        {
            if (!File.Exists(_path))
            {
                Console.WriteLine("File not found.");
                return null;
            }

            string value = FindKeyValue(File.ReadLines(_path), key);
            return value;
        }



        // Updates the value of a key if found
        private bool UpdateKeyIfExists(string[] lines, string key, string value)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(key + "="))
                {
                    lines[i] = $"{key}={value}";
                    return true;
                }
            }
            return false;
        }



        // Adds a new key-value pair to the file. Environment.NewLine is used to 
        private void AddNewKey(string key, string value)
        {
            File.AppendAllText(_path, $"{key}={value}\r\n");
        }



        // Searches the collection of lines for a given key and returns its value if found.
        private string FindKeyValue(IEnumerable<string> lines, string key)
        {
            foreach (var line in lines)
            {
                if (line.StartsWith(key + "="))
                {
                    string value = line.Substring((key + "=").Length);
                    return value;
                }
            }

            Console.WriteLine($"Key not found: {key}");
            return null;
        }
    }
}
