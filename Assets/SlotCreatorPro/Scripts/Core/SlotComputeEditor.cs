using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aSlot;

namespace aSlot
{
	public struct ResultCount
	{
		public string name;
		public int occurenceCount;
		public float probability;
		public int net;
		public float EV;
		public float AD;
		public float E2;
		public float CxF;
		public int winTotal;
		public int matches;
	}
}
public class SlotComputeEditor : MonoBehaviour
{

	List<List<int>> symbolsResult = new List<List<int>>();

	private Slot slot;

	public Dictionary<string, ResultCount> resultCounts = new Dictionary<string, ResultCount>();

	private int itterationsCount;
	public int itterations;
	public int betPerLineEditor;
	public int linesPlayedEditor;
	public float progress;
	public int totalwon;
	public int totalbet;

	public float timeToCompute;
	public int timeToComputeIterations;
	public float estimatedTimeToCompute;

	public float netPayi;
	public float variance;
	public float standardDeviation;
	public float volitility;

	#region Start
	void Start()
	{
		slot = GetComponent<Slot>();
	}
	#endregion

	#region SINGLE-PLAYER LOCAL CODE
	/// <summary>
	/// SINGLE-PLAYER: Editor calculation functions - only used in single-player mode
	/// These functions simulate local RNG and calculation for testing purposes
	/// In multiplayer mode, all calculations are done server-side
	/// </summary>

	int getSymbolCountCurrentlyOnReel(int reelIndex, int index)
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Local symbol counting for editor simulation
		int count = 0;
		foreach (int symbolIndex in symbolsResult[reelIndex])
		{
			if (symbolIndex == index)
				count++;
		}
		return count;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	int getSymbolCountCurrentlyTotal(int index)
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Local total symbol counting for editor simulation
		int count = 0;
		foreach (List<int> reel in symbolsResult)
		{
			foreach (int symbolIndex in reel)
			{
				if (symbolIndex == index)
					count++;
			}
		}
		return count;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	public int getSymbol(int reelIndex)
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Local RNG symbol generation for editor simulation
		List<int> cumulativeFrequencyList = new List<int>();
		int totalFrequency = 0;
		for (int index = 0; index < slot.symbolFrequencies.Count; index++)
		{
			while (slot.symbolInfo.Count <= index) slot.symbolInfo.Add(new SlotSymbolInfo());
			SlotSymbolInfo symbol = slot.symbolInfo[index];
			if (symbol.perReelFrequency)
			{
				totalFrequency += slot.reelFrequencies[index].freq[reelIndex];
			}
			else
			{
				totalFrequency += slot.symbolFrequencies[index];
			}
			cumulativeFrequencyList.Add(totalFrequency);
		}

		int chosen = -1;
		while (chosen == -1)
		{
			uint selectedFrequency = RNGManager.getRandomRange(slot.activeRNG, 1, totalFrequency + 1);

			if (selectedFrequency == totalFrequency + 1)
			{
				Debug.Log("This shouldn't happen.");
			}
			for (int index = 0; index < cumulativeFrequencyList.Count; index++)
			{
				if (selectedFrequency <= cumulativeFrequencyList[index]) { chosen = index; break; }
			}
			if (!slot.symbolInfo[chosen].active) { chosen = -1; continue; }

			int maxPerReel = slot.symbolInfo[chosen].clampPerReel;
			if (maxPerReel > 0)
			{
				if (getSymbolCountCurrentlyOnReel(reelIndex, chosen) >= maxPerReel) { chosen = -1; continue; }
				int maxTotal = slot.symbolInfo[chosen].clampTotal;
				if (maxTotal > 0)
					if (getSymbolCountCurrentlyTotal(chosen) >= maxTotal) chosen = -1;
			}

			// Linked symbols cannot be organically drawn
			if (slot.symbolInfo[chosen].linked)
			{
				if (slot.symbolInfo[chosen].linkPosition != LinkPosition.Bottom)
					chosen = -1;
			}
		}

