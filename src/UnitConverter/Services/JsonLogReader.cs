using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using UnitConverter.Models;

namespace UnitConverter.Services;

public class JsonLogReader : ILogReader
{
    private readonly string _logDirectory;

    public JsonLogReader(IWebHostEnvironment environment)
    {
        _logDirectory = Path.Combine(environment.ContentRootPath, "Logs");
    }

    public IEnumerable<LogEntry> Read()
    {
        var entries = new List<LogEntry>();

        if (!Directory.Exists(_logDirectory))
        {
            return entries;
        }

        string[] logFiles = Directory.GetFiles(_logDirectory, "*.json");

        foreach (string logFile in logFiles)
        {
            try
            {
                string[] lines = File.ReadAllLines(logFile);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        LogEntry? entry = JsonSerializer.Deserialize<LogEntry>(line);
                        if (entry != null)
                        {
                            if (string.IsNullOrEmpty(entry.Level))
                            {
                                entry.Level = "Information";
                            }
                            entries.Add(entry);
                        }
                    }
                    catch (JsonException)
                    {
                        continue;
                    }
                }
            }
            catch (IOException)
            {
                continue;
            }
        }

        return entries;
    }
}
