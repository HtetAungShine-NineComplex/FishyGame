// Brad Lima - 11/2019
//
using UnityEngine;
using DG.Tweening;
using aSlot;

public class SlotCredits : MonoBehaviour
{

	[HideInInspector]
	public bool persistant = true;
	[HideInInspector]
	public int betPerLineDefaultIndex;
	//public int maxBetPerLine = 5;
	[HideInInspector]
	public int betPerLineIndex = 0;
	[HideInInspector]
	public int credits;
	[HideInInspector]
	public int queue;
	[HideInInspector]
	public int countingQueue;
	[HideInInspector]
	public int lastWin;
	[HideInInspector]
	public int betPerLine = 1;
	[HideInInspector]
	public int linesPlayed = 1;

	//--- MULTIPLAYER SERVER CODE START ---
	// These are display-only values from server in multiplayer mode
	// In single-player mode, these are calculated locally
	//--- MULTIPLAYER SERVER CODE END ---
	public int totalIn;
	public int totalOut;

	private Tweener creditsTween;

	private Slot slot;


	void Awake()
	{
		restore();
	}
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{

	}

	#region Save/Restore machine
	public void restore()
	{
		slot = GetComponent<Slot>();
		if (persistant)
		{
			slot.log("restoring slot settings");

			//--- MULTIPLAYER SERVER CODE START ---
			// MULTIPLAYER: Don't restore credits from local storage - server manages credits
			//--- MULTIPLAYER SERVER CODE END ---
			//--- SINGLE-PLAYER LOCAL CODE START ---
			if (!slot.IsMultiplayer)
			{
				// SINGLE-PLAYER: Restore credits from local storage
				credits = PlayerPrefs.GetInt(slot.name + "_credits", credits);
			}
			//--- SINGLE-PLAYER LOCAL CODE END ---

			//--- SHARED CODE (BOTH MODES) ---
			// User preferences saved locally in both modes
			betPerLineIndex = PlayerPrefs.GetInt(slot.name + "_betPerLineIndex", betPerLineIndex);
			betPerLine = slot.betsPerLine[betPerLineIndex].value;
			linesPlayed = PlayerPrefs.GetInt(slot.name + "_linesPlayed", linesPlayed);

			for (int index = 0; index < slot.betsPerLine.Count; index++)
			{
				slot.betsPerLine[index].canBet = PlayerPrefsX.GetBool(slot.name + "_betsPerLine" + index, slot.betsPerLine[index].canBet);
			}
			if (linesPlayed > slot.lines.Count) linesPlayed = slot.lines.Count;
		}
		else
		{
			if (betPerLineDefaultIndex > slot.betsPerLine.Count) { slot.logConfigError("Your machine default bet per line index is greater than the actual bets per line"); return; }
			betPerLine = slot.betsPerLine[betPerLineDefaultIndex].value;
			betPerLineIndex = betPerLineDefaultIndex;
		}
	}

	void save()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Don't save credits locally - server manages credits
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		if (!slot.IsMultiplayer)
		{
			// SINGLE-PLAYER: Save credits locally
			PlayerPrefs.SetInt(slot.name + "_credits", credits);
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---

		//--- SHARED CODE (BOTH MODES) ---
		// User preferences saved locally in both modes
		PlayerPrefs.SetInt(slot.name + "_betPerLineIndex", betPerLineIndex);
		PlayerPrefs.SetInt(slot.name + "_betPerLine", betPerLine);
		PlayerPrefs.SetInt(slot.name + "_linesPlayed", linesPlayed);
		for (int index = 0; index < slot.betsPerLine.Count; index++)
		{
			PlayerPrefsX.SetBool(slot.name + "_betsPerLine" + index, slot.betsPerLine[index].canBet);
		}
	}
	#endregion

	#region Credit Management
	public void setCredits(int creds)
	{
		slot.log("setting credits = " + creds);
		credits = creds;
	}
	public void depositCredits(int deposit)
	{
		slot.log("setting slot credits = " + deposit);
		credits += deposit;
		//HOTween.To (this, 2.0f, new TweenParms().Prop ("credits", credits + deposit).Ease(EaseType.EaseOutBack));
	}

