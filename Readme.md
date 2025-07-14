# Client-Side Multiplayer Implementation Documentation

This document lists all client-side scripts that use the `IsMultiplayer` boolean value to separate server-side and client-side code logic in the slot machine game.

## Overview

The `IsMultiplayer` flag is the core mechanism that determines whether the slot machine runs in:
- **Single-player mode** (`IsMultiplayer = false`): Local RNG, local credit management, offline gameplay
- **Multiplayer mode** (`IsMultiplayer = true`): Server-managed RNG, server credit validation, SmartFoxServer communication

---

## Core Game Scripts

### 1. **Slot.cs** - Main Slot Machine Controller
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/Slot.cs`

#### Functions using IsMultiplayer:

- **`IsConnectedToServer()`** - Checks SmartFoxServer connection status (multiplayer only)
- **`ProcessServerSpinResponse()`** - Processes spin results from server (multiplayer only)
- **`StartSpinningAnimation()`** - Starts reel animation after bet deduction (multiplayer only)
- **`ProcessServerSpinResponseWithWins()`** - Enhanced server result processing with animations (multiplayer only)
- **`spinWithServerResult()`** - Uses existing spin infrastructure with server-supplied results (multiplayer only)
- **`StartVisualReelAnimation()`** - Starts visual reel animation without server communication (multiplayer only)
- **`ApplyServerResultsAfterDelay()`** - Coroutine to apply server results after minimum animation duration (multiplayer only)
- **`TriggerInsufficientCreditsEvent()`** - Handles insufficient credits from server or local validation
- **`spin()`** - Main spin method with branching logic:
  - **Multiplayer**: Sends requests to SmartFoxServer via `networkManager.HandleSlotSpin()`
  - **Single-player**: Local bet validation and credit deduction

#### Key Variables:
- **`public bool IsMultiplayer`** - Core flag determining game mode
- **`private GameplayNetworkManager networkManager`** - Server communication (multiplayer only)
- **`private int pendingWinAmount/pendingFinalBalance`** - Animation data storage (multiplayer only)
- **`private int[,] pendingServerResults`** - Server results for delayed application (multiplayer only)

---

### 2. **SlotCredits.cs** - Credit and Balance Management
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/SlotCredits.cs`

#### Functions using IsMultiplayer:

- **`restore()`** - Restore slot settings:
  - **Multiplayer**: Skips credit restoration from PlayerPrefs (server manages credits)
  - **Single-player**: Restores credits from local storage
  
- **`save()`** - Save slot settings:
  - **Multiplayer**: Skips saving credits locally (server manages credits)
  - **Single-player**: Saves credits to local storage

- **`updateCreditsFromServer()`** - Updates credits from server response (multiplayer only)

- **`animateBalanceDecrease()`** - Immediate balance reduction after bet deduction (multiplayer only)

- **`animateWinAndBalanceIncrease()`** - Animates win amount and balance increase (multiplayer only)

- **`placeBet()`** - Place bet logic:
  - **Multiplayer**: Always returns true (server validates)
  - **Single-player**: Local credit validation and deduction

- **`canPlaceBet()`** - Check if bet can be placed:
  - **Multiplayer**: Always returns true (server validates)
  - **Single-player**: Local credit validation

- **`awardWin()`** - Award win credits:
  - **Multiplayer**: Skips `totalOut` update (server manages)
  - **Single-player**: Updates `totalOut` locally

- **`awardBonus()`** - Award bonus credits:
  - **Multiplayer**: Skips `totalOut` update (server manages)
  - **Single-player**: Updates `totalOut` locally

---

