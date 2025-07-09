// Brad Lima - 11/2019
//
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SlotCompute : MonoBehaviour
{

	public List<SlotWinData> lineResultData = new List<SlotWinData>();
	public SlotWinSpin slotWinSpin;

	private Slot slot;

	#region Start
	void Start()
	{
		slot = GetComponent<Slot>();
	}
	#endregion

	#region MULTIPLAYER SERVER CODE
	/// <summary>
	/// MULTIPLAYER: Processes win data received from SmartFoxServer
	/// Converts server win data into local display format
	/// </summary>
	public SlotWinSpin processServerWinData(List<SlotWinData> serverWinData, int totalWon)
	{
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Process server-provided win data for display
		lineResultData.Clear();
		slotWinSpin.totalWon = 0;
		slotWinSpin.totalWonAdjusted = 0;

		// Copy server win data to local display data
		foreach (SlotWinData winData in serverWinData)
		{
			lineResultData.Add(winData);
			slotWinSpin.totalWon += winData.paid;
			slot.computedWinLine(winData);
			slotWinSpin.totalWonAdjusted += winData.paid;
		}

		// Set total from server
		slotWinSpin.totalWon = totalWon;
		slotWinSpin.totalWonAdjusted = totalWon;

		return slotWinSpin;
		//--- MULTIPLAYER SERVER CODE END ---
	}
	#endregion

	#region SINGLE-PLAYER LOCAL CODE
	/// <summary>
	/// SINGLE-PLAYER: Calculates all line wins locally using built-in algorithm
	/// Only used when IsMultiplayer = false
	/// </summary>
	public SlotWinSpin calculateAllLinesWins()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Local win calculation
		lineResultData.Clear();
		slotWinSpin.totalWon = 0;
		slotWinSpin.totalWonAdjusted = 0;

		if ((slot.lines.Count == 0) || (slot.refs.credits.linesPlayed > slot.lines.Count))
		{
			slot.logConfigError(SlotErrors.NO_LINES);
			return slotWinSpin;
		}

		// Calculate wins for each played line
		bool payHighestLineWinOnly = false; // TODO: make configurable
		int topPay = 0;
		for (int lineToCalculate = 0; lineToCalculate < slot.GetComponent<SlotCredits>().linesPlayed; lineToCalculate++)
		{
			int pay = calculatePayForLine(lineToCalculate);
			if (payHighestLineWinOnly)
			{
				if (pay > 0)
				{
					if (pay >= topPay)
					{
						SlotWinData thisLine = lineResultData.Last();
						topPay = pay;
						lineResultData.Clear();
						lineResultData.Add(thisLine);
						slotWinSpin.totalWon = thisLine.paid;
						slot.computedWinLine(thisLine);
						slotWinSpin.totalWonAdjusted = thisLine.paid;
					}
					else
					{
						SlotWinData erase = lineResultData.Last();
						lineResultData.Remove(erase);
						slotWinSpin.totalWon -= erase.paid;
						slotWinSpin.totalWonAdjusted -= erase.paid;
					}
				}
			}
		}

		// Calculate scatter and linked symbol wins
		calculateScatterPays();
		calculateLinkedSymbols();

		return slotWinSpin;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	/// <summary>
	/// SINGLE-PLAYER: Calculate linked symbols locally
	/// </summary>
	void calculateLinkedSymbols()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		if (slot.symbolSets.Count == 0)
		{
			slot.logConfigError(SlotErrors.NO_SYMBOLSETS);
			return;
		}

		LinkPosition linkState = LinkPosition.Top;
		bool doingLink = false;
		int index = -1;
		string linkName = "";
		for (int reel = 0; reel < slot.numberOfReels; reel++)
		{
			doingLink = false;
			linkState = LinkPosition.Top;
			for (int range = slot.reelIndent; range < (slot.reelHeight - slot.reelIndent); range++)
			{
				int symbolIndexToCompare = slot.reels[reel].symbols[range].GetComponent<SlotSymbol>().symbolIndex;

				if (slot.symbolInfo[symbolIndexToCompare].linked == true)
				{
					switch (slot.symbolInfo[symbolIndexToCompare].linkPosition)
					{
						case LinkPosition.Top:
							if (!doingLink)
							{
								doingLink = true;
								linkName = slot.symbolInfo[symbolIndexToCompare].linkName;
								index = symbolIndexToCompare;
							}
							break;
						case LinkPosition.Mid:
							if ((doingLink) && (linkName == slot.symbolInfo[symbolIndexToCompare].linkName))
								linkState = LinkPosition.Mid;
							else
							{
								linkState = LinkPosition.Top;
								doingLink = false;
							}
							break;
						case LinkPosition.Bottom:
							if ((doingLink) && (linkName == slot.symbolInfo[symbolIndexToCompare].linkName))
								if (linkState == LinkPosition.Mid)
								{
									// entire linked symbol on screen
									slot.linkedSymbolLanded(reel, slot.symbolInfo[symbolIndexToCompare].linkName);
									linkState = LinkPosition.Bottom;
									doingLink = false;
								}
								else
								{
									linkState = LinkPosition.Top;
									doingLink = false;
								}
							break;
					}
				}
			}
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	/// <summary>
	/// SINGLE-PLAYER: Calculate scatter pays locally
	/// </summary>
	void calculateScatterPays()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		if (slot.symbolSets.Count == 0)
		{
			slot.logConfigError(SlotErrors.NO_SYMBOLSETS);
			return;
		}

		for (int currentSymbolSetIndex = 0; currentSymbolSetIndex < slot.symbolSets.Count; currentSymbolSetIndex++)
		{
			SetsWrapper currentSet = slot.symbolSets[currentSymbolSetIndex];
			if (currentSet.typeofSet != SetsType.scatter) continue;

			int matches = 0;
			SlotWinData winData = new SlotWinData(-1);

			for (int reel = 0; reel < slot.numberOfReels; reel++)
			{
				for (int range = slot.reelIndent; range < (slot.reelHeight - slot.reelIndent); range++)
				{
					int symbolIndexToCompare = slot.reels[reel].symbols[range].GetComponent<SlotSymbol>().symbolIndex;
					foreach (int symbolInSet in currentSet.symbols)
					{
						if (symbolInSet == symbolIndexToCompare)
						{
							matches++;
							winData.symbols.Add(slot.reels[reel].symbols[range]);
							break;
						}
					}
				}
			}

			if (matches > 0)
			{
				if (slot.setPays[currentSymbolSetIndex].pays.Count < matches)
				{
					slot.logConfigError(SlotErrors.CLAMP_SCATTER);
					return;
				}
				int pay = slot.setPays[currentSymbolSetIndex].pays[matches - 1] * slot.GetComponent<SlotCredits>().betPerLine;
				if (pay > 0)
				{
					winData.lineNumber = -1;
					winData.matches = matches;
					winData.paid = pay;
					winData.setType = currentSet.typeofSet;
					winData.setIndex = currentSymbolSetIndex;
					winData.setName = slot.symbolSetNames[winData.setIndex];
					winData.readout = winData.matches + " " + winData.setName + " SCATTER PAYS " + winData.paid;
					winData.readout = winData.readout.ToUpper();
					lineResultData.Add(winData);

					slotWinSpin.totalWon += lineResultData[lineResultData.Count - 1].paid;
					slot.computedWinLine(winData);
					slotWinSpin.totalWonAdjusted += lineResultData[lineResultData.Count - 1].paid;
				}
			}
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	/// <summary>
	/// SINGLE-PLAYER: Calculate pay for individual line locally
	/// </summary>
	int calculatePayForLine(int lineNumber)
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		int highMatches = 0;
		int highPaid = 0;
		int highSet = 0;

		if (slot.symbolSets.Count == 0)
		{
			slot.logConfigError(SlotErrors.NO_SYMBOLSETS);
			return 0;
		}

		SlotWinData winData = new SlotWinData(lineNumber);

		List<int> linePositions = slot.lines[lineNumber].positions;

		for (int currentSymbolSetIndex = 0; currentSymbolSetIndex < slot.symbolSets.Count; currentSymbolSetIndex++)
		{
			SetsWrapper currentSet = slot.symbolSets[currentSymbolSetIndex];
			if (currentSet.typeofSet != SetsType.normal &&
				currentSet.typeofSet != SetsType.anywhere) continue;

			int numberOfMatchingSymbols = 0;
			int numberOfMatchingNonWildSymbols = 0;
			List<GameObject> winningSymbols = new List<GameObject>();

			for (int currentLinePosition = 0; currentLinePosition < linePositions.Count; currentLinePosition++)
			{
				int symbolIndexToCompare = slot.reels[currentLinePosition].symbols[linePositions[currentLinePosition]].GetComponent<SlotSymbol>().symbolIndex;

				bool foundMatchingSymbolInSet = false;
				if (currentSet.explicitOrder)
				{
					if (currentLinePosition < currentSet.symbols.Count)
					{
						if (currentSet.symbols[currentLinePosition] == symbolIndexToCompare || (slot.symbolInfo[symbolIndexToCompare].isWild && currentSet.allowWilds))
						{
							if (currentSet.symbols[currentLinePosition] == symbolIndexToCompare) numberOfMatchingNonWildSymbols++;

							foundMatchingSymbolInSet = true;
							numberOfMatchingSymbols++;

							winningSymbols.Add(slot.reels[currentLinePosition].symbols[linePositions[currentLinePosition]]);
						}
					}
				}
				else
				{
					foreach (int symbolInSet in currentSet.symbols)
					{
						if ((symbolInSet == symbolIndexToCompare) || (slot.symbolInfo[symbolIndexToCompare].isWild && currentSet.allowWilds))
						{
							if (symbolInSet == symbolIndexToCompare) numberOfMatchingNonWildSymbols++;

							foundMatchingSymbolInSet = true;
							numberOfMatchingSymbols++;
							winningSymbols.Add(slot.reels[currentLinePosition].symbols[linePositions[currentLinePosition]]);
							break;
						}
					}
				}
				// if no match is found, abort the search, since the set is no longer consecutive, unless SetType is anywhere
				if (!foundMatchingSymbolInSet && currentSet.typeofSet != SetsType.anywhere)
					break;
			}

			// ensure this win has at least one of the actual symbols
			if (numberOfMatchingNonWildSymbols == 0)
			{
				numberOfMatchingSymbols = 0;
				winningSymbols.Clear();
			}

			if ((numberOfMatchingSymbols >= highMatches) && numberOfMatchingSymbols > 0)
			{
				int pay = slot.setPays[currentSymbolSetIndex].pays[numberOfMatchingSymbols - 1] * slot.refs.credits.betPerLine;

				if (pay > highPaid)
				{
					highMatches = numberOfMatchingSymbols;
					highPaid = pay;
					highSet = currentSymbolSetIndex;
					winData.symbols = winningSymbols;
				}
			}
		}

		if (highPaid > 0)
		{
			winData.lineNumber = lineNumber;
			winData.matches = highMatches;
			winData.paid = highPaid;
			winData.setIndex = highSet;
			winData.setType = slot.symbolSets[winData.setIndex].typeofSet;
			winData.setName = slot.symbolSetNames[winData.setIndex];
			winData.readout = winData.matches + " " + winData.setName + " ON LINE " + (winData.lineNumber + 1) + " PAYS " + winData.paid;
			winData.readout = winData.readout.ToUpper();
			lineResultData.Add(winData);

			slotWinSpin.totalWon += lineResultData[lineResultData.Count - 1].paid;
			slot.computedWinLine(winData);
			slotWinSpin.totalWonAdjusted += lineResultData[lineResultData.Count - 1].paid;
		}

		return highPaid;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}
	#endregion

}