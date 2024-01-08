using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar: MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingTxt;
    [SerializeField] private float duration;

    private void Start()
    {
        StartCoroutine(StartLoading());
    }

    private IEnumerator StartLoading()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            loadingSlider.value = progress;
            loadingTxt.text = (int)(progress * 100) + "%";

            yield return null;
        }

        // Loading completed, do any necessary actions here
        Debug.Log("Loading completed!");
    }
}