### 3. **SlotReel.cs** - Individual Reel Management
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/SlotReel.cs`

#### Functions using IsMultiplayer:

- **`Start()`** - Reel initialization:
  - **Multiplayer**: Skips symbol frequency caching (server provides symbols)
  - **Single-player**: Caches symbol frequency for local RNG

- **`OnEnable()`** - Reel creation:
  - **Multiplayer**: Uses server-provided symbol data
  - **Single-player**: Creates reels normally with local logic

- **`createReelSymbolsFromServer()`** - Creates reel symbols from server data (multiplayer only)

- **`setServerSymbolData()`** - Sets server-provided symbol data (multiplayer only)

- **`chooseRandomSymbol()`** - Choose random symbol for reel:
  - **Multiplayer**: Uses server-provided symbol data
  - **Single-player**: Uses local RNG and symbol frequency

- **`spinReel()`** - Start reel spinning:
  - **Multiplayer**: Skips frequency recaching (server manages)
  - **Single-player**: Recaches symbol frequency for each spin

- **`handleScatterAnticipation()`** - Scatter symbol anticipation:
  - **Multiplayer**: Skips local scatter calculation (server handles)
  - **Single-player**: Local scatter detection and anticipation

- **`stopReelOnSymbolIndex()`** - Stop reel on specific symbol:
  - **Multiplayer**: Skips scatter detection (server handles)
  - **Single-player**: Local scatter detection during reel stop

#### Key Variables:
- **`public List<int> ReelStrip`** - Local reel strips (single-player only)
- **`private List<int> serverSymbolData`** - Server-provided symbols (multiplayer only)

---

### 4. **BeachDaysGUI.cs** - User Interface Management
**Location**: `/Assets/SlotCreatorPro/Example/BeachDays/scripts/BeachDaysGUI.cs`

#### Functions using IsMultiplayer:

- **`Update()`** - UI state management with multiplayer protection:
  - **`SlotState.playingwins`**: Only calls `updateWon()` if not showing multiplayer win
  - **`SlotState.spinning`**: Only shows "GOOD LUCK!" if not showing multiplayer win
  - **`default`**: Only clears won text if not showing multiplayer win

- **`showWinAmount()`** - Display win amount in won text field (multiplayer only)

- **`clearWinDisplay()`** - Clear win display (multiplayer only)

- **`showGoodLuck()`** - Show "Good Luck" message during reel animation (multiplayer only)

#### Key Variables:
- **`private bool isShowingMultiplayerWin`** - Flag to prevent Update method from overriding multiplayer win display

---

## Supporting Scripts

### 5. **GameModeManager.cs** - Game Mode Detection
**Location**: `/Assets/Script/Managers/GameModeManager.cs`

#### Functions using IsMultiplayer:
- **`Awake()`** - Detects if SmartFoxServer is connected and sets `IsMultiplayer` flag on all slot components

### 6. **GameplayNetworkManager.cs** - Network Communication
**Location**: `/Assets/Script/Server/GameplayNetworkManager.cs`

#### Functions using IsMultiplayer:
- **`OnJoinRoomResponse()`** - Handles multiplayer room joining and initial board setup
- **`OnBetDeducted()`** - Processes bet deduction from server
- **`OnSpinResult()`** - Processes spin results from server
- **`IsSlotGameReady()`** - Checks if multiplayer game is ready

---

## Additional Scripts with IsMultiplayer Usage

### 7. **SlotWins.cs** - Win Processing
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/SlotWins.cs`
- **`setServerWinData()`** - Sets win data from server (multiplayer only)

### 8. **SlotLines.cs** - Payline Management  
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/SlotLines.cs`
- **`createPaylinesAfterServerData()`** - Creates paylines after server data is received (multiplayer only)

### 9. **SlotCompute.cs** - Win Calculation
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/SlotCompute.cs`
- **`processServerWinData()`** - Processes win data from server (multiplayer only)

### 10. **UI Button Scripts**
**Location**: `/Assets/SlotCreatorPro/Example/BeachDays/scripts/`
- **`SpinButton.cs`**, **`MaxBetButton.cs`**, **`BetButton.cs`**
- These scripts check `IsMultiplayer` to determine whether to use local or server-based interactions

---

## Summary

**Total Scripts Using IsMultiplayer**: 21 files

**Core Separation Logic**:
- **Single-Player Mode**: Local RNG, local credit management, offline validation, PlayerPrefs storage
- **Multiplayer Mode**: Server RNG, server credit management, server validation, network communication

**Key Benefits**:
1. **Code Reusability**: Same codebase supports both modes
2. **Clear Separation**: Easy to identify single-player vs multiplayer logic
3. **Security**: Multiplayer mode ensures server-side validation and fairness
4. **Flexibility**: Can switch between modes by changing a single boolean flag

This architecture allows the slot machine to function both as a standalone offline game and as a server-connected multiplayer experience with minimal code duplication.