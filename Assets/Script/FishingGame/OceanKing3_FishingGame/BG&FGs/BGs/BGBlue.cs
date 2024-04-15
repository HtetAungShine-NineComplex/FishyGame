using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGBlue : GameBG
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CanvasGroup _group;

    protected override void OnEnterNormalStage(int mapIndex)
    {
        base.OnEnterNormalStage(mapIndex);
        if (mapIndex != this.mapIndex) return;
        gameObject.SetActive(true);
    }

    protected override void OnEnterBossStage(int mapIndex)
    {
        base.OnEnterBossStage(mapIndex);
        if (mapIndex != this.mapIndex) return;

        _animator.SetBool("IsBossFight", true);
    }

    protected override void OnEnterBonusStage(int mapIndex)
    {
        _animator.SetBool("IsBossFight", false);

        base.OnEnterBonusStage(mapIndex);
        if (mapIndex == this.mapIndex)
        {
            _group.alpha = 1;
            gameObject.SetActive(true);
        }
        else if (gameObject.activeSelf)
        {
            StartCoroutine(Fade());
        }


    }

    IEnumerator Fade()
    {
        float fadeDuration = 2f;
        float fadeRate = 1 / fadeDuration;

        while (_group.alpha > 0)
        {
            _group.alpha -= fadeRate * Time.deltaTime;

            if (_group.alpha <= 0)
            {
                _group.alpha = 0;
                gameObject.SetActive(false);
            }

            yield return new WaitForEndOfFrame();
        }
    }

}
