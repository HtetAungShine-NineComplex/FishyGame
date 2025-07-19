// Brad Lima - 11/2019
//
// This is the customized Callbacks script for Beach Days Slot
//
using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
public class BeachDaysCallbacks : MonoBehaviour
{

	[HideInInspector]
	public Slot slot;
	public BeachDaysGUI deck;
	public Leveling leveling;
	public Scatters scatters;

	void Awake()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Example of setting board result on startup
		// This would now come from server instead of being set locally
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Example of setting the board result on startup locally
		//slot = GetComponent<Slot>();
		//slot.suppliedResult = new int[5,4] { { 1,1,1,1 }, { 1,1,1,1 }, { 1,1,1,1 }, { 1,1,1,1 }, { 1,1,1,1 }};
		//slot.useSuppliedResult = true;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	void Start()
	{
	}

	#region Enable/Disable
	void OnEnable()
	{

		slot = GetComponent<Slot>();

		//--- SHARED CODE (BOTH MODES) ---
		// Event subscriptions used by both multiplayer and single-player modes
		Slot.OnSlotStateChangeTo += OnSlotStateChangeTo;
		Slot.OnSlotStateChangeFrom += OnSlotStateChangeFrom;

		Slot.OnSpinBegin += OnSpinBegin;
		Slot.OnSpinInsufficentCredits += OnSpinInsufficentCredits;
		Slot.OnSpinSnap += OnSpinSnap;
		Slot.OnSpinDone += OnSpinDone;
		Slot.OnSpinDoneNoWins += OnSpinDoneNoWins;
		Slot.OnReelLand += OnReelLand;

		Slot.OnLineWinComputed += OnLineWinComputed;
		Slot.OnLineWinDisplayed += OnLineWinDisplayed;
		Slot.OnAllWinsComputed += OnAllWinsComputed;
		Slot.OnScatterSymbolHit += OnScatterSymbolHit;
		Slot.OnAnticipationScatterBegin += OnAnticipationScatterBegin;

		Slot.OnScatterSymbolLanded += OnScatterSymbolLanded;
		Slot.OnWinDisplayedCycle += OnWinDisplayedCycle;
		Slot.OnLinkedSymbolLanded += OnLinkedSymbolLanded;

		Slot.OnBeginCreditWinCountOff += OnBeginCreditWinCountOff;
		Slot.OnCompletedCreditCountOff += OnCompletedCreditCountOff;

		Slot.OnSymbolReturningToPool += OnSymbolReturningToPool;
	}

	#endregion

	void OnDisable()
	{

		//--- SHARED CODE (BOTH MODES) ---
		// Event unsubscriptions used by both modes
		Slot.OnSlotStateChangeTo -= OnSlotStateChangeTo;
		Slot.OnSlotStateChangeFrom -= OnSlotStateChangeFrom;

		Slot.OnSpinBegin -= OnSpinBegin;
		Slot.OnSpinInsufficentCredits -= OnSpinInsufficentCredits;
		Slot.OnSpinSnap -= OnSpinSnap;
		Slot.OnSpinDone -= OnSpinDone;
		Slot.OnSpinDoneNoWins -= OnSpinDoneNoWins;

		Slot.OnLineWinDisplayed -= OnLineWinDisplayed;
		Slot.OnAllWinsComputed -= OnAllWinsComputed;

		Slot.OnScatterSymbolHit -= OnScatterSymbolHit;
		Slot.OnAnticipationScatterBegin -= OnAnticipationScatterBegin;

		Slot.OnScatterSymbolLanded -= OnScatterSymbolLanded;
		Slot.OnWinDisplayedCycle -= OnWinDisplayedCycle;
		Slot.OnLinkedSymbolLanded -= OnLinkedSymbolLanded;

		Slot.OnBeginCreditWinCountOff -= OnBeginCreditWinCountOff;
		Slot.OnCompletedCreditCountOff -= OnCompletedCreditCountOff;

		Slot.OnSymbolReturningToPool -= OnSymbolReturningToPool;

	}

	#region Update Callback

