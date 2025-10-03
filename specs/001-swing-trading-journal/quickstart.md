# Quickstart Guide: Swing Trading Journal App

**Purpose**: User-executable test procedures to validate app functionality without technical knowledge  
**Feature**: 001-swing-trading-journal  
**Date**: 2025-10-03

---

## Prerequisites

- Windows 10 or Windows 11 PC
- Trading Journal App installed
- Mouse and keyboard
- 5-10 minutes of time

---

## Test Scenario 1: First-Time Setup and Create Your First Trade

**Goal**: Launch the app, complete initial setup, and create your first trade record

**Expected Time**: 3-5 minutes

### Steps:

1. **Launch the App**
   - Double-click the "Trading Journal" icon on your desktop or Start Menu
   - ✅ **Verify**: App window opens within 3 seconds
   - ✅ **Verify**: You see the Dashboard screen with "No trades yet" message

2. **Check Theme Settings**
   - Click the hamburger menu (☰) in the top-left corner
   - Click "Settings"
   - Find "Theme" dropdown
   - Try switching between "Light", "Dark", and "System"
   - ✅ **Verify**: App colors change immediately without restarting

3. **Create Your First Trade**
   - Click the hamburger menu (☰) again
   - Click "New Trade"
   - Fill in the trade form with these example values:
     - Symbol: **AAPL**
     - Direction: **Long**
     - Entry Date & Time: **Today at 09:30 AM**
     - Entry Price: **150.00**
     - Stop-Loss: **148.50**
     - Take-Profit: **155.00**
     - Position Size: **100**
     - Risk %: **2.0**
     - Timeframe: **1D**
     - Setup Type: **Breakout**
   - ✅ **Verify**: "Planned R:R" shows approximately **3.33** (calculated automatically)
   - Click "Save Trade"
   - ✅ **Verify**: Success message appears
   - ✅ **Verify**: You're redirected to the Trades list
   - ✅ **Verify**: Your AAPL trade appears in the list

4. **View Trade Details**
   - Click on the AAPL trade in the list
   - ✅ **Verify**: Trade details page opens
   - ✅ **Verify**: All information you entered is displayed correctly
   - ✅ **Verify**: Status shows "Open" (no exit yet)

**✅ Scenario 1 PASSED** if all verifications succeeded

---

## Test Scenario 2: Log Psychology and Exit a Trade

**Goal**: Add emotional notes to a trade and record its exit

**Expected Time**: 2-3 minutes

### Steps:

1. **Add Psychology Notes**
   - From the Trade Details page (or navigate to your AAPL trade)
   - Scroll to the "Psychology" section
   - In "Emotion at Entry", type: **Confident**
   - In "Discipline Score" (1-10), enter: **8**
   - In "Notes" field, type: **Entry was clean on volume breakout. Followed my plan.**
   - Click "Save Psychology"
   - ✅ **Verify**: "Psychology saved" message appears
   - ✅ **Verify**: Your notes remain visible after saving

2. **Record Trade Exit (Winning Trade)**
   - Scroll to the "Exit Trade" section
   - Enter Exit Date & Time: **Tomorrow at 02:00 PM** (or 1 day later)
   - Enter Exit Price: **153.00**
   - In "Emotion at Exit", type: **Satisfied**
   - Click "Exit Trade"
   - ✅ **Verify**: Confirmation dialog asks "Are you sure you want to exit this trade?"
   - Click "Yes"
   - ✅ **Verify**: Trade status changes to "Closed"
   - ✅ **Verify**: **Realized R:R** shows approximately **2.00**
   - ✅ **Verify**: **Profit/Loss** shows **+$300.00**
   - ✅ **Verify**: **Profit in R** shows **+2.00 R**
   - ✅ **Verify**: **Holding Time** shows approximately **28.5 hours** or **1 day 4.5 hours**

**✅ Scenario 2 PASSED** if all verifications succeeded

---

## Test Scenario 3: Add Quality Review and Screenshot

**Goal**: Rate the trade quality, document mistakes/lessons, and attach a chart screenshot

**Expected Time**: 3-4 minutes

### Steps:

