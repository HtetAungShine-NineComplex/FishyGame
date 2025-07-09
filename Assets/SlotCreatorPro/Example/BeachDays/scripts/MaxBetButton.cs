using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MaxBetButton : MonoBehaviour
{

	public Sprite enabledSprite;
	public Sprite disabledSprite;

	Slot slot;

	private Button button;

	void Start()
	{
		button = GetComponent<Button>();
		slot = GameObject.Find("BeachDays").GetComponent<Slot>();
	}

	void Update()
	{
		//--- SHARED CODE (BOTH MODES) ---
		// Max bet button state management used by both multiplayer and single-player modes
		switch (slot.state)
		{
			case SlotState.playingwins:
				//--- MULTIPLAYER SERVER CODE START ---
				// MULTIPLAYER: Visual state - max bet allowed during win display
				// Server will handle bet validation when spin is requested
				//--- MULTIPLAYER SERVER CODE END ---
				//--- SINGLE-PLAYER LOCAL CODE START ---
				// SINGLE-PLAYER: Visual state - max bet allowed during win display
				// Local validation happens when spin is triggered
				//--- SINGLE-PLAYER LOCAL CODE END ---
				//--- SHARED CODE (BOTH MODES) ---
				button.image.sprite = enabledSprite;
				button.interactable = true;
				break;
			case SlotState.ready:
				//--- MULTIPLAYER SERVER CODE START ---
				// MULTIPLAYER: Visual state - max bet allowed when ready
				// Server will handle bet validation when spin is requested
				//--- MULTIPLAYER SERVER CODE END ---
				//--- SINGLE-PLAYER LOCAL CODE START ---
				// SINGLE-PLAYER: Visual state - max bet allowed when ready
				// Local validation happens when spin is triggered
				//--- SINGLE-PLAYER LOCAL CODE END ---
				//--- SHARED CODE (BOTH MODES) ---
				button.image.sprite = enabledSprite;
				button.interactable = true;
				break;
			case SlotState.snapping:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state - disable max bet during snapping for both modes
				button.image.sprite = disabledSprite;
				button.interactable = false;
				break;
			case SlotState.spinning:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state - disable max bet during spinning for both modes
				button.image.sprite = disabledSprite;
				button.interactable = false;
				break;
		}
	}
}