	private void OnSlotUpdate()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Any update logic would be presentation-only
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Local update logic
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}
	#endregion


	#region State Callbacks 

	private void OnSlotStateChangeFrom(SlotState state)
	{
		slot.log("onSlotStateChangeFrom " + state);
		switch (state)
		{
			case SlotState.playingwins:
				//--- MULTIPLAYER SERVER CODE START ---
				// MULTIPLAYER: Visual state changes only
				//--- MULTIPLAYER SERVER CODE END ---
				//--- SINGLE-PLAYER LOCAL CODE START ---
				// SINGLE-PLAYER: Visual state changes only
				//--- SINGLE-PLAYER LOCAL CODE END ---
				break;
			case SlotState.ready:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state changes used by both modes
				break;
			case SlotState.snapping:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state changes used by both modes
				break;
			case SlotState.spinning:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state changes used by both modes
				break;
		}
	}
	public void OnSlotStateChangeTo(SlotState state)
	{
		slot.log("OnSlotStateChangeTo " + state);
		switch (state)
		{
			case SlotState.playingwins:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state changes used by both modes
				break;
			case SlotState.ready:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state changes used by both modes
				break;
			case SlotState.snapping:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state changes used by both modes
				break;
			case SlotState.spinning:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state changes used by both modes
				break;
		}
	}
	#endregion

	#region Spin Callbacks
	private void OnSpinBegin(SlotWinData data)
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual effects only - hide lines and award XP for bet used by both modes
		slot.refs.lines.hideLines();
		leveling.AwardXp(slot.refs.credits.totalBet());
		slot.log("OnSpinBegin Callback");
	}

	private void OnSpinInsufficentCredits()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Visual feedback for insufficient credits (server validated)
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Visual feedback for insufficient credits (locally validated)
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		slot.log("OnSpinInsufficentCredits Callback");
	}

	void OnReelLand(int obj)
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual effects for reel landing used by both modes
	}

	private void OnSpinSnap()
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual effects for spin snap used by both modes
		slot.log("OnSpinSnap Callback");
	}

	int freespins = 0;
	private void OnSpinDone(int totalWon, int timesWin)
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Frozen positions demo (local feature only)
		// This would be server-controlled in multiplayer mode
		/*
		if (!slot.IsMultiplayer && freespins < 10)
		{
			DOTween.Sequence().AppendInterval(1).AppendCallback(()=> {

				int[,] result = slot.getResultArray();

				for (int i = 0; i < result.GetLength(0); i++)
				{
					for (int ii = 0; ii < result.GetLength(1); ii++)
					{
						if (result[i,ii] == 1)
							slot.frozenPositions[i, ii] = 1;
					}
				}
				
				slot.spin(true);
				freespins++;
			});
		} else {
			slot.resetFrozenPositions();
		}
		*/
		//--- SINGLE-PLAYER LOCAL CODE END ---

		//--- SHARED CODE (BOTH MODES) ---
		slot.log("OnSpinDone Callback");
	}

	private void OnSpinDoneNoWins()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Visual feedback for no wins (server determined)
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Visual feedback for no wins (locally determined)
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		slot.log("OnSpinDoneNoWins Callback");
	}

	#endregion

	#region Win Callbacks
	private void OnLineWinComputed(SlotWinData win)
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Process server-provided win data for display
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Process locally calculated win data for display
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		slot.log("OnLineWinComputed Callback");

		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Example of parsing server win data for special bonuses
		// This logic would now be handled by server in multiplayer, but visual effects can still be triggered
		/*
		if (!slot.IsMultiplayer)
		{
			int extraWin = 10;
			
			SlotWinData winLine = slot.refs.compute.lineResultData.Find(item => item.setName == "Any Sevens");
			if (winLine != null)
			{
				List<SlotSymbol> symbols = new List<SlotSymbol>();
				winLine.symbols.ForEach(item => symbols.Add(item.GetComponent<SlotSymbol>()));

				if (symbols.Find(item => item.reelIndex == 0 && item.symbolIndex == INDEX_OF_RED_SEVEN_SYMBOL) &&
					symbols.Find(item => item.reelIndex == 1 && item.symbolIndex == INDEX_OF_WHITE_SEVEN_SYMBOL) &&
					symbols.Find(item => item.reelIndex == 2 && item.symbolIndex == INDEX_OF_BLUE_SEVEN_SYMBOL))
					{
						slot.refs.credits.awardWin(extraWin);
					}
			}
		}
		*/
		//--- SINGLE-PLAYER LOCAL CODE END ---

		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Jackpot logic (server handles jackpot calculations in multiplayer)
		/*
		if (!slot.IsMultiplayer)
		{
			int matches = 5;
			while (matches < 8) {
				// search line results for the set name and the matches
				int jackPot = slot.refs.compute.lineResultData.FindIndex(item => item.setName == "Jackpot" && item.matches == matches);

				if (jackPot > -1) {
					win.setPaid(scatters.GetScatter(matches-5));
					slot.refs.compute.lineResultData[jackPot] = win;
					scatters.ResetScatter(matches-5);
					matches=8;
				}
				matches++;
			}
		}
		*/
		//--- SINGLE-PLAYER LOCAL CODE END ---

		//--- SHARED CODE (BOTH MODES) ---
		slot.log("win line " + win.lineNumber + " :: set: " + win.setName + " (" + win.setIndex + ") paid: " + win.paid + " matches: " + win.matches);
	}

	private void OnLineWinDisplayed(SlotWinData win, bool isFirstLoop)
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual animations for win data used by both modes
		Debug.Log($"=== OnLineWinDisplayed CALLED ===");
		Debug.Log($"Win Line: {win.lineNumber}, Symbols Count: {win.symbols.Count}, IsMultiplayer: {slot.IsMultiplayer}");

		// Iterate through symbols that make up the win
		for (int i = 0; i < win.symbols.Count; i++)
		{
			GameObject symbol = win.symbols[i];
			Debug.Log($"Symbol {i}: {symbol.name}, Has Animator: {symbol.GetComponent<Animator>() != null}");

			// if there is an animator component, play the win animation
			if (symbol.GetComponent<Animator>())
			{
				symbol.GetComponent<Animator>().SetTrigger("playwin");
				Debug.Log($"Triggered 'playwin' animation on symbol {i}: {symbol.name}");
			}
			else
			{
				Debug.LogWarning($"Symbol {i} ({symbol.name}) has no Animator component!");
			}
		}
		slot.log("OnLineWinDisplayed Callback");
		slot.log("win line " + win.lineNumber + " :: set: " + win.setName + " (" + win.setIndex + ") paid: " + win.paid + " matches: " + win.matches);
	}

	private void OnAllWinsComputed(SlotWinSpin win, int timesBet)
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Display all server-computed wins
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Display all locally computed wins
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		slot.log("OnAllWinsComputed Callback");
	}

	private void OnScatterSymbolLanded(GameObject symbol, int count)
	{

		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Visual effects for server-determined scatter symbols
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Visual effects for locally determined scatter symbols
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		// we don't want to play the scatter animations if the reels are snapping
		if (slot.state == SlotState.snapping) return;

		// trigger animation when scatter symbol lands if there is one
		Animator anim = symbol.GetComponent<Animator>();
		if (anim)
		{
			anim.SetTrigger("playwin");
		}

	}

	private void OnScatterSymbolHit(SlotScatterHitData hit)
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Visual effects for server-determined scatter hits
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Visual effects for locally determined scatter hits
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		slot.log("OnScatterSymbolHit Callback");
	}

	private void OnAnticipationScatterBegin(SlotScatterHitData hit)
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Visual effects for server-determined scatter anticipation
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Visual effects for locally determined scatter anticipation
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		slot.log("OnAnticipationScatterBegin Callback");
	}

	public void OnLinkedSymbolLanded(int reel, string linkName)
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Visual effects for server-determined linked symbols
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Visual effects for locally determined linked symbols
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		slot.log("OnLinkedSymbolLanded:" + reel + " : " + linkName);
		Debug.Log("OnLinkedSymbolLanded:" + reel + " : " + linkName);
	}

	void OnWinDisplayedCycle(int count)
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual effects for win display cycling used by both modes
		slot.log("OnWinDisplayedCycle Callback");
	}

	void OnBeginCreditWinCountOff(int obj)
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual effects for credit count-off start used by both modes
	}

	void OnCompletedCreditCountOff(int obj)
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual effects for credit count-off completion used by both modes
	}

	void OnSymbolReturningToPool(GameObject obj)
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual cleanup when symbols return to pool used by both modes
	}

	#endregion
}