1. **Add Quality Rating**
   - Still on the Trade Details page for AAPL
   - Scroll to "Review" section
   - Set "Quality Rating" (1-10): **7**
   - In "Mistakes" field, type: **Entry slightly early, should have waited for confirmation candle**
   - In "Lessons Learned" field, type: **Patience on entry improves R:R. Target hit perfectly.**
   - Click "Save Review"
   - ✅ **Verify**: "Review saved" message appears

2. **Attach Screenshot**
   - In the "Attachments" section, click "Add Screenshot"
   - File picker dialog opens
   - Select any image file from your computer (e.g., a chart screenshot or any .jpg/.png file)
   - ✅ **Verify**: File must be under 10 MB
   - In "Caption" field, type: **AAPL daily chart breakout**
   - Click "Upload"
   - ✅ **Verify**: Screenshot appears as a thumbnail in the Attachments section
   - Click on the thumbnail
   - ✅ **Verify**: Full-size preview opens
   - Close the preview

**✅ Scenario 3 PASSED** if all verifications succeeded

---

## Test Scenario 4: Create Multiple Trades and View Analytics

**Goal**: Add several more trades and check performance statistics

**Expected Time**: 5-7 minutes

### Steps:

1. **Create Second Trade (Losing Trade)**
   - Navigate to "New Trade" from the menu
   - Fill in:
     - Symbol: **TSLA**
     - Direction: **Long**
     - Entry Date: **Today at 10:00 AM**
     - Entry Price: **250.00**
     - Stop-Loss: **247.00**
     - Take-Profit: **258.00**
     - Position Size: **50**
     - Risk %: **2.0**
     - Timeframe: **4H**
     - Setup Type: **Pullback**
   - Save the trade
   - Go back to Trade Details
   - Exit the trade with:
     - Exit Date: **Today at 03:00 PM** (5 hours later)
     - Exit Price: **247.00** (hit stop-loss)
   - ✅ **Verify**: Profit/Loss shows **-$150.00**
   - ✅ **Verify**: Profit in R shows **-1.00 R**

2. **Create Third Trade (Small Win)**
   - Create another trade:
     - Symbol: **MSFT**
     - Direction: **Short**
     - Entry Price: **380.00**
     - Stop-Loss: **383.00**
     - Take-Profit: **374.00**
     - Position Size: **30**
   - Exit at: **378.00**
   - ✅ **Verify**: This is a winning short trade (entry > exit)

3. **View Dashboard Analytics**
   - Navigate to "Dashboard" from the menu
   - ✅ **Verify**: You see an overview card showing:
     - **Total Trades**: 3
     - **Win Rate**: Should be around **66.67%** (2 wins, 1 loss)
     - **Total P/L**: Around **+$210** (300 - 150 + 60)
     - **Average R:R**: Calculated from all trades
   - Scroll down to see charts:
     - ✅ **Verify**: Equity Curve chart shows upward trend (starts at 0, ends at ~+210)
     - ✅ **Verify**: R/R Distribution chart shows bars at different R multiples
     - ✅ **Verify**: Performance by Setup chart shows Breakout and Pullback categories

4. **Filter Trades by Symbol**
   - Navigate to "Trades" from the menu
   - Find the search/filter box at the top
   - In "Symbol" filter, type: **AAPL**
   - Press Enter or click "Filter"
   - ✅ **Verify**: Only the AAPL trade is shown
   - Clear the filter (click "X" or clear button)
   - ✅ **Verify**: All 3 trades are shown again

**✅ Scenario 4 PASSED** if all verifications succeeded

---

## Test Scenario 5: Export Data and Create Backup

**Goal**: Export your trade data to CSV and create a backup

**Expected Time**: 2-3 minutes

### Steps:

1. **Export to CSV**
   - Navigate to "Export & Backup" from the menu
   - Click "Export to CSV"
   - File save dialog opens
   - Choose a location (e.g., Desktop) and filename: **MyTrades.csv**
   - Click "Save"
   - ✅ **Verify**: Success message "Exported 3 trades to MyTrades.csv"
   - Open the CSV file in Excel or Notepad
   - ✅ **Verify**: File contains header row and 3 data rows
   - ✅ **Verify**: All trade details are present (Symbol, Entry Price, Exit Price, P/L, etc.)

