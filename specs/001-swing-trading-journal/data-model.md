# Data Model: Swing Trading Journal App

**Feature**: 001-swing-trading-journal  
**Date**: 2025-10-03  
**Purpose**: Entity definitions, relationships, and validation rules

---

## Entity Relationship Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                            Trade                                 │
├─────────────────────────────────────────────────────────────────┤
│ + Id: Guid (PK)                                                 │
│ + Symbol: string                                                │
│ + Direction: TradeDirection (enum)                             │
│ + EntryDateTime: DateTime                                       │
│ + ExitDateTime: DateTime?                                       │
│ + EntryPrice: decimal                                           │
│ + ExitPrice: decimal?                                           │
│ + StopLoss: decimal                                             │
│ + TakeProfit: decimal                                           │
│ + PositionSize: decimal                                         │
│ + RiskPercentage: decimal                                       │
│ + Timeframe: string                                             │
│ + SetupType: string                                             │
│ + MarketStructure: string?                                      │
│ + PlannedRRRatio: decimal                                       │
│ + RealizedRRRatio: decimal?                                     │
│ + ProfitLossCurrency: decimal?                                  │
│ + ProfitLossR: decimal?                                         │
│ + HoldingTime: TimeSpan?                                        │
│ + EmotionAtEntry: string?                                       │
│ + EmotionDuringTrade: string?                                   │
│ + EmotionAtExit: string?                                        │
│ + DisciplineScore: int?                                         │
│ + Notes: string?                                                │
│ + QualityRating: int?                                           │
│ + Mistakes: string?                                             │
│ + LessonsLearned: string?                                       │
│ + CreatedAt: DateTime                                           │
│ + UpdatedAt: DateTime                                           │
└─────────────────────────────────────────────────────────────────┘
           │ 1
           │
           │ N
┌──────────┴──────────────────────────────────────────────────────┐
│                   ManagementAdjustment                           │
├─────────────────────────────────────────────────────────────────┤
│ + Id: Guid (PK)                                                 │
│ + TradeId: Guid (FK)                                            │
│ + AdjustmentDateTime: DateTime                                  │
│ + AdjustmentType: AdjustmentType (enum)                        │
│ + PreviousValue: decimal?                                       │
│ + NewValue: decimal                                             │
│ + Reason: string?                                               │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                       Attachment                                 │
├─────────────────────────────────────────────────────────────────┤
│ + Id: Guid (PK)                                                 │
│ + TradeId: Guid (FK)                                            │
│ + FileName: string                                              │
│ + FileType: string                                              │
│ + FileSizeBytes: long                                           │
│ + StoragePath: string                                           │
│ + Caption: string?                                              │
│ + UploadedAt: DateTime                                          │
└─────────────────────────────────────────────────────────────────┘
           │ N
           │
           │ 1
           └─────────> Trade

┌─────────────────────────────────────────────────────────────────┐
│                    StrategyCategory                              │
├─────────────────────────────────────────────────────────────────┤
│ + Id: Guid (PK)                                                 │
│ + Name: string                                                  │
│ + Description: string?                                          │
│ + ColorHex: string                                              │
│ + IsSystemDefined: bool                                         │
│ + CreatedAt: DateTime                                           │
└─────────────────────────────────────────────────────────────────┘
           │ 1
           │
           │ N
           └─────────> Trade.SetupType (references by name)

