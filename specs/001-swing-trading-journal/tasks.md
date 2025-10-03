# Tasks: Swing Trading Journal App

**Feature**: 001-swing-trading-journal
**Input**: Design documents from `/specs/001-swing-trading-journal/`
**Prerequisites**: plan.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅, quickstart.md ✅

---

## Phase 3.1: Setup & Infrastructure

### Project Structure
- [] **T001** Create solution structure: `src/TradingJournal/`, `src/TradingJournal.Core/`, `tests/TradingJournal.UnitTests/`, `tests/TradingJournal.IntegrationTests/`
- [x] **T002** Initialize .NET MAUI Windows Desktop project with .NET 8.0 SDK targeting `net8.0-windows10.0.19041.0`
- [x] **T003** [P] Create folder structure: `Models/`, `ViewModels/`, `Views/`, `Services/`, `Data/`, `Helpers/`, `Resources/`, `Platforms/Windows/` in `src/TradingJournal/`
- [x] **T004** [P] Create Core library structure: `Calculations/`, `Validators/` in `src/TradingJournal.Core/`

### Dependencies & Configuration
- [x] **T005** [P] Install NuGet packages: `Microsoft.EntityFrameworkCore.Sqlite` (v8.0.0), `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools` in TradingJournal project
- [x] **T006** [P] Install testing packages: `xUnit` (v2.6.2) using core `Assert`, `NSubstitute` (v5.0.0), `Microsoft.EntityFrameworkCore.InMemory` in test projects
- [x] **T007** [P] Install Syncfusion MAUI Charts: `Syncfusion.Maui.Charts` (v24.1.41, Community License) in TradingJournal project
- [x] **T008** [P] Install QuestPDF: `QuestPDF` (v2023.12.3) for PDF generation in TradingJournal project
- [x] **T009** [P] Install CommunityToolkit packages: `CommunityToolkit.Mvvm` (v8.2.2), `CommunityToolkit.Maui` (v7.0.0) in TradingJournal project

### Quality Gates & Tooling
- [x] **T010** [P] Configure `.editorconfig` with C# formatting rules: 4-space indents, `var` preferences, brace placement, naming conventions
- [x] **T011** [P] Setup StyleCop.Analyzers: Install `StyleCop.Analyzers` NuGet, create `stylecop.json` with rule customizations (disable SA1633 file headers)
- [x] **T012** [P] Configure `Directory.Build.props`: Enable nullable reference types, warnings as errors, TreatWarningsAsErrors=true, Code Analysis level 8
- [x] **T013** [P] Create `.gitignore` additions: `*.db`, `*.db-shm`, `*.db-wal`, `bin/`, `obj/`, `*.user`, `.vs/`, `Screenshots/`

---

## Phase 3.2: Tests First - Data Layer (TDD) ⚠️ MUST COMPLETE BEFORE PHASE 3.3

### Entity Tests
 [x] **T014** [P] Entity test: `TradeTests.cs` in `tests/TradingJournal.UnitTests/Models/` - Test Trade entity instantiation, property setters, validation attributes
 [x] **T015** [P] Entity test: `TradeBusinessRulesTests.cs` - Test Long direction rules (StopLoss < EntryPrice < TakeProfit), Short direction rules, ExitDateTime > EntryDateTime validation
 [x] **T016** [P] Entity test: `ManagementAdjustmentTests.cs` - Test adjustment entity, foreign key relationship, AdjustmentType enum validation
 [x] **T017** [P] Entity test: `AttachmentTests.cs` - Test attachment entity, file size validation (< 10MB), allowed file types (.png, .jpg, .pdf, .txt)
 [x] **T018** [P] Entity test: `StrategyCategoryTests.cs` - Test category entity, unique name constraint, IsSystemDefined logic
 [x] **T019** [P] Entity test: `UserPreferencesTests.cs` - Test singleton pattern, Theme enum, default values, CustomFieldDefinitions JSON validation

### Calculation Tests
 [x] **T020** [P] Calculation test: `RiskRewardCalculatorTests.cs` in `tests/TradingJournal.UnitTests/Calculations/` - Test PlannedRR for Long trades: (TP - Entry) / (Entry - SL)
 [x] **T021** [P] Calculation test: `RiskRewardCalculatorTests_Short.cs` - Test PlannedRR for Short trades: (Entry - TP) / (SL - Entry)
 [x] **T022** [P] Calculation test: `RiskRewardCalculatorTests_Realized.cs` - Test RealizedRR with ExitPrice for wins, losses, break-even scenarios
 [x] **T023** [P] Calculation test: `ProfitLossCalculatorTests.cs` - Test P/L in currency: (ExitPrice - EntryPrice) * PositionSize for Long, inverse for Short
 [x] **T024** [P] Calculation test: `ProfitLossInRCalculatorTests.cs` - Test P/L in R units: ProfitLossCurrency / RiskAmount
 [x] **T025** [P] Calculation test: `HoldingTimeCalculatorTests.cs` - Test TimeSpan calculation: ExitDateTime - EntryDateTime

### Validation Tests
- [x] **T026** [P] Validation test: `TradeValidatorTests.cs` in `tests/TradingJournal.UnitTests/Validators/` - Test Symbol required and max 20 chars, prices > 0, RiskPercentage 0.1-100 range
- [x] **T027** [P] Validation test: `TradeValidatorTests_CrossField.cs` - Test ExitDateTime > EntryDateTime, DisciplineScore 1-10 if set, QualityRating 1-10 if set
- [x] **T028** [P] Validation test: `AttachmentValidatorTests.cs` - Test file size < 10MB, file type in allowed list, storage path format validation

### DbContext Tests
- [x] **T029** [P] DbContext test: `TradingDbContextTests.cs` in `tests/TradingJournal.IntegrationTests/Data/` - Test DbContext instantiation with SQLite in-memory, DbSet properties exist
- [x] **T030** [P] DbContext test: `TradingDbContextConfigurationTests.cs` - Test entity configurations: indexes on Trade.Symbol, Trade.EntryDateTime, Trade.SetupType, composite index
- [x] **T031** [P] DbContext test: `RelationshipTests.cs` - Test 1:N Trade→ManagementAdjustment navigation, 1:N Trade→Attachment navigation, cascade delete behavior
- [x] **T032** [P] DbContext test: `SeedDataTests.cs` - Test default StrategyCategory records seeded (Breakout, Pullback, Reversal, Trend Continuation), UserPreferences singleton created

### Migration Tests
- [x] **T033** [P] Migration test: `InitialMigrationTests.cs` - Test migration creates all tables: Trades, ManagementAdjustments, Attachments, StrategyCategories, UserPreferences
- [x] **T034** [P] Migration test: `IndexCreationTests.cs` - Test all indexes created: IX_Trade_Symbol, IX_Trade_EntryDateTime, IX_Trade_SetupType, IX_Trade_Symbol_EntryDateTime

