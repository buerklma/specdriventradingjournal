# Service Contract: IExportService

**Purpose**: Exports trade data to various formats (CSV, Excel, PDF) and manages backup/restore operations

---

## Interface Definition

```csharp
public interface IExportService
{
    // Export to Files
    Task<string> ExportToCsvAsync(List<Trade> trades, string outputPath);
    Task<string> ExportToExcelAsync(List<Trade> trades, string outputPath, bool includeCharts = false);
    Task<string> ExportToPdfAsync(TradingStatistics statistics, List<Trade> recentTrades, string outputPath);
    
    // Backup & Restore
    Task<string> CreateBackupAsync(string backupDirectory);
    Task<BackupInfo> GetBackupInfoAsync(string backupFilePath);
    Task RestoreFromBackupAsync(string backupFilePath, bool overwriteExisting = false);
    
    // Import
    Task<ImportResult> ImportFromCsvAsync(string csvFilePath, CsvMappingProfile mappingProfile);
}
```

---

## Method Contracts

### ExportToCsvAsync

**Input**: 
- `List<Trade> trades`
- `string outputPath` (full file path ending in .csv)

**Output**: `string` - path to created CSV file
**Side Effects**: Creates CSV file at outputPath

**CSV Structure**:
```
Symbol,Direction,EntryDate,EntryPrice,ExitDate,ExitPrice,StopLoss,TakeProfit,PositionSize,RiskPct,Setup,PlannedRR,RealizedRR,ProfitLossCurrency,ProfitLossR,HoldingTimeHours,DisciplineScore,QualityRating,Notes
AAPL,Long,2025-01-15 09:30,150.00,2025-01-16 14:00,153.00,148.50,155.00,100,2.0,Breakout,3.33,2.00,300.00,2.00,28.5,8,7,"Good entry on breakout"
```

**Pre-conditions**:
- outputPath is writable
- trades list is not null (can be empty)

**Post-conditions**:
- CSV file created with header row and data rows
- All decimal values formatted to 2 decimal places
- DateTime values in ISO 8601 format
- Notes field quoted and escaped properly

---

### ExportToExcelAsync

**Input**: 
- `List<Trade> trades`
- `string outputPath` (full file path ending in .xlsx)
- `bool includeCharts` (if true, adds charts sheet)

**Output**: `string` - path to created Excel file
**Side Effects**: Creates Excel file at outputPath

**Excel Structure**:
- **Sheet 1: Trades** - All trade data in table format with filters
- **Sheet 2: Summary** - Aggregate statistics
- **Sheet 3: Charts** (if includeCharts=true) - Embedded equity curve and R/R distribution charts

**Pre-conditions**:
- outputPath is writable
- trades list is not null

**Post-conditions**:
- Excel file created with formatted worksheets
- Table headers are bold
- Numeric columns are formatted as numbers/currency
- Date columns are formatted as dates
- Conditional formatting: Green for profits, Red for losses

---

### ExportToPdfAsync

**Input**: 
- `TradingStatistics statistics`
- `List<Trade> recentTrades` (e.g., last 10 trades)
- `string outputPath` (full file path ending in .pdf)

**Output**: `string` - path to created PDF file
**Side Effects**: Creates PDF report at outputPath

**PDF Structure**:
1. **Cover Page**: Title, Date Range, Generation Date
2. **Summary Page**: Key statistics (win rate, profit factor, total P/L, drawdown)
3. **Performance Charts**: Equity curve, R/R distribution, performance by setup
4. **Recent Trades Table**: Last N trades with key details
5. **Psychology Summary**: Emotion frequency, discipline correlation

**Pre-conditions**:
- outputPath is writable
- statistics is not null
- recentTrades is not null (can be empty)

**Post-conditions**:
- PDF file created with embedded charts and tables
- Professional formatting with consistent fonts and colors
- File size < 5 MB

---

### CreateBackupAsync

**Input**: `string backupDirectory` (where to save backup file)
**Output**: `string` - full path to created backup file
**Side Effects**: Creates ZIP file containing database and screenshots

**Backup Contents**:
- `tradingjour nal.db` (SQLite database file)
- `Screenshots/` folder (all trade attachments)
- `backup-info.json` (metadata: backup date, trade count, app version)

**Backup File Naming**: `TradingJournal_Backup_2025-10-03_143025.zip`

**Pre-conditions**:
- backupDirectory exists and is writable
- Database file is accessible
- No active database transactions (lock the DB briefly)

**Post-conditions**:
- ZIP file created in backupDirectory
- ZIP contains all files with relative paths preserved
- backup-info.json contains accurate metadata

---

### RestoreFromBackupAsync

**Input**: 
- `string backupFilePath` (path to backup ZIP file)
- `bool overwriteExisting` (if false, restore fails if data exists)

**Output**: None (throws on error)
**Side Effects**: 
- Replaces database file
- Replaces Screenshots folder
- Updates UserPreferences if needed

**Errors**:
- `FileNotFoundException` if backupFilePath invalid
- `InvalidBackupException` if backup corrupted or invalid format
- `InvalidOperationException` if overwriteExisting=false and data exists

**Pre-conditions**:
- backupFilePath is valid ZIP file
- ZIP contains required files (tradingjour nal.db, backup-info.json)
- No active database connections

**Post-conditions**:
- Database restored to backup state
- All screenshots restored
- Application requires restart to reload data

---

### ImportFromCsvAsync

**Input**: 
- `string csvFilePath`
- `CsvMappingProfile mappingProfile` (maps CSV columns to Trade properties)

**Output**: `ImportResult`:
```csharp
public class ImportResult
{
    public int TotalRows { get; set; }
    public int SuccessfulImports { get; set; }
    public int FailedImports { get; set; }
    public List<string> Errors { get; set; } = new();
}
```

**Side Effects**: Inserts trades into database

**CsvMappingProfile Example**:
```csharp
public class CsvMappingProfile
{
    public string SymbolColumn { get; set; } = "Symbol";
    public string EntryDateColumn { get; set; } = "Entry Date";
    public string EntryPriceColumn { get; set; } = "Entry Price";
    public string ExitDateColumn { get; set; } = "Exit Date";
    public string ExitPriceColumn { get; set; } = "Exit Price";
    // ... more column mappings
    public string DateFormat { get; set; } = "yyyy-MM-dd";
}
```

**Pre-conditions**:
- csvFilePath is valid and accessible
- CSV has header row
- mappingProfile columns exist in CSV

**Post-conditions**:
- Valid rows imported as Trade records
- Invalid rows skipped with error messages in result
- All imported trades have calculated fields set

---

## Performance Requirements

- `ExportToCsvAsync`: < 2s for 10,000 trades
- `ExportToExcelAsync`: < 5s for 10,000 trades with charts
- `ExportToPdfAsync`: < 3s with charts and tables
- `CreateBackupAsync`: < 10s for database + 1GB of screenshots
- `RestoreFromBackupAsync`: < 15s
- `ImportFromCsvAsync`: < 10s for 10,000 rows

---

## Testing Requirements

### Unit Tests
- Test CSV generation with various datasets
- Test Excel sheet creation and formatting
- Test PDF generation and layout
- Test backup file creation and extraction
- Test CSV import with valid and invalid data

### Integration Tests
- Test full export workflow (retrieve trades → export → verify file)
- Test backup and restore cycle (backup → delete data → restore → verify)
- Test import with real broker CSV files

### Contract Tests
- Verify all export methods create valid files
- Verify backup contains all required files
- Verify import handles malformed CSV gracefully

---

**Status**: IExportService contract defined ✅
