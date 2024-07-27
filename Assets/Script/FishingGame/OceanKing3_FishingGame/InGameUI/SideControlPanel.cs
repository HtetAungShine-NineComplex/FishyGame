using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;

public class SideControlPanel : MonoBehaviour
{
    [SerializeField] private Button _toggleBtn;
    [SerializeField] private RectTransform _root;

    [Header("Side Panel Button")]
    [SerializeField] private Button _fishStatsBtn;
    [SerializeField] private Button _quitBtn;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private Button _exchangeGoldBtn;

    [Header("Fishing Game Name")]
    [SerializeField] private bool isLiKui;
    [SerializeField] private bool isChasingWind;
    [SerializeField] private bool isCoinTree;
    [SerializeField] private bool isFisher;

    private bool _isOpened = false;

    private void Start()
    {
        _toggleBtn.onClick.AddListener(OnToggleBtnClick);
        _fishStatsBtn.onClick.AddListener(OnCLickFishStatsBtn);
        _quitBtn.onClick.AddListener(OnCLickQuitBtn);
        _settingBtn.onClick.AddListener(OnCLickSettingBtn);
        _exchangeGoldBtn.onClick.AddListener(OnClickExchangeGoldBtn);
    }

    private void OnClickExchangeGoldBtn()
    {
        UIManager.Instance.ShowUI(GLOBALCONST.UI_INGAMETAKESCORE);
    }
    private void OnCLickSettingBtn()
    {
        UIManager.Instance.ShowUI(GLOBALCONST.UI_INGAMESETTING);
    }
    private void OnCLickQuitBtn()
    {        
        UIManager.Instance.ShowUI(GLOBALCONST.UI_INGAMEQUIT);
    }
    private void OnCLickFishStatsBtn()
    {
        if (isChasingWind)
            UIManager.Instance.ShowUI(GLOBALCONST.UI_FISHSTATS_CHASINGWIND);

        else if (isLiKui)
            UIManager.Instance.ShowUI(GLOBALCONST.UI_FISHSTATS_LIKUI);

        else if (isCoinTree)
            UIManager.Instance.ShowUI(GLOBALCONST.UI_FISHSTATS_COINTREE);

        else if (isFisher)
            UIManager.Instance.ShowUI(GLOBALCONST.UI_FISHSTATS_FISHER);

        else
            UIManager.Instance.ShowUI(GLOBALCONST.UI_FISHSTATS);
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
