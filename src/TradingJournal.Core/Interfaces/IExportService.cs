using System.Threading.Tasks;
using TradingJournal.Core.Models;

namespace TradingJournal.Core.Interfaces
{
    /// <summary>
    /// Service interface for exporting trade data and managing backups.
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// Exports trades to CSV format.
        /// </summary>
        Task<string> ExportToCsvAsync(string outputPath, bool includeHeader = true);

        /// <summary>
        /// Exports trades to Excel format with multiple sheets.
        /// </summary>
        Task<string> ExportToExcelAsync(string outputPath, bool includeCharts = false);

        /// <summary>
        /// Exports trades to PDF report format.
        /// </summary>
        Task<string> ExportToPdfAsync(string outputPath, bool includeSummary = true, bool includeCharts = true);

        /// <summary>
        /// Creates a backup ZIP file containing database and screenshots.
        /// </summary>
        Task<string> CreateBackupAsync(string outputPath, bool includeScreenshots = true);

        /// <summary>
        /// Generates a timestamped backup filename.
        /// </summary>
        string GenerateBackupFilename(string? customPath = null);

        /// <summary>
        /// Restores data from a backup ZIP file.
        /// </summary>
        Task RestoreFromBackupAsync(string backupPath, bool overwriteExisting = false);

        /// <summary>
        /// Imports trades from a CSV file.
        /// </summary>
        Task<ImportResult> ImportFromCsvAsync(string csvPath, CsvMappingProfile mappingProfile);
    }
}
