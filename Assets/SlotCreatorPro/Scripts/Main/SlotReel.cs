// Brad Lima - 11/2019
//
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class SlotReel : MonoBehaviour
{

	public static event Action<int> OnReelDoneSpinning;

	public int reelIndex;
	public float speed;

	//--- SINGLE-PLAYER LOCAL CODE START ---
	// SINGLE-PLAYER: Local reel strips used when IsMultiplayer = false
	public List<int> ReelStrip = new List<int>();
	private int reelStripIndex;
	//--- SINGLE-PLAYER LOCAL CODE END ---

	[HideInInspector]
	public List<GameObject> symbols = new List<GameObject>();
	List<Transform> symbolsParents = new List<Transform>();
	List<Transform> symbolsChildren = new List<Transform>();

	private bool snapped = false;
	private bool stopped = false;

	public float symbolHeight;
	public float symbolWidth;
	private float heightOffset;
	private float widthOffset;
	private float symbolPadding;
	private float reelPadding;

	Slot slot;

	private bool anticipation;

	private int symbolsSpinRemaining;

	//--- MULTIPLAYER SERVER CODE START ---
	// MULTIPLAYER: Server-provided symbol data for this reel
	private List<int> serverSymbolData = new List<int>();
	private int serverSymbolIndex = 0;
	//--- MULTIPLAYER SERVER CODE END ---

	#region Config
	void Awake()
	{
	}
	void Start()
	{

	}

	void OnEnable()
	{

		slot = transform.parent.gameObject.GetComponent<Slot>();

		if (slot.symbolPrefabs.Count == 0)
		{
			slot.logConfigError(SlotErrors.NO_SYMBOLS);
			return;
		}

		//--- SHARED CODE (BOTH MODES) ---
		// Symbol size calculation used by both modes
		if (slot.symbolPrefabs[0] == null) return;
		GameObject symb = (GameObject)Instantiate(slot.symbolPrefabs[0]);

		symb.transform.localScale = Vector3.Scale(symb.transform.localScale, transform.parent.transform.localScale);
		if (symb.GetComponent<SpriteRenderer>())
		{
			Vector3 size = symb.GetComponent<SpriteRenderer>().sprite.bounds.size;
			symbolHeight = size.y * symb.transform.localScale.y;
			symbolWidth = size.x * symb.transform.localScale.x;
		}
		else
		if (symb.GetComponent<MeshFilter>())
		{
			symbolHeight = symb.GetComponent<MeshFilter>().mesh.bounds.size.y * slot.transform.localScale.y;
			symbolWidth = symb.GetComponent<MeshFilter>().mesh.bounds.size.x * slot.transform.localScale.y;
		}
		else
		{
			slot.logConfigError(SlotErrors.MISSING_RENDERER);
			return;
		}

		reelPadding = symbolWidth * (slot.horizontalReelPaddingPercent / 100.0f);
		symbolPadding = symbolHeight * (slot.verticalSymbolPaddingPercent / 100.0f);

		heightOffset = -transform.parent.transform.localPosition.y + (symbolHeight * (slot.reelHeight / 2) + (symbolPadding * (slot.reelHeight / 2)));
		widthOffset = -transform.parent.transform.localPosition.x + (symbolWidth * (slot.numberOfReels / 2) + (reelPadding * (slot.numberOfReels / 2)));
		Destroy(symb);

		transform.localPosition = new Vector3((-widthOffset + (symbolWidth * (reelIndex - 1)) + (reelPadding * (reelIndex - 1))), transform.localPosition.x, transform.localPosition.z);

		if (!slot.useSuppliedResult)
		{
			createReelSymbols();
		}
		else
		{
			for (int i = 0; i < slot.reelHeight; i++)
			{
				createReelSymbolStartup(i);
			}
		}
	}
	#endregion

	#region MULTIPLAYER SERVER CODE
	/// <summary>
	/// MULTIPLAYER: Sets server-provided symbol data for this reel
	/// Called when server sends symbol results for display
	/// </summary>
	public void setServerSymbolData(List<int> symbolData)
	{
		//--- MULTIPLAYER SERVER CODE START ---
		serverSymbolData = symbolData;
		serverSymbolIndex = 0;
		//--- MULTIPLAYER SERVER CODE END ---
	}
	#endregion

	#region Create Reel
	public void createReelSymbols()
	{
		for (int i = 0; i < slot.reelHeight; i++)
		{
			createSymbol(i);
		}
	}


	int lastSelected;

	public int getSymbol()
	{
		//--- MULTIPLAYER SERVER CODE START ---
		if (slot.IsMultiplayer)
		{
			// MULTIPLAYER: Use server-provided symbol data
			if (serverSymbolData.Count > 0 && serverSymbolIndex < serverSymbolData.Count)
			{
				int serverSymbol = serverSymbolData[serverSymbolIndex];
				serverSymbolIndex++;
				lastSelected = serverSymbol;
				return serverSymbol;
			}
		}
		//--- MULTIPLAYER SERVER CODE END ---

		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Use local RNG and reel strips
		int chosen = -1;

		if (ReelStrip.Count > 0)
		{
			chosen = ReelStrip[reelStripIndex];
			reelStripIndex++;
			if (reelStripIndex > ReelStrip.Count - 1)
				reelStripIndex = 0;
		}
		else
		{
			chosen = 0;
		}

		lastSelected = chosen;
		return chosen;
		//--- SINGLE-PLAYER LOCAL CODE END ---

	}

	void createSymbol(int slotIndex)
	{
		int symbolIndex;
		if (slot.useSuppliedResult)
		{
			if ((symbolsSpinRemaining >= slot.reelIndent) && (symbolsSpinRemaining < (slot.reelHeight - slot.reelIndent)))
			{
				if (slot.suppliedResult[reelIndex - 1, symbolsSpinRemaining - slot.reelIndent] > -1)
				{
					symbolIndex = slot.suppliedResult[reelIndex - 1, symbolsSpinRemaining - slot.reelIndent];
				}
				else
				{
					symbolIndex = getSymbol();
				}
			}
			else
			{
				symbolIndex = getSymbol();
			}
		}
		else
		{
			//--- SINGLE-PLAYER LOCAL CODE START ---
			if (!slot.IsMultiplayer)
			{
				// SINGLE-PLAYER: Check frozen positions for local gameplay features
				if ((symbolsSpinRemaining >= slot.reelIndent) && (symbolsSpinRemaining < (slot.reelHeight - slot.reelIndent)))
				{
					if (slot.frozenPositions[reelIndex - 1, symbolsSpinRemaining - slot.reelIndent] > -1)
					{
						Debug.Log("slot reel test : " + slot.frozenPositions[reelIndex - 1, symbolsSpinRemaining - slot.reelIndent]);
						symbolIndex = slot.frozenPositions[reelIndex - 1, symbolsSpinRemaining - slot.reelIndent];
					}
					else
					{
						symbolIndex = getSymbol();
					}
				}
				else
				{
					symbolIndex = getSymbol();
				}
			}
			//--- SINGLE-PLAYER LOCAL CODE END ---
			//--- MULTIPLAYER SERVER CODE START ---
			else
			{
				// MULTIPLAYER: Always use server-provided symbols
				symbolIndex = getSymbol();
			}
			//--- MULTIPLAYER SERVER CODE END ---
		}

		//--- SHARED CODE (BOTH MODES) ---
		// Symbol instantiation used by both modes
		GameObject symb;
		if (slot.usePool)
		{
			symb = getFromPool(symbolIndex);
		}
		else
		{
			symb = (GameObject)Instantiate(slot.symbolPrefabs[symbolIndex]);
			symb.GetComponent<SlotSymbol>().symbolIndex = symbolIndex;
			symb.transform.localScale = Vector3.Scale(symb.transform.localScale, transform.parent.transform.localScale);
		}


		symb.transform.parent = transform;
		if (symb.GetComponent<SpriteRenderer>())
			symb.transform.localPosition = new Vector3(slot.reelCenter.position.x + 0, slot.reelCenter.position.y + (-heightOffset + (slotIndex * symbolHeight - transform.localPosition.y)) + (symbolPadding * slotIndex), 0);
		if (symb.GetComponent<MeshFilter>())
			symb.transform.localPosition = new Vector3(slot.reelCenter.position.x + 0, slot.reelCenter.position.y + (-heightOffset + (slotIndex * symbolHeight - transform.localPosition.y)) + (symbolPadding * slotIndex), 0);

		symbols.Insert(0, symb);
	}

	void createReelSymbolStartup(int slotIndex)
	{
		int symbolIndex = -1;
		if ((slotIndex >= slot.reelIndent) && (slotIndex < (slot.reelHeight - slot.reelIndent)))
		{
			symbolIndex = slot.suppliedResult[reelIndex - 1, slotIndex - slot.reelIndent];
		}
		else
		{
			symbolIndex = getSymbol();
		}

		//--- SHARED CODE (BOTH MODES) ---
		// Symbol instantiation used by both modes
		GameObject symb;
		if (slot.usePool)
		{
			symb = getFromPool(symbolIndex);
		}
		else
		{
			symb = (GameObject)Instantiate(slot.symbolPrefabs[symbolIndex]);
			symb.GetComponent<SlotSymbol>().symbolIndex = symbolIndex;
			symb.transform.localScale = Vector3.Scale(symb.transform.localScale, transform.parent.transform.localScale);
		}

		symb.transform.parent = transform;
		if (symb.GetComponent<SpriteRenderer>())
			symb.transform.localPosition = new Vector3(slot.reelCenter.position.x + 0, slot.reelCenter.position.y + (-heightOffset + (slotIndex * symbolHeight - transform.localPosition.y)) + (symbolPadding * slotIndex), 0);
		if (symb.GetComponent<MeshFilter>())
			symb.transform.localPosition = new Vector3(slot.reelCenter.position.x + 0, slot.reelCenter.position.y + (-heightOffset + (slotIndex * symbolHeight - transform.localPosition.y)) + (symbolPadding * slotIndex), 0);

		symbols.Insert(0, symb);
	}
	#endregion

	#region Actions
	public void spinReel(float spinTime)
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Reel spinning mechanics used by both modes
		retatchBackgrounds();

		snapped = false;
		stopped = false;
		speed = slot.spinningSpeed;

		symbolsSpinRemaining = (int)(spinTime / speed);
		transform.DOLocalMoveY(-symbolHeight, speed).OnComplete(OnNextSymbol).SetRelative(true).SetEase(Ease.InOutBounce);
	}

	public void snapReel()
	{
		Debug.Log("waiting: " + slot.waitingForResult);
		if (snapped) return;
		if (slot.waitingForResult) return;

		snapped = true;
		symbolsSpinRemaining = (int)(slot.reelHeight - slot.reelIndent);
	}
	#endregion

	#region Symbol Misc
	GameObject getFromPool(int symbolIndex)
	{
		GameObject symbol = slot.getSymbolFromPool(symbolIndex);
		return symbol;
	}
	void returnToPool()
	{
		if (symbols.Count == 0)
		{
			slot.logConfigError(SlotErrors.NO_SYMBOLS);
			return;
		}
		GameObject symb = symbols[symbols.Count - 1];

		slot.returnSymbolToPool(symb);
	}

	void detatchBackgrounds()
	{
		foreach (GameObject symbol in symbols)
		{

			foreach (Transform child in symbol.transform)
			{
				symbolsParents.Add(child.parent);
				child.parent = child.parent.transform.parent;
				symbolsChildren.Add(child);
			}
		}
	}

	void retatchBackgrounds()
	{
		for (int i = 0; i < symbolsChildren.Count; i++) symbolsChildren[i].parent = symbolsParents[i];
		symbolsChildren.Clear();
		symbolsParents.Clear();
	}

	#endregion

	#region DOTween Callbacks
	void OnNextSymbol()
	{
		if (symbols.Count == 0)
		{
			slot.logConfigError(SlotErrors.NO_SYMBOLS);
			return;
		}

		//--- SHARED CODE (BOTH MODES) ---
		// Symbol cycling logic used by both modes
		if (slot.usePool) returnToPool();
		else
			Destroy(symbols[symbols.Count - 1]);
		symbols.RemoveAt(symbols.Count - 1);

		createSymbol(slot.reelHeight - 1);

		symbolsSpinRemaining--;
		if (symbolsSpinRemaining == 0)
		{
			stopped = true;
		}

		if (slot.waitingForResult)
		{
			symbolsSpinRemaining = slot.reelHeight;
			stopped = false;
		}
		if (stopped)
		{
			transform.DOLocalMoveY(-symbolHeight - symbolPadding, slot.easeOutTime).SetRelative(true).OnComplete(OnReelStopped).SetEase(slot.reelEase);
			Invoke("checkScatterLanded", slot.easeOutTime / 2.0f);
			slot.reelLanded(reelIndex);

		}
		else
		{
			if (anticipation)
			{
				transform.DOLocalMoveY(-symbolHeight - symbolPadding, speed / 2.0f).SetRelative(true).OnComplete(OnNextSymbol).SetEase(Ease.Linear);
			}
			else
			{
				transform.DOLocalMoveY(-symbolHeight - symbolPadding, speed).SetRelative(true).OnComplete(OnNextSymbol).SetEase(Ease.Linear);
			}
		}
	}

	void finishSpinning()
	{
		stopped = true;
	}

	void OnReelStopped()
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Reel stop processing used by both modes
		if (slot.usePool) returnToPool();
		else
			Destroy(symbols[symbols.Count - 1]);
		symbols.RemoveAt(symbols.Count - 1);

		createSymbol(slot.reelHeight - 1);

		if (OnReelDoneSpinning != null)
			OnReelDoneSpinning(reelIndex);

		detatchBackgrounds();

		//--- SINGLE-PLAYER LOCAL CODE START ---
		if (!slot.IsMultiplayer)
		{
			// SINGLE-PLAYER: Local scatter calculation and anticipation
			inlineScatterCalc();
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---

		slot.reels[reelIndex - 1].anticipation = false;

	}

	void checkScatterLanded()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Local scatter detection during reel stop
		if (slot.IsMultiplayer) return; // Server handles scatter detection in multiplayer

		for (int currentSymbolSetIndex = 0; currentSymbolSetIndex < slot.symbolSets.Count; currentSymbolSetIndex++)
		{

			SetsWrapper currentSet = slot.symbolSets[currentSymbolSetIndex];
			if (currentSet.typeofSet != SetsType.scatter) continue;
			foreach (int symbol in currentSet.symbols)
			{
				for (int i = slot.reelIndent - 1; i < (slot.reelHeight - slot.reelIndent) - 1; i++)
				{
					int reelSymbolIndex = symbols[i].GetComponent<SlotSymbol>().symbolIndex;
					if (reelSymbolIndex == symbol)
					{
						slot.scatterSymbolLanded(symbols[i], currentSet.scatterCount + 1);
					}
				}
			}
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	void inlineScatterCalc()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Local scatter calculation and anticipation logic
		for (int currentSymbolSetIndex = 0; currentSymbolSetIndex < slot.symbolSets.Count; currentSymbolSetIndex++)
		{

			SetsWrapper currentSet = slot.symbolSets[currentSymbolSetIndex];
			if (currentSet.typeofSet != SetsType.scatter) continue;
			int matches = 0;
			SlotScatterHitData hit = new SlotScatterHitData(reelIndex);

			foreach (int symbol in currentSet.symbols)
			{
				for (int i = slot.reelIndent; i < (slot.reelHeight - slot.reelIndent); i++)
				{
					int reelSymbolIndex = symbols[i].GetComponent<SlotSymbol>().symbolIndex;
					if (reelSymbolIndex == symbol)
					{
						currentSet.scatterCount++;
						matches++;
						hit.hits = currentSet.scatterCount;
						hit.setIndex = currentSymbolSetIndex;
						hit.setType = SetsType.scatter;
						hit.setName = slot.symbolSetNames[hit.setIndex];
						hit.symbol = symbols[i];
						slot.scatterSymbolHit(hit);
					}
				}
			}
			// anticipation
			if (currentSet.scatterCount > 0)
			{

				if ((currentSet.scatterCount < slot.numberOfReels) && (reelIndex < slot.numberOfReels))
				{
					if ((slot.setPays[currentSymbolSetIndex].anticipate[currentSet.scatterCount - 1]) == true)
					{
						slot.reels[reelIndex].anticipation = true;
						for (int i = reelIndex; i < slot.numberOfReels; i++)
						{
							//Debug.Log("anticipate reel:" + i);
							slot.reels[i].symbolsSpinRemaining += 60;

							slot.anticipationScatterBegin(hit);
						}
					}
				}
			}
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}
	#endregion

}