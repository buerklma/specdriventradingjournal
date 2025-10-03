# Service Layer Tests - Phase 3.3

This directory contains contract tests for the service layer interfaces. These tests define the expected behavior of services before implementation (TDD approach).

## Test Organization

### ITradeService Tests (T035-T050) ✅ Complete
- **TradeService_CreateTests.cs** (T035) - Tests CreateTradeAsync generates Id, timestamps, and calculates PlannedRR
- **TradeService_CreateValidationTests.cs** (T036) - Tests validation exceptions for invalid trade data
- **TradeService_GetByIdTests.cs** (T037) - Tests retrieval by ID with navigation properties
- **TradeService_GetAllTests.cs** (T038) - Tests retrieval of all trades ordered by EntryDateTime
- **TradeService_GetBySymbolTests.cs** (T039) - Tests symbol filtering with case-insensitive matching
- **TradeService_GetByDateRangeTests.cs** (T040) - Tests date range filtering with inclusive bounds
- **TradeService_GetPagedTests.cs** (T041) - Tests pagination logic, page size, TotalPages, HasNext/PreviousPage
- **TradeService_GetPagedFilterTests.cs** (T042) - Tests filtering by Symbol, DateRange, SetupType, Direction, IsOpen, IsProfitable
- **TradeService_UpdateTests.cs** (T043) - Tests update operations and UpdatedAt timestamp
- **TradeService_UpdateExitTests.cs** (T044) - Tests exit updates with calculated fields (RealizedRR, P/L, HoldingTime)
- **TradeService_DeleteTests.cs** (T045) - Tests delete operations with cascade deletes
- **TradeService_AdjustmentTests.cs** (T046-T047) - Tests management adjustments creation and retrieval
- **TradeService_AttachmentTests.cs** (T048-T050) - Tests attachment file operations (copy, create, delete)

### IAnalyticsService Tests (T051-T066) 🚧 In Progress
- Tests for analytics calculations, statistics aggregation, and chart data generation

### IExportService Tests (T067-T076) ⏳ Pending
- Tests for CSV/Excel/PDF export and backup/restore operations

## Test Structure

All tests follow this pattern:
```csharp
[Fact]
public async Task MethodName_ShouldExpectedBehavior_WhenCondition()
{
    // Arrange - Set up test data

    // Act - Call the method (commented out until service implementation)
    // var result = await service.MethodAsync(...);

    // Assert - Verify expected behavior (commented out)
    // result.Should()...

    // Stub assertion for now
    Assert.True(true, "Test stub - will be implemented when ITradeService is available");
}
```

## Implementation Status

- ✅ **ITradeService**: All 16 test files created (T035-T050)
- 🚧 **IAnalyticsService**: 0/16 test files created (T051-T066)
- ⏳ **IExportService**: 0/10 test files created (T067-T076)

## Next Steps

1. Create IAnalyticsService test files
2. Create IExportService test files
3. Implement Phase 3.6 (Service Layer Implementation)
4. Uncomment and run these tests against actual implementations
5. Write Phase 3.4 (Integration Tests) to verify end-to-end workflows

## Notes

- Tests use FluentAssertions for assertion syntax (currently causing compile errors - need to add package)
- Tests currently return `Assert.True(true, ...)` stubs since services aren't implemented yet
- All test assertions are commented out and will be uncommented during Phase 3.6 implementation
- TradeFilter class is defined in TradeService_GetPagedFilterTests.cs and should be moved to Core layer

## Dependencies

- xUnit
- NSubstitute (for mocking)
- FluentAssertions (needs to be added)
- TradingJournal.Data (for models)
- TradingJournal.Core (will contain service interfaces and implementations)
