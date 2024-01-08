using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLoading : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingTxt;

    private void Start()
    {
        StartCoroutine(StartLoading());
    }

    private IEnumerator StartLoading()
    {
        float duration = 5f; // Set the desired loading time in seconds
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
