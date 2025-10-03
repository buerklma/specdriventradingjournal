# Research: Swing Trading Journal App

**Feature**: 001-swing-trading-journal  
**Date**: 2025-10-03  
**Purpose**: Research and decision documentation for technology choices and architectural patterns

---

## Technology Stack Decisions

### 1. Application Framework: .NET MAUI

**Decision**: Use .NET MAUI (Multi-platform App UI) for cross-platform desktop and potential mobile support

**Rationale**:
- Native Windows desktop performance with WinUI 3 backend
- Single codebase for future Android/iOS/macOS expansion
- Strong MVVM support with data binding
- Access to full .NET 8 ecosystem
- Excellent Visual Studio tooling and debugging
- Community and Microsoft support

**Alternatives Considered**:
- **WPF**: Windows-only, no mobile path, older technology
- **Avalonia**: Less mature, smaller ecosystem
- **Electron/Web**: Performance concerns, larger memory footprint, less native feel

### 2. Database: SQLite with Entity Framework Core

**Decision**: Use SQLite database with EF Core ORM for local data storage

**Rationale**:
- Serverless, zero-configuration embedded database
- Perfect for offline-first single-user applications
- EF Core provides type-safe LINQ queries and migrations
- Excellent performance for 10,000+ records
- File-based storage easy to backup
- Support for encryption via SQLCipher extension

**Alternatives Considered**:
- **LiteDB**: C#-native but less mature, smaller ecosystem
- **Realm**: More complex, overkill for this use case
- **JSON files**: Poor query performance, no relational integrity

### 3. Charting Library: Syncfusion MAUI Charts

**Decision**: Use Syncfusion Community License for MAUI Charts (free for companies <$1M revenue)

**Rationale**:
- Professional chart components (line, bar, pie, scatter)
- Smooth animations and 60fps performance
- Touch and mouse interaction support
- Responsive and customizable
- Dark/light theme support
- Free community license available

**Alternatives Considered**:
- **Microcharts**: Simpler but limited customization
- **LiveCharts**: Not yet stable for MAUI
- **OxyPlot**: Limited MAUI support
- **Custom D3.js in WebView**: Performance overhead, complexity

### 4. PDF Export: Syncfusion PDF or QuestPDF

**Decision**: Use QuestPDF for report generation (MIT license, fully free)

**Rationale**:
- Fluent API for document composition
- High-quality output with charts and tables
- MIT license, completely free
- Active development and community
- Works cross-platform
- Good documentation

**Alternatives Considered**:
- **Syncfusion PDF**: Powerful but requires license
- **iTextSharp**: Complex API, AGPL license
- **PdfSharp**: Limited .NET 6+ support

### 5. Testing Framework: xUnit + FluentAssertions

**Decision**: Use xUnit for unit/integration tests with FluentAssertions for readable assertions

**Rationale**:
- Industry-standard .NET testing framework
- Excellent Visual Studio Test Explorer integration
- Parallel test execution
- FluentAssertions makes tests more readable
- Wide community adoption

**Alternatives Considered**:
- **NUnit**: Equally good, xUnit slightly more modern
- **MSTest**: Less features, older API

---

## Architectural Patterns

### 1. MVVM (Model-View-ViewModel)

**Decision**: Implement strict MVVM pattern with `CommunityToolkit.Mvvm` (MVVM Toolkit)

**Rationale**:
- Native to MAUI and XAML-based frameworks
- Clear separation of concerns
- Testable ViewModels without UI dependencies
- MVVM Toolkit provides source generators for boilerplate reduction
- Supports two-way data binding and commanding

**Pattern Structure**:
- **Models**: Entity classes (Trade, ManagementAdjustment, etc.)
- **ViewModels**: Presentation logic, commands, observable properties
- **Views**: XAML UI with data binding to ViewModels
- **Services**: Business logic and data access

### 2. Dependency Injection

**Decision**: Use built-in .NET DI container configured in `MauiProgram.cs`

**Rationale**:
- Native to .NET, no additional dependencies
- Service lifetime management (Singleton, Transient, Scoped)
- Constructor injection for testability
- MAUI MauiProgram initialization hook

**Service Registration Strategy**:
```csharp
builder.Services.AddSingleton<ITradingJournalContext, TradingJournalContext>();
builder.Services.AddTransient<ITradeService, TradeService>();
builder.Services.AddTransient<IAnalyticsService, AnalyticsService>();
builder.Services.AddTransient<TradeListViewModel>();
```

### 3. Repository Pattern (Simplified)

**Decision**: Services directly use EF Core DbContext without separate repository layer

**Rationale**:
- EF Core DbContext already implements Unit of Work and Repository patterns
- Avoiding over-abstraction for single-database application
- Simpler code, easier to test with In-Memory provider
- Can refactor to repositories later if needed

**Alternatives Considered**:
- **Generic Repository**: Over-engineered for this scale
- **CQRS**: Overkill for CRUD-focused application

---

## Data Storage Strategy

### Local Storage Locations

**Decision**: Use platform-specific standard paths