┌─────────────────────────────────────────────────────────────────┐
│                    UserPreferences                               │
├─────────────────────────────────────────────────────────────────┤
│ + Id: Guid (PK)                                                 │
│ + Theme: ThemeMode (enum)                                       │
│ + DefaultCurrency: string                                       │
│ + DefaultRiskPercentage: decimal                                │
│ + EnableBiometricAuth: bool                                     │
│ + PinHash: string?                                              │
│ + DatabaseEncrypted: bool                                       │
│ + CustomFieldDefinitions: string (JSON)                         │
│ + UpdatedAt: DateTime                                           │
└─────────────────────────────────────────────────────────────────┘
```

---

## Entity Definitions

### Trade

**Purpose**: Core entity representing a single trading transaction with all details, psychology, and review data.

**Fields**:

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | Guid | No | Primary key | Auto-generated |
| Symbol | string | No | Ticker symbol (e.g., "AAPL", "EUR/USD") | Required, max 20 chars |
| Direction | TradeDirection | No | Long or Short | Enum: Long, Short |
| EntryDateTime | DateTime | No | When trade was entered | Required, <= Now |
| ExitDateTime | DateTime | Yes | When trade was exited | Must be > EntryDateTime |
| EntryPrice | decimal | No | Entry price | > 0 |
| ExitPrice | decimal | Yes | Exit price | > 0 if set |
| StopLoss | decimal | No | Stop-loss price | > 0 |
| TakeProfit | decimal | No | Take-profit target price | > 0 |
| PositionSize | decimal | No | Number of shares/contracts | > 0 |
| RiskPercentage | decimal | No | Risk as % of capital | 0.1 to 100 |
| Timeframe | string | No | Chart timeframe (e.g., "1D", "4H") | Required, max 10 chars |
| SetupType | string | No | Strategy/setup name | Required, max 50 chars |
| MarketStructure | string | Yes | Context notes | Max 200 chars |
| PlannedRRRatio | decimal | No | Planned risk/reward | >= 0 |
| RealizedRRRatio | decimal | Yes | Actual risk/reward | Calculated |
| ProfitLossCurrency | decimal | Yes | P/L in currency | Calculated |
| ProfitLossR | decimal | Yes | P/L in R multiples | Calculated |
| HoldingTime | TimeSpan | Yes | Duration of trade | Calculated |
| EmotionAtEntry | string | Yes | Emotion at entry | Max 100 chars |
| EmotionDuringTrade | string | Yes | Emotion during trade | Max 100 chars |
| EmotionAtExit | string | Yes | Emotion at exit | Max 100 chars |
| DisciplineScore | int | Yes | Discipline rating 1-10 | 1 to 10 |
| Notes | string | Yes | Free-form notes | Max 2000 chars |
| QualityRating | int | Yes | Setup quality 1-10 | 1 to 10 |
| Mistakes | string | Yes | Mistakes made | Max 1000 chars |
| LessonsLearned | string | Yes | Lessons learned | Max 1000 chars |
| CreatedAt | DateTime | No | Record creation timestamp | Auto-set |
| UpdatedAt | DateTime | No | Last update timestamp | Auto-updated |

**Navigation Properties**:
- `List<ManagementAdjustment> Adjustments` (1:N)
- `List<Attachment> Attachments` (1:N)

**Indexes**:
- `IX_Trade_Symbol` on `Symbol`
- `IX_Trade_EntryDateTime` on `EntryDateTime`
- `IX_Trade_SetupType` on `SetupType`
- `IX_Trade_Symbol_EntryDateTime` composite index

**Business Rules**:
- If `Direction == Long`: StopLoss < EntryPrice < TakeProfit
- If `Direction == Short`: TakeProfit < EntryPrice < StopLoss
- ExitPrice must be between StopLoss and TakeProfit (or beyond) based on direction
- ProfitLossR = (ExitPrice - EntryPrice) / (EntryPrice - StopLoss) for Long trades
- ProfitLossR = (EntryPrice - ExitPrice) / (StopLoss - EntryPrice) for Short trades

---

### ManagementAdjustment

**Purpose**: Records changes made to an active trade (stop-loss adjustments, partial exits, etc.)

**Fields**:

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | Guid | No | Primary key | Auto-generated |
| TradeId | Guid | No | Foreign key to Trade | Required |
| AdjustmentDateTime | DateTime | No | When adjustment was made | Required, >= Trade.EntryDateTime |
| AdjustmentType | AdjustmentType | No | Type of adjustment | Enum: StopLoss, TakeProfit, PartialExit |
| PreviousValue | decimal | Yes | Value before adjustment | >= 0 |
| NewValue | decimal | No | Value after adjustment | > 0 |
| Reason | string | Yes | Reason for adjustment | Max 500 chars |

**Navigation Properties**:
- `Trade Trade` (N:1)

**Business Rules**:
- AdjustmentDateTime must be between Trade.EntryDateTime and Trade.ExitDateTime
- For PartialExit: NewValue < PositionSize

---

### Attachment

**Purpose**: Files (screenshots, documents) attached to trades for reference

**Fields**:

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | Guid | No | Primary key | Auto-generated |
| TradeId | Guid | No | Foreign key to Trade | Required |
| FileName | string | No | Original filename | Required, max 255 chars |
| FileType | string | No | MIME type or extension | Required, max 50 chars |
| FileSizeBytes | long | No | File size in bytes | > 0, < 10 MB |
| StoragePath | string | No | Relative path in storage | Required, max 500 chars |
| Caption | string | Yes | Optional description | Max 200 chars |
| UploadedAt | DateTime | No | Upload timestamp | Auto-set |

**Navigation Properties**:
- `Trade Trade` (N:1)

**Business Rules**:
- Supported file types: .png, .jpg, .jpeg, .pdf, .txt
- Maximum file size: 10 MB per file
- StoragePath format: `{TradeId}/{FileName}`

---

### StrategyCategory

**Purpose**: User-defined or system trading strategy classifications

**Fields**:

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | Guid | No | Primary key | Auto-generated |
| Name | string | No | Strategy name | Required, unique, max 50 chars |
| Description | string | Yes | Optional description | Max 500 chars |
| ColorHex | string | No | Color for UI display | Required, valid hex color (#RRGGBB) |
| IsSystemDefined | bool | No | System vs user-defined | Default: false |
| CreatedAt | DateTime | No | Creation timestamp | Auto-set |

**Business Rules**:
- Name must be unique
- System-defined categories cannot be deleted
- Default categories: "Breakout", "Pullback", "Reversal", "Trend Continuation"

---

### UserPreferences

**Purpose**: Application settings and user customizations (singleton entity)

**Fields**:

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | Guid | No | Primary key (single record) | Fixed GUID |
| Theme | ThemeMode | No | UI theme | Enum: Light, Dark, System |
| DefaultCurrency | string | No | Currency symbol | Required, 3 chars (e.g., "USD") |
| DefaultRiskPercentage | decimal | No | Default risk per trade | 0.1 to 10 |
| EnableBiometricAuth | bool | No | Windows Hello enabled | Default: false |
| PinHash | string | Yes | Hashed PIN for auth | BCrypt hash |
| DatabaseEncrypted | bool | No | DB encryption status | Default: false |
| CustomFieldDefinitions | string | Yes | JSON array of custom fields | Valid JSON |
| UpdatedAt | DateTime | No | Last update timestamp | Auto-updated |

**Business Rules**:
- Only one UserPreferences record exists (singleton pattern)
- If EnableBiometricAuth is true, PinHash should be null
- CustomFieldDefinitions schema: `[{"Name":"string","Type":"string","DefaultValue":""}]`

---

## Enumerations

### TradeDirection
```csharp
public enum TradeDirection
{
    Long = 1,
    Short = 2
}
```

### AdjustmentType
```csharp
public enum AdjustmentType
{
    StopLossMove = 1,
    TakeProfitMove = 2,
    PartialExit = 3
}
```

### ThemeMode
```csharp
public enum ThemeMode
{
    Light = 1,
    Dark = 2,
    System = 3  // Follow Windows theme
}
```

---

## Calculated Fields

### Trade Calculations

**RealizedRRRatio** (calculated on ExitPrice set):
```csharp
if (Direction == TradeDirection.Long)
    RealizedRRRatio = (ExitPrice.Value - EntryPrice) / (EntryPrice - StopLoss);