---

## Phase 3.3: Tests First - Service Layer (TDD) ⚠️ MUST COMPLETE BEFORE PHASE 3.4

### ITradeService Contract Tests
- [ ] **T035** [P] Contract test: `TradeService_CreateTests.cs` in `tests/TradingJournal.UnitTests/Services/` - Test CreateTradeAsync generates Id, sets timestamps, calculates PlannedRR
- [ ] **T036** [P] Contract test: `TradeService_CreateValidationTests.cs` - Test CreateTradeAsync throws ValidationException for: empty Symbol, negative prices, invalid RiskPercentage
- [ ] **T037** [P] Contract test: `TradeService_GetByIdTests.cs` - Test GetTradeByIdAsync returns trade with navigation properties loaded, returns null for non-existent Id
- [ ] **T038** [P] Contract test: `TradeService_GetAllTests.cs` - Test GetAllTradesAsync returns all trades, ordered by EntryDateTime descending
- [ ] **T039** [P] Contract test: `TradeService_GetBySymbolTests.cs` - Test GetTradesBySymbolAsync filters correctly, case-insensitive symbol matching
- [ ] **T040** [P] Contract test: `TradeService_GetByDateRangeTests.cs` - Test GetTradesByDateRangeAsync filters by EntryDateTime within range, inclusive bounds
- [ ] **T041** [P] Contract test: `TradeService_GetPagedTests.cs` - Test GetTradesPagedAsync returns correct page size, calculates TotalPages correctly, HasNextPage/HasPreviousPage flags
- [ ] **T042** [P] Contract test: `TradeService_GetPagedFilterTests.cs` - Test GetTradesPagedAsync with TradeFilter: Symbol, DateRange, SetupType, Direction, IsOpen, IsProfitable filters
- [ ] **T043** [P] Contract test: `TradeService_UpdateTests.cs` - Test UpdateTradeAsync updates properties, updates UpdatedAt timestamp, returns updated trade
- [ ] **T044** [P] Contract test: `TradeService_UpdateExitTests.cs` - Test UpdateTradeExitAsync sets ExitPrice/ExitDateTime, calculates RealizedRR, ProfitLossCurrency, ProfitLossR, HoldingTime
- [ ] **T045** [P] Contract test: `TradeService_DeleteTests.cs` - Test DeleteTradeAsync removes trade, returns false for non-existent Id, cascade deletes adjustments and attachments
- [ ] **T046** [P] Contract test: `TradeService_AddAdjustmentTests.cs` - Test AddAdjustmentAsync creates adjustment linked to trade, generates Id, validates adjustment datetime within trade lifetime
- [ ] **T047** [P] Contract test: `TradeService_GetAdjustmentsTests.cs` - Test GetAdjustmentsForTradeAsync returns all adjustments for trade, ordered by AdjustmentDateTime
- [ ] **T048** [P] Contract test: `TradeService_AddAttachmentTests.cs` - Test AddAttachmentAsync copies file to storage, creates attachment record, populates FileSize/FileType
- [ ] **T049** [P] Contract test: `TradeService_GetAttachmentsTests.cs` - Test GetAttachmentsForTradeAsync returns all attachments, ordered by UploadedAt
- [ ] **T050** [P] Contract test: `TradeService_DeleteAttachmentTests.cs` - Test DeleteAttachmentAsync removes file from storage and database record

### IAnalyticsService Contract Tests
- [ ] **T051** [P] Contract test: `AnalyticsService_OverallStatisticsTests.cs` in `tests/TradingJournal.UnitTests/Services/` - Test GetOverallStatisticsAsync calculates TotalTrades, WinningTrades, LosingTrades, WinRate correctly
- [ ] **T052** [P] Contract test: `AnalyticsService_StatisticsBySetupTests.cs` - Test GetStatisticsBySetupTypeAsync filters by SetupType, calculates statistics only for matching trades
- [ ] **T053** [P] Contract test: `AnalyticsService_StatisticsBySymbolTests.cs` - Test GetStatisticsBySymbolAsync filters by Symbol, includes case-insensitive matching
- [ ] **T054** [P] Contract test: `AnalyticsService_WinRateTests.cs` - Test CalculateWinRateAsync formula: (WinningTrades / TotalTrades) * 100, returns 0 for no trades
- [ ] **T055** [P] Contract test: `AnalyticsService_AverageRRTests.cs` - Test CalculateAverageRRRatioAsync sums RealizedRR and divides by trade count, excludes open trades
- [ ] **T056** [P] Contract test: `AnalyticsService_ProfitFactorTests.cs` - Test CalculateProfitFactorAsync: GrossProfit / GrossLoss, returns 0 if no losses, handles edge cases
- [ ] **T057** [P] Contract test: `AnalyticsService_MaxDrawdownTests.cs` - Test CalculateMaxDrawdownAsync builds equity curve, finds peak-to-trough difference, calculates percentage
- [ ] **T058** [P] Contract test: `AnalyticsService_EquityCurveTests.cs` - Test GetEquityCurveDataAsync returns points ordered by date, cumulative P/L calculated correctly
- [ ] **T059** [P] Contract test: `AnalyticsService_RRDistributionTests.cs` - Test GetRRDistributionDataAsync buckets trades by RealizedRR: <-5, -5 to -4, ..., 4 to 5, >5
- [ ] **T060** [P] Contract test: `AnalyticsService_PerformanceBySetupTests.cs` - Test GetPerformanceBySetupDataAsync groups by SetupType, aggregates stats, orders by profitability
- [ ] **T061** [P] Contract test: `AnalyticsService_MonthlyPerformanceTests.cs` - Test GetMonthlyPerformanceDataAsync groups by month, calculates P/L per month for given year
- [ ] **T062** [P] Contract test: `AnalyticsService_EmotionFrequencyTests.cs` - Test GetEmotionFrequencyAsync counts unique emotions for Entry/During/Exit stages
- [ ] **T063** [P] Contract test: `AnalyticsService_DisciplineScoreTests.cs` - Test GetAverageDisciplineScoreAsync averages DisciplineScore field, excludes null values
- [ ] **T064** [P] Contract test: `AnalyticsService_DisciplineCorrelationTests.cs` - Test AnalyzeDisciplineCorrelationAsync groups by DisciplineScore 1-10, calculates avg RR and win rate per level
- [ ] **T065** [P] Contract test: `AnalyticsService_MistakesTests.cs` - Test GetTopMistakesAsync parses Mistakes field, counts frequency, returns top N
- [ ] **T066** [P] Contract test: `AnalyticsService_LessonsTests.cs` - Test GetTopLessonsAsync parses LessonsLearned field, counts frequency, returns top N

