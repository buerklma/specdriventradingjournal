using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TradingJournal.Core.Interfaces;
using TradingJournal.Core.Models;
using TradingJournal.Data;
using TradingJournal.Data.Models;

namespace TradingJournal.Core.Services
{
    /// <summary>
    /// Service for exporting trade data to various formats and managing backups.
    /// </summary>
    public class ExportService : IExportService
    {
        private readonly TradingDbContext _context;

        public ExportService(TradingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<string> ExportToCsvAsync(string outputPath, bool includeHeader = true)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("Output path cannot be null or whitespace.", nameof(outputPath));
            }

            var trades = await _context.Trades.OrderBy(t => t.EntryDateTime).ToListAsync();

            var sb = new StringBuilder();

            if (includeHeader)
            {
                sb.AppendLine("Id,Symbol,SetupType,Direction,EntryDateTime,EntryPrice,PositionSize,StopLoss,TakeProfit,ExitDateTime,ExitPrice,ProfitLossCurrency,RealizedRRRatio,Notes");
            }

            foreach (var trade in trades)
            {
                sb.AppendLine(string.Join(",",
                    trade.Id,
                    EscapeCsvField(trade.Symbol),
                    EscapeCsvField(trade.SetupType ?? string.Empty),
                    trade.Direction,
                    trade.EntryDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    trade.EntryPrice,
                    trade.PositionSize,
                    trade.StopLoss,
                    trade.TakeProfit,
                    trade.ExitDateTime?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty,
                    trade.ExitPrice ?? 0,
                    trade.ProfitLossCurrency ?? 0,
                    trade.RealizedRRRatio ?? 0,
                    EscapeCsvField(trade.Notes ?? string.Empty)
                ));
            }

            await File.WriteAllTextAsync(outputPath, sb.ToString());
            return outputPath;
        }

