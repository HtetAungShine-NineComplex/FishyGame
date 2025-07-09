// Brad Lima - 11/2019
//
// This script handles the loading off all examples along with persistent credit management.
//
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlotManager : MonoBehaviour
{
	//--- MULTIPLAYER SERVER CODE START ---
	// MULTIPLAYER: Credits managed by server, local storage disabled
	public bool loadCredits = false; // Disabled since server manages credits
	public int credits; // Display only - actual credits come from server
						//--- MULTIPLAYER SERVER CODE END ---

	//--- SINGLE-PLAYER LOCAL CODE START ---
	// SINGLE-PLAYER: Local credit management enabled
	// Note: In single-player mode, loadCredits can be set to true to use PlayerPrefs
	//--- SINGLE-PLAYER LOCAL CODE END ---

	//--- SHARED CODE (BOTH MODES) ---
	// UI skin used by both modes
	public GUISkin skin;

	private GameObject title;

	[HideInInspector]
	public Slot slot;

	private static SlotManager s_Instance = null;
	public static SlotManager instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = FindObjectOfType(typeof(SlotManager)) as SlotManager;
			}

			if (s_Instance == null)
			{
				GameObject obj = new GameObject("SlotManager");
				s_Instance = obj.AddComponent(typeof(SlotManager)) as SlotManager;
			}

			return s_Instance;
		}
	}

	void OnApplicationQuit()
	{
		s_Instance = null;
	}

	void Awake()
	{
		title = GameObject.Find("title");

		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Load credits from local storage if enabled
		if (loadCredits && (slot == null || !slot.IsMultiplayer))
		{
			credits = PlayerPrefs.GetInt("credits", credits);
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	//--- SINGLE-PLAYER LOCAL CODE START ---
	/// <summary>
	/// SINGLE-PLAYER: Saves credits to local storage
	/// Only used in single-player mode
	/// </summary>
	void saveCredits()
	{
		if (slot != null && !slot.IsMultiplayer)
		{
			PlayerPrefs.SetInt("credits", credits);
		}
	}
	//--- SINGLE-PLAYER LOCAL CODE END ---
	//--- SHARED CODE (BOTH MODES) ---

	#region Load / Unload
	public void loadSlot(string slotPrefabName)
	{
		GameObject go = (GameObject)Instantiate(Resources.Load(slotPrefabName));
		slot = go.GetComponent<Slot>();

		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Set local credits when loading slot
		if (!slot.IsMultiplayer)
		{
			slot.refs.credits.setCredits(credits);
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Credits will be set by server response
		// No local credit initialization needed
		//--- MULTIPLAYER SERVER CODE END ---
	}

	public void unloadSlot()
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		if (!slot.IsMultiplayer)
		{
			// SINGLE-PLAYER: Save credits locally when unloading
			credits = slot.refs.credits.credits;
			saveCredits();
		}
		//--- SINGLE-PLAYER LOCAL CODE END ---
		//--- MULTIPLAYER SERVER CODE START ---
		// MULTIPLAYER: Credits are managed by server, no local save needed
		//--- MULTIPLAYER SERVER CODE END ---

		//--- SHARED CODE (BOTH MODES) ---
		// Cleanup used by both modes
		Destroy(slot.gameObject);
	}
	#endregion

	void OnGUI()
	{
		//--- SHARED CODE (BOTH MODES) ---
		// UI rendering used by both modes
		GUI.skin = skin;

		if (slot == null)
		{
			// Slot selection menu
			GUILayout.BeginArea(new Rect(Screen.width / 4, Screen.height / 3, Screen.width / 2, Screen.height / 2));
			GUILayout.BeginHorizontal("box");
			GUIStyle style = new GUIStyle();
			style.fontSize = 20;
			if (GUILayout.Button("3-Reel 2D Slot"))
			{
				title.SetActive(false);
				loadSlot("3ReelSlot2D");
			}
			if (GUILayout.Button("5-Reel 2D Slot"))
			{
				title.SetActive(false);
				loadSlot("5ReelSlot2D");
			}
			if (GUILayout.Button("3-Reel Retro 2D Slot"))
			{
				title.SetActive(false);
				loadSlot("3ReelRetroSlot2D");
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUILayout.BeginArea(new Rect(Screen.width / 4, Screen.height / 3 + 50, Screen.width / 2, Screen.height / 2));
			GUILayout.BeginHorizontal("box");
			if (GUILayout.Button("5-Reel Retro 2D Slot"))
			{
				title.SetActive(false);
				loadSlot("5ReelRetroSlot2D");
			}
			if (GUILayout.Button("Hearts Attack"))
			{
				title.SetActive(false);
				loadSlot("TinyRetro");
			}
			if (GUILayout.Button("3D Example"))
			{
				title.SetActive(false);
				loadSlot("3DSlot");
			}
			if (GUILayout.Button("Beach Days Slot!"))
			{
				SceneManager.LoadScene(2);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();

		}
		else
		{
			// Exit slot button
			if (GUI.Button(new Rect(50, 25, 100, 50), "Exit Slot"))
			{
				unloadSlot();
				title.SetActive(true);
			}
		}
	}
}