### IExportService Contract Tests
- [ ] **T067** [P] Contract test: `ExportService_CsvTests.cs` in `tests/TradingJournal.UnitTests/Services/` - Test ExportToCsvAsync creates file at outputPath, includes header row, escapes comma/quotes in data
- [ ] **T068** [P] Contract test: `ExportService_ExcelTests.cs` - Test ExportToExcelAsync creates .xlsx file, multiple sheets (Trades, Summary), conditional formatting (green profits, red losses)
- [ ] **T069** [P] Contract test: `ExportService_ExcelChartsTests.cs` - Test ExportToExcelAsync with includeCharts=true adds Charts sheet with embedded equity curve and R/R distribution
- [ ] **T070** [P] Contract test: `ExportService_PdfTests.cs` - Test ExportToPdfAsync creates PDF with cover page, summary page, charts, recent trades table, file size < 5MB
- [ ] **T071** [P] Contract test: `ExportService_BackupTests.cs` - Test CreateBackupAsync creates ZIP file, contains tradingjournal.db, Screenshots folder, backup-info.json
- [ ] **T072** [P] Contract test: `ExportService_BackupNamingTests.cs` - Test CreateBackupAsync generates filename: TradingJournal_Backup_YYYY-MM-DD_HHMMSS.zip
- [ ] **T073** [P] Contract test: `ExportService_RestoreTests.cs` - Test RestoreFromBackupAsync extracts ZIP, replaces database file, restores Screenshots folder
- [ ] **T074** [P] Contract test: `ExportService_RestoreValidationTests.cs` - Test RestoreFromBackupAsync throws InvalidBackupException for corrupted ZIP, throws InvalidOperationException if overwriteExisting=false and data exists
- [ ] **T075** [P] Contract test: `ExportService_ImportCsvTests.cs` - Test ImportFromCsvAsync maps columns via CsvMappingProfile, creates Trade records, returns ImportResult with counts
- [ ] **T076** [P] Contract test: `ExportService_ImportValidationTests.cs` - Test ImportFromCsvAsync skips invalid rows, populates Errors list in ImportResult, does not throw on malformed data

---

## Phase 3.4: Tests First - Integration Tests (TDD) ⚠️ MUST COMPLETE BEFORE PHASE 3.5

### Quickstart Scenario Tests
- [ ] **T077** [P] Integration test: `Scenario1_FirstTimeSetupTests.cs` in `tests/TradingJournal.IntegrationTests/Scenarios/` - Test create first trade workflow: NewTradeViewModel → TradeService.CreateTradeAsync → DB insert → verification
- [ ] **T078** [P] Integration test: `Scenario2_PsychologyAndExitTests.cs` - Test add psychology notes → exit trade → calculated fields updated (RealizedRR, P/L, HoldingTime)
- [ ] **T079** [P] Integration test: `Scenario3_QualityReviewTests.cs` - Test add quality rating → attach screenshot → verify attachment stored and retrievable
- [ ] **T080** [P] Integration test: `Scenario4_AnalyticsTests.cs` - Test create 3 trades (2 wins, 1 loss) → analytics service returns correct win rate (~66.67%), total P/L, equity curve
- [ ] **T081** [P] Integration test: `Scenario5_ExportBackupTests.cs` - Test export to CSV → verify file contents → create backup → verify ZIP contents
- [ ] **T082** [P] Integration test: `Scenario6_ThemeAndShortcutsTests.cs` - Test theme switching (Light/Dark/System) → verify UserPreferences updated → keyboard shortcuts trigger correct ViewModels

### Workflow Tests
- [ ] **T083** [P] Integration test: `FullTradeLifecycleTests.cs` - Test complete trade lifecycle: Create → Add psychology → Add adjustment → Exit → Add review → Attach screenshot → Export
- [ ] **T084** [P] Integration test: `ConcurrentTradeCreationTests.cs` - Test multiple trades created simultaneously, verify no database locking issues, verify all trades persisted
- [ ] **T085** [P] Integration test: `LargeDatasetPerformanceTests.cs` - Test create 1000 trades → analytics calculation < 2s, equity curve generation < 500ms, paging < 100ms per page
- [ ] **T086** [P] Integration test: `BackupRestoreCycleTests.cs` - Test full cycle: Create trades → Create backup → Delete all data → Restore from backup → Verify all data restored

---

## Phase 3.5: Core Implementation - Data Layer (ONLY after tests are failing)

### Entity Implementations
- [ ] **T087** [P] Implement Trade entity in `src/TradingJournal/Models/Trade.cs` with all 35+ properties, TradeDirection enum, navigation properties (Adjustments, Attachments)
- [ ] **T088** [P] Implement ManagementAdjustment entity in `src/TradingJournal/Models/ManagementAdjustment.cs` with AdjustmentType enum, foreign key to Trade
- [ ] **T089** [P] Implement Attachment entity in `src/TradingJournal/Models/Attachment.cs` with file metadata properties, StoragePath, Caption
- [ ] **T090** [P] Implement StrategyCategory entity in `src/TradingJournal/Models/StrategyCategory.cs` with Name, ColorHex, IsSystemDefined flag
- [ ] **T091** [P] Implement UserPreferences entity in `src/TradingJournal/Models/UserPreferences.cs` with ThemeMode enum, singleton pattern logic
- [ ] **T092** [P] Implement enums: TradeDirection, AdjustmentType, ThemeMode in `src/TradingJournal/Models/Enums.cs`

### Calculation Implementations
- [ ] **T093** [P] Implement RiskRewardCalculator in `src/TradingJournal.Core/Calculations/RiskRewardCalculator.cs` with static methods: CalculatePlannedRR, CalculateRealizedRR
- [ ] **T094** [P] Implement ProfitLossCalculator in `src/TradingJournal.Core/Calculations/ProfitLossCalculator.cs` with methods: CalculateProfitLossCurrency, CalculateProfitLossR
- [ ] **T095** [P] Implement HoldingTimeCalculator in `src/TradingJournal.Core/Calculations/HoldingTimeCalculator.cs` with method: CalculateHoldingTime (TimeSpan)

### Validation Implementations
- [ ] **T096** [P] Implement TradeValidator in `src/TradingJournal.Core/Validators/TradeValidator.cs` using FluentValidation: Symbol required, prices > 0, RiskPercentage 0.1-100, direction-specific SL/TP validation
- [ ] **T097** [P] Implement AttachmentValidator in `src/TradingJournal.Core/Validators/AttachmentValidator.cs`: File size < 10MB, allowed extensions, storage path format

