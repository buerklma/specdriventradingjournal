# Service Contract: ITradeService

**Purpose**: Manages trade CRUD operations, calculations, and queries

---

## Interface Definition

```csharp
public interface ITradeService
{
    // Create
    Task<Trade> CreateTradeAsync(Trade trade);
    
    // Read
    Task<Trade?> GetTradeByIdAsync(Guid id);
    Task<List<Trade>> GetAllTradesAsync();
    Task<List<Trade>> GetTradesBySymbolAsync(string symbol);
    Task<List<Trade>> GetTradesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<Trade>> GetTradesBySetupTypeAsync(string setupType);
    Task<PagedResult<Trade>> GetTradesPagedAsync(int pageNumber, int pageSize, TradeFilter? filter = null);
    
    // Update
    Task<Trade> UpdateTradeAsync(Trade trade);
    Task<bool> UpdateTradeExitAsync(Guid tradeId, decimal exitPrice, DateTime exitDateTime);
    
    // Delete
    Task<bool> DeleteTradeAsync(Guid id);
    Task<int> DeleteTradesBySymbolAsync(string symbol);
    
    // Management Adjustments
    Task<ManagementAdjustment> AddAdjustmentAsync(Guid tradeId, ManagementAdjustment adjustment);
    Task<List<ManagementAdjustment>> GetAdjustmentsForTradeAsync(Guid tradeId);
    
    // Attachments
    Task<Attachment> AddAttachmentAsync(Guid tradeId, string filePath, string? caption = null);
    Task<List<Attachment>> GetAttachmentsForTradeAsync(Guid tradeId);
    Task<bool> DeleteAttachmentAsync(Guid attachmentId);
    
    // Calculations
    Task RecalculateTradeMetricsAsync(Guid tradeId);
    decimal CalculatePlannedRR(decimal entryPrice, decimal stopLoss, decimal takeProfit, TradeDirection direction);
    decimal CalculateRealizedRR(decimal entryPrice, decimal exitPrice, decimal stopLoss, TradeDirection direction);
    decimal CalculateProfitLoss(decimal entryPrice, decimal exitPrice, decimal positionSize, TradeDirection direction);
    
    // Validation
    Task<ValidationResult> ValidateTradeAsync(Trade trade);
}
```

---

## Method Contracts

### CreateTradeAsync

**Input**: `Trade` object (without Id, CreatedAt, UpdatedAt)
**Output**: `Trade` object with generated Id and timestamps
**Side Effects**: Inserts record into database, calculates PlannedRRRatio
**Errors**: 
- `ArgumentNullException` if trade is null
- `ValidationException` if validation fails
- `DbUpdateException` if database error

**Pre-conditions**:
- Trade.Symbol must not be empty
- Trade.EntryPrice > 0
- Trade.StopLoss > 0
- Trade.TakeProfit > 0
- Trade.PositionSize > 0

**Post-conditions**:
- Trade.Id is generated (non-empty Guid)
- Trade.CreatedAt is set to current UTC time
- Trade.UpdatedAt is set to current UTC time
- Trade.PlannedRRRatio is calculated

---

### GetTradeByIdAsync

**Input**: `Guid id`
**Output**: `Trade?` (null if not found)
**Side Effects**: None (read-only)
**Errors**: None (returns null for not found)

**Pre-conditions**:
- id must not be Guid.Empty

**Post-conditions**:
- Returns Trade with all navigation properties loaded (Adjustments, Attachments)
- Or returns null if Id not found

---

### GetTradesPagedAsync

**Input**: 
- `int pageNumber` (1-based)
- `int pageSize` (e.g., 50, 100)
- `TradeFilter? filter` (optional filtering criteria)