	public int withdrawCredits()
	{
		return credits;
	}

	//--- MULTIPLAYER SERVER CODE START ---
	/// <summary>
	/// MULTIPLAYER: Updates credits from server response
	/// Called when server sends updated credit balance
	/// </summary>
	public void updateCreditsFromServer(int newCredits)
	{
		if (slot == null)
			slot = GetComponent<Slot>();

		// Only update from server in multiplayer mode
		if (slot.IsMultiplayer)
		{
			slot.log("Updating credits from server: " + newCredits);
			Debug.Log($"[SlotCredits] Updating credits from {credits} to {newCredits}");
			credits = newCredits;

			// Force completion of any active credit counting animations
			finishCreditCount();

			// Update UI to reflect new credits
			Debug.Log($"[SlotCredits] Triggering UI update for new credits: {newCredits}");
			updateUI();

			// Save the updated credits (visual preferences, not actual balance in multiplayer)
			if (persistant)
			{
				save();
			}
		}
		else
		{
			slot.log("Ignoring server credit update in single-player mode");
		}
	}

	/// <summary>
	/// Trigger UI update - finds and calls updateCredits on GUI components
	/// </summary>
	private void updateUI()
	{
		// Find the GUI component and update credits display
		BeachDaysGUI gui = FindObjectOfType<BeachDaysGUI>();
		if (gui != null)
		{
			// Debug.Log("[SlotCredits] Found BeachDaysGUI, calling updateCredits()");
			gui.updateCredits();
		}
		else
		{
			Debug.LogWarning("[SlotCredits] No BeachDaysGUI found, credits UI may not update");
		}
	}
	/// <summary>
	/// MULTIPLAYER: Immediately update balance after bet deduction (no animation)
	/// </summary>
	public void balanceDecrease(int newBalance)
	{
		if (slot == null)
			slot = GetComponent<Slot>();

		// Only update in multiplayer mode
		if (slot.IsMultiplayer)
		{
			// Debug.Log($"[SlotCredits] Immediately updating balance from {credits} to {newBalance}");

			// Stop any existing animation
			finishCreditCount();

			// Update balance immediately without animation
			credits = newBalance;
			updateUI();

			// Debug.Log($"[SlotCredits] Balance updated immediately: {credits}");
		}
		else
		{
			slot.log("Ignoring balance decrease in single-player mode");
		}
	}

	/// <summary>
	/// MULTIPLAYER: Display win amount and animate balance increase
	/// </summary>
	public void animateWinAndBalanceIncrease(int winAmount, int finalBalance)
	{
		if (slot == null)
			slot = GetComponent<Slot>();

		// Only animate in multiplayer mode
		if (slot.IsMultiplayer)
		{
			Debug.Log($"[SlotCredits] Processing win: {winAmount}, final balance: {finalBalance}");

			if (winAmount > 0)
			{
				// Show win amount in BeachDaysGUI.won text field
				BeachDaysGUI gui = FindObjectOfType<BeachDaysGUI>();
				if (gui != null)
				{
					gui.showWinAmount(winAmount);
					Debug.Log($"[SlotCredits] Displayed win amount {winAmount} in BeachDaysGUI.won");
				}
				else
				{
					Debug.LogWarning("[SlotCredits] No BeachDaysGUI found to display win amount");
				}

				// Store win amount
				lastWin = winAmount;

				// Animate balance increase from current to final
				float animationTime = Mathf.Clamp(winAmount * 0.0001f, 1.0f, 3.0f);
				creditsTween = DOTween.To(() => this.credits, x => this.credits = x, finalBalance, animationTime)
					.OnUpdate(() =>
					{
						// Update UI during animation to show smooth balance increase
						updateUI();
					})
					.OnComplete(() =>
					{
						Debug.Log($"[SlotCredits] Win animation completed: {credits}");
						updateUI();
					});

				// Trigger win display UI callbacks
				slot.beginCreditWinCountOff(winAmount);
			}
			else
			{
				// No win, just update balance immediately
				credits = finalBalance;
				updateUI();

				// Clear win display
				BeachDaysGUI gui = FindObjectOfType<BeachDaysGUI>();
				if (gui != null)
				{
					gui.clearWinDisplay();
				}
			}
		}
		else
		{
			slot.log("Ignoring win animation in single-player mode");
		}
	}
	//--- MULTIPLAYER SERVER CODE END ---

