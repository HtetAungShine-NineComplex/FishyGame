using Sfs2X.Core;
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayNetworkManager : MonoBehaviour
{
    [SerializeField]
    private string _gameRoom = "fish";

    [Header("Fishing Game References")]
    public List<PlayerManager> _playerMangers;
    public List<FishSpawnData> fishSpawnDatas;
    public Dictionary<string, FishManager> fishManagerDic;
    private string currentSessionToken;

    [Header("Slot Game References")]
    public Slot currentSlot;
    public bool isSlotGameActive = false;

    private void Awake()
    {
        fishManagerDic = new Dictionary<string, FishManager>();
        if (GlobalManager.Instance != null)
        {
            GlobalManager.Instance.ExtensionResponse += OnExtensionResponse;
        }

        foreach (FishSpawnData data in fishSpawnDatas)
        {
            fishManagerDic.Add(data.fishName, data.manager);
        }

        // Find slot if it exists in scene
        if (currentSlot == null)
        {
            currentSlot = FindObjectOfType<Slot>();
        }
    }

    private void Start()
    {
        Debug.Log($"GameplayNetworkManager Start() called on {gameObject.name} - Instance: {GetInstanceID()}");
        Debug.Log($"Stack trace: {System.Environment.StackTrace}");
        GlobalManager.Instance.RequestJoinRoom(_gameRoom);
    }

    private void OnDestroy()
    {
        GlobalManager.Instance.ExtensionResponse -= OnExtensionResponse;
    }

    #region Network Message Routing

    private void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string)evt.Params["cmd"];
        ISFSObject sfsobject = (SFSObject)evt.Params["params"];

        switch (cmd)
        {
            case "errorResponse":
                Debug.LogError("SPIN ERROR: " + sfsobject.GetDump());
                break;

            // Existing fishing game commands
            case "roomPlayerList":
                OnRoomPlayerList(sfsobject);
                break;

            case "shoot":
                OnShoot(sfsobject);
                break;

            case "fishSpawn":
                OnFishSpawn(sfsobject);
                break;

            case "stageUpdate":
                OnStageUpdate(sfsobject);
                break;

            // New slot game commands

            case "joinRoomResponse":
                OnJoinRoomResponse(sfsobject);
                break;

            case "slotGameState":
                OnSlotGameState(sfsobject);
                break;

            case "spinResult":
                OnSpinResult(sfsobject);
                break;

            case "jackpotUpdate":
                OnJackPotUpdate(sfsobject);
                break;

            case "slotSpinResponse":
                OnSlotSpinResponse(sfsobject);
                break;

            case "slotBetChangeResponse":
                OnSlotBetChangeResponse(sfsobject);
                break;

            case "slotCreditUpdate":
                OnSlotCreditUpdate(sfsobject);
                break;

            case "slotError":
                OnSlotError(sfsobject);
                break;

            default:
                Debug.LogWarning("Unknown command received: " + cmd);
                break;
        }
    }

    #endregion

    #region Fishing Game Methods (Existing)

    private PlayerManager GetPlayerByName(string name)
    {
        foreach (PlayerManager player in _playerMangers)
        {
            if (player.playerName == name)
            {
                return player;
            }
        }

        return null;
    }

    private void OnRoomPlayerList(ISFSObject obj)
    {
        ISFSArray sFSArray = obj.GetSFSArray("userArray");

        for (int i = 0; i < sFSArray.Size(); i++)
        {
            ISFSObject data = sFSArray.GetSFSObject(i);

            string userName = data.GetUtfString("userName");
            int balance = data.GetInt("balance");

            _playerMangers[i].SetPlayerData(userName, balance, userName == GlobalManager.Instance.GetSfsClient().MySelf.Name);
        }
    }

    private void OnShoot(ISFSObject obj)
    {

        string userName = obj.GetUtfString("userName");
        int balance = obj.GetInt("balance");
        float z = obj.GetFloat("zRotation");

        if (userName != GlobalManager.Instance.GetSfsClient().MySelf.Name)
        {
            GetPlayerByName(userName).OnNetworkShoot(z, balance);
        }
    }

    void OnFishSpawn(ISFSObject data)
    {

        string fishType = data.GetUtfString("fishType");
        float normalizedX = data.GetFloat("x");
        float normalizedY = data.GetFloat("y");
        string spawnSide = data.GetUtfString("spawnSide");
        float endX = data.GetFloat("endX");
        float endY = data.GetFloat("endY");

        // Convert normalized spawn position to world position
        Vector3 spawnPoint = SpawnpointManager.Instance.GetSpawnPointOnline(normalizedX, normalizedY, spawnSide);
        Vector3 endPoint = SpawnpointManager.Instance.GetEndPointOnline(endX, endY);

        // Spawn the fish
        //SpawnFish(fishType, spawnPoint, endPoint);
        fishManagerDic[fishType].SpawnFish(spawnPoint, endPoint);
    }

    void OnStageUpdate(ISFSObject data)
    {
        int newStage = data.GetInt("stage");

        Debug.Log("Stage changed to: " + newStage);

        switch (newStage)
        {
            case 1:
                WaveManager.Instance.NormalStage();
                break;

            case 2:
                WaveManager.Instance.BossStage();
                break;

            case 3:
                WaveManager.Instance.BonusStage();
                break;

            default:
                break;
        }
    }

    #endregion

    #region Slot Game Methods (New)

    public void SetSlotGameActive(Slot slot)
    {
        currentSlot = slot;
        isSlotGameActive = true;

        // Request initial game state from server
        // RequestSlotGameState();
    }

    public void SetSlotGameInactive()
    {
        isSlotGameActive = false;
        currentSlot = null;
    }

    // Send slot spin request to server

    public void SendSlotSpinRequest(int betPerLine, int linesPlayed, bool freeSpin = false)
    {
        if (!isSlotGameActive || string.IsNullOrEmpty(currentSessionToken))
        {
            Debug.LogError("Slot game not active or no valid session token");
            return;
        }

        // Create the data object that contains the spin parameters
        SFSObject dataObject = new SFSObject();
        dataObject.PutInt("betPerLine", betPerLine);
        dataObject.PutInt("linesPlayed", linesPlayed);
        dataObject.PutBool("freeSpin", freeSpin);
        dataObject.PutUtfString("sessionToken", GetCurrentSessionToken());

        // Create the main request object with the data nested inside
        SFSObject requestData = new SFSObject();
        requestData.PutUtfString("action", "SPIN_REQUEST");
        requestData.PutSFSObject("data", dataObject);

        // Send request via GlobalManager
        GlobalManager.Instance.SendToExtension("SPIN_REQUEST", requestData);
        Debug.Log($"Sent slot spin request: bet={betPerLine}, lines={linesPlayed}, freeSpin={freeSpin}");
    }

    // Send bet change request to server
    public void SendSlotBetChangeRequest(int newBetPerLine, int newLinesPlayed)
    {
        if (!isSlotGameActive || currentSlot == null)
        {
            Debug.LogError("Slot game not active or slot not found");
            return;
        }

        // Create the data object that contains the bet parameters
        SFSObject dataObject = new SFSObject();
        dataObject.PutInt("betPerLine", newBetPerLine);
        dataObject.PutInt("linesPlayed", newLinesPlayed);

        // Create the main request object with the data nested inside
        SFSObject requestData = new SFSObject();
        requestData.PutUtfString("action", "SET_BET");
        requestData.PutSFSObject("data", dataObject);

        // Send request via GlobalManager
        GlobalManager.Instance.SendToExtension("SET_BET", requestData);
        Debug.Log($"Sent bet change request: bet={newBetPerLine}, lines={newLinesPlayed}");
    }

    // Request initial game state
    private void RequestSlotGameState()
    {
        SFSObject requestData = new SFSObject();
        requestData.PutUtfString("action", "GET_GAME_STATE");

        // Send request via GlobalManager
        GlobalManager.Instance.SendToExtension("GET_GAME_STATE", requestData);
        Debug.Log("Requested slot game state");
    }

    private void OnJoinRoomResponse(ISFSObject data)
    {
        Debug.Log("OnJoinRoomResponse received");

        bool success = data.GetBool("success");

        if (success)
        {
            if (data.ContainsKey("sessionToken"))
            {
                currentSessionToken = data.GetUtfString("sessionToken");
                Debug.Log("Session token received and stored: " + currentSessionToken);
            }

            string roomName = data.GetUtfString("roomName");
            int roomId = data.GetInt("roomId");
            int credits = data.GetInt("credits");
            int betPerLine = data.GetInt("betPerLine");
            int linesPlayed = data.GetInt("linesPlayed");
            int userCount = data.GetInt("userCount");
            int maxUsers = data.GetInt("maxUsers");
            string message = data.GetUtfString("message");

            // Update slot credits and bet settings
            currentSlot.refs.credits.updateCreditsFromServer(credits);
            currentSlot.refs.credits.betPerLine = betPerLine;
            currentSlot.refs.credits.linesPlayed = linesPlayed;

            // Handle initial board from server (multiplayer mode)
            if (currentSlot.IsMultiplayer && data.ContainsKey("initialBoard"))
            {
                try
                {
                    ISFSArray initialBoardArray = data.GetSFSArray("initialBoard");

                    Debug.Log($"Server sent initialBoard with {initialBoardArray.Size()} reels");

                    for (int r = 0; r < initialBoardArray.Size(); r++)
                    {
                        ISFSArray reelData = initialBoardArray.GetSFSArray(r);
                        Debug.Log($"Reel {r} has {reelData.Size()} symbols: [{string.Join(",", GetReelSymbols(reelData))}]");
                    }

                    int[,] initialBoard = ParseReelResults(initialBoardArray);

                    Debug.Log($"Parsed initialBoard: {initialBoard.GetLength(0)} reels x {initialBoard.GetLength(1)} symbols");

                    for (int reel = 0; reel < initialBoard.GetLength(0); reel++)
                    {
                        List<int> reelSymbols = new List<int>();
                        for (int row = 0; row < initialBoard.GetLength(1); row++)
                        {
                            reelSymbols.Add(initialBoard[reel, row]);
                        }
                        Debug.Log($"Parsed reel {reel}: [{string.Join(",", reelSymbols)}]");
                    }
                    DistributeInitialBoardToReels(initialBoard);

                    currentSlot.suppliedResult = initialBoard;
                    currentSlot.useSuppliedResult = true;

                    Debug.Log("Received and distributed initial board from server for multiplayer mode");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error processing initial board: " + e.Message);
                    Debug.LogError("Stack trace: " + e.StackTrace);
                    currentSlot.useSuppliedResult = false;
                }
            }

            isSlotGameActive = true;
            Debug.Log($"isSlotGameActive: {isSlotGameActive}");
            Debug.Log($"currentSessionToken: {currentSessionToken}");
            Debug.Log($"sessionToken isEmpty: {string.IsNullOrEmpty(currentSessionToken)}");

            InitializeSlotGame();
        }
        else
        {
            string error = data.GetUtfString("error");
            string roomName = data.GetUtfString("roomName");
            Debug.LogError($"Failed to join room {roomName}: {error}");
            isSlotGameActive = false;
            currentSessionToken = null;
        }
    }

    private List<int> GetReelSymbols(ISFSArray reelData)
    {
        List<int> symbols = new List<int>();
        for (int i = 0; i < reelData.Size(); i++)
        {
            symbols.Add(reelData.GetInt(i));
        }
        return symbols;
    }

    private void DistributeInitialBoardToReels(int[,] initialBoard)
    {
        if (initialBoard == null || currentSlot == null)
        {
            Debug.LogError("DistributeInitialBoardToReels: initialBoard or currentSlot is null");
            return;
        }

        int reelCount = initialBoard.GetLength(0);
        int symbolCount = initialBoard.GetLength(1);

        Debug.Log($"DistributeInitialBoardToReels: {reelCount} reels x {symbolCount} symbols");
        Debug.Log($"currentSlot.REEL_COUNT: {currentSlot.numberOfReels}");
        Debug.Log($"currentSlot.reels.Count: {currentSlot.reels.Count}");

        for (int reel = 0; reel < reelCount && reel < currentSlot.numberOfReels; reel++)
        {
            Debug.Log($"Processing reel {reel}");

            List<int> reelSymbols = new List<int>();

            for (int row = 0; row < symbolCount; row++)
            {
                try
                {
                    int symbolIndex = initialBoard[reel, row];
                    Debug.Log($"Reel {reel}, Row {row}: Symbol {symbolIndex}");
                    reelSymbols.Add(symbolIndex);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error accessing initialBoard[{reel}, {row}]: {e.Message}");
                    break;
                }
            }

            // Set the symbols for this specific reel
            if (currentSlot.reels.ContainsKey(reel))
            {
                Debug.Log($"Setting {reelSymbols.Count} symbols for reel {reel}");
                currentSlot.reels[reel].setServerSymbolData(reelSymbols);

                // ✅ Call createReelSymbolsFromServer after setting server data
                currentSlot.reels[reel].createReelSymbolsFromServer();

                Debug.Log($"Successfully created symbols for reel {reel}");
            }
            else
            {
                Debug.LogError($"Reel {reel} not found in currentSlot.reels dictionary");
            }
        }

        // ✅ After all reels are created, create paylines
        Debug.Log("All reels processed, now creating paylines");
        SlotLines slotLines = currentSlot.GetComponent<SlotLines>();
        if (slotLines != null)
        {
            slotLines.createPaylinesAfterServerData();
        }
        else
        {
            Debug.LogError("SlotLines component not found!");
        }
    }

    private string GetCurrentSessionToken()
    {
        return currentSessionToken;
    }

    private void InitializeSlotGame()
    {
        Debug.Log("Slot game initialized with session token");
    }
    private void OnSlotGameState(ISFSObject data)
    {
        if (!isSlotGameActive || currentSlot == null) return;

        try
        {
            int credits = data.GetInt("credits");
            int betPerLine = data.GetInt("betPerLine");
            int linesPlayed = data.GetInt("linesPlayed");

            Debug.Log($"Received slot game state: credits={credits}, bet={betPerLine}, lines={linesPlayed}");

            Debug.Log("User Offline Credits: " + currentSlot.refs.credits);
            // Update slot with server state
            currentSlot.refs.credits.updateCreditsFromServer(credits);
            currentSlot.refs.credits.betPerLine = betPerLine;
            currentSlot.refs.credits.linesPlayed = linesPlayed;
        }
        catch (Exception e)
        {
            Debug.LogError("Error processing slot game state: " + e.Message);
        }
    }

    private void OnSpinResult(ISFSObject data)
    {
        if (!isSlotGameActive || currentSlot == null) return;

        if (currentSlot.IsMultiplayer)
        {
            try
            {
                Debug.Log("OnSpinResult received - processing server spin data");
                Debug.Log("Full response: " + data.GetDump());

                // Check if data object exists
                if (!data.ContainsKey("data"))
                {
                    Debug.LogError("Server response missing 'data' object");
                    return;
                }

                ISFSObject dataObj = data.GetSFSObject("data");
                if (dataObj == null)
                {
                    Debug.LogError("Server 'data' object is null");
                    return;
                }

                // Check if gameState exists
                if (!dataObj.ContainsKey("gameState"))
                {
                    Debug.LogError("Server response missing 'gameState' object");
                    return;
                }

                ISFSObject gameState = dataObj.GetSFSObject("gameState");
                if (gameState == null)
                {
                    Debug.LogError("Server 'gameState' object is null");
                    return;
                }

                // Convert server data to client format
                int[,] reelResults = ConvertServerReelResults(dataObj);
                List<SlotWinData> winData = ConvertServerWinData(dataObj);
                int totalWon = (int)gameState.GetLong("totalWin");
                int newCredits = (int)gameState.GetLong("newBalance");

                Debug.Log($"Converted spin result: totalWon={totalWon}, newCredits={newCredits}, wins={winData.Count}");

                // Feed converted data to existing slot logic
                currentSlot.ProcessServerSpinResponse(reelResults, winData, totalWon, newCredits);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error processing spin result: " + e.Message);
                Debug.LogError("Stack trace: " + e.StackTrace);
            }
        }
    }

    // Convert server reel results to client format
    private int[,] ConvertServerReelResults(ISFSObject dataObj)
    {
        ISFSArray reelResultsArray = dataObj.GetSFSArray("reelResults");
        return ParseReelResults(reelResultsArray); // Use existing method
    }

    // Convert server win data (combine winLines + scatterWins) to client format
    private List<SlotWinData> ConvertServerWinData(ISFSObject dataObj)
    {
        List<SlotWinData> winData = new List<SlotWinData>();

        // Process win lines
        ISFSArray winLinesArray = dataObj.GetSFSArray("winLines");
        for (int i = 0; i < winLinesArray.Size(); i++)
        {
            ISFSObject winObj = winLinesArray.GetSFSObject(i);
            SlotWinData win = ConvertWinLineToSlotWinData(winObj);
            winData.Add(win);
        }

        // Process scatter wins
        ISFSArray scatterWinsArray = dataObj.GetSFSArray("scatterWins");
        for (int i = 0; i < scatterWinsArray.Size(); i++)
        {
            ISFSObject scatterObj = scatterWinsArray.GetSFSObject(i);
            SlotWinData win = ConvertScatterToSlotWinData(scatterObj);
            winData.Add(win);
        }

        return winData;
    }

    // Convert server win line to client SlotWinData format (simplified)
    private SlotWinData ConvertWinLineToSlotWinData(ISFSObject winObj)
    {
        SlotWinData win = new SlotWinData(winObj.GetInt("lineNumber"));
        win.paid = winObj.GetInt("winAmount");
        win.matches = winObj.GetInt("matches");
        win.setIndex = winObj.GetInt("symbolIndex");
        
        // Generate basic readout without symbol name (client has symbol mapping)
        win.readout = $"LINE {win.lineNumber + 1} PAYS {win.paid}";

        // Client will calculate positions from payline definition
        win.symbols = new List<GameObject>();

        return win;
    }

    // Convert server scatter win to client SlotWinData format (simplified)
    private SlotWinData ConvertScatterToSlotWinData(ISFSObject scatterObj)
    {
        SlotWinData win = new SlotWinData(-1); // -1 for scatter (no line)
        win.paid = scatterObj.GetInt("winAmount");
        win.matches = scatterObj.GetInt("scatterCount");
        win.setIndex = scatterObj.GetInt("symbolIndex");
        
        // Generate basic readout without symbol name
        win.readout = $"{win.matches} SCATTERS PAY {win.paid}";

        // Client will find scatter positions from reel results
        win.symbols = new List<GameObject>();

        return win;
    }


    private void OnJackPotUpdate(ISFSObject data)
    {
        Debug.Log("OnJackPotUpdate <<!!>>");
    }

    // Handle spin response from server
    private void OnSlotSpinResponse(ISFSObject data)
    {
        if (!isSlotGameActive || currentSlot == null) return;

        try
        {
            // Parse reel results
            ISFSArray reelResultsArray = data.GetSFSArray("reelResults");
            int[,] reelResults = ParseReelResults(reelResultsArray);

            // Parse win data
            ISFSArray winDataArray = data.GetSFSArray("winData");
            List<SlotWinData> winData = ParseWinData(winDataArray);

            // Get totals
            int totalWon = data.GetInt("totalWon");
            int newCredits = data.GetInt("newCredits");

            Debug.Log($"Received spin response: totalWon={totalWon}, newCredits={newCredits}, wins={winData.Count}");

            // Process the server response
            ProcessSlotSpinResponse(reelResults, winData, totalWon, newCredits);
        }
        catch (Exception e)
        {
            Debug.LogError("Error processing slot spin response: " + e.Message);
        }
    }

    // Handle bet change confirmation
    private void OnSlotBetChangeResponse(ISFSObject data)
    {
        if (!isSlotGameActive || currentSlot == null) return;

        try
        {
            bool success = data.GetBool("success");

            if (success)
            {
                int newBetPerLine = data.GetInt("betPerLine");
                int newLinesPlayed = data.GetInt("linesPlayed");

                Debug.Log($"Bet change confirmed: bet={newBetPerLine}, lines={newLinesPlayed}");

                // Update local display
                currentSlot.refs.credits.betPerLine = newBetPerLine;
                currentSlot.refs.credits.linesPlayed = newLinesPlayed;
            }
            else
            {
                string errorMessage = data.GetUtfString("error");
                Debug.LogWarning("Bet change rejected: " + errorMessage);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error processing bet change response: " + e.Message);
        }
    }

    // Handle credit updates
    private void OnSlotCreditUpdate(ISFSObject data)
    {
        if (!isSlotGameActive || currentSlot == null) return;

        try
        {
            int newCredits = data.GetInt("credits");
            string reason = data.GetUtfString("reason");

            Debug.Log($"Credit update: {newCredits} (reason: {reason})");

            currentSlot.refs.credits.updateCreditsFromServer(newCredits);
        }
        catch (Exception e)
        {
            Debug.LogError("Error processing credit update: " + e.Message);
        }
    }

    // Handle slot errors
    private void OnSlotError(ISFSObject data)
    {
        try
        {
            string errorType = data.GetUtfString("errorType");
            string errorMessage = data.GetUtfString("message");

            Debug.LogError($"Slot error ({errorType}): {errorMessage}");

            // Handle specific error types
            switch (errorType)
            {
                case "INSUFFICIENT_CREDITS":
                    if (currentSlot != null)
                    {
                        // Trigger insufficient credits callback through the slot instance
                        currentSlot.TriggerInsufficientCreditsEvent();
                    }
                    break;
                case "INVALID_BET":
                    Debug.LogWarning("Invalid bet parameters");
                    break;
                default:
                    Debug.LogError("Unknown slot error: " + errorMessage);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error processing slot error: " + e.Message);
        }
    }

    // Parse reel results from server data
    private int[,] ParseReelResults(ISFSArray reelResultsArray)
    {
        try
        {
            int reelCount = reelResultsArray.Size();

            if (reelCount == 0)
            {
                Debug.LogError("ParseReelResults: No reels in server data!");
                return new int[0, 0];
            }

            ISFSArray firstReel = reelResultsArray.GetSFSArray(0);
            int symbolCount = firstReel.Size();

            if (symbolCount == 0)
            {
                Debug.LogError("ParseReelResults: First reel has no symbols!");
                return new int[0, 0];
            }

            Debug.Log($"ParseReelResults: Creating {reelCount}x{symbolCount} array");

            int[,] results = new int[reelCount, symbolCount];

            for (int reel = 0; reel < reelCount; reel++)
            {
                ISFSArray reelArray = reelResultsArray.GetSFSArray(reel);

                if (reelArray.Size() != symbolCount)
                {
                    Debug.LogError($"ParseReelResults: Reel {reel} has {reelArray.Size()} symbols, expected {symbolCount}");
                    continue;
                }

                for (int symbol = 0; symbol < symbolCount; symbol++)
                {
                    results[reel, symbol] = reelArray.GetInt(symbol);
                }
            }

            return results;
        }
        catch (Exception e)
        {
            Debug.LogError($"ParseReelResults error: {e.Message}");
            Debug.LogError($"ParseReelResults stack trace: {e.StackTrace}");

            return new int[0, 0];
        }
    }

    // Parse win data from server data
    private List<SlotWinData> ParseWinData(ISFSArray winDataArray)
    {
        List<SlotWinData> winData = new List<SlotWinData>();

        for (int i = 0; i < winDataArray.Size(); i++)
        {
            ISFSObject winObj = winDataArray.GetSFSObject(i);

            SlotWinData win = new SlotWinData(winObj.GetInt("lineNumber"));
            win.paid = winObj.GetInt("paid");
            win.matches = winObj.GetInt("matches");
            win.setIndex = winObj.GetInt("setIndex");
            win.setName = winObj.GetUtfString("setName");
            win.readout = winObj.GetUtfString("readout");

            // Parse symbol positions for visual display
            ISFSArray symbolPositions = winObj.GetSFSArray("symbolPositions");
            win.symbols = new List<GameObject>();

            // Convert symbol positions to actual symbol GameObjects
            for (int j = 0; j < symbolPositions.Size(); j++)
            {
                ISFSObject symbolPos = symbolPositions.GetSFSObject(j);
                int reelIndex = symbolPos.GetInt("reel");
                int symbolIndex = symbolPos.GetInt("position");

                if (currentSlot != null && currentSlot.reels.ContainsKey(reelIndex))
                {
                    var reel = currentSlot.reels[reelIndex];
                    if (symbolIndex < reel.symbols.Count)
                    {
                        win.symbols.Add(reel.symbols[symbolIndex]);
                    }
                }
            }

            winData.Add(win);
        }

        return winData;
    }

    // Process complete spin response
    private void ProcessSlotSpinResponse(int[,] reelResults, List<SlotWinData> winData, int totalWon, int newCredits)
    {
        if (currentSlot == null) return;

        // Update credits
        currentSlot.refs.credits.updateCreditsFromServer(newCredits);

        // Set reel results for display
        currentSlot.suppliedResult = reelResults;
        currentSlot.useSuppliedResult = true;
        currentSlot.waitingForResult = false;

        // Set win data for display
        currentSlot.refs.wins.setServerWinData(winData);
        currentSlot.refs.compute.processServerWinData(winData, totalWon);

        Debug.Log("Slot spin response processed successfully");
    }

    #endregion

    #region Public Interface for Slot Integration

    public bool IsSlotGameReady()
    {
        // Check if game is active, slot exists, and SFS client is connected
        return isSlotGameActive && currentSlot != null &&
               GlobalManager.Instance != null &&
               GlobalManager.Instance.GetSfsClient() != null &&
               GlobalManager.Instance.GetSfsClient().IsConnected;
    }

    public void HandleSlotSpin(int betPerLine, int linesPlayed, bool freeSpin = false)
    {
        if (IsSlotGameReady())
        {
            SendSlotSpinRequest(betPerLine, linesPlayed, freeSpin);
        }
        else
        {
            Debug.LogError("Cannot send slot spin - game not ready or not connected");
        }
    }

    public void HandleSlotBetChange(int newBetPerLine, int newLinesPlayed)
    {
        if (IsSlotGameReady())
        {
            SendSlotBetChangeRequest(newBetPerLine, newLinesPlayed);
        }
        else
        {
            Debug.LogError("Cannot change bet - game not ready or not connected");
        }
    }

    #endregion
}

[Serializable]
public class FishSpawnData
{
    public string fishName;
    public FishManager manager;
}