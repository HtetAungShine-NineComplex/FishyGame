using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;

public class BeachDaysGUI : MonoBehaviour
{

	public Text bet;
	public Text lines;
	public Text won;
	public Text credits;
	public Text winReadout;

	public Text scatter5;
	public Text scatter6;
	public Text scatter7;

	public Leveling leveling;
	public Text level;

	public Scatters scatters;

	Slot slot;
	NumberFormatInfo nfi;

	void Start()
	{
		slot = GameObject.Find("BeachDays").GetComponent<Slot>();

		nfi = new CultureInfo("en-US", false).NumberFormat;
		nfi.CurrencyDecimalDigits = 0;

		updateUI();
	}

	void Update()
	{
		//--- SHARED CODE (BOTH MODES) ---
		// UI state management used by both multiplayer and single-player modes
		switch (slot.state)
		{
			case SlotState.playingwins:
				//--- MULTIPLAYER SERVER CODE START ---
				// MULTIPLAYER: Display server-provided win data (don't override multiplayer win display)
				//--- MULTIPLAYER SERVER CODE END ---
				//--- SINGLE-PLAYER LOCAL CODE START ---
				// SINGLE-PLAYER: Display locally calculated win data
				//--- SINGLE-PLAYER LOCAL CODE END ---
				//--- SHARED CODE (BOTH MODES) ---
				// Win display logic used by both modes
				if (slot.refs.wins.currentWin == null) return;

				if (slot.refs.wins.isBetweenWins())
					winReadout.text = "";
				else
					winReadout.text = slot.refs.wins.currentWin.readout.ToString();
				
				// Only update won display if not showing multiplayer win
				if (!slot.IsMultiplayer || !isShowingMultiplayerWin)
				{
					updateWon();
				}
				updateCredits();
				updateScatters();
				break;
			case SlotState.spinning:
				//--- SHARED CODE (BOTH MODES) ---
				// Spinning state display used by both modes
				if (!slot.IsMultiplayer || !isShowingMultiplayerWin)
				{
					won.text = "GOOD LUCK!";
				}
				winReadout.text = "";
				updateScatters();
				break;

			default:
				//--- SHARED CODE (BOTH MODES) ---
				// Default state display used by both modes
				if (!slot.IsMultiplayer || !isShowingMultiplayerWin)
				{
					won.text = "";
				}
				winReadout.text = "";
				updateScatters();
				break;
		}
	}

	public void updateUI()
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Full UI update used by both modes
		updateBet();
		updateWon();
		updateCredits();
		updateLines();
		updateLevel();
		updateScatters();
	}
	void updateBet()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Display current bet (visual only, server validates actual bets)
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Display current bet (matches local validation)
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		bet.text = slot.refs.credits.totalBet().ToString();
	}
	void updateWon()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Display server-provided win amounts
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Display locally calculated win amounts
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		won.text = slot.refs.credits.lastWin.ToString();
	}
	public void updateCredits()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Display server-managed credits
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Display locally managed credits
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		credits.text = slot.refs.credits.totalCreditsReadout().ToString();
		Debug.Log($"[BeachDaysGUI] Updated credits display to: {credits.text}");
	}
	void updateLines()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Display current lines played (visual only, server validates actual lines)
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Display current lines played (matches local validation)
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		lines.text = slot.refs.credits.linesPlayed.ToString();
	}
	void updateLevel()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Display level (could be server-managed in full implementation)
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Display level (local visual feature)
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		level.text = leveling.GetLevel().ToString();
	}
	void updateScatters()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Display scatter values (could be server-managed in real implementation)
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Display scatter values (local progressive feature)
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- SHARED CODE (BOTH MODES) ---
		scatter5.text = scatters.GetScatter(0).ToString("C", nfi);
		scatter6.text = scatters.GetScatter(1).ToString("C", nfi);
		scatter7.text = scatters.GetScatter(2).ToString("C", nfi);
	}

	//--- MULTIPLAYER SERVER CODE START ---
	/// <summary>
	/// MULTIPLAYER: Flag to prevent Update method from overriding multiplayer win display
	/// </summary>
	private bool isShowingMultiplayerWin = false;

	/// <summary>
	/// MULTIPLAYER: Show win amount in the won text field
	/// </summary>
	public void showWinAmount(int winAmount)
	{
		if (winAmount > 0)
		{
			won.text = winAmount.ToString();
			isShowingMultiplayerWin = true;
			Debug.Log($"[BeachDaysGUI] Showing win amount: {winAmount}");
		}
		else
		{
			won.text = "";
			isShowingMultiplayerWin = false;
		}
	}

	/// <summary>
	/// MULTIPLAYER: Clear win display (for no-win spins)
	/// </summary>
	public void clearWinDisplay()
	{
		won.text = "";
		isShowingMultiplayerWin = false;
		Debug.Log("[BeachDaysGUI] Cleared win display");
	}

	/// <summary>
	/// MULTIPLAYER: Show "Good Luck" message during reel animation
	/// </summary>
	public void showGoodLuck()
	{
		won.text = "GOOD LUCK!";
		isShowingMultiplayerWin = true; // Prevent Update method from overriding
		Debug.Log("[BeachDaysGUI] Showing Good Luck message");
	}
	//--- MULTIPLAYER SERVER CODE END ---
}