	#endregion

	#region Betting functions
	public int totalCreditsReadout()
	{
		return credits + countingQueue;
	}
	public int totalCredits()
	{
		return credits + queue;
	}
	public int totalBet()
	{
		return betPerLine * linesPlayed;
	}

	public void betMaxPerLine()
	{
		int max = -1;
		for (int index = 0; index < slot.betsPerLine.Count; index++)
		{
			BetsWrapper bet = slot.betsPerLine[index];
			if ((bet.canBet) && (bet.value > max))
			{
				max = index;
			}
		}
		betPerLineIndex = max;
		betPerLine = slot.betsPerLine[max].value;

		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Send bet change to server
		// TODO: Implement server communication for bet changes
		//--- MULTIPLAYER SERVER CODE END ---
	}
	public void incBetPerLine()
	{
		slot = GetComponent<Slot>();
		switch (slot.state)
		{
			case SlotState.ready:

				betPerLineIndex++;
				if (betPerLineIndex > slot.betsPerLine.Count - 1) betPerLineIndex = 0;
				while (!slot.betsPerLine[betPerLineIndex].canBet)
				{
					betPerLineIndex++;
					if (betPerLineIndex > slot.betsPerLine.Count - 1) betPerLineIndex = 0;
				}
				betPerLine = slot.betsPerLine[betPerLineIndex].value;
				slot.incrementedBet(betPerLine);

				//--- MULTIPLAYER SERVER CODE START ---
				// MULTIPLAYER: Send bet change to server
				// TODO: Implement server communication for bet changes
				//--- MULTIPLAYER SERVER CODE END ---

				break;
			case SlotState.playingwins:
				slot.setState(SlotState.ready);
				incBetPerLine();
				break;
		}

		save();
	}

	public void decBetPerLine()
	{
		slot = GetComponent<Slot>();
		switch (slot.state)
		{
			case SlotState.ready:

				betPerLineIndex--;
				if (betPerLineIndex < 0) betPerLineIndex = slot.betsPerLine.Count - 1;
				while (!slot.betsPerLine[betPerLineIndex].canBet)
				{
					betPerLineIndex--;
					if (betPerLineIndex < 0) betPerLineIndex = slot.betsPerLine.Count - 1;
				}
				betPerLine = slot.betsPerLine[betPerLineIndex].value;
				slot.decrementedBet(betPerLine);

				//--- MULTIPLAYER SERVER CODE START ---
				// MULTIPLAYER: Send bet change to server
				// TODO: Implement server communication for bet changes
				//--- MULTIPLAYER SERVER CODE END ---

				break;
			case SlotState.playingwins:
				slot.setState(SlotState.ready);
				decBetPerLine();
				break;
		}

		save();
	}
	public void incLinesPlayed()
	{
		switch (slot.state)
		{
			case SlotState.ready:
				linesPlayed++;
				if (linesPlayed > slot.lines.Count) { linesPlayed = 1; }
				slot.incrementedLinesPlayed(linesPlayed);
				slot.refs.lines.displayLines(linesPlayed);

				//--- MULTIPLAYER SERVER CODE START ---
				// MULTIPLAYER: Send lines change to server
				// TODO: Implement server communication for lines changes
				//--- MULTIPLAYER SERVER CODE END ---
				break;
			case SlotState.playingwins:
				slot.setState(SlotState.ready);
				slot.refs.wins.reset();
				incLinesPlayed();
				break;
		}

		save();
	}

