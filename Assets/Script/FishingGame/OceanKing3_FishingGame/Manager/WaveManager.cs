using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaveStage
{
    Normal,
    BossFight,
    Bonus
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public event Action<int> EnterNormalStage;

    public event Action<int> EnterBossStage;
    public event Action<int> ExitBossStage;

    public event Action<int> EnterBonusStage;

    private WaveStage _currentStage = 0;

    private float _counter = 0f;
    [SerializeField] private float _normalStageDuration = 3; //3 minutes
    [SerializeField] private float _bossFightDuration = 3; //3 minutes
    [SerializeField] private float _bonusStageDuration = 3; //3 minutes
    [SerializeField] private float _maxMap = 3; //3 

    [SerializeField] private GameObject[] _bossTitles;

    private int mapIndex = 0;

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _normalStageDuration *= 60;
        _bossFightDuration *= 60;
        _bonusStageDuration *= 60;

        NormalStage();
    }

    void Update()
    {
        _counter += Time.deltaTime;

        if (_currentStage == WaveStage.Normal && _counter >= _normalStageDuration)
        {
            BossStage();
        }
        else if (_currentStage == WaveStage.BossFight && _counter >= _bossFightDuration)
        {
            BonusStage();
        }
        else if (_currentStage == WaveStage.Bonus && _counter >= _bonusStageDuration)
        {
            NormalStage();
        }

        
    }

    private void NormalStage()
    {
        _currentStage = WaveStage.Normal;
        _counter = 0;
        EnterNormalStage?.Invoke(mapIndex);
        Debug.Log("Normal Stage");
    }

    private void BossStage()
    {
        _currentStage = WaveStage.BossFight;
        _counter = 0;
        EnterBossStage?.Invoke(mapIndex);
        StartCoroutine(ShowTitle(mapIndex));
        Debug.Log("Boss Fight Stage");
        mapIndex++;
        if(mapIndex > _maxMap)
        {
            mapIndex = 0;
        }
    }

    IEnumerator ShowTitle(int index)
    {
        if (_bossTitles[index] != null)
        {
            _bossTitles[index].SetActive(true);
        }

        yield return new WaitForSeconds(6);

        if (_bossTitles[index] != null)
        {
            _bossTitles[index].SetActive(false);
        }
    }

    private void BonusStage()
    {
        ExitBossStage?.Invoke(mapIndex - 1);
        _currentStage = WaveStage.Bonus;
        _counter = 0;
        EnterBonusStage?.Invoke(mapIndex);
        Debug.Log("Bonus Stage");
    }
}