### DbContext Implementation
- [ ] **T098** Implement TradingDbContext in `src/TradingJournal/Data/TradingDbContext.cs` with DbSet properties for all entities, constructor accepting DbContextOptions
- [ ] **T099** Add entity configurations in TradingDbContext.OnModelCreating: Trade indexes (Symbol, EntryDateTime, SetupType, composite), unique constraints, relationship configurations
- [ ] **T100** Configure cascade delete for Trade→ManagementAdjustment, Trade→Attachment relationships in TradingDbContext
- [ ] **T101** Implement seed data in TradingDbContext: Default StrategyCategory records (Breakout, Pullback, Reversal, Trend Continuation with colors), UserPreferences singleton with default values

### Migrations
- [ ] **T102** Generate initial migration: `dotnet ef migrations add InitialCreate --project src/TradingJournal --startup-project src/TradingJournal`
- [ ] **T103** Review migration file: Verify all tables created (Trades, ManagementAdjustments, Attachments, StrategyCategories, UserPreferences), all indexes added
- [ ] **T104** Apply migration to development database: `dotnet ef database update --project src/TradingJournal`

---

## Phase 3.6: Core Implementation - Service Layer (ONLY after data layer tests passing)

### TradeService Implementation
- [ ] **T105** Implement ITradeService interface in `src/TradingJournal/Services/TradeService.cs` with constructor injecting TradingDbContext
- [ ] **T106** Implement TradeService.CreateTradeAsync: Generate Id, set timestamps, calculate PlannedRR using calculator, validate via TradeValidator, insert into DbContext, SaveChangesAsync
- [ ] **T107** Implement TradeService.GetTradeByIdAsync: Query DbContext.Trades.Include(Adjustments).Include(Attachments).FirstOrDefaultAsync(id)
- [ ] **T108** Implement TradeService.GetAllTradesAsync: Return DbContext.Trades.OrderByDescending(EntryDateTime).ToListAsync()
- [ ] **T109** Implement TradeService.GetTradesBySymbolAsync: Filter DbContext.Trades.Where(Symbol == symbol, case-insensitive).ToListAsync()
- [ ] **T110** Implement TradeService.GetTradesByDateRangeAsync: Filter DbContext.Trades.Where(EntryDateTime between startDate and endDate).ToListAsync()
- [ ] **T111** Implement TradeService.GetTradesPagedAsync: Apply TradeFilter, calculate skip = (pageNumber - 1) * pageSize, return PagedResult with TotalCount from CountAsync
- [ ] **T112** Implement TradeService.UpdateTradeAsync: Update entity properties, set UpdatedAt = DateTime.UtcNow, SaveChangesAsync, return updated trade
- [ ] **T113** Implement TradeService.UpdateTradeExitAsync: Set ExitPrice/ExitDateTime, call calculator methods to set RealizedRR/ProfitLoss/HoldingTime, SaveChangesAsync
- [ ] **T114** Implement TradeService.DeleteTradeAsync: Remove trade from DbContext, SaveChangesAsync (cascade deletes handled by EF Core)
- [ ] **T115** Implement TradeService.AddAdjustmentAsync: Create ManagementAdjustment with generated Id, link to TradeId, validate datetime range, insert, SaveChangesAsync
- [ ] **T116** Implement TradeService.GetAdjustmentsForTradeAsync: Query DbContext.ManagementAdjustments.Where(TradeId == tradeId).OrderBy(AdjustmentDateTime).ToListAsync()
- [ ] **T117** Implement TradeService.AddAttachmentAsync: Copy file from filePath to %USERPROFILE%\Documents\TradingJournal\Screenshots\{tradeId}\{filename}, create Attachment record with metadata, SaveChangesAsync
- [ ] **T118** Implement TradeService.GetAttachmentsForTradeAsync: Query DbContext.Attachments.Where(TradeId == tradeId).OrderBy(UploadedAt).ToListAsync()
- [ ] **T119** Implement TradeService.DeleteAttachmentAsync: Retrieve attachment, delete file from filesystem using File.Delete(StoragePath), remove from DbContext, SaveChangesAsync
- [ ] **T120** Implement TradeService.ValidateTradeAsync: Instantiate TradeValidator, call ValidateAsync, return ValidationResult with IsValid and Errors list

### AnalyticsService Implementation
- [ ] **T121** Implement IAnalyticsService interface in `src/TradingJournal/Services/AnalyticsService.cs` with constructor injecting TradingDbContext
- [ ] **T122** Implement AnalyticsService.GetOverallStatisticsAsync: Query closed trades (ExitDateTime != null), calculate all TradingStatistics fields: WinRate, ProfitFactor, AvgRR, MaxDrawdown, etc.
- [ ] **T123** Implement AnalyticsService.GetStatisticsBySetupTypeAsync: Filter trades by SetupType, call GetOverallStatisticsAsync logic for filtered set
- [ ] **T124** Implement AnalyticsService.GetStatisticsBySymbolAsync: Filter trades by Symbol (case-insensitive), calculate statistics
- [ ] **T125** Implement AnalyticsService.CalculateWinRateAsync: Count winning trades (ProfitLossCurrency > 0), divide by total closed trades, multiply by 100
- [ ] **T126** Implement AnalyticsService.CalculateAverageRRRatioAsync: Sum RealizedRRRatio for all closed trades, divide by count
- [ ] **T127** Implement AnalyticsService.CalculateProfitFactorAsync: Sum gross profits (P/L > 0), sum absolute gross losses (P/L < 0), divide profits by losses
- [ ] **T128** Implement AnalyticsService.CalculateMaxDrawdownAsync: Build equity curve array, iterate to find peak-to-trough difference, calculate percentage, track start/end dates
- [ ] **T129** Implement AnalyticsService.GetEquityCurveDataAsync: Order trades by ExitDateTime, calculate cumulative P/L after each trade, return List<EquityCurvePoint>
- [ ] **T130** Implement AnalyticsService.GetRRDistributionDataAsync: Create buckets (<-5, -5 to -4, ..., >5), count trades in each bucket by RealizedRR, return List<RRDistributionBucket>
- [ ] **T131** Implement AnalyticsService.GetPerformanceBySetupDataAsync: Group trades by SetupType, calculate stats per group (TotalTrades, WinRate, AvgRR, Total P/L), order by profitability
- [ ] **T132** Implement AnalyticsService.GetMonthlyPerformanceDataAsync: Filter trades by year, group by month, sum P/L per month, return List<MonthlyPerformance>
- [ ] **T133** Implement AnalyticsService.GetEmotionFrequencyAsync: Extract emotion field based on EmotionStage, count unique occurrences, return Dictionary<string, int> ordered by frequency
- [ ] **T134** Implement AnalyticsService.GetAverageDisciplineScoreAsync: Filter trades with DisciplineScore != null, calculate average
- [ ] **T135** Implement AnalyticsService.AnalyzeDisciplineCorrelationAsync: Group trades by DisciplineScore (1-10), calculate avg RR and win rate per group, return List<DisciplineCorrelation>
- [ ] **T136** Implement AnalyticsService.GetTopMistakesAsync: Parse Trade.Mistakes field (split by comma/newline), count frequency, calculate avg P/L impact, return top N as List<MistakeFrequency>
- [ ] **T137** Implement AnalyticsService.GetTopLessonsAsync: Parse Trade.LessonsLearned field, count frequency, return top N as List<LessonFrequency>

