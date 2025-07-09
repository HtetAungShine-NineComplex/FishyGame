// Brad Lima - 11/2019
//
// This class illustrates all callback usage, and demonstrates how you can easily hook into all Slot events,
// along with some simple mechanics used in the examples.
//
// You can use this as the basis for your own slots
//
using UnityEngine;
using System.Collections;

public class SlotCallbacks : MonoBehaviour
{

	[HideInInspector]
	public Slot slot;

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

		Slot.OnLineWinComputed += OnLineWinComputed;
		Slot.OnLineWinDisplayed += OnLineWinDisplayed;
		Slot.OnAllWinsComputed += OnAllWinsComputed;
		Slot.OnScatterSymbolHit += OnScatterSymbolHit;
		Slot.OnAnticipationScatterBegin += OnAnticipationScatterBegin;

		Slot.OnWinDisplayedCycle += OnWinDisplayedCycle;
		Slot.OnLinkedSymbolLanded += OnLinkedSymbolLanded;
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
		Slot.OnLineWinDisplayed -= OnLineWinDisplayed;
		Slot.OnAllWinsComputed -= OnAllWinsComputed;
		Slot.OnScatterSymbolHit -= OnScatterSymbolHit;
		Slot.OnAnticipationScatterBegin -= OnAnticipationScatterBegin;

		Slot.OnWinDisplayedCycle -= OnWinDisplayedCycle;
		Slot.OnLinkedSymbolLanded -= OnLinkedSymbolLanded;
	}

	#region Update Callback

	private void OnSlotUpdate()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Any slot update logic would be presentation-only
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Local slot update logic
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
	private void OnSlotStateChangeTo(SlotState state)
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
		// Visual effects only - hide lines for spin start used by both modes
		slot.refs.lines.hideLines();
		slot.log("OnSpinBegin Callback");
	}

	private void OnSpinInsufficentCredits()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Display insufficient credits message - server validated this
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Display insufficient credits message - locally validated this
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		slot.log("OnSpinInsufficentCredits Callback");
	}

	private void OnSpinSnap()
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual feedback for spin snap used by both modes
		slot.log("OnSpinSnap Callback");
	}

	private void OnSpinDone(int totalWon, int timesWin)
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Display server-calculated results
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Display locally calculated results
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
		// MULTIPLAYER: Display server-computed win data
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Display locally computed win data
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		slot.log("OnLineWinComputed Callback");

		slot.log("win line " + win.lineNumber + " :: set: " + win.setName + " (" + win.setIndex + ") paid: " + win.paid + " matches: " + win.matches);
	}

	private void OnLineWinDisplayed(SlotWinData win, bool isFirstLoop)
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Visual display of server-provided win data
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Visual display of locally computed win data
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
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
		hit.symbol.transform.eulerAngles = new Vector2(0, 0);
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

	#endregion

}