        public async Task<string> ExportToExcelAsync(string outputPath, bool includeCharts = false)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("Output path cannot be null or whitespace.", nameof(outputPath));
            }

            var trades = await _context.Trades.OrderBy(t => t.EntryDateTime).ToListAsync();

            // Note: This is a simplified implementation
            // In production, use a library like ClosedXML or EPPlus for full Excel support
            // For now, we'll create a basic CSV file with .xlsx extension as a placeholder

            var csvContent = new StringBuilder();
            csvContent.AppendLine("Id,Symbol,SetupType,Direction,EntryDateTime,EntryPrice,PositionSize,ExitPrice,ProfitLossCurrency,RealizedRRRatio");

            foreach (var trade in trades)
            {
                csvContent.AppendLine(string.Join(",",
                    trade.Id,
                    trade.Symbol,
                    trade.SetupType ?? "",
                    trade.Direction,
                    trade.EntryDateTime.ToString("yyyy-MM-dd"),
                    trade.EntryPrice,
                    trade.PositionSize,
                    trade.ExitPrice ?? 0,
                    trade.ProfitLossCurrency ?? 0,
                    trade.RealizedRRRatio ?? 0
                ));
            }

            // Create a temporary directory for Excel files
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(outputPath, csvContent.ToString());

            // TODO: Implement actual Excel generation with multiple sheets and charts
            // This requires adding ClosedXML or EPPlus NuGet package

            return outputPath;
        }

        public async Task<string> ExportToPdfAsync(string outputPath, bool includeSummary = true, bool includeCharts = true)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("Output path cannot be null or whitespace.", nameof(outputPath));
            }

            // Configure QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;

            var trades = await _context.Trades
                .OrderBy(t => t.EntryDateTime)
                .ToListAsync();

            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Calculate statistics
            var totalTrades = trades.Count;
            var closedTrades = trades.Where(t => t.ExitDateTime.HasValue).ToList();
            var totalProfitLoss = closedTrades.Sum(t => t.ProfitLossCurrency ?? 0);
            var winningTrades = closedTrades.Count(t => t.ProfitLossCurrency > 0);
            var losingTrades = closedTrades.Count(t => t.ProfitLossCurrency < 0);
            var winRate = closedTrades.Count > 0 ? (decimal)winningTrades / closedTrades.Count * 100 : 0;

            var grossWins = closedTrades.Where(t => t.ProfitLossCurrency > 0).Sum(t => t.ProfitLossCurrency ?? 0);
            var grossLosses = Math.Abs(closedTrades.Where(t => t.ProfitLossCurrency < 0).Sum(t => t.ProfitLossCurrency ?? 0));
            var profitFactor = grossLosses > 0 ? grossWins / grossLosses : 0;

            var avgRMultiple = closedTrades.Count > 0 ? closedTrades.Average(t => t.ProfitLossR ?? 0) : 0;

            // Calculate max drawdown
            var cumulativePL = 0m;
            var peak = 0m;
            var maxDrawdown = 0m;
            foreach (var trade in closedTrades.OrderBy(t => t.ExitDateTime))
            {
                cumulativePL += trade.ProfitLossCurrency ?? 0;
                if (cumulativePL > peak)
                {
                    peak = cumulativePL;
                }
                var drawdown = peak - cumulativePL;
                if (drawdown > maxDrawdown)
                {
                    maxDrawdown = drawdown;
                }
            }
            var maxDrawdownPercent = peak > 0 ? (maxDrawdown / peak) * 100 : 0;

            // Generate PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item().AlignCenter().Text("Trading Journal Report")
                                .FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                            column.Item().AlignCenter().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                                .FontSize(10).FontColor(Colors.Grey.Darken1);
                            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        });

                    page.Content()
                        .Column(column =>
                        {
                            if (includeSummary)
                            {
                                // Summary Section
                                column.Item().PaddingTop(10).Text("Performance Summary")
                                    .FontSize(16).Bold().FontColor(Colors.Blue.Darken1);

                                column.Item().PaddingTop(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Cell().Border(1).Padding(5).Text("Total Trades").Bold();
                                    table.Cell().Border(1).Padding(5).AlignRight().Text(totalTrades.ToString());

                                    table.Cell().Border(1).Padding(5).Text("Closed Trades").Bold();
                                    table.Cell().Border(1).Padding(5).AlignRight().Text(closedTrades.Count.ToString());

                                    table.Cell().Border(1).Padding(5).Text("Win Rate").Bold();
                                    table.Cell().Border(1).Padding(5).AlignRight().Text($"{winRate:F2}%")
                                        .FontColor(winRate >= 50 ? Colors.Green.Darken1 : Colors.Red.Darken1);

                                    table.Cell().Border(1).Padding(5).Text("Winning Trades").Bold();
                                    table.Cell().Border(1).Padding(5).AlignRight().Text(winningTrades.ToString())
                                        .FontColor(Colors.Green.Darken1);

                                    table.Cell().Border(1).Padding(5).Text("Losing Trades").Bold();
                                    table.Cell().Border(1).Padding(5).AlignRight().Text(losingTrades.ToString())
                                        .FontColor(Colors.Red.Darken1);

                                    table.Cell().Border(1).Padding(5).Text("Total P/L").Bold();
                                    table.Cell().Border(1).Padding(5).AlignRight().Text($"${totalProfitLoss:F2}")
                                        .FontColor(totalProfitLoss >= 0 ? Colors.Green.Darken1 : Colors.Red.Darken1);

                                    table.Cell().Border(1).Padding(5).Text("Profit Factor").Bold();
                                    table.Cell().Border(1).Padding(5).AlignRight().Text($"{profitFactor:F2}");

                                    table.Cell().Border(1).Padding(5).Text("Avg R-Multiple").Bold();
                                    table.Cell().Border(1).Padding(5).AlignRight().Text($"{avgRMultiple:F2}R")
                                        .FontColor(avgRMultiple >= 0 ? Colors.Green.Darken1 : Colors.Red.Darken1);

                                    table.Cell().Border(1).Padding(5).Text("Max Drawdown").Bold();
                                    table.Cell().Border(1).Padding(5).AlignRight().Text($"{maxDrawdownPercent:F2}%")
                                        .FontColor(Colors.Red.Darken1);
                                });
                            }

                            // Recent Trades Section
                            column.Item().PaddingTop(20).Text("Recent Trades (Last 10)")
                                .FontSize(16).Bold().FontColor(Colors.Blue.Darken1);

                            var recentTrades = closedTrades.OrderByDescending(t => t.ExitDateTime).Take(10).ToList();

                            column.Item().PaddingTop(5).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                // Header
                                table.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten2)
                                    .Text("Symbol").Bold().FontSize(8);
                                table.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten2)
                                    .Text("Direction").Bold().FontSize(8);
                                table.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten2)
                                    .Text("Entry Date").Bold().FontSize(8);
                                table.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten2)
                                    .Text("Exit Date").Bold().FontSize(8);
                                table.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten2)
                                    .Text("P/L $").Bold().FontSize(8);
                                table.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten2)
                                    .Text("P/L R").Bold().FontSize(8);

                                // Data rows
                                foreach (var trade in recentTrades)
                                {
                                    table.Cell().Border(1).Padding(3).Text(trade.Symbol ?? "").FontSize(8);
                                    table.Cell().Border(1).Padding(3).Text(trade.Direction.ToString()).FontSize(8);
                                    table.Cell().Border(1).Padding(3).Text(trade.EntryDateTime.ToString("MM/dd/yyyy")).FontSize(8);
                                    table.Cell().Border(1).Padding(3).Text(trade.ExitDateTime?.ToString("MM/dd/yyyy") ?? "Open").FontSize(8);

                                    var plColor = (trade.ProfitLossCurrency ?? 0) >= 0 ? Colors.Green.Darken1 : Colors.Red.Darken1;
                                    table.Cell().Border(1).Padding(3).AlignRight()
                                        .Text($"${trade.ProfitLossCurrency ?? 0:F2}").FontSize(8).FontColor(plColor);
                                    table.Cell().Border(1).Padding(3).AlignRight()
                                        .Text($"{trade.ProfitLossR ?? 0:F2}R").FontSize(8).FontColor(plColor);
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default.FontSize(9).FontColor(Colors.Grey.Darken1));
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                });
            });

            document.GeneratePdf(outputPath);

            return outputPath;
        }

        public async Task<string> CreateBackupAsync(string outputPath, bool includeScreenshots = true)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("Output path cannot be null or whitespace.", nameof(outputPath));
            }

            // Create temporary directory for backup contents
            var tempDir = Path.Combine(Path.GetTempPath(), $"backup_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempDir);

            try
            {
                // Export database
                var dbPath = Path.Combine(tempDir, "trading.db");
                // TODO: Copy actual SQLite database file
                await File.WriteAllTextAsync(dbPath, "Database backup placeholder");

                // Create metadata file
                var metadata = new
                {
                    BackupDate = DateTime.UtcNow,
                    TradeCount = await _context.Trades.CountAsync(),
                    Version = "1.0"
                };

                var metadataPath = Path.Combine(tempDir, "backup-info.json");
                await File.WriteAllTextAsync(metadataPath, JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true }));

                // Include screenshots if requested
                if (includeScreenshots)
                {
                    var screenshotsDir = Path.Combine(tempDir, "screenshots");
                    Directory.CreateDirectory(screenshotsDir);
                    // TODO: Copy actual screenshot files
                }

                // Create ZIP archive
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }

                ZipFile.CreateFromDirectory(tempDir, outputPath, CompressionLevel.Optimal, false);

                return outputPath;
            }
            finally
            {
                // Clean up temporary directory
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        public string GenerateBackupFilename(string? customPath = null)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss", CultureInfo.InvariantCulture);
            var filename = $"TradingJournal_Backup_{timestamp}.zip";

            if (!string.IsNullOrWhiteSpace(customPath))
            {
                return Path.Combine(customPath, filename);
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename);
        }

        public async Task RestoreFromBackupAsync(string backupPath, bool overwriteExisting = false)
        {
            if (string.IsNullOrWhiteSpace(backupPath))
            {
                throw new ArgumentException("Backup path cannot be null or whitespace.", nameof(backupPath));
            }

            if (!File.Exists(backupPath))
            {
                throw new FileNotFoundException("Backup file not found.", backupPath);
            }

            if (!backupPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Backup file must be a ZIP archive.");
            }

            // Create temporary directory for extraction
            var tempDir = Path.Combine(Path.GetTempPath(), $"restore_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempDir);

            try
            {
                // Extract ZIP archive
                ZipFile.ExtractToDirectory(backupPath, tempDir);

                // Verify backup structure
                var metadataPath = Path.Combine(tempDir, "backup-info.json");
                if (!File.Exists(metadataPath))
                {
                    throw new InvalidOperationException("Invalid backup file: missing backup-info.json");
                }

                var dbPath = Path.Combine(tempDir, "trading.db");
                if (!File.Exists(dbPath))
                {
                    throw new InvalidOperationException("Invalid backup file: missing database file");
                }

                // Check if database exists and overwrite flag
                // TODO: Implement actual database restoration logic

                await Task.CompletedTask;
            }
            finally
            {
                // Clean up temporary directory
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        public async Task<ImportResult> ImportFromCsvAsync(string csvPath, CsvMappingProfile mappingProfile)
        {
            if (string.IsNullOrWhiteSpace(csvPath))
            {
                throw new ArgumentException("CSV path cannot be null or whitespace.", nameof(csvPath));
            }

            if (mappingProfile == null)
            {
                throw new ArgumentNullException(nameof(mappingProfile));
            }

            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("CSV file not found.", csvPath);
            }

            var result = new ImportResult
            {
                TotalRows = 0,
                SuccessCount = 0,
                ErrorCount = 0,
                Errors = new List<string>()
            };

            var lines = await File.ReadAllLinesAsync(csvPath);

            // Skip header if present
            var startIndex = mappingProfile.HasHeader ? 1 : 0;
            result.TotalRows = lines.Length - startIndex;

            for (int i = startIndex; i < lines.Length; i++)
            {
                try
                {
                    var fields = ParseCsvLine(lines[i]);

                    var trade = new Trade
                    {
                        Symbol = GetFieldValue(fields, mappingProfile.SymbolColumn),
                        Direction = ParseDirection(GetFieldValue(fields, mappingProfile.DirectionColumn)),
                        EntryDateTime = DateTime.Parse(GetFieldValue(fields, mappingProfile.EntryDateColumn), CultureInfo.InvariantCulture),
                        EntryPrice = decimal.Parse(GetFieldValue(fields, mappingProfile.EntryPriceColumn), CultureInfo.InvariantCulture),
                        PositionSize = decimal.Parse(GetFieldValue(fields, mappingProfile.SharesColumn), CultureInfo.InvariantCulture)
                    };

                    _context.Trades.Add(trade);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Errors.Add($"Row {i + 1}: {ex.Message}");
                }
            }

            if (result.SuccessCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return result;
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return field;
            }

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }

        private static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++; // Skip next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            fields.Add(currentField.ToString());
            return fields.ToArray();
        }

        private static string GetFieldValue(string[] fields, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= fields.Length)
            {
                return string.Empty;
            }

            return fields[columnIndex];
        }

        private static TradeDirection ParseDirection(string direction)
        {
            return direction.ToLower() switch
            {
                "long" => TradeDirection.Long,
                "short" => TradeDirection.Short,
                _ => throw new ArgumentException($"Invalid direction: {direction}")
            };
        }
    }
}
