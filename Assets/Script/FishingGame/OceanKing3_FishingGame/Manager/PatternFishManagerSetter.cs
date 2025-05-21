using UnityEngine;

public class PatternFishManagerSetter : MonoBehaviour
{
    public float curAddValue = 1;
    public PatternFishManager[] fishManagers; // Assign all fish spawn points from top to bottom
    public float totalCurveTime = 5f; // Total time from first to last fish
    public AnimationCurve spawnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Delay curve for fish spawns

    [ContextMenu("Set Half Circle ')' Shape Delays")]
    private void SetHalfCircleDelays()
    {
        if (fishManagers == null || fishManagers.Length == 0)
        {
            Debug.LogWarning("No fish managers assigned!");
            return;
        }

        int count = fishManagers.Length;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / (count - 1); // Normalize the index [0..1] along the list of managers
            float curveValue = spawnCurve.Evaluate(t); // Smooth curve value for the delay
            float delay = curveValue * totalCurveTime; // Calculate the delay based on the curve

            // Assign delay to each fish manager
            fishManagers[i]._delay = delay;
        }

        Debug.Log("Set half-circle delays successfully!");
    }

    [ContextMenu("Add delays")]
    private void AddTooAllDelay()
    {
        foreach (var fishManager in fishManagers)
        {
            fishManager._delay += curAddValue;
        }
    }
}
