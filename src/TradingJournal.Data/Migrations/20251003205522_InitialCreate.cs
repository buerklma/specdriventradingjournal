using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TradingJournal.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StrategyCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ColorHex = table.Column<string>(type: "TEXT", nullable: false),
                    IsSystemDefined = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Direction = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExitDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EntryPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExitPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    StopLoss = table.Column<decimal>(type: "TEXT", nullable: false),
                    TakeProfit = table.Column<decimal>(type: "TEXT", nullable: false),
                    PositionSize = table.Column<decimal>(type: "TEXT", nullable: false),
                    RiskPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    Timeframe = table.Column<string>(type: "TEXT", nullable: false),
                    SetupType = table.Column<string>(type: "TEXT", nullable: false),
                    MarketStructure = table.Column<string>(type: "TEXT", nullable: true),
                    PlannedRRRatio = table.Column<decimal>(type: "TEXT", nullable: false),
                    RealizedRRRatio = table.Column<decimal>(type: "TEXT", nullable: true),
                    ProfitLossCurrency = table.Column<decimal>(type: "TEXT", nullable: true),
                    ProfitLossR = table.Column<decimal>(type: "TEXT", nullable: true),
                    HoldingTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    EmotionAtEntry = table.Column<string>(type: "TEXT", nullable: true),
                    EmotionDuringTrade = table.Column<string>(type: "TEXT", nullable: true),
                    EmotionAtExit = table.Column<string>(type: "TEXT", nullable: true),
                    DisciplineScore = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    QualityRating = table.Column<int>(type: "INTEGER", nullable: true),
                    Mistakes = table.Column<string>(type: "TEXT", nullable: true),
                    LessonsLearned = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Theme = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    DefaultRiskPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    EnableBiometricAuth = table.Column<bool>(type: "INTEGER", nullable: false),
                    PinHash = table.Column<string>(type: "TEXT", nullable: true),
                    DatabaseEncrypted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CustomFieldDefinitions = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FileType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    StoragePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_Trades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagementAdjustments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AdjustmentDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AdjustmentType = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    NewValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagementAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagementAdjustments_Trades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "StrategyCategories",
                columns: new[] { "Id", "ColorHex", "CreatedAt", "Description", "IsSystemDefined", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "#FF5733", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Breakout" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "#33FF57", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Pullback" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "#3357FF", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Reversal" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "#FF33A1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Trend Continuation" }
                });

            migrationBuilder.InsertData(
                table: "UserPreferences",
                columns: new[] { "Id", "CustomFieldDefinitions", "DatabaseEncrypted", "DefaultCurrency", "DefaultRiskPercentage", "EnableBiometricAuth", "PinHash", "Theme", "UpdatedAt" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "{}", false, "USD", 1m, false, null, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_TradeId",
                table: "Attachments",
                column: "TradeId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementAdjustments_TradeId",
                table: "ManagementAdjustments",
                column: "TradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_EntryDateTime",
                table: "Trades",
                column: "EntryDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_SetupType",
                table: "Trades",
                column: "SetupType");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_Symbol",
                table: "Trades",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_Symbol_EntryDateTime",
                table: "Trades",
                columns: new[] { "Symbol", "EntryDateTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "ManagementAdjustments");

            migrationBuilder.DropTable(
                name: "StrategyCategories");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "Trades");
        }
    }
}
