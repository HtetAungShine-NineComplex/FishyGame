using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;
public class SlotUIButton : MonoBehaviour
{
    [SerializeField] private Button _btn;
    [SerializeField] private GameType _gameType; //the game type that will be loaded on click
    [SerializeField] private Image m_SplashScreen;
    [SerializeField] private Sprite m_SplashScreenArt;
    [SerializeField] private float fadeDuration = 0.75f;

    private void OnEnable()
    {
        _btn.onClick.AddListener(GoToGameUI);
    }

    private void OnDisable()
    {
        _btn.onClick.RemoveAllListeners();
    }

    private void GoToGameUI()
    {
        StartCoroutine(IGoTOGameUI());
    }

    IEnumerator IGoTOGameUI()
    {
        m_SplashScreen.gameObject.SetActive(true);
        m_SplashScreen.sprite = m_SplashScreenArt;
        yield return new WaitForSeconds(fadeDuration);
        m_SplashScreen.gameObject.SetActive(false);
        UIManager.Instance.CloseUI(GLOBALCONST.UI_MAIN_MENU);
        UIManager.Instance.ShowUI(GLOBALCONST.UI_SLOT_1);
    }
}