		return chosen;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	void generateSymbolsEditor()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Generate symbols locally for editor simulation
		symbolsResult.Clear();
		for (int reelIndex = 0; reelIndex < slot.numberOfReels; reelIndex++)
		{
			symbolsResult.Add(new List<int>());
			for (int position = 0; position < slot.reelHeight; position++)
			{
				symbolsResult[symbolsResult.Count - 1].Add(getSymbol(reelIndex));
			}
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	void calculateNetPayi()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Calculate statistics for editor analysis
		netPayi = 0;
		foreach (KeyValuePair<string, ResultCount> res in resultCounts)
		{
			netPayi += (res.Value.net * res.Value.occurenceCount);

		}
		variance = 0;
		List<string> keys = new List<string>(resultCounts.Keys);
		foreach (string key in keys)
		{
			ResultCount temp = resultCounts[key];

			temp.probability = (float)resultCounts[key].occurenceCount / ((float)itterations * linesPlayedEditor);
			temp.EV = netPayi / ((float)itterations * linesPlayedEditor);
			temp.AD = (float)resultCounts[key].net - temp.EV;
			temp.E2 = temp.AD * temp.AD;
			temp.CxF = temp.probability * temp.E2;

			variance += temp.CxF;
			resultCounts[key] = temp;
		}

		standardDeviation = Mathf.Sqrt(variance);
		volitility = standardDeviation * 1.65f;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	void calcOne()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Single iteration calculation for editor simulation
		int winThisItteration = 0;
		generateSymbolsEditor();
		for (int i = 0; i < linesPlayedEditor; i++)
		{
			winThisItteration += calculatePayLineForEditor(i);
		}
		winThisItteration += calculateScatterPaysForEditor();

		if (winThisItteration == 0) addResultCount("Loss", 0, 0);

		totalwon += winThisItteration;
		totalbet += slot.betsPerLine[betPerLineEditor].value * linesPlayedEditor;
		progress = (float)itterationsCount / (float)itterations;

		itterationsCount++;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	void doneCalculating()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Finalize calculation results for editor display
		Dictionary<string, ResultCount> test = new Dictionary<string, ResultCount>();
		foreach (KeyValuePair<string, ResultCount> item in resultCounts.OrderByDescending(i => i.Value.winTotal))
		{
			test.Add(item.Key, item.Value);
		}
		resultCounts = test;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	public int calculateReturnForEditor(int times)
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Main editor calculation method - only works in single-player mode
		// In multiplayer mode, this would not be used as server handles all calculations
		if (slot.IsMultiplayer)
		{
			Debug.LogWarning("Editor calculations not available in multiplayer mode - server handles all RNG and calculations");
			return 0;
		}

		slot = GetComponent<Slot>();
		itterations = times;
		itterationsCount = 0;
		totalwon = 0;
		totalbet = 0;
		resultCounts.Clear();

		timeToCompute = Time.realtimeSinceStartup;
		timeToComputeIterations = itterations;

		for (int i = 0; i < itterations; i++)
			calcOne();
		estimatedTimeToCompute = Time.realtimeSinceStartup - timeToCompute;

		calculateNetPayi();
		doneCalculating();
		return totalwon;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	#region Per Line Calcs
	int calculatePayLineForEditor(int lineNumber)
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Local line calculation for editor simulation
		slot = GetComponent<Slot>();

		int highMatches = 0;
		int highPaid = 0;

		// for each winning symbol combination
		for (int currentSymbolSetIndex = 0; currentSymbolSetIndex < slot.symbolSets.Count; currentSymbolSetIndex++)
		{
			int matches = 0;

			List<int> pos = slot.lines[lineNumber].positions;
			for (int reel = 0; reel < pos.Count; reel++)
			{
				bool match = false;
				foreach (int symbol in slot.symbolSets[currentSymbolSetIndex].symbols)
				{
					if ((symbol == symbolsResult[reel][pos[reel]]) || (slot.symbolInfo[symbolsResult[reel][pos[reel]]].isWild && slot.symbolSets[currentSymbolSetIndex].allowWilds)) { match = true; }
				}
				if (match)
					matches++;
				else
					break;
			}

			if (matches > 0)
			{
				int pay = slot.setPays[currentSymbolSetIndex].pays[matches - 1] * slot.betsPerLine[betPerLineEditor].value;
				if (matches >= highMatches)
				{
					if (pay > highPaid)
					{
						highMatches = matches;
						highPaid = pay;
					}
				}

				if (pay > 0)
				{
					addResultCount(slot.symbolSetNames[currentSymbolSetIndex], pay, matches);
				}
			}
		}

		return highPaid;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	int calculateScatterPaysForEditor()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Local scatter calculation for editor simulation
		int totalWon = 0;
		for (int currentSymbolSetIndex = 0; currentSymbolSetIndex < slot.symbolSets.Count; currentSymbolSetIndex++)
		{
			SetsWrapper currentSet = slot.symbolSets[currentSymbolSetIndex];
			if (currentSet.typeofSet != SetsType.scatter) continue;

			int matches = 0;
			for (int reel = 0; reel < slot.numberOfReels; reel++)
			{
				for (int range = slot.reelIndent; range < (slot.reelHeight - slot.reelIndent); range++)
				{
					int symbolIndexToCompare = symbolsResult[reel][range];
					foreach (int symbolInSet in currentSet.symbols)
					{
						if (symbolInSet == symbolIndexToCompare)
						{
							matches++;
							break;
						}
					}
				}
			}

			if (matches > 0)
			{
				int pay = slot.setPays[currentSymbolSetIndex].pays[matches - 1] * slot.betsPerLine[betPerLineEditor].value;
				totalWon += pay;

				if (pay > 0)
				{
					addResultCount(slot.symbolSetNames[currentSymbolSetIndex], pay, matches);
				}
			}
		}

		return totalWon;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}
	#endregion

	void addResultCount(string name, int pay, int matches)
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Add result to editor statistics tracking
		string key = name + ":" + matches;
		ResultCount res = new ResultCount();
		if (resultCounts.ContainsKey(key))
		{
			res = resultCounts[key];
		}
		else
		{
			resultCounts.Add(key, res);
		}
		res.name = name;
		res.winTotal += pay;
		res.net = (pay / slot.betsPerLine[betPerLineEditor].value) - 1;
		res.occurenceCount++;
		res.matches = matches;
		resultCounts[key] = res;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}
	#endregion

}