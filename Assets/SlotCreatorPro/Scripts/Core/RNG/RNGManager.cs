using UnityEngine;
using System.Collections;

public enum pRNGs
{
	RngNativeDotNet,
	RngMersenneTwister,
	RngCryptoServiceProvider,
}

public class RNGManager : MonoBehaviour
{
	//--- MULTIPLAYER SERVER CODE START ---
	// MULTIPLAYER: RNG is handled by SmartFoxServer for fairness and security
	// All random number generation is done server-side when IsMultiplayer = true
	//--- MULTIPLAYER SERVER CODE END ---

	//--- SINGLE-PLAYER LOCAL CODE START ---
	// SINGLE-PLAYER: Local RNG used when IsMultiplayer = false
	public static MersenneTwister rng = new MersenneTwister();
	//--- SINGLE-PLAYER LOCAL CODE END ---

	/// <summary>
	/// Gets random number in range using specified RNG type
	/// MULTIPLAYER MODE: Returns placeholder value - server handles all RNG
	/// SINGLE-PLAYER MODE: Uses local RNG for gameplay
	/// </summary>
	public static uint getRandomRange(pRNGs activeRNG, int min, int max)
	{
		//--- SINGLE-PLAYER LOCAL CODE START ---
		// SINGLE-PLAYER: Use local RNG algorithms
		uint result = 0;
		switch (activeRNG)
		{
			case pRNGs.RngNativeDotNet:
				result = (uint)UnityEngine.Random.Range(min, max);
				break;
			case pRNGs.RngMersenneTwister:
				result = rng.NextUInt32((uint)min, (uint)max);
				break;
			case pRNGs.RngCryptoServiceProvider:
				//#if !UNITY_IPHONE && !UNITY_ANDROID
				result = (uint)RNCrypto.GetRandomIntBetween(min, max);
				//#endif
				break;
		}
		return result;
		//--- SINGLE-PLAYER LOCAL CODE END ---
	}

	//--- MULTIPLAYER SERVER CODE START ---
	/// <summary>
	/// MULTIPLAYER: Placeholder method for compatibility during migration
	/// This should not be used for actual game logic in multiplayer mode
	/// Server handles all RNG to ensure fairness and prevent cheating
	/// </summary>
	public static uint getRandomRangeMultiplayerFallback(pRNGs activeRNG, int min, int max)
	{
		Debug.LogWarning("RNGManager.getRandomRange() called in multiplayer mode - server should handle all RNG");
		// Return a placeholder value to prevent crashes during migration
		return (uint)UnityEngine.Random.Range(min, max);
	}
	//--- MULTIPLAYER SERVER CODE END ---

	//--- SHARED CODE (BOTH MODES) ---
	// Static initialization used by both modes
	void Start()
	{
		// Initialize RNG seed for single-player mode
		if (rng == null)
		{
			rng = new MersenneTwister();
		}
	}
	//--- SHARED CODE (BOTH MODES) ---

	// TODO: After full server migration, remove local RNG methods and keep only server communication
}