else // Short
    RealizedRRRatio = (EntryPrice - ExitPrice.Value) / (StopLoss - EntryPrice);
```

**ProfitLossCurrency**:
```csharp
if (Direction == TradeDirection.Long)
    ProfitLossCurrency = (ExitPrice.Value - EntryPrice) * PositionSize;
else // Short
    ProfitLossCurrency = (EntryPrice - ExitPrice.Value) * PositionSize;
```

**ProfitLossR**:
```csharp
decimal riskAmount = Math.Abs(EntryPrice - StopLoss) * PositionSize;
ProfitLossR = ProfitLossCurrency / riskAmount;
```

**HoldingTime**:
```csharp
if (ExitDateTime.HasValue)
    HoldingTime = ExitDateTime.Value - EntryDateTime;
```

---

## Database Schema (SQLite)

### Migrations Strategy

1. **Initial Migration**: Create all tables with indexes
2. **Future Migrations**: Add columns, indexes, or tables as needed
3. **Seeding**: Insert default StrategyCategory records and UserPreferences singleton

### Connection String

**Development**:
```
Data Source=%LOCALAPPDATA%\TradingJournal\tradingjour nal.db;Cache=Shared;
```

**Production with Encryption**:
```
Data Source=%LOCALAPPDATA%\TradingJournal\tradingjournal.db;Password={userPassword};Cache=Shared;
```

---

## Validation Rules Summary

### Trade Entity Validation
- Symbol: Required, non-empty, max 20 chars
- Entry/Exit prices: Must be positive decimals
- Stop-loss/Take-profit: Must follow direction logic (Long: SL < Entry < TP)
- Position size: Must be positive
- Risk percentage: 0.1% to 100%
- Discipline score: 1 to 10 (if set)
- Quality rating: 1 to 10 (if set)

### Cross-Field Validation
- ExitDateTime must be after EntryDateTime
- Exit price must align with trade outcome (win/loss)
- Management adjustments must fall within trade lifetime

### File Attachments
- Max file size: 10 MB
- Allowed types: .png, .jpg, .jpeg, .pdf, .txt
- Total attachments per trade: unlimited (but UI may warn beyond 10)

---

## Data Integrity

### Referential Integrity
- Cascade delete: Deleting a Trade deletes all ManagementAdjustments and Attachments
- No orphan records allowed

### Concurrency
- Use `UpdatedAt` timestamp for optimistic concurrency detection
- EF Core row versioning with `[Timestamp]` attribute on entities

### Backup Strategy
- Full database backup copies entire .db file
- Attachment files must be backed up separately or included in zip
- Export to CSV includes only Trade entity flattened data

---

**Status**: Data model complete ✅  
**Next Step**: Generate contracts and quickstart scenarios
