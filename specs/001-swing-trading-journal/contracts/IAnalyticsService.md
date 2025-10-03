# Service Contract: IAnalyticsService

**Purpose**: Calculates trading statistics, generates performance metrics, and provides data for charts

---

## Interface Definition

```csharp
public interface IAnalyticsService
{
    // Overall Statistics
    Task<TradingStatistics> GetOverallStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<TradingStatistics> GetStatisticsBySetupTypeAsync(string setupType, DateTime? startDate = null, DateTime? endDate = null);
    Task<TradingStatistics> GetStatisticsBySymbolAsync(string symbol, DateTime? startDate = null, DateTime? endDate = null);
    
    // Performance Metrics
    Task<decimal> CalculateWinRateAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> CalculateAverageRRRatioAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> CalculateProfitFactorAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<MaxDrawdown> CalculateMaxDrawdownAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> CalculateTotalProfitLossRAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    // Chart Data
    Task<List<EquityCurvePoint>> GetEquityCurveDataAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<RRDistributionBucket>> GetRRDistributionDataAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<PerformanceBySetup>> GetPerformanceBySetupDataAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<MonthlyPerformance>> GetMonthlyPerformanceDataAsync(int year);
    
    // Psychology Analytics
    Task<Dictionary<string, int>> GetEmotionFrequencyAsync(EmotionStage stage, DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> GetAverageDisciplineScoreAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<DisciplineCorrelation>> AnalyzeDisciplineCorrelationAsync();
    
    // Mistake Analysis
    Task<List<MistakeFrequency>> GetTopMistakesAsync(int topN = 10);
    Task<List<LessonFrequency>> GetTopLessonsAsync(int topN = 10);
}
```

---

## Method Contracts

### GetOverallStatisticsAsync

**Input**: 
- `DateTime? startDate` (optional filter)
- `DateTime? endDate` (optional filter)

**Output**: `TradingStatistics` object:
```csharp
public class TradingStatistics
{
    public int TotalTrades { get; set; }
    public int WinningTrades { get; set; }
    public int LosingTrades { get; set; }
    public int BreakEvenTrades { get; set; }
    public decimal WinRate { get; set; }  // Percentage
    public decimal AverageRRRatio { get; set; }
    public decimal AverageWinRR { get; set; }
    public decimal AverageLossRR { get; set; }
    public decimal ProfitFactor { get; set; }
    public decimal TotalProfitLossCurrency { get; set; }
    public decimal TotalProfitLossR { get; set; }
    public decimal MaxDrawdownCurrency { get; set; }
    public decimal MaxDrawdownPercentage { get; set; }
    public decimal AverageHoldingTimeHours { get; set; }
    public decimal LargestWinCurrency { get; set; }
    public decimal LargestLossCurrency { get; set; }
    public decimal AverageDisciplineScore { get; set; }
    public decimal AverageQualityRating { get; set; }
}
```

**Side Effects**: None (read-only aggregation)
**Errors**: None

**Pre-conditions**: None

**Post-conditions**:
- All metrics calculated from closed trades (ExitDateTime != null)
- If date range specified, only trades within range included
- WinRate = (WinningTrades / TotalTrades) * 100
- ProfitFactor = GrossProfit / GrossLoss (or 0 if GrossLoss == 0)

---

### CalculateWinRateAsync

**Input**: Date range (optional)
**Output**: `decimal` win rate as percentage (0-100)
**Side Effects**: None
**Errors**: None

**Formula**:
```csharp
int totalTrades = closedTrades.Count;
int winningTrades = closedTrades.Count(t => t.ProfitLossCurrency > 0);
return totalTrades > 0 ? (decimal)winningTrades / totalTrades * 100 : 0;
```

**Pre-conditions**: None

**Post-conditions**:
- Returns value between 0 and 100
- Returns 0 if no closed trades

---

### CalculateProfitFactorAsync

**Input**: Date range (optional)
**Output**: `decimal` profit factor
**Side Effects**: None
**Errors**: None

**Formula**:
```csharp
decimal grossProfit = closedTrades.Where(t => t.ProfitLossCurrency > 0).Sum(t => t.ProfitLossCurrency.Value);
decimal grossLoss = Math.Abs(closedTrades.Where(t => t.ProfitLossCurrency < 0).Sum(t => t.ProfitLossCurrency.Value));
return grossLoss > 0 ? grossProfit / grossLoss : 0;
```

**Pre-conditions**: None

**Post-conditions**:
- Returns 0 if no losing trades (undefined profit factor)
- Returns positive decimal
- Profit factor > 1 means profitable overall
- Profit factor < 1 means unprofitable overall

---

### CalculateMaxDrawdownAsync

**Input**: Date range (optional)
**Output**: `MaxDrawdown` object:
```csharp
public class MaxDrawdown
{
    public decimal DrawdownCurrency { get; set; }
    public decimal DrawdownPercentage { get; set; }
    public DateTime DrawdownStartDate { get; set; }
    public DateTime DrawdownEndDate { get; set; }
    public int TradesInDrawdown { get; set; }
}
```

**Side Effects**: None
**Errors**: None

**Calculation**:
1. Build equity curve (cumulative P/L over time)
2. Track peak equity and current equity
3. Drawdown = Peak - Current at each point
4. Max drawdown = largest difference between peak and trough

**Pre-conditions**: None

**Post-conditions**:
- Returns largest historical drawdown
- DrawdownPercentage = (DrawdownCurrency / PeakEquity) * 100
- If no trades, returns all zeros

---

### GetEquityCurveDataAsync

**Input**: Date range (optional)
**Output**: `List<EquityCurvePoint>`:
```csharp
public class EquityCurvePoint
{
    public DateTime Date { get; set; }
    public decimal CumulativeProfitLossCurrency { get; set; }
    public decimal CumulativeProfitLossR { get; set; }
    public int TradeNumber { get; set; }
}
```

**Side Effects**: None
**Errors**: None

**Calculation**:
1. Order all closed trades by ExitDateTime
2. Calculate cumulative P/L after each trade
3. Return list of points for charting

**Pre-conditions**: None

**Post-conditions**:
- List ordered by Date ascending
- CumulativeProfitLossCurrency starts at 0
- Each point represents state after a trade exit

---

### GetRRDistributionDataAsync

**Input**: Date range (optional)
**Output**: `List<RRDistributionBucket>`:
```csharp
public class RRDistributionBucket
{
    public string BucketLabel { get; set; }  // e.g., "-5 to -4", "-4 to -3", "0 to 1", "1 to 2"
    public decimal BucketMin { get; set; }
    public decimal BucketMax { get; set; }
    public int TradeCount { get; set; }
}
```

**Side Effects**: None
**Errors**: None

**Bucketing Strategy**:
- Buckets: < -5, -5 to -4, -4 to -3, -3 to -2, -2 to -1, -1 to 0, 0 to 1, 1 to 2, 2 to 3, 3 to 4, 4 to 5, > 5
- Count trades in each bucket based on RealizedRRRatio

**Pre-conditions**: None

**Post-conditions**:
- Returns buckets with non-zero counts
- Useful for histogram/bar chart visualization

---

### GetPerformanceBySetupDataAsync

**Input**: Date range (optional)
**Output**: `List<PerformanceBySetup>`:
```csharp
public class PerformanceBySetup
{
    public string SetupType { get; set; }
    public int TotalTrades { get; set; }
    public int WinningTrades { get; set; }
    public decimal WinRate { get; set; }
    public decimal AverageRRRatio { get; set; }
    public decimal TotalProfitLossCurrency { get; set; }
    public decimal TotalProfitLossR { get; set; }
}
```

**Side Effects**: None
**Errors**: None

**Calculation**:
- Group trades by SetupType
- Calculate aggregate statistics for each setup
- Order by TotalProfitLossR descending

**Pre-conditions**: None

**Post-conditions**:
- List ordered by profitability (best setups first)
- Each setup has complete statistics

---

### GetEmotionFrequencyAsync

**Input**: 
- `EmotionStage stage` (Entry, During, Exit)
- Date range (optional)

**Output**: `Dictionary<string, int>` mapping emotion to frequency count

```csharp
public enum EmotionStage
{
    Entry,
    During,
    Exit
}
```

**Side Effects**: None
**Errors**: None

**Calculation**:
1. Extract emotion field based on stage (EmotionAtEntry, EmotionDuringTrade, EmotionAtExit)
2. Count occurrences of each unique emotion string
3. Order by frequency descending

**Pre-conditions**: None

**Post-conditions**:
- Dictionary keys are emotion strings (e.g., "Confident", "Fearful", "Calm")
- Values are occurrence counts
- Empty dictionary if no emotions recorded

---

### AnalyzeDisciplineCorrelationAsync

**Input**: None
**Output**: `List<DisciplineCorrelation>`:
```csharp
public class DisciplineCorrelation
{
    public int DisciplineScore { get; set; }  // 1-10
    public int TradeCount { get; set; }
    public decimal AverageRRRatio { get; set; }
    public decimal WinRate { get; set; }
}
```

**Side Effects**: None
**Errors**: None

**Calculation**:
- Group trades by DisciplineScore
- Calculate average R/R and win rate for each discipline level
- Shows correlation between discipline and performance

**Pre-conditions**: None

**Post-conditions**:
- List contains entries for discipline scores 1-10 (if trades exist)
- Can be used to visualize discipline impact on results

---

### GetTopMistakesAsync

**Input**: `int topN` (default 10)
**Output**: `List<MistakeFrequency>`:
```csharp
public class MistakeFrequency
{
    public string Mistake { get; set; }
    public int Frequency { get; set; }
    public decimal AverageImpactR { get; set; }  // Average P/L of trades with this mistake
}
```

**Side Effects**: None
**Errors**: None

**Calculation**:
1. Parse Trade.Mistakes field (comma-separated or line-separated)
2. Count frequency of each unique mistake
3. Calculate average P/L for trades with each mistake
4. Return top N most frequent

**Pre-conditions**: None

**Post-conditions**:
- List ordered by Frequency descending
- Returns up to topN items
- Empty list if no mistakes recorded

---

## Performance Requirements

- `GetOverallStatisticsAsync`: < 1s for 10,000 trades
- `CalculateWinRateAsync`: < 200ms
- `CalculateProfitFactorAsync`: < 200ms
- `CalculateMaxDrawdownAsync`: < 500ms (requires iteration)
- `GetEquityCurveDataAsync`: < 500ms for 10,000 points
- `GetRRDistributionDataAsync`: < 300ms
- `GetPerformanceBySetupDataAsync`: < 500ms
- All psychology/mistake analysis methods: < 500ms

---

## Caching Strategy

Analytics data can be cached with invalidation on trade updates:
- Cache key: `analytics_{filterHash}_{lastTradeUpdateTimestamp}`
- Invalidate cache when any trade is created, updated, or deleted
- Cache expiry: 5 minutes for frequently accessed statistics

---

## Testing Requirements

### Unit Tests
- Test calculation formulas with known datasets
- Test edge cases (no trades, all wins, all losses, zero risk trades)
- Test date filtering logic
- Test bucketing and grouping algorithms

### Integration Tests
- Test with realistic dataset (100-1000 trades)
- Verify performance targets met
- Test concurrent access to analytics

### Contract Tests
- Verify all methods return correct types
- Verify aggregation accuracy with known test data

---

**Status**: IAnalyticsService contract defined ✅
