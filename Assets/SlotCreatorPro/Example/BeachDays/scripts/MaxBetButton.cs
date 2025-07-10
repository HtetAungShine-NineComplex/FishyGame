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
				if (slot.IsMultiplayer)
				{
					// MULTIPLAYER: Allow max bet during win display, server will handle validation
					button.image.sprite = enabledSprite;
					button.interactable = true;
				}
				//--- MULTIPLAYER SERVER CODE END ---
				//--- SINGLE-PLAYER LOCAL CODE START ---
				else
				{
					// SINGLE-PLAYER: Allow max bet during win display
					button.image.sprite = enabledSprite;
					button.interactable = true;
				}
				//--- SINGLE-PLAYER LOCAL CODE END ---
				break;
			case SlotState.ready:
				//--- MULTIPLAYER SERVER CODE START ---
				if (slot.IsMultiplayer)
				{
					// MULTIPLAYER: Allow max bet when ready, server will handle validation
					button.image.sprite = enabledSprite;
					button.interactable = true;
				}
				//--- MULTIPLAYER SERVER CODE END ---
				//--- SINGLE-PLAYER LOCAL CODE START ---
				else
				{
					// SINGLE-PLAYER: Allow max bet when ready
					button.image.sprite = enabledSprite;
					button.interactable = true;
				}
				//--- SINGLE-PLAYER LOCAL CODE END ---
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