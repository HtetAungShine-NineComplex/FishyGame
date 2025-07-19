using UnityEngine;
using UnityEngine.UI;

public class SpinButton : MonoBehaviour
{

	public Sprite enabledSprite;
	public Sprite disabledSprite;
	public Sprite autoSprite;
	public Sprite stopSprite;


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
		// Button state visual management used by both multiplayer and single-player modes
		switch (slot.state)
		{
			case SlotState.playingwins:
				//--- MULTIPLAYER SERVER CODE START ---
				if (slot.IsMultiplayer)
				{
					// MULTIPLAYER: Show stop sprite when waiting for server results, otherwise enable normally
					if (!slot.IsReadyForNewSpinRequest())
					{
						button.image.sprite = stopSprite;
						button.interactable = true; // Keep button clickable for stop/snap functionality
					}
					else
					{
						button.image.sprite = enabledSprite;
						button.interactable = true;
					}
				}
				//--- MULTIPLAYER SERVER CODE END ---
				//--- SINGLE-PLAYER LOCAL CODE START ---
				else
				{
					// SINGLE-PLAYER: Check local credit validation
					button.image.sprite = enabledSprite;
					button.interactable = slot.refs.credits.canPlaceBet();
				}
				//--- SINGLE-PLAYER LOCAL CODE END ---
				break;
			case SlotState.ready:
				//--- MULTIPLAYER SERVER CODE START ---
				if (slot.IsMultiplayer)
				{
					// MULTIPLAYER: Show stop sprite when waiting for server results, otherwise enable normally
					if (!slot.IsReadyForNewSpinRequest())
					{
						button.image.sprite = stopSprite;
						button.interactable = true; // Keep button clickable for stop/snap functionality
					}
					else
					{
						button.image.sprite = enabledSprite;
						button.interactable = true;
					}
				}
				//--- MULTIPLAYER SERVER CODE END ---
				//--- SINGLE-PLAYER LOCAL CODE START ---
				else
				{
					// SINGLE-PLAYER: Check local credit validation
					button.image.sprite = enabledSprite;
					button.interactable = slot.refs.credits.canPlaceBet();
				}
				//--- SINGLE-PLAYER LOCAL CODE END ---
				break;
			case SlotState.snapping:
				//--- SHARED CODE (BOTH MODES) ---
				// Disable button during snapping for both modes
				button.image.sprite = disabledSprite;
				button.interactable = false;
				break;
			case SlotState.spinning:
				//--- SHARED CODE (BOTH MODES) ---
				// Show stop sprite during spinning for both modes
				button.image.sprite = stopSprite;
				button.interactable = true; // Allow stopping spin in both modes
				break;
		}
	}
}