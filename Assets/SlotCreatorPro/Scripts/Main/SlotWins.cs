// Brad Lima - 11/2019
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


enum PlayingWinsState
{
	starting,
	playing,
	between,
	suspended
}
public class SlotWins : MonoBehaviour
{

	public float showWinTime = 2.0f;
	public float delayBetweenShowingWins = 0.25f;
	public SlotWinData currentWin = null;

	private PlayingWinsState playingstate = PlayingWinsState.starting;
	private List<GameObject> winboxes = new List<GameObject>();
	private List<GameObject> lineboxes = new List<GameObject>();
	private float winTimeout = 0;
	private int winLineOffset = 0;
	private int winDisplayCount = 0;
	private bool cycled = false;
	private Slot slot;

	//--- MULTIPLAYER SERVER CODE START ---
	// MULTIPLAYER: Win data comes from server instead of local calculation
	private List<SlotWinData> serverWinData = new List<SlotWinData>();
	//--- MULTIPLAYER SERVER CODE END ---

	private bool pause = false;
	#region Start
	void Start()
	{

		slot = GetComponent<Slot>();
	}
	#endregion

	#region Update
	void Update()
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Win display state machine used by both multiplayer and single-player modes
		SlotState currentSlotState = GetComponent<Slot>().state;
		if (currentSlotState == SlotState.playingwins)
		{
			Debug.Log($"[FLOW] SlotWins.Update: In playingwins state, playingstate={playingstate}, serverWinData.Count={serverWinData?.Count ?? 0}");
		}
		