- **Database**: `%LOCALAPPDATA%\TradingJournal\tradingjour nal.db`
- **Screenshots**: `%USERPROFILE%\Documents\TradingJournal\Screenshots\`
- **Backups**: `%USERPROFILE%\Documents\TradingJournal\Backups\`
- **Exports**: User-selected location via file picker

**Rationale**:
- LOCALAPPDATA for app-specific data (hidden from users)
- Documents folder for user-accessible files
- Follows Windows conventions
- Easy to locate for manual backup

### Database Encryption

**Decision**: Implement optional SQLite encryption using SQLCipher NuGet package

**Rationale**:
- Financial data requires encryption at rest
- SQLCipher is industry-standard for SQLite encryption
- Transparent to EF Core after connection string configuration
- Minimal performance overhead

**Configuration**:
```csharp
connectionString = $"Data Source={dbPath};Password={userPassword};"
```

---

## Performance Optimization Strategies

### 1. Database Indexing

**Decision**: Create indexes on frequently queried columns

**Indexes to Create**:
- `Trade.Symbol` (filter by ticker)
- `Trade.EntryDateTime` (date range queries)
- `Trade.SetupType` (filter by strategy)
- `Trade.UserId` (if multi-user support added)
- Composite index on `(Symbol, EntryDateTime)`

### 2. Lazy Loading and Pagination

**Decision**: Implement virtualization for trade lists

**Strategy**:
- Use `CollectionView` with item templating
- Load trades in pages of 100
- Eager load navigation properties with `.Include()` when needed
- Avoid loading screenshots until detail view opened

### 3. Background Tasks

**Decision**: Use `Task.Run()` for heavy calculations (analytics, exports)

**Rationale**:
- Keeps UI thread responsive
- Show loading indicators during background work
- Cancel tokens for user cancellation

---

## Security & Privacy

### 1. Authentication

**Decision**: Optional PIN or Windows Hello biometric authentication on app launch

**Implementation**:
- Use `Microsoft.Maui.Authentication` for Windows Hello
- Store PIN hash in secure storage (`SecureStorage` API)
- Encrypt database with user's authentication credential

### 2. Data Privacy

**Decision**: No telemetry, no cloud storage by default, completely offline

**Rationale**:
- Financial data is highly sensitive
- Users must explicitly opt-in to cloud features (future)
- Comply with data protection regulations (GDPR, etc.)

---

## UI/UX Design Decisions

### 1. Theme Support

**Decision**: Implement dark and light themes using MAUI Resource Dictionaries

**Theme Colors**:
- **Light Theme**: White background, dark text, accent blue (#0078D4)
- **Dark Theme**: Dark gray background (#1E1E1E), light text, accent light blue (#60CDFF)
- Follow Windows 11 design language

### 2. Navigation Structure

**Decision**: Use Shell with FlyoutMenu for desktop navigation

**Menu Structure**:
- Dashboard (home, overview charts)
- Trades (list view with filters)
- New Trade (quick entry form)
- Psychology Log (emotion tracking)
- Review (quality ratings, lessons)
- Analytics (detailed statistics)
- Export & Backup
- Settings

### 3. Keyboard Shortcuts

**Decision**: Implement accelerator keys for common actions

**Shortcuts**:
- `Ctrl+N`: New trade
- `Ctrl+S`: Save current trade
- `Ctrl+F`: Focus search/filter
- `Ctrl+E`: Export
- `Ctrl+,`: Settings
- `F5`: Refresh analytics

---

## Testing Strategy

### 1. Unit Tests (90%+ coverage target)

**Scope**:
- Calculation methods (R/R ratio, profit/loss, statistics)
- Validation logic
- Service layer business logic
- ViewModel command handlers

**Tools**: xUnit, FluentAssertions, Moq for mocking

### 2. Integration Tests

**Scope**:
- Database operations (CRUD)
- Service workflows (create trade → calculate → save)
- Export functionality

**Tools**: xUnit with EF Core In-Memory provider

### 3. UI Tests

**Scope**:
- Critical user workflows (trade entry, view analytics)
- Theme switching
- Navigation flows

**Tools**: Appium with WinAppDriver or MAUI UITest (when stable)

---

## Future Extensibility

### Cloud Sync (Phase 2)

**Research Notes**:
- Use REST API (ASP.NET Core) or Supabase
- Conflict resolution strategy: last-write-wins with timestamps
- Store screenshots in blob storage (Azure Blob, Supabase Storage)
- Implement sync queue with retry logic

### Broker Import (Phase 3)

**Research Notes**:
- CSV parsing with CsvHelper library
- Map broker columns to Trade entities
- Support Interactive Brokers, TD Ameritrade, Schwab CSV formats
- Allow custom column mapping

### AI Insights (Phase 4)

**Research Notes**:
- Use ML.NET for local pattern recognition
- Analyze psychology notes for emotional patterns
- Identify common mistakes across losing trades
- Suggest optimal R/R ratios based on historical data

---

## Summary

All technology choices prioritize:
1. **Offline-first** architecture with local data storage
2. **Performance** targets (<3s load, 60fps UI, handles 10K+ trades)
3. **Privacy** with encryption and no external data transmission
4. **Testability** with TDD-friendly DI and service architecture
5. **Extensibility** for future cloud sync and mobile platforms

No major risks or blockers identified. All dependencies are mature, well-documented, and have active communities.

---

**Status**: Research complete ✅  
**Next Phase**: Phase 1 (Design & Contracts)