	public void decLinesPlayed()
	{
		switch (slot.state)
		{
			case SlotState.ready:
				linesPlayed--;
				if (linesPlayed < 1) { linesPlayed = slot.lines.Count - 1; }
				slot.decrementedLinesPlayed(linesPlayed);
				slot.refs.lines.displayLines(linesPlayed);

				//--- MULTIPLAYER SERVER CODE START ---
				// MULTIPLAYER: Send lines change to server
				// TODO: Implement server communication for lines changes
				//--- MULTIPLAYER SERVER CODE END ---
				break;
			case SlotState.playingwins:
				slot.setState(SlotState.ready);
				slot.refs.wins.reset();
				decLinesPlayed();
				break;
		}

		save();
	}

	public bool placeBet()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		if (slot.IsMultiplayer)
		{
			// MULTIPLAYER: Always return true, let server handle validation
			// Server will reject spin if insufficient credits
			finishCreditCount();
			return true;
		}
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		else
		{
			// SINGLE-PLAYER: Local bet validation and processing
			if (!canPlaceBet()) return false;

			finishCreditCount();
			credits -= totalBet();
			totalIn += totalBet();
			save();
			return true;
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	public bool canPlaceBet()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		if (slot.IsMultiplayer)
		{
			// MULTIPLAYER: Always return true, let server handle validation
			return true;
		}
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		else
		{
			// SINGLE-PLAYER: Local credit validation
			if (totalCredits() < totalBet())
			{
				return false;
			}
			return true;
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}
	#endregion

	#region Win Display
	public void awardWin(int amount)
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual effects for credit count-off used by both modes
		if (creditsTween != null)
			if (creditsTween.IsActive()) creditsTween.Complete();

		lastWin = amount;
		queue = amount;
		float countOffTime = Mathf.Clamp(amount * 0.05f, 2.0f, 3.0f);

		creditsTween = DOTween.To(() => this.countingQueue, x => this.countingQueue = x, amount, countOffTime).OnComplete(completedCreditTween);
		slot.beginCreditWinCountOff(lastWin);

		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Don't update totalOut locally - server manages this
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		if (!slot.IsMultiplayer)
		{
			// SINGLE-PLAYER: Update totalOut locally
			totalOut += amount;
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---

		//--- SHARED CODE (BOTH MODES) ---
		save();
	}

	public void awardBonus(int amount, bool addToTotalIn = true)
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Visual effects for bonus credit count-off used by both modes
		if (creditsTween != null)
			creditsTween.Complete();

		queue = amount;
		float countOffTime = Mathf.Clamp(amount * 0.0005f, 2.0f, 3.0f);
		creditsTween = DOTween.To(() => this.countingQueue, x => this.countingQueue = x, amount, countOffTime).OnComplete(completedBonusCreditTween);
		slot.beginCreditBonusCountOff(amount);

		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Don't update totalOut locally - server manages this
		//--- MULTIPLAYER SERVER CODE END ---
		//--- SINGLE-PLAYER LOCAL CODE START ---
		if (!slot.IsMultiplayer && addToTotalIn)
		{
			// SINGLE-PLAYER: Update totalOut locally
			totalOut += amount;
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---

		//--- SHARED CODE (BOTH MODES) ---
		save();
	}

	public void finishCreditCount()
	{
		if (creditsTween == null) return;
		if (creditsTween.IsActive()) if (!creditsTween.IsComplete()) creditsTween.Complete();
	}
	void completedBonusCreditTween()
	{
		credits += queue;
		queue = 0;
		countingQueue = 0;
		slot.completedBonusCreditCountOff();
	}
	void completedCreditTween()
	{
		credits += queue;
		queue = 0;
		countingQueue = 0;
		slot.completedCreditCountOff(lastWin);
	}
	public void enableBet(int index)
	{
		slot.betsPerLine[index].canBet = true;
		save();
	}
	public void disableBet(int index)
	{
		slot.betsPerLine[index].canBet = false;
		save();
	}
	#endregion
}