		switch (currentSlotState)
		{
			case SlotState.playingwins:
				switch (playingstate)
				{
					case PlayingWinsState.starting:
						Debug.Log($"[FLOW] PlayingWinsState.starting - calling showWin(), serverWinData.Count={serverWinData?.Count ?? 0}");
						//if (!pause)
						//{
						showWin();
						playingstate = PlayingWinsState.playing;
						//}
						break;
					case PlayingWinsState.playing:
						if (winTimeout > showWinTime)
						{
							//currentWin.readout = "";
							winTimeout = 0;
							releaseWinBoxes();
							releaseLineBoxes();
							playingstate = PlayingWinsState.between;
							slot.beginDelayOnWin(currentWin);
							return;
						}
						winTimeout += Time.deltaTime;
						break;
					case PlayingWinsState.between:
						if (pause)
						{
							playingstate = PlayingWinsState.suspended;
							return;
						}
						if (winTimeout > delayBetweenShowingWins)
						{
							playingstate = PlayingWinsState.playing;
							showWin();
							winTimeout = 0;
							return;
						}
						winTimeout += Time.deltaTime;
						break;
					case PlayingWinsState.suspended:
						if (!pause) playingstate = PlayingWinsState.between;
						break;
				}
				break;
		}
	}
	#endregion

	#region MULTIPLAYER SERVER CODE
	/// <summary>
	/// MULTIPLAYER: Sets win data received from SmartFoxServer
	/// Called when server sends win results for display
	/// </summary>
	public void setServerWinData(List<SlotWinData> winData)
	{
		//--- MULTIPLAYER SERVER CODE START ---
		if (slot.IsMultiplayer)
		{
			Debug.Log($"[FLOW] setServerWinData called with {winData?.Count ?? 0} wins on instance {GetInstanceID()}");
			slot.log($"[FLOW] Setting server win data: {winData?.Count ?? 0} wins");
			serverWinData.Clear();
			serverWinData.AddRange(winData);
			Debug.Log($"[FLOW] serverWinData now contains {serverWinData.Count} wins on instance {GetInstanceID()}");
			if (winData != null && winData.Count > 0)
			{
				for (int i = 0; i < winData.Count; i++)
				{
					Debug.Log($"[FLOW] Win {i}: line={winData[i].lineNumber}, paid={winData[i].paid}, matches={winData[i].matches}, symbols={winData[i].symbols?.Count ?? 0}");
				}
			}
		}
		else
		{
			slot.log("Ignoring server win data in single-player mode");
		}
		//--- MULTIPLAYER SERVER CODE END ---
	}

	/// <summary>
	/// MULTIPLAYER: Returns server-provided win data for processing
	/// </summary>
	public List<SlotWinData> GetSlotWinData()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		if (slot.IsMultiplayer)
		{
			Debug.Log($"[FLOW] GetSlotWinData called on instance {GetInstanceID()}, returning {serverWinData?.Count ?? 0} wins");
			return serverWinData;
		}
		else
		{
			// Single-player mode: return local computation results
			return slot.refs.compute.lineResultData;
		}
		//--- MULTIPLAYER SERVER CODE END ---
	}
	#endregion

	#region SHARED CODE (BOTH MODES)
	public void resetStateOnly()
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Reset only the playing state, keep win data intact
		Debug.Log($"[FLOW] resetStateOnly(): NOT clearing serverWinData. Current count: {serverWinData?.Count ?? 0}");
		playingstate = PlayingWinsState.starting;
		releaseWinBoxes();
		releaseLineBoxes();
		winLineOffset = -1;
		winTimeout = 0;
		winDisplayCount = 0;
		cycled = false;
		currentWin = null;
		slot.refs.lines.hideLines();
	}

	public void reset()
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Reset win display state used by both modes
		playingstate = PlayingWinsState.starting;
		releaseWinBoxes();
		releaseLineBoxes();
		//CancelInvoke(resumeWinsInvoke);
		//currentWin.readout = "";
		winLineOffset = -1;
		winTimeout = 0;
		winDisplayCount = 0;

		cycled = false;
		currentWin = null;

		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Clear server win data
		if (slot.IsMultiplayer)
		{
			Debug.Log($"[FLOW] reset(): Clearing serverWinData which had {serverWinData?.Count ?? 0} wins");
			serverWinData.Clear();
		}
		//--- MULTIPLAYER SERVER CODE END ---

		//--- SHARED CODE (BOTH MODES) ---
		slot.refs.lines.hideLines();
	}

	public bool isBetweenWins()
	{
		return playingstate == PlayingWinsState.between;
	}
	public void pauseWins()
	{
		pause = true;
		//releaseWinBoxes();
		//releaseLineBoxes();
	}
	public void resumeWins()
	{
		pause = false;
		//Invoke("resumeWinsInvoke", inTime);
	}
	void resumeWinsInvoke()
	{

	}
	public void releaseLineBoxes()
	{
		foreach (GameObject wb in lineboxes) Destroy(wb);
		lineboxes.Clear();
	}
	public void releaseWinBoxes()
	{
		if (slot.usePool)
		{
			for (int i = 0; i < winboxes.Count; i++)
			{
				//GameObject wb = winboxes[i];
				slot.returnWinboxToPool(winboxes[i], currentWin.symbols[i].GetComponent<SlotSymbol>().symbolIndex);
			}
		}
		else
		{
			foreach (GameObject wb in winboxes) Destroy(wb);
		}
		winboxes.Clear();

	}
	#endregion

	#region Show Wins
	int findNextWin()
	{
		winLineOffset++;

		//--- MULTIPLAYER SERVER CODE START ---
		if (slot.IsMultiplayer)
		{
			// MULTIPLAYER: Use server win data instead of local computation
				Debug.Log($"[FLOW] findNextWin: serverWinData.Count = {serverWinData?.Count ?? 0}, winLineOffset was {winLineOffset-1}, now {winLineOffset}");
			if (serverWinData != null && serverWinData.Count > 0)
			{
				for (int i = 0; i < serverWinData.Count; i++)
				{
					Debug.Log($"[FLOW] findNextWin: serverWinData[{i}]: line={serverWinData[i].lineNumber}, paid={serverWinData[i].paid}, matches={serverWinData[i].matches}");
				}
			}
			if (serverWinData == null || serverWinData.Count == 0) 
			{
				Debug.LogError("findNextWin: serverWinData is null or empty, returning -1");
				return -1;
			}
			if (winLineOffset > serverWinData.Count - 1)
			{
				winLineOffset = 0;
				cycled = true;
				if (cycled)
				{
					winDisplayCount++;
					GetComponent<Slot>().completedWinDisplayCycle(winDisplayCount);
				}
			}
		}
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		else
		{
			// SINGLE-PLAYER: Use local computation results
			if (slot.refs.compute.lineResultData.Count == 0) return -1;
			if (winLineOffset > slot.refs.compute.lineResultData.Count - 1)
			{
				winLineOffset = 0;
				cycled = true;
				if (cycled)
				{
					winDisplayCount++;
					GetComponent<Slot>().completedWinDisplayCycle(winDisplayCount);
				}
			}
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---

		return winLineOffset;
	}

	void showWin()
	{
		Debug.Log($"[FLOW] === showWin() CALLED === IsMultiplayer: {slot.IsMultiplayer}, state: {slot.state}, serverWinData.Count: {serverWinData?.Count ?? 0}");

		winLineOffset = findNextWin();
		Debug.Log($"[FLOW] findNextWin() returned: {winLineOffset}, serverWinData.Count: {serverWinData?.Count ?? 0}");

		if (winLineOffset == -1)
		{
			Debug.LogWarning("[FLOW] findNextWin() returned -1, no wins found! serverWinData.Count: " + (serverWinData?.Count ?? 0));
			return;
		}

		winTimeout = 0;

		//--- MULTIPLAYER SERVER CODE START ---
		if (slot.IsMultiplayer)
		{
			// MULTIPLAYER: Use server win data instead of local computation
			Debug.Log($"[FLOW] showWin: Using serverWinData[{winLineOffset}], serverWinData.Count: {serverWinData?.Count ?? 0}");
			if (serverWinData == null || serverWinData.Count == 0)
			{
				Debug.LogError($"[FLOW] showWin ERROR: serverWinData is null or empty! Count: {serverWinData?.Count ?? 0}");
				Debug.LogError($"[FLOW] showWin ERROR: This should not happen - server data should be preserved!");
				return;
			}
			if (winLineOffset >= serverWinData.Count)
			{
				Debug.LogError($"[FLOW] showWin ERROR: winLineOffset {winLineOffset} >= serverWinData.Count {serverWinData.Count}");
				return;
			}
			currentWin = serverWinData[winLineOffset];
			Debug.Log($"[FLOW] showWin: currentWin set from serverWinData[{winLineOffset}]: lineNumber={currentWin.lineNumber}, paid={currentWin.paid}, symbols={currentWin.symbols?.Count ?? 0}");
		}
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		else
		{
			// SINGLE-PLAYER: Use local computation results
			currentWin = slot.refs.compute.lineResultData[winLineOffset];
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---

		Debug.Log($"[FLOW] showWin: About to call displayedWinLine with win: Line {currentWin.lineNumber}, Symbols: {currentWin.symbols?.Count ?? 0}");

		//--- SHARED CODE (BOTH MODES) ---
		// Visual win display logic used by both modes
		Debug.Log($"[FLOW] showWin: Calling displayedWinLine for line {currentWin.lineNumber}");
		GetComponent<Slot>().displayedWinLine(currentWin, !cycled);

		//for (int i = 0; i < slot.displayLineBoxes; i++)
		int winSymbolCount = slot.reels.Count;
		switch (currentWin.setType)
		{
			case SetsType.normal:
				break;
			case SetsType.anywhere:
				winSymbolCount = currentWin.symbols.Count;
				break;
			case SetsType.scatter:
				winSymbolCount = currentWin.symbols.Count;
				break;
		}

		Debug.Log($"[FLOW] showWin: Starting win animation loop: winSymbolCount={winSymbolCount}, matches={currentWin.matches}");
		for (int i = 0; i < winSymbolCount; i++)
		{
			//if ((currentWin.setType == SetsType.normal) && (i >= slot.displayLineBoxes)) continue;
			Debug.Log($"[FLOW] showWin: Processing symbol {i}: i < matches? {i < currentWin.matches}");
			if (i < currentWin.matches)
			{
				//if (slot.displayLineBoxes[currentWin.symbols[i].GetComponent<SlotSymbol>().reelIndex] == false) continue;
				//Debug.Log ("reel:" + currentWin.symbols[i].GetComponent<SlotSymbol>().reelIndex);
				Debug.Log($"[FLOW] showWin: Symbol {i}: symbolIndex={currentWin.symbols[i].GetComponent<SlotSymbol>().symbolIndex}");
				if (slot.winboxPrefabs[currentWin.symbols[i].GetComponent<SlotSymbol>().symbolIndex] != null)
				{
					Debug.Log($"[FLOW] showWin: Creating winbox for symbol {i}");
					GameObject winbox;
					if (slot.usePool)
					{
						winbox = slot.getWinboxFromPool(currentWin.symbols[i].GetComponent<SlotSymbol>().symbolIndex);
					}
					else
					{
						winbox = (GameObject)Instantiate(slot.winboxPrefabs[currentWin.symbols[i].GetComponent<SlotSymbol>().symbolIndex]);
					}
					winbox.transform.localScale = Vector3.Scale(winbox.transform.localScale, transform.localScale);
					winbox.transform.parent = currentWin.symbols[i].transform;//.parent.transform;// GetComponent<Slot>().reels[i].symbols[pos[i]].transform;
					winbox.transform.position = currentWin.symbols[i].transform.position;
					winboxes.Add(winbox);
					Debug.Log($"[FLOW] showWin: Winbox created and positioned at {winbox.transform.position}");
				}
				else
				{
					Debug.LogWarning($"[FLOW] showWin: No winbox prefab found for symbol index {currentWin.symbols[i].GetComponent<SlotSymbol>().symbolIndex}");
				}
			}
			else
			{
				//if (slot.displayLineBoxes[i] == false) continue;
				if ((slot.lineboxPrefab != null) && (currentWin.setType != SetsType.scatter || currentWin.setType != SetsType.anywhere))
				{
					GameObject linebox;
					linebox = (GameObject)Instantiate(slot.lineboxPrefab);

					linebox.transform.localScale = Vector3.Scale(linebox.transform.localScale, transform.localScale);

					// Access line data safely when available
					if (currentWin.lineNumber >= 0 && currentWin.lineNumber < slot.lines.Count)
					{
						// Validate reel and symbol exist
						if (i < slot.reels.Count && 
							slot.lines[currentWin.lineNumber].positions[i] < slot.reels[i].symbols.Count)
						{
							linebox.transform.parent = slot.reels[i].symbols[slot.lines[currentWin.lineNumber].positions[i]].transform;
							linebox.transform.position = slot.reels[i].symbols[slot.lines[currentWin.lineNumber].positions[i]].transform.position;
							lineboxes.Add(linebox);
						}
						else
						{
							Debug.LogWarning($"Invalid symbol position for reel {i}, position {slot.lines[currentWin.lineNumber].positions[i]}");
							Destroy(linebox);
						}
					}
					else
					{
						lineboxes.Add(linebox);
					}
				}
			}

		}
	}
	#endregion
}