**Output**: `PagedResult<Trade>` containing:
```csharp
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

**Side Effects**: None (read-only)
**Errors**: 
- `ArgumentOutOfRangeException` if pageNumber < 1 or pageSize < 1

**Pre-conditions**:
- pageNumber >= 1
- pageSize >= 1, <= 1000

**Post-conditions**:
- Returns up to pageSize trades
- Items are ordered by EntryDateTime descending (newest first)
- TotalCount reflects filtered result count

---

### UpdateTradeExitAsync

**Input**: 
- `Guid tradeId`
- `decimal exitPrice`
- `DateTime exitDateTime`

**Output**: `bool` (true if successful, false if trade not found)
**Side Effects**: 
- Updates ExitPrice, ExitDateTime
- Calculates and sets RealizedRRRatio, ProfitLossCurrency, ProfitLossR, HoldingTime
- Updates UpdatedAt timestamp

**Errors**: 
- `ArgumentException` if exitPrice <= 0
- `ValidationException` if exitDateTime <= Trade.EntryDateTime
- `DbUpdateException` if database error

**Pre-conditions**:
- Trade with tradeId exists
- exitPrice > 0
- exitDateTime > Trade.EntryDateTime

**Post-conditions**:
- Trade.ExitPrice is set
- Trade.ExitDateTime is set
- All calculated fields (RealizedRRRatio, etc.) are updated

---

### AddAdjustmentAsync

**Input**: 
- `Guid tradeId`
- `ManagementAdjustment adjustment` (without Id, TradeId)

**Output**: `ManagementAdjustment` with generated Id
**Side Effects**: Inserts adjustment record linked to Trade

**Errors**: 
- `ArgumentNullException` if adjustment is null
- `EntityNotFoundException` if tradeId not found
- `ValidationException` if adjustment datetime outside trade lifetime

**Pre-conditions**:
- Trade with tradeId exists
- adjustment.AdjustmentDateTime between Trade.EntryDateTime and Trade.ExitDateTime (or Now if still open)

**Post-conditions**:
- Adjustment.Id generated
- Adjustment.TradeId set to tradeId
- Adjustment stored in database

---

### AddAttachmentAsync

**Input**: 
- `Guid tradeId`
- `string filePath` (full path to source file)
- `string? caption`

**Output**: `Attachment` object with generated Id and storage information
**Side Effects**: 
- Copies file to `%USERPROFILE%\Documents\TradingJournal\Screenshots\{tradeId}\`
- Inserts Attachment record into database

**Errors**: 
- `FileNotFoundException` if filePath does not exist
- `ArgumentException` if file size > 10 MB
- `NotSupportedException` if file type not allowed
- `EntityNotFoundException` if tradeId not found

**Pre-conditions**:
- File at filePath exists
- File size <= 10 MB
- File extension in allowed list (.png, .jpg, .jpeg, .pdf, .txt)
- Trade with tradeId exists

**Post-conditions**:
- File copied to storage location
- Attachment record created with StoragePath set
- Attachment.FileName, FileType, FileSizeBytes populated

---

### CalculatePlannedRR

**Input**: 
- `decimal entryPrice`
- `decimal stopLoss`
- `decimal takeProfit`
- `TradeDirection direction`

**Output**: `decimal` planned R/R ratio
**Side Effects**: None (pure calculation)
**Errors**: 
- `ArgumentException` if inputs are invalid (e.g., negative prices)

**Formula**:
```csharp
if (direction == TradeDirection.Long)
    return (takeProfit - entryPrice) / (entryPrice - stopLoss);
else // Short
    return (entryPrice - takeProfit) / (stopLoss - entryPrice);
```

**Pre-conditions**:
- All prices > 0
- For Long: stopLoss < entryPrice < takeProfit
- For Short: takeProfit < entryPrice < stopLoss

**Post-conditions**:
- Returns positive decimal representing R multiple

---

### ValidateTradeAsync

**Input**: `Trade` object
**Output**: `ValidationResult` containing:
```csharp
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}
```

**Side Effects**: None (validation only)
**Errors**: None (errors returned in ValidationResult)

**Validation Rules Checked**:
- Symbol not empty, max 20 chars
- EntryPrice, StopLoss, TakeProfit, PositionSize > 0
- RiskPercentage between 0.1 and 100
- If Long: StopLoss < EntryPrice < TakeProfit
- If Short: TakeProfit < EntryPrice < StopLoss
- If ExitDateTime set: must be >= EntryDateTime
- DisciplineScore (if set): 1 to 10
- QualityRating (if set): 1 to 10
- Timeframe, SetupType not empty

**Pre-conditions**: None

**Post-conditions**:
- ValidationResult.IsValid is true if all rules pass
- ValidationResult.Errors contains list of error messages if validation fails

---

## Supporting Types

### TradeFilter

```csharp
public class TradeFilter
{
    public string? Symbol { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SetupType { get; set; }
    public TradeDirection? Direction { get; set; }
    public bool? IsOpen { get; set; }  // true = no exit, false = has exit
    public bool? IsProfitable { get; set; }  // true = P/L > 0, false = P/L <= 0
    public int? MinDisciplineScore { get; set; }
    public int? MinQualityRating { get; set; }
}
```

---

## Performance Requirements

- `GetTradeByIdAsync`: < 50ms
- `GetAllTradesAsync`: < 500ms for 10,000 trades (use paging in UI instead)
- `GetTradesPagedAsync`: < 100ms per page
- `CreateTradeAsync`: < 200ms
- `UpdateTradeAsync`: < 200ms
- `DeleteTradeAsync`: < 100ms
- `AddAttachmentAsync`: < 1s for files up to 5MB
- Calculation methods: < 1ms

---

## Testing Requirements

### Unit Tests
- Test all calculation methods with various inputs (long/short, profit/loss)
- Test validation logic for edge cases
- Mock database context for service method tests

### Integration Tests
- Test full CRUD workflow: Create → Read → Update → Delete
- Test filtering and paging with large datasets
- Test attachment file operations (copy, delete)
- Test concurrent updates with optimistic concurrency

### Contract Tests
- Verify all methods exist with correct signatures
- Verify return types match specification
- Verify exceptions thrown for invalid inputs

---

**Status**: ITradeService contract defined ✅
