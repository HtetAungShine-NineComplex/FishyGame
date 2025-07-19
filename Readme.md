# FishyGame - Client-Side Multiplayer Implementation Documentation

This document provides a comprehensive analysis of all functions that use the `IsMultiplayer` boolean flag to separate single-player and multiplayer logic in the Unity slot machine game.

## Table of Contents
1. [Overview](#overview)
2. [Core Scripts Analysis](#core-scripts-analysis)
   - [Slot.cs](#1-slotcs---main-slot-machine-controller)
   - [SlotCredits.cs](#2-slotcreditscs---credit-and-balance-management)
   - [SlotReel.cs](#3-slotreelcs---individual-reel-management)
   - [GameplayNetworkManager.cs](#4-gameplaynetworkmanagercs---network-communication)
   - [GameModeManager.cs](#5-gamemodemanagercs---game-mode-detection)
   - [BeachDaysGUI.cs](#6-beachdaysguics---user-interface-management)
3. [Supporting Scripts](#supporting-scripts)
4. [Summary](#summary)

---

## Overview

The `IsMultiplayer` flag is the central mechanism that determines whether the slot machine operates in:
- **Single-player mode** (`IsMultiplayer = false`): Local RNG, offline gameplay, PlayerPrefs storage
- **Multiplayer mode** (`IsMultiplayer = true`): Server-controlled RNG, SmartFoxServer communication, server-side validation

---

## Core Scripts Analysis

### 1. **Slot.cs** - Main Slot Machine Controller
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/Slot.cs`

#### Key Variable:
- **`public bool IsMultiplayer`** (Line 45) - Core flag determining game mode
- **`private GameplayNetworkManager networkManager`** - Server communication (multiplayer only)
- **`private int[,] pendingServerResults`** - Server results for delayed application (multiplayer only)

#### Functions Using IsMultiplayer:

- **`Start()`** (Line 423) - Initialization that skips local credit setup in multiplayer mode
- **`ProcessServerSpinResponse()`** (Line 509) - Processes spin results from server (multiplayer only)
- **`IsConnectedToServer()`** (Line 551) - Checks SmartFoxServer connection status (multiplayer only)
- **`ProcessServerSpinResponseWithWins()`** (Line 1400) - Enhanced server result processing with animations (multiplayer only)
- **`StartSpinningAnimation()`** (Line 1250) - Starts reel animation after bet deduction (multiplayer only)
- **`spinWithServerResult()`** (Line 1286) - Uses existing spin infrastructure with server-supplied results (multiplayer only)
- **`StartVisualReelAnimation()`** - Starts visual reel animation without server communication (multiplayer only)
- **`ApplyServerResultsAfterDelay()`** - Coroutine to apply server results after minimum animation duration (multiplayer only)
- **`TriggerInsufficientCreditsEvent()`** - Handles insufficient credits from server or local validation
- **`spin()`** (Line 922) - Main spin method with branching logic:
  - **Multiplayer**: Sends requests to SmartFoxServer via `networkManager.HandleSlotSpin()`
  - **Single-player**: Local bet validation and credit deduction
- **`spinEnd()`** (Line 1039) - Spin completion handler:
  - **Multiplayer**: Uses server-provided win data
  - **Single-player**: Calculates wins locally
- **`getSymbolCountCurrentlyTotal()`** (Line 836) - Counts symbols on reels (single-player only)

---

### 2. **SlotCredits.cs** - Credit and Balance Management
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/SlotCredits.cs`

#### Functions Using IsMultiplayer:

- **`restore()`** (Line 68) - Restore slot settings:
  - **Multiplayer**: Skips credit restoration from PlayerPrefs (server manages credits)
  - **Single-player**: Restores credits from local storage
- **`save()`** (Line 101) - Save slot settings:
  - **Multiplayer**: Skips saving credits locally (server manages credits)
  - **Single-player**: Saves credits to PlayerPrefs
- **`updateCreditsFromServer()`** (Line 149) - Updates credits from server response (multiplayer only)
- **`animateBalanceDecrease()`** (Line 200) - Immediate balance reduction after bet deduction (multiplayer only)
- **`animateWinAndBalanceIncrease()`** (Line 228) - Animates win amount and balance increase (multiplayer only)
- **`placeBet()`** (Line 438) - Place bet logic:
  - **Multiplayer**: Always returns true (server validates)
  - **Single-player**: Local credit validation and deduction
- **`canPlaceBet()`** (Line 464) - Check if bet can be placed:
  - **Multiplayer**: Always returns true (server validates)
  - **Single-player**: Local credit validation
- **`awardWin()`** (Line 503) - Award win credits:
  - **Multiplayer**: Skips `totalOut` update (server manages)
  - **Single-player**: Updates `totalOut` locally
- **`awardBonus()`** (Line 530) - Award bonus credits:
  - **Multiplayer**: Skips `totalOut` update (server manages)
  - **Single-player**: Updates `totalOut` locally

---

### 3. **SlotReel.cs** - Individual Reel Management
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/SlotReel.cs`

#### Key Variables:
- **`public List<int> ReelStrip`** - Local reel strips (single-player only)
- **`private List<int> serverSymbolData`** - Server-provided symbols (multiplayer only)

#### Functions Using IsMultiplayer:

- **`Start()`** (Line 81) - Reel initialization:
  - **Multiplayer**: Skips symbol frequency caching (server provides symbols)
  - **Single-player**: Caches symbol frequency for local RNG
- **`OnEnable()`** (Line 120) - Reel creation:
  - **Multiplayer**: Waits for server-provided symbol data
  - **Single-player**: Creates reels normally with local logic
- **`createReelSymbolsFromServer()`** (Line 139) - Creates reel symbols from server data (multiplayer only)
- **`setServerSymbolData()`** (Line 175) - Sets server-provided symbol data (multiplayer only)
- **`bufferServerSymbols()`** (Line 202) - Buffers extra symbols for smooth animation (multiplayer only)
- **`chooseRandomSymbol()`** (Line 275) - Choose random symbol for reel:
  - **Multiplayer**: Uses server-provided symbol data
  - **Single-player**: Uses local RNG and symbol frequency
- **`spinReel()`** (Line 481) - Start reel spinning:
  - **Multiplayer**: Uses linear easing, skips frequency recaching
  - **Single-player**: Recaches symbol frequency for each spin
- **`stopReelEase()`** (Line 581) - Reel stop animation:
  - **Multiplayer**: Uses OutQuad easing for consistency
  - **Single-player**: Uses configured reel easing
- **`handleScatterAnticipation()`** (Line 616) - Scatter symbol anticipation:
  - **Multiplayer**: Skips local scatter calculation (server handles)
  - **Single-player**: Local scatter detection and anticipation
- **`stopReelOnSymbolIndex()`** (Line 630) - Stop reel on specific symbol:
  - **Multiplayer**: Skips scatter detection (server handles)
  - **Single-player**: Local scatter detection during reel stop

---

### 4. **GameplayNetworkManager.cs** - Network Communication Hub
**Location**: `/Assets/Script/Server/GameplayNetworkManager.cs`

**Purpose**: Central network communication manager handling all client-server slot game interactions, message routing, and data processing.

#### **Key Slot Game Variables:**
- **`public Slot currentSlot`** (Line 19) - Reference to active slot machine instance
- **`public bool isSlotGameActive`** (Line 20) - Flag indicating slot game state
- **`private string currentSessionToken`** (Line 16) - Server session authentication token

#### **Core Initialization & Setup:**

- **`Awake()`** (Line 22-40) - Component initialization:
  - Subscribes to `GlobalManager.ExtensionResponse` event for server messages
  - Auto-finds slot instance if not manually assigned: `FindObjectOfType<Slot>()`
  - Sets up network event handlers

- **`Start()`** (Line 42-57) - Game session startup:
  - Sends slot configuration to server on room join
  - Calls `currentSlot.GetReelConfigurationString()` for server setup
  - Initiates room join request via `GlobalManager.RequestJoinRoom()`

#### **Message Routing & Command Processing:**

- **`OnExtensionResponse()`** (Line 66-144) - Central message dispatcher:
  - Routes 10+ slot-specific server commands to appropriate handlers
  - **Slot Commands Handled**:
    - `"joinRoomResponse"` → Room joining confirmation
    - `"betDeducted"` → Balance deduction confirmation  
    - `"spinResult"` → Main spin results from server
    - `"slotSpinResponse"` → Alternative spin response format
    - `"slotBetChangeResponse"` → Bet change confirmations
    - `"slotCreditUpdate"` → Balance update notifications
    - `"slotError"` → Server error handling
    - `"FREE_SPINS_AWARDED"` → Free spins notifications
    - `"JACKPOT_AWARDED"` → Jackpot win notifications
    - `"JACKPOT_WON_BROADCAST"` → Multi-player jackpot announcements

#### **Client-to-Server Communication:**

- **`SendSlotSpinRequest()`** (Line 256-281) - Main spin request sender:
  - Validates game state and session token
  - Packages bet parameters: `betPerLine`, `linesPlayed`, `freeSpin` flag
  - Sends `"SPIN_REQUEST"` action to server via `GlobalManager.SendToExtension()`
  - Includes session authentication for security

- **`SendSlotBetChangeRequest()`** (Line 284-305) - Bet modification requests:
  - Sends `"SET_BET"` action with new betting parameters
  - Server validates and confirms changes

- **`HandleSlotSpin()`** (Line 999-1010) - Public interface for slot spinning:
  - Checks readiness with `IsSlotGameReady()`
  - Called by slot machine UI buttons
  - **Used only when `IsMultiplayer = true`**

- **`HandleSlotBetChange()`** (Line 1012-1022) - Public interface for bet changes:
  - **Used only when `IsMultiplayer = true`**

#### **Server-to-Client Message Handlers:**

- **`OnJoinRoomResponse()`** (Line 318-399) - Room joining and initialization:
  - Extracts session token for authentication
  - Updates slot credits with server balance: `currentSlot.refs.credits.updateCreditsFromServer()`
  - **Processes initial board state when `IsMultiplayer = true`**:
    - Parses `initialBoard` array from server
    - Distributes symbols to reels via `DistributeInitialBoardToReels()`
    - Sets `currentSlot.suppliedResult` and `useSuppliedResult = true`

- **`OnBetDeducted()`** (Line 471-495) - Balance deduction confirmation:
  - Animates balance decrease: `currentSlot.refs.credits.balanceDecrease()`
  - Confirms bet was successfully processed on server

- **`OnSpinResult()`** (Line 497-557) - **Primary spin result processor**:
  - **Only processes when `IsMultiplayer = true`**
  - Extracts win amount and final balance from server response
  - Logs detailed win/loss information with multipliers
  - Converts server data to client format via `ConvertServerReelResults()` and `ConvertServerWinData()`
  - Triggers slot animation: `currentSlot.spinWithServerResult()`

- **`OnSlotSpinResponse()`** (Line 685-712) - Alternative spin response handler:
  - Parses reel results and win data
  - Calls `ProcessSlotSpinResponse()` for final processing

- **`OnSlotBetChangeResponse()`** (Line 715-744) - Bet change confirmations:
  - Updates local display with confirmed bet values
  - Handles server rejection with error messages

- **`OnSlotCreditUpdate()`** (Line 747-764) - Balance update notifications:
  - Processes balance changes from server events
  - Updates local display via `currentSlot.refs.credits.updateCreditsFromServer()`

- **`OnSlotError()`** (Line 767-798) - Server error handling:
  - Processes different error types: `"INSUFFICIENT_CREDITS"`, `"INVALID_BET"`
  - Triggers appropriate client responses via `currentSlot.TriggerInsufficientCreditsEvent()`

#### **New Feature Handlers (2024):**

- **`OnFreeSpinsAwarded()`** (Line 800-821) - Free spins notifications:
  - Logs free spin awards with spin count information
  - **Extension point**: Add custom UI celebrations here

- **`OnJackpotAwarded()`** (Line 823-845) - Personal jackpot wins:
  - Logs jackpot amount and new balance
  - **Extension point**: Add jackpot celebration UI here

- **`OnJackpotWonBroadcast()`** (Line 847-868) - Multi-player jackpot announcements:
  - Notifies all players when someone wins jackpot
  - **Extension point**: Add spectator notification UI here

#### **Data Processing & Conversion:**

- **`ParseReelResults()`** (Line 871-921) - **Core reel data parser**:
  - Converts server `ISFSArray` to client `int[,]` format
  - Handles variable reel counts and symbol counts
  - Comprehensive error handling for malformed server data

- **`ConvertServerReelResults()`** (Line 560-564) - Reel result converter:
  - Uses `ParseReelResults()` for server spin data

- **`ConvertServerWinData()`** (Line 567-590) - Win data aggregator:
  - Combines win lines and scatter wins from server
  - Returns unified `List<SlotWinData>` for client processing

- **`ConvertWinLineToSlotWinData()`** (Line 593-616) - Win line converter:
  - Maps server win objects to client `SlotWinData` format
  - Handles payline position mapping for symbol highlighting

- **`GetSymbolsFromPaylinePositions()`** (Line 619-659) - **Symbol highlighting mapper**:
  - Converts server payline positions to client `GameObject` references
  - Enables accurate win line animations and symbol highlighting
  - Validates reel and symbol indices for safety

- **`ConvertScatterToSlotWinData()`** (Line 662-676) - Scatter win converter:
  - Handles scatter-specific win data from server

#### **Game State Management:**

- **`SetSlotGameActive()`** (Line 239-246) - Activates slot game mode:
  - Sets current slot reference and active flag
  - **Extension point**: Request initial game state

- **`SetSlotGameInactive()`** (Line 248-252) - Deactivates slot game mode:
  - Cleans up slot references and state

- **`IsSlotGameReady()`** (Line 990-997) - **Readiness validator**:
  - Checks: game active, slot exists, SFS client connected
  - Used by all public interface methods for safety

- **`GetCurrentSessionToken()`** (Line 466-469) - Session authentication:
  - Returns current session token for server requests

#### **Initial Board Setup (Multiplayer):**

- **`DistributeInitialBoardToReels()`** (Line 411-464) - **Initial reel setup**:
  - **Only used when `IsMultiplayer = true`**
  - Distributes server-provided initial symbols to each reel
  - Calls `reel.setServerSymbolData()` and `reel.createReelSymbolsFromServer()`
  - Triggers payline creation: `slotLines.createPaylinesAfterServerData()`

- **`GetReelSymbols()`** (Line 401-409) - Helper for initial board logging:
  - Converts server symbol arrays to readable format for debugging

#### **Legacy/Alternative Handlers:**

- **`ParseWinData()`** (Line 924-964) - Alternative win data parser:
  - Used by `OnSlotSpinResponse()` for different server response format
  - Maps symbol positions to client GameObjects

- **`ProcessSlotSpinResponse()`** (Line 967-984) - Alternative spin processor:
  - Updates credits, sets reel results, processes win data
  - Used by alternative spin response pathway

---

### 5. **GameModeManager.cs** - Game Mode Detection
**Location**: `/Assets/Script/Managers/GameModeManager.cs`

#### Functions Using IsMultiplayer:

- **`Awake()`** (Line 39) - Detects game mode on startup:
  - Sets `slot.IsMultiplayer = useMultiplayer` based on SmartFoxServer connection
  - Determines entire game operation mode

---

### 6. **BeachDaysGUI.cs** - User Interface Management
**Location**: `/Assets/SlotCreatorPro/Example/BeachDays/scripts/BeachDaysGUI.cs`

#### Key Variables:
- **`private bool isShowingMultiplayerWin`** - Flag to prevent Update from overriding multiplayer win display

#### Functions Using IsMultiplayer:

- **`Update()`** - UI state management with multiplayer protection:
  - **Line 60**: `SlotState.playingwins` - Only calls `updateWon()` if not showing multiplayer win
  - **Line 70**: `SlotState.spinning` - Only shows "GOOD LUCK!" if not showing multiplayer win
  - **Line 81**: `default` - Only clears won text if not showing multiplayer win
- **`showWinAmount()`** - Display win amount in won text field (multiplayer only)
- **`clearWinDisplay()`** - Clear win display (multiplayer only)
- **`showGoodLuck()`** - Show "Good Luck" message during reel animation (multiplayer only)

---

## Supporting Scripts

### Additional Scripts with IsMultiplayer Usage:

#### **SlotWins.cs** - Win Processing
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/SlotWins.cs`
- **`setServerWinData()`** - Sets win data from server (multiplayer only)

#### **SlotLines.cs** - Payline Management  
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/SlotLines.cs`
- **`createPaylinesAfterServerData()`** - Creates paylines after server data is received (multiplayer only)

#### **SlotCompute.cs** - Win Calculation
**Location**: `/Assets/SlotCreatorPro/Scripts/Main/SlotCompute.cs`
- **`processServerWinData()`** - Processes win data from server (multiplayer only)

#### **UI Button Scripts**
**Location**: `/Assets/SlotCreatorPro/Example/BeachDays/scripts/`
- **`SpinButton.cs`** - Checks `IsMultiplayer` for spin button behavior
- **`MaxBetButton.cs`** - Checks `IsMultiplayer` for max bet functionality
- **`BetButton.cs`** - Checks `IsMultiplayer` for bet adjustment

#### **RNGManager.cs** - Random Number Generation
**Location**: `/Assets/SlotCreatorPro/Scripts/Core/RNG/RNGManager.cs`
- Used only when `IsMultiplayer = false` for local random number generation

#### **SlotManager.cs** - Slot Management
**Location**: `/Assets/SlotCreatorPro/Example/Scripts/SlotManager.cs`
- High-level slot management with `IsMultiplayer` checks

---

---

## Recent Updates & New Features

### **🎯 Payline Animation Fix (2024)**
**Issue Resolved**: Payline tween animations now work correctly in multiplayer mode
- **Problem**: Server win data was being cleared before animations could use it
- **Solution**: Added `resetStateOnly()` method to preserve server data during animations
- **Files Updated**: `Slot.cs`, `SlotWins.cs`
- **Impact**: Multiplayer mode now shows proper winning line animations just like single-player mode

### **🎁 Free Spins Feature**
**New Feature**: Automatic free spins after reaching configured spin count
- **How it Works**: Players receive 3 free spins after every 2 spins (configurable)
- **Server Logic**: `SlotGame.java` - `checkAndAwardFreeSpins()` and `awardFreeSpins()`
- **Client Logic**: `GameplayNetworkManager.cs` - `OnFreeSpinsAwarded()`
- **Configuration**: Change `freeSpinAwardAfterSpins` in `SlotConfiguration.java`
- **Multiplayer Safe**: Free spins don't deduct balance, proper session management

### **🏆 Jackpot Feature**
**New Feature**: First player to reach spin count wins jackpot
- **How it Works**: First player to reach 3 spins wins 500,000 coins (configurable)
- **Server Logic**: `SlotGame.java` - `checkAndAwardJackpot()` and `awardJackpot()`
- **Client Logic**: `GameplayNetworkManager.cs` - `OnJackpotAwarded()` and `OnJackpotWonBroadcast()`
- **Configuration**: Change `jackpotTriggerAfterSpins` and `jackpotAwardAmount` in `SlotConfiguration.java`
- **Multiplayer Features**: 
  - Only first player wins (prevents duplicate awards)
  - All players in room see jackpot win notifications
  - Proper balance management and session tracking

### **⚙️ Configuration Made Easy**
**File**: `/FishServer/FishyGame/src/game/slot/game/SlotConfiguration.java`
```java
// Easy to change for testing:
public int freeSpinAwardAfterSpins = 2;     // Free spins after X spins
public int freeSpinAwardCount = 3;          // Number of free spins awarded
public int jackpotTriggerAfterSpins = 3;    // Jackpot after X spins  
public long jackpotAwardAmount = 500000;    // Jackpot amount
```

### **🛠️ Technical Improvements**
- **Spin Counting Fix**: Free spins now properly increment spin counter
- **Session Management**: Free spin sessions properly end, balance deduction resumes
- **Debug Logging**: Comprehensive logging for troubleshooting
- **Error Handling**: Robust error handling for all new features
- **State Management**: Proper cleanup of completed sessions

---

## Summary

**Total Scripts Using IsMultiplayer**: 21+ files

**Core Separation Logic**:
- **Single-Player Mode**: Local RNG, local credit management, offline validation, PlayerPrefs storage
- **Multiplayer Mode**: Server RNG, server credit management, server validation, network communication

**Key Benefits**:
1. **Code Reusability**: Same codebase supports both modes
2. **Clear Separation**: Easy to identify single-player vs multiplayer logic
3. **Security**: Multiplayer mode ensures server-side validation and fairness
4. **Flexibility**: Can switch between modes by changing a single boolean flag
5. **New Features**: Free spins and jackpot work seamlessly in both modes
6. **Visual Polish**: Payline animations work consistently across all modes

This architecture allows the slot machine to function both as a standalone offline game and as a server-connected multiplayer experience with minimal code duplication, now enhanced with engaging player retention features.