2. **Create Backup**
   - Back in the app, click "Create Backup"
   - Choose a backup location (e.g., Documents folder)
   - Click "Start Backup"
   - ✅ **Verify**: Progress indicator shows backup in progress
   - ✅ **Verify**: Success message appears within 10 seconds
   - ✅ **Verify**: A new ZIP file is created with name like **TradingJournal_Backup_2025-10-03_143025.zip**
   - Check the file size
   - ✅ **Verify**: File size is reasonable (a few KB to MB depending on attachments)

3. **Verify Backup Contents (Optional)**
   - Right-click the backup ZIP file → "Extract All"
   - Extract to a temporary folder
   - ✅ **Verify**: You see files:
     - `tradingjour nal.db`
     - `Screenshots/` folder (if you added screenshots)
     - `backup-info.json`
   - Open `backup-info.json` in Notepad
   - ✅ **Verify**: It contains backup date, trade count, and app version

**✅ Scenario 5 PASSED** if all verifications succeeded

---

## Test Scenario 6: Dark Mode and Keyboard Shortcuts

**Goal**: Test theme switching and keyboard shortcuts for power users

**Expected Time**: 2 minutes

### Steps:

1. **Switch to Dark Mode**
   - Press **Ctrl + ,** (Ctrl + Comma) to open Settings
   - ✅ **Verify**: Settings page opens via keyboard shortcut
   - Switch Theme to "Dark"
   - ✅ **Verify**: App immediately switches to dark color scheme
   - ✅ **Verify**: Text is light-colored, background is dark gray

2. **Test Keyboard Shortcuts**
   - Press **Ctrl + N**
   - ✅ **Verify**: New Trade form opens
   - Press **Escape** to cancel (or navigate away)
   - Navigate to Trades list
   - Press **Ctrl + F**
   - ✅ **Verify**: Search/Filter box receives focus (cursor blinks in the input field)
   - Press **F5**
   - ✅ **Verify**: Page refreshes or data reloads

3. **Verify Responsiveness**
   - Resize the app window (drag corner to make it smaller)
   - ✅ **Verify**: Layout adapts gracefully, no overlapping text
   - ✅ **Verify**: Charts remain readable
   - Maximize the window again
   - ✅ **Verify**: Everything scales properly

**✅ Scenario 6 PASSED** if all verifications succeeded

---

## Success Criteria

You have successfully validated the Trading Journal App if:

- ✅ All 6 test scenarios passed
- ✅ App loads in under 3 seconds
- ✅ Trade creation takes under 2 seconds
- ✅ Analytics and charts load smoothly
- ✅ Dark and light themes work perfectly
- ✅ Export and backup functions work correctly
- ✅ Keyboard shortcuts respond immediately
- ✅ No crashes or freezes occurred during testing

---

## Troubleshooting

### App won't launch
- Check Windows version (must be Windows 10/11)
- Try restarting your computer
- Reinstall the app

### Trade won't save
- Verify all required fields are filled in (Symbol, Prices, Position Size)
- Check that Entry Price, Stop-Loss, and Take-Profit follow correct order for direction
- Ensure Position Size and Risk % are positive numbers

### Charts not displaying
- Ensure you have at least 3 closed trades for meaningful charts
- Check that Exit Prices are set for trades

### Export/Backup fails
- Verify you have write permissions to the chosen folder
- Ensure sufficient disk space (at least 100 MB free)
- Close any files (CSV, Excel) that might be open from previous exports

---

## Next Steps

After completing these scenarios, you're ready to:
1. Start recording your real trades
2. Explore Psychology logging for better discipline tracking
3. Use Analytics to identify your best setups
4. Review lessons learned to improve performance
5. Set up regular backups (weekly recommended)

**Happy Trading! 📈**

---

**Status**: Quickstart scenarios defined ✅  
**Validation**: User-testable without technical knowledge ✅  
**Milestone**: Ready for Phase 2 (Task Generation) ✅
