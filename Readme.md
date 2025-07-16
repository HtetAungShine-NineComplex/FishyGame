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

### 4. **GameplayNetworkManager.cs** - Network Communication
**Location**: `/Assets/Script/Server/GameplayNetworkManager.cs`

#### Functions Using IsMultiplayer:

- **`OnJoinRoomResponse()`** (Line 339) - Handles multiplayer room joining:
  - Processes initial board state from server if `IsMultiplayer` is true
  - Sets up initial balance and reel positions
- **`HandleSlotSpin()`** (Line 515) - Sends spin request to server:
  - Only executes if `IsMultiplayer` is true
  - Sends bet amount and line count to SmartFoxServer

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

This architecture allows the slot machine to function both as a standalone offline game and as a server-connected multiplayer experience with minimal code duplication.