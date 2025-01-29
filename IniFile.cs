using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RibbonWin
{


    public class IniFile
    {
        private readonly Dictionary<string, Dictionary<string, string>> sections;

        private string filePath = "";

        public IniFile(string argPath)
        {
            filePath = argPath;

            sections = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            Load();
        }

        public void Load()
        {
            sections.Clear();

            string currentSection = string.Empty;

            if (!File.Exists(filePath))
            {
                var myfile = File.Create(filePath);
                myfile.Close();
            }

            foreach (var line in File.ReadLines(filePath))
            {
                if (line.Trim().StartsWith("[") && line.Trim().EndsWith("]"))
                {
                    currentSection = line.Trim('[', ']');
                    sections[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else if (!string.IsNullOrWhiteSpace(currentSection))
                {
                    var parts = line.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        sections[currentSection][key] = value;
                    }
                }
            }
        }

        public void Save()
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var section in sections)
                {
                    writer.WriteLine($"[{section.Key}]");
                    foreach (var entry in section.Value)
                    {
                        writer.WriteLine($"{entry.Key}={entry.Value}");
                    }
                    writer.WriteLine();
                }
            }
        }

        public string GetValue(string section, string key, string defaultValue = "")
        {
            if (sections.TryGetValue(section, out var values) && values.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public void SetValue(string section, string key, string value)
        {
            if (!sections.TryGetValue(section, out var values))
            {
                values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                sections[section] = values;
            }

            values[key] = value;
        }

        public List<string> GetSectionNames()
        {
            return sections.Keys.ToList();
        }
    }

}




