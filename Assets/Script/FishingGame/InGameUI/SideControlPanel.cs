using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideControlPanel : MonoBehaviour
{
    [SerializeField] private Button _toggleBtn;
    [SerializeField] private RectTransform _root;

    private bool _isOpened = false;

    private void Start()
    {
        _toggleBtn.onClick.AddListener(OnToggleBtnClick);
    }

    private void OnToggleBtnClick()
    {
        _isOpened = !_isOpened;

        if(_isOpened)
        {
            _toggleBtn.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            _toggleBtn.transform.localScale = new Vector3(-1, 1, 1);
        }

        StartCoroutine(LerpPosition(new Vector2(_root.anchoredPosition.x * -1, _root.anchoredPosition.y), 0.2f));
    }

    private IEnumerator LerpPosition(Vector2 targetPosition, float duration)
    {
        float time = 0;
        Vector2 startPosition = _root.anchoredPosition;

        while (time < duration)
        {
            time += Time.deltaTime;
            _root.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, time / duration);
            yield return null;
        }

        _root.anchoredPosition = targetPosition;
    }
}
