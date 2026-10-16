using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using UnitConverter.Models;
using UnitConverter.Services;

namespace UnitConverter.Pages;

public class LogsModel : PageModel
{
    private readonly ILogReader _logReader;

    public LogsModel(ILogReader logReader)
    {
        _logReader = logReader;
    }

    public IEnumerable<LogEntry> Entries { get; set; } = new List<LogEntry>();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Level { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ConversionType { get; set; }

    public void OnGet()
    {
        // Read all logs from disk
        var allEntries = _logReader.Read();

        // Apply basic step-by-step filtering loops as required by Section 10
        var filteredList = new List<LogEntry>();

        foreach (var entry in allEntries)
        {
            // 1. Filter by Log Level
            if (!string.IsNullOrEmpty(Level) && !string.Equals(entry.Level, Level, System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // 2. Filter by Conversion Type
            if (!string.IsNullOrEmpty(ConversionType) && !string.Equals(entry.ConversionType, ConversionType, System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // 3. Filter by Search Query (message or exception text)
            if (!string.IsNullOrEmpty(Search))
            {
                bool messageMatches = entry.Message?.Contains(Search, System.StringComparison.OrdinalIgnoreCase) ?? false;
                bool exceptionMatches = entry.Exception?.Contains(Search, System.StringComparison.OrdinalIgnoreCase) ?? false;

                if (!messageMatches && !exceptionMatches)
                {
                    continue;
                }
            }

            filteredList.Add(entry);
        }

        Entries = filteredList;
    }
}
