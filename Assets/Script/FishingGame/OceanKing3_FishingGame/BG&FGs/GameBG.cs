using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBG : MonoBehaviour
{
    [SerializeField] protected int mapIndex = 1;

    protected virtual void Start()
    {
        WaveManager.Instance.EnterNormalStage += OnEnterNormalStage;
        WaveManager.Instance.EnterBossStage += OnEnterBossStage;
        WaveManager.Instance.EnterBonusStage += OnEnterBonusStage;
    }

    protected virtual void OnDestroy()
    {
        WaveManager.Instance.EnterNormalStage -= OnEnterNormalStage;
        WaveManager.Instance.EnterBossStage -= OnEnterBossStage;
        WaveManager.Instance.EnterBonusStage -= OnEnterBonusStage;
    }

    protected virtual void OnEnterNormalStage(int mapIndex)
    {

    }

    protected virtual void OnEnterBossStage(int mapIndex)
    {

    }

    protected virtual void OnEnterBonusStage(int mapIndex)
    {

    }

    protected virtual IEnumerator Fade()
    {
        yield return null;
    }
}

