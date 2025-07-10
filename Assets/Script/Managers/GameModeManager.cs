using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    [Header("Game Mode Settings")]
    [SerializeField] private bool useMultiplayer = false;

    [Header("References")]
    [SerializeField] private GameObject gameplayNetworkManagerObject;
    [SerializeField] private Slot slot;

    void Awake()
    {
        if (gameplayNetworkManagerObject == null)
            gameplayNetworkManagerObject = FindObjectOfType<GameplayNetworkManager>()?.gameObject;

        if (slot == null)
            slot = FindObjectOfType<Slot>();

        ApplyGameMode();
    }

    private void ApplyGameMode()
    {
        if (slot != null)
        {
            slot.IsMultiplayer = useMultiplayer;
            Debug.Log($"Set slot multiplayer mode: {useMultiplayer}");
        }

        if (gameplayNetworkManagerObject != null)
        {
            gameplayNetworkManagerObject.SetActive(useMultiplayer);
            Debug.Log($"Set GameplayNetworkManager active: {useMultiplayer}");
        }
    }

    [ContextMenu("Switch to Multiplayer")]
    public void SwitchToMultiplayer()
    {
        useMultiplayer = true;
        ApplyGameMode();
    }

    [ContextMenu("Switch to Local")]
    public void SwitchToLocal()
    {
        useMultiplayer = false;
        ApplyGameMode();
    }
}