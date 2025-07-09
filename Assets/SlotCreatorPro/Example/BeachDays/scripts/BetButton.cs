using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BetButton : MonoBehaviour
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
		// Bet button state management used by both multiplayer and single-player modes
		switch (slot.state)
		{
			case SlotState.playingwins:
				//--- MULTIPLAYER SERVER CODE START ---
				// MULTIPLAYER: Visual state - bet changes allowed during win display
				// Server will handle bet validation when spin is requested
				//--- MULTIPLAYER SERVER CODE END ---
				//--- SINGLE-PLAYER LOCAL CODE START ---
				// SINGLE-PLAYER: Visual state - bet changes allowed during win display
				// Local validation happens when spin is triggered
				//--- SINGLE-PLAYER LOCAL CODE END ---
				//--- SHARED CODE (BOTH MODES) ---
				button.image.sprite = enabledSprite;
				button.interactable = true;
				break;
			case SlotState.ready:
				//--- MULTIPLAYER SERVER CODE START ---
				// MULTIPLAYER: Visual state - bet changes allowed when ready
				// Server will handle bet validation when spin is requested
				//--- MULTIPLAYER SERVER CODE END ---
				//--- SINGLE-PLAYER LOCAL CODE START ---
				// SINGLE-PLAYER: Visual state - bet changes allowed when ready
				// Local validation happens when spin is triggered
				//--- SINGLE-PLAYER LOCAL CODE END ---
				//--- SHARED CODE (BOTH MODES) ---
				button.image.sprite = enabledSprite;
				button.interactable = true;
				break;
			case SlotState.snapping:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state - disable bet changes during snapping for both modes
				button.image.sprite = disabledSprite;
				button.interactable = false;
				break;
			case SlotState.spinning:
				//--- SHARED CODE (BOTH MODES) ---
				// Visual state - disable bet changes during spinning for both modes
				button.image.sprite = disabledSprite;
				button.interactable = false;
				break;
		}

	}
}