### ExportService Implementation
- [ ] **T138** Implement IExportService interface in `src/TradingJournal/Services/ExportService.cs` with constructor injecting TradingDbContext and IConfiguration (for paths)
- [ ] **T139** Implement ExportService.ExportToCsvAsync: Use CsvHelper library, write header row, iterate trades and write rows with proper escaping, return outputPath
- [ ] **T140** Implement ExportService.ExportToExcelAsync: Use ClosedXML library, create Trades worksheet with data, create Summary worksheet with statistics
- [ ] **T141** Implement ExportService.ExportToExcelAsync chart support: If includeCharts=true, add Charts worksheet, use ClosedXML charting to embed equity curve and R/R distribution
- [ ] **T142** Implement ExportService.ExportToPdfAsync: Use QuestPDF, create Document with Cover page (title, date range), Summary page (statistics), Charts page (Syncfusion chart images), Recent Trades table, generate PDF
- [ ] **T143** Implement ExportService.CreateBackupAsync: Create ZIP using System.IO.Compression, add %LOCALAPPDATA%\TradingJournal\tradingjournal.db to ZIP, add Screenshots folder recursively
- [ ] **T144** Add backup-info.json to CreateBackupAsync: Serialize JSON with fields: BackupDate (DateTime.UtcNow), TradeCount, AppVersion (from Assembly), write to ZIP
- [ ] **T145** Implement ExportService.RestoreFromBackupAsync: Extract ZIP to temp folder, validate backup-info.json exists, copy tradingjour nal.db to %LOCALAPPDATA%\TradingJournal\, restore Screenshots folder
- [ ] **T146** Add overwriteExisting logic to RestoreFromBackupAsync: If false, check if database exists and has records, throw InvalidOperationException if data present
- [ ] **T147** Implement ExportService.ImportFromCsvAsync: Use CsvHelper with CsvMappingProfile, read CSV, map columns to Trade properties, validate each row via TradeValidator
- [ ] **T148** Add error handling to ImportFromCsvAsync: Catch validation errors per row, skip invalid rows, add error messages to ImportResult.Errors, continue processing

---

## Phase 3.7: Core Implementation - ViewModels (MVVM)

### Base ViewModel
- [ ] **T149** [P] Implement BaseViewModel in `src/TradingJournal/ViewModels/BaseViewModel.cs` inheriting from ObservableObject (CommunityToolkit.Mvvm), add IsBusy property, Title property

### Main ViewModels
- [ ] **T150** [P] Implement DashboardViewModel in `src/TradingJournal/ViewModels/DashboardViewModel.cs` with properties: TotalTrades, WinRate, TotalProfitLoss, LoadStatisticsCommand (calls IAnalyticsService.GetOverallStatisticsAsync)
- [ ] **T151** [P] Implement TradesListViewModel in `src/TradingJournal/ViewModels/TradesListViewModel.cs` with properties: ObservableCollection<Trade> Trades, TradeFilter CurrentFilter, LoadTradesPagedCommand, NavigateToTradeDetailCommand
- [ ] **T152** [P] Implement TradeDetailViewModel in `src/TradingJournal/ViewModels/TradeDetailViewModel.cs` with properties: Trade CurrentTrade, LoadTradeCommand(Guid id), SaveTradeCommand, DeleteTradeCommand
- [ ] **T153** [P] Implement NewTradeViewModel in `src/TradingJournal/ViewModels/NewTradeViewModel.cs` with properties: All Trade input fields, CalculatePlannedRRCommand (auto-updates PlannedRR on price changes), CreateTradeCommand (calls TradeService.CreateTradeAsync)
- [ ] **T154** [P] Implement PsychologyViewModel in `src/TradingJournal/ViewModels/PsychologyViewModel.cs` with properties: EmotionAtEntry, EmotionDuringTrade, EmotionAtExit, DisciplineScore, Notes, SavePsychologyCommand
- [ ] **T155** [P] Implement ReviewViewModel in `src/TradingJournal/ViewModels/ReviewViewModel.cs` with properties: QualityRating, Mistakes, LessonsLearned, SaveReviewCommand
- [ ] **T156** [P] Implement AnalyticsViewModel in `src/TradingJournal/ViewModels/AnalyticsViewModel.cs` with properties: EquityCurveData, RRDistributionData, PerformanceBySetupData, LoadAnalyticsCommand
- [ ] **T157** [P] Implement ExportViewModel in `src/TradingJournal/ViewModels/ExportViewModel.cs` with commands: ExportToCsvCommand, ExportToExcelCommand, ExportToPdfCommand, CreateBackupCommand, RestoreBackupCommand
- [ ] **T158** [P] Implement SettingsViewModel in `src/TradingJournal/ViewModels/SettingsViewModel.cs` with properties: Theme (bound to UserPreferences.Theme), DefaultCurrency, DefaultRiskPercentage, SaveSettingsCommand

### Additional ViewModels
- [ ] **T159** [P] Implement AttachmentsViewModel in `src/TradingJournal/ViewModels/AttachmentsViewModel.cs` with properties: ObservableCollection<Attachment> Attachments, AddAttachmentCommand (opens file picker), DeleteAttachmentCommand
- [ ] **T160** [P] Implement AdjustmentsViewModel in `src/TradingJournal/ViewModels/AdjustmentsViewModel.cs` with properties: ObservableCollection<ManagementAdjustment> Adjustments, AddAdjustmentCommand, LoadAdjustmentsCommand

---

## Phase 3.8: Core Implementation - Views (XAML)

### Main Views
- [ ] **T161** [P] Create DashboardView in `src/TradingJournal/Views/DashboardView.xaml` with layout: Grid with 4 summary cards (Total Trades, Win Rate, Total P/L, Max Drawdown), RefreshButton bound to LoadStatisticsCommand
- [ ] **T162** [P] Create TradesListView in `src/TradingJournal/Views/TradesListView.xaml` with CollectionView bound to Trades, filter UI (Entry fields for Symbol, Date Range, Setup Type), NewTradeButton
- [ ] **T163** [P] Create TradeDetailView in `src/TradingJournal/Views/TradeDetailView.xaml` with layout: ScrollView containing all trade details (read-only labels for closed trades, editable entries for open trades), EditButton, DeleteButton
- [ ] **T164** [P] Create NewTradeView in `src/TradingJournal/Views/NewTradeView.xaml` with form layout: Entry fields for all Trade properties, Picker for Direction, DatePicker + TimePicker for EntryDateTime, SaveButton bound to CreateTradeCommand, CancelButton
- [ ] **T165** [P] Create PsychologyView in `src/TradingJournal/Views/PsychologyView.xaml` embedded in TradeDetailView or separate tab: Entry fields for emotions (Entry/During/Exit), Slider for DisciplineScore (1-10), Editor for Notes
- [ ] **T166** [P] Create ReviewView in `src/TradingJournal/Views/ReviewView.xaml` embedded in TradeDetailView: Slider for QualityRating (1-10), Editor for Mistakes (multiline), Editor for LessonsLearned (multiline), SaveButton
- [ ] **T167** [P] Create AnalyticsView in `src/TradingJournal/Views/AnalyticsView.xaml` with layout: ScrollView containing Syncfusion charts: EquityCurve (LineSeries), RRDistribution (ColumnSeries), PerformanceBySetup (BarSeries), RefreshButton
- [ ] **T168** [P] Create ExportView in `src/TradingJournal/Views/ExportView.xaml` with button layout: ExportToCsvButton, ExportToExcelButton, ExportToPdfButton, CreateBackupButton, RestoreBackupButton, each bound to respective commands
- [ ] **T169** [P] Create SettingsView in `src/TradingJournal/Views/SettingsView.xaml` with form layout: Picker for Theme (Light/Dark/System), Entry for DefaultCurrency, Entry for DefaultRiskPercentage, SaveButton
- [ ] **T170** [P] Create MainPage (Shell) in `src/TradingJournal/AppShell.xaml` with FlyoutMenu: Dashboard, Trades, Analytics, Export, Settings menu items, define routes for navigation

### Supporting Views
- [ ] **T171** [P] Create AttachmentsView in `src/TradingJournal/Views/AttachmentsView.xaml` embedded in TradeDetailView: CollectionView of Attachment thumbnails, AddButton (opens file picker), DeleteButton per attachment
- [ ] **T172** [P] Create AdjustmentsView in `src/TradingJournal/Views/AdjustmentsView.xaml` embedded in TradeDetailView: ListView of ManagementAdjustment records, AddAdjustmentButton, shows AdjustmentType, DateTime, NewValue

---

## Phase 3.9: Integration - Wiring & DI

### Dependency Injection
- [ ] **T173** Configure dependency injection in `src/TradingJournal/MauiProgram.cs`: Register TradingDbContext as Scoped with SQLite connection string to %LOCALAPPDATA%\TradingJournal\tradingjour nal.db
- [ ] **T174** Register services in MauiProgram.cs: AddScoped<ITradeService, TradeService>, AddScoped<IAnalyticsService, AnalyticsService>, AddScoped<IExportService, ExportService>
- [ ] **T175** Register ViewModels in MauiProgram.cs: AddTransient<DashboardViewModel>, <TradesListViewModel>, <TradeDetailViewModel>, <NewTradeViewModel>, <AnalyticsViewModel>, <ExportViewModel>, <SettingsViewModel>
- [ ] **T176** Register Views in MauiProgram.cs: AddTransient<DashboardView>, <TradesListView>, <TradeDetailView>, <NewTradeView>, <AnalyticsView>, <ExportView>, <SettingsView>
- [ ] **T177** Add navigation routing in MauiProgram.cs: Register routes for all views using Routing.RegisterRoute

### Navigation
- [ ] **T178** Implement NavigateToTradeDetailCommand in TradesListViewModel: Use Shell.Current.GoToAsync with trade Id as query parameter
- [ ] **T179** Implement QueryProperty attribute in TradeDetailViewModel to receive trade Id from navigation, call LoadTradeCommand in OnAppearing
- [ ] **T180** Implement NavigateBackCommand in ViewModels: Use Shell.Current.GoToAsync("..")
- [ ] **T181** Test navigation flow: Dashboard → Trades → Trade Detail → Edit → Back to list

### Database Initialization
- [ ] **T182** Implement database initialization in `src/TradingJournal/App.xaml.cs` constructor: Ensure %LOCALAPPDATA%\TradingJournal\ directory exists, apply pending migrations using context.Database.MigrateAsync()
- [ ] **T183** Add error handling for database initialization: Catch MigrationException, show alert dialog to user with error message, prevent app from starting if migration fails

---

## Phase 3.10: Integration - Charts & PDF

### Syncfusion Charts Integration
- [ ] **T184** Configure Syncfusion license in `src/TradingJournal/MauiProgram.cs`: Call Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey) (use Community License key)
- [ ] **T185** Implement equity curve chart in AnalyticsView.xaml: Use SfCartesianChart with LineSeries, bind to EquityCurveData (DateTime X-axis, decimal Y-axis for cumulative P/L)
- [ ] **T186** Implement R/R distribution chart in AnalyticsView.xaml: Use SfCartesianChart with ColumnSeries, bind to RRDistributionData (string bucket labels X-axis, int trade count Y-axis)
- [ ] **T187** Implement performance by setup chart in AnalyticsView.xaml: Use SfCartesianChart with BarSeries, bind to PerformanceBySetupData (string setup type Y-axis, decimal total P/L X-axis)
- [ ] **T188** Style charts: Set chart titles, axis labels, legend, tooltip templates, color palettes (green for profits, red for losses)

### QuestPDF Integration
- [ ] **T189** Implement PDF document structure in ExportService.ExportToPdfAsync: Create QuestPDF Document with Page layout, add Cover page with Title "Trading Journal Report", date range, generation timestamp
- [ ] **T190** Add Summary page to PDF: Table with key statistics (Win Rate, Profit Factor, Total P/L, Max Drawdown, Avg RR), styled with bold headers
- [ ] **T191** Add Charts page to PDF: Capture Syncfusion chart controls as images (use chart.SaveAsImage or screenshot), embed images in PDF using QuestPDF.Image
- [ ] **T192** Add Recent Trades table to PDF: Table with columns: Symbol, Direction, Entry/Exit Dates, P/L Currency, P/L R, render last 10 trades
- [ ] **T193** Generate PDF file: Call document.GeneratePdf(outputPath), ensure file size < 5MB by optimizing chart image resolution

---

## Phase 3.11: Polish & Performance

### Performance Optimization
- [ ] **T194** [P] Optimize TradeService.GetTradesPagedAsync: Add AsNoTracking() for read-only queries, ensure indexes used (check query execution plan)
- [ ] **T195** [P] Optimize AnalyticsService queries: Use AsNoTracking(), batch calculations in memory instead of multiple DB round-trips, consider caching for GetOverallStatisticsAsync
- [ ] **T196** [P] Implement caching for analytics: Use MemoryCache with 5-minute expiry, cache key based on filter parameters, invalidate on trade create/update/delete
- [ ] **T197** [P] Optimize equity curve generation: Fetch only necessary fields (ExitDateTime, ProfitLossCurrency), project in LINQ, avoid loading navigation properties

### UI Performance
- [ ] **T198** [P] Implement virtualization in TradesListView: Ensure CollectionView uses ItemsUpdatingScrollMode.KeepLastItemInView, test with 1000+ trades
- [ ] **T199** [P] Add loading indicators: Show ActivityIndicator when IsBusy = true in ViewModels, apply to all long-running commands (LoadStatisticsCommand, LoadTradesCommand, etc.)
- [ ] **T200** [P] Implement debouncing for filter inputs: Use Task.Delay(300ms) in filter entry TextChanged event before triggering LoadTradesPagedCommand

### Error Handling
- [ ] **T201** [P] Implement global exception handling in `src/TradingJournal/App.xaml.cs`: Subscribe to AppDomain.CurrentDomain.UnhandledException, log errors, show user-friendly alert
- [ ] **T202** [P] Add try-catch blocks in all ViewModel commands: Wrap service calls in try-catch, set IsBusy = false in finally, show error alerts using DisplayAlert
- [ ] **T203** [P] Implement validation feedback in NewTradeView: Show error messages below invalid Entry fields, disable SaveButton if validation fails

### Accessibility
- [ ] **T204** [P] Add accessibility labels to all interactive controls in XAML: Set AutomationProperties.Name and AutomationProperties.HelpText for buttons, entries, pickers
- [ ] **T205** [P] Ensure keyboard navigation: Set TabIndex on all interactive controls, test Tab key navigation flow
- [ ] **T206** [P] Add high contrast theme support: Define HighContrast resource dictionary, override colors for Windows High Contrast mode

---

## Phase 3.12: Quality Gates & Validation

### Static Analysis
- [ ] **T207** Run StyleCop analysis: Execute `dotnet build` with TreatWarningsAsErrors=true, fix all SA warnings (document structure, naming, spacing)
- [ ] **T208** Run code coverage: Use `dotnet test --collect:"XPlat Code Coverage"`, verify coverage > 80% for Core and Services projects
- [ ] **T209** [P] Fix all compiler warnings: Set WarningLevel to 8, fix all CA (Code Analysis) warnings, nullable reference warnings

### Performance Validation
- [ ] **T210** Measure app load time: Launch app → first frame rendered, target < 3 seconds, use Stopwatch in App.xaml.cs constructor
- [ ] **T211** Measure trade entry time: Click New Trade → fill form → Save → redirect to list, target < 2 seconds total
- [ ] **T212** Measure analytics generation time: Create 10,000 test trades, run GetOverallStatisticsAsync, target < 2 seconds
- [ ] **T213** Measure UI frame rate: Use Visual Studio Performance Profiler, verify 60fps during scrolling in TradesListView with 1000+ items

### Manual Testing (from quickstart.md)
- [ ] **T214** Execute Test Scenario 1: First-Time Setup and Create Your First Trade, verify all ✅ checkpoints pass
- [ ] **T215** Execute Test Scenario 2: Log Psychology and Exit a Trade, verify calculated fields (RealizedRR, P/L, HoldingTime) correct
- [ ] **T216** Execute Test Scenario 3: Add Quality Review and Screenshot, verify attachment upload and thumbnail display
- [ ] **T217** Execute Test Scenario 4: Create Multiple Trades and View Analytics, verify win rate, equity curve, filters
- [ ] **T218** Execute Test Scenario 5: Export Data and Create Backup, verify CSV export, ZIP backup creation, backup contents
- [ ] **T219** Execute Test Scenario 6: Dark Mode and Keyboard Shortcuts, verify theme switching, Ctrl+N, Ctrl+F shortcuts

### Constitutional Compliance
- [ ] **T220** Verify Principle I: Code Quality - Run StyleCop, ensure 0 warnings, verify XML documentation on public APIs
- [ ] **T221** Verify Principle II: Test-First Development - Confirm all tests written before implementations, test count > 200
- [ ] **T222** Verify Principle III: UX Consistency - Review all Views for consistent spacing, font sizes, button styles, color palette adherence
- [ ] **T223** Verify Principle IV: Performance Requirements - Run all performance validation tasks (T210-T213), confirm targets met
- [ ] **T224** Verify Principle V: Spec-Driven - Confirm all functional requirements from spec.md implemented, all contracts honored

### Milestone Validation
- [ ] **T225** Create user-testable milestone validation document: Write `milestone-validation.md` with step-by-step instructions for non-technical users to validate app against quickstart scenarios

---

## Dependencies

### Critical Path (Blocking Dependencies)
- **T001-T013 (Setup)** must complete before any other work
- **T014-T034 (Data Layer Tests)** must complete before **T087-T104 (Data Layer Implementation)**
- **T035-T076 (Service Tests)** must complete before **T105-T148 (Service Implementation)**
- **T077-T086 (Integration Tests)** must complete before **T173-T183 (DI & Navigation)**
- **T087-T104 (Data Layer)** must complete before **T105-T148 (Service Layer)**
- **T105-T148 (Service Layer)** must complete before **T149-T160 (ViewModels)**
- **T149-T160 (ViewModels)** must complete before **T161-T172 (Views)**
- **T161-T172 (Views)** must complete before **T173-T183 (DI & Wiring)**
- **T173-T183 (DI & Wiring)** must complete before **T184-T193 (Charts & PDF)**
- All implementation must complete before **T194-T206 (Polish & Performance)**
- All polish must complete before **T207-T225 (Quality Gates & Validation)**

### Phase Dependencies
```
Phase 3.1 (Setup) → Phase 3.2 (Data Tests) → Phase 3.5 (Data Implementation)
Phase 3.3 (Service Tests) → Phase 3.6 (Service Implementation)
Phase 3.4 (Integration Tests) → Phase 3.9 (DI & Wiring)
Phase 3.7 (ViewModels) depends on Phase 3.6 (Services complete)
Phase 3.8 (Views) depends on Phase 3.7 (ViewModels complete)
Phase 3.9 (DI & Wiring) depends on Phases 3.6, 3.7, 3.8
Phase 3.10 (Charts & PDF) depends on Phase 3.9
Phase 3.11 (Polish) depends on Phase 3.10
Phase 3.12 (Quality Gates) depends on all previous phases
```

### Specific Task Dependencies
- T102 (Generate Migration) blocks T103 (Review Migration) blocks T104 (Apply Migration)
- T139 (CSV Export) requires CsvHelper NuGet package installation (add to T005-T009)
- T140-T141 (Excel Export) requires ClosedXML NuGet package (add to dependencies)
- T184 (Syncfusion License) blocks T185-T188 (Chart Implementation)
- T189 (PDF Document) blocks T190-T193 (PDF Pages)
- T207 (StyleCop) depends on T011 (StyleCop Config)
- T214-T219 (Manual Tests) depend on T173-T183 (App Runnable)

---

## Parallel Execution Examples

### Parallel Group 1: Setup Tasks (After T001-T002 Complete)
```bash
# Can run simultaneously (different files):
Task: T003 [P] Create folder structure
Task: T004 [P] Create Core library structure
Task: T005 [P] Install EF Core packages
Task: T006 [P] Install testing packages
Task: T007 [P] Install Syncfusion Charts
Task: T008 [P] Install QuestPDF
Task: T009 [P] Install CommunityToolkit packages
Task: T010 [P] Configure .editorconfig
Task: T011 [P] Setup StyleCop
Task: T012 [P] Configure Directory.Build.props
Task: T013 [P] Create .gitignore additions
```

### Parallel Group 2: Entity Tests (After T013 Complete)
```bash
# Can run simultaneously (separate test files):
Task: T014 [P] TradeTests.cs
Task: T015 [P] TradeBusinessRulesTests.cs
Task: T016 [P] ManagementAdjustmentTests.cs
Task: T017 [P] AttachmentTests.cs
Task: T018 [P] StrategyCategoryTests.cs
Task: T019 [P] UserPreferencesTests.cs
```

### Parallel Group 3: Calculation Tests (After T019 Complete)
```bash
Task: T020 [P] RiskRewardCalculatorTests.cs
Task: T021 [P] RiskRewardCalculatorTests_Short.cs
Task: T022 [P] RiskRewardCalculatorTests_Realized.cs
Task: T023 [P] ProfitLossCalculatorTests.cs
Task: T024 [P] ProfitLossInRCalculatorTests.cs
Task: T025 [P] HoldingTimeCalculatorTests.cs
```

### Parallel Group 4: Service Contract Tests (After T034 Complete)
```bash
# Split across 3 developers:
# Developer 1: TradeService tests T035-T050
# Developer 2: AnalyticsService tests T051-T066
# Developer 3: ExportService tests T067-T076
```

### Parallel Group 5: Entity Implementations (After T086 Complete)
```bash
Task: T087 [P] Trade entity
Task: T088 [P] ManagementAdjustment entity
Task: T089 [P] Attachment entity
Task: T090 [P] StrategyCategory entity
Task: T091 [P] UserPreferences entity
Task: T092 [P] Enums
Task: T093 [P] RiskRewardCalculator
Task: T094 [P] ProfitLossCalculator
Task: T095 [P] HoldingTimeCalculator
Task: T096 [P] TradeValidator
Task: T097 [P] AttachmentValidator
```

### Parallel Group 6: ViewModels (After T148 Complete)
```bash
Task: T150 [P] DashboardViewModel
Task: T151 [P] TradesListViewModel
Task: T152 [P] TradeDetailViewModel
Task: T153 [P] NewTradeViewModel
Task: T154 [P] PsychologyViewModel
Task: T155 [P] ReviewViewModel
Task: T156 [P] AnalyticsViewModel
Task: T157 [P] ExportViewModel
Task: T158 [P] SettingsViewModel
Task: T159 [P] AttachmentsViewModel
Task: T160 [P] AdjustmentsViewModel
```

### Parallel Group 7: Views (After T160 Complete)
```bash
Task: T161 [P] DashboardView.xaml
Task: T162 [P] TradesListView.xaml
Task: T163 [P] TradeDetailView.xaml
Task: T164 [P] NewTradeView.xaml
Task: T165 [P] PsychologyView.xaml
Task: T166 [P] ReviewView.xaml
Task: T167 [P] AnalyticsView.xaml
Task: T168 [P] ExportView.xaml
Task: T169 [P] SettingsView.xaml
Task: T171 [P] AttachmentsView.xaml
Task: T172 [P] AdjustmentsView.xaml
```

### Parallel Group 8: Polish & Performance (After T193 Complete)
```bash
Task: T194 [P] Optimize paged queries
Task: T195 [P] Optimize analytics queries
Task: T196 [P] Implement caching
Task: T197 [P] Optimize equity curve
Task: T198 [P] UI virtualization
Task: T199 [P] Loading indicators
Task: T200 [P] Debouncing filters
Task: T201 [P] Global exception handling
Task: T202 [P] ViewModel error handling
Task: T203 [P] Validation feedback
Task: T204 [P] Accessibility labels
Task: T205 [P] Keyboard navigation
Task: T206 [P] High contrast theme
```

---

## Validation Checklist

Before marking this task list complete, verify:

- [x] All ITradeService contract methods have corresponding tests (T035-T050: 16 test tasks)
- [x] All IAnalyticsService contract methods have corresponding tests (T051-T066: 16 test tasks)
- [x] All IExportService contract methods have corresponding tests (T067-T076: 10 test tasks)
- [x] All entities have model creation tasks (T087-T092: 6 entities + enums)
- [x] All calculation classes have implementation tasks (T093-T095: 3 calculators)
- [x] All validation classes have implementation tasks (T096-T097: 2 validators)
- [x] All tests come before implementation (Phase 3.2-3.4 before Phase 3.5-3.6)
- [x] Parallel tasks are truly independent (marked with [P], different files)
- [x] Each task specifies exact file path in description
- [x] No task modifies same file as another [P] task
- [x] All 6 quickstart scenarios have corresponding integration tests (T077-T082)
- [x] All 6 quickstart scenarios have manual validation tasks (T214-T219)
- [x] Performance requirements have validation tasks (T210-T213)
- [x] Constitutional principles have verification tasks (T220-T224)
- [x] Milestone validation document creation task exists (T225)

---

## Notes

- **Total Tasks**: 225 tasks
- **Estimated Duration**: 8-10 weeks for solo developer, 4-6 weeks for team of 3
- **Test-First Approach**: 86 test tasks (T014-T086, T077-T086) must complete before 93 implementation tasks (T087-T148, T105-T148)
- **Parallelization Potential**: ~120 tasks marked [P] for parallel execution across multiple developers
- **Critical Path Length**: ~80 sequential tasks (Setup → Data Tests → Data Impl → Service Tests → Service Impl → ViewModels → Views → DI → Charts → Polish → Quality Gates)
- **Quality Gates**: 19 validation tasks (T207-T225) at end ensure constitutional compliance
- **Performance Targets**: App load <3s, trade entry <2s, analytics <2s, 60fps UI (validated in T210-T213)
- **Testability**: All 6 quickstart scenarios have automated integration tests + manual validation procedures

---

**Status**: Task breakdown complete ✅
**Next Phase**: Implementation execution (manually or via automation tools)
**Constitution Compliance**: All 5 principles honored ✅
**Milestone Validation**: User-testable procedures defined ✅
