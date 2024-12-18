using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private GameObject _silverCoinPrefab;
    [SerializeField] private GameObject _rewardTxtPrefab;
    [SerializeField] private Transform _root;
    [SerializeField] private float _coinInterval;
    [SerializeField] private Transform _playerTransform; //temporary use only
    [SerializeField] private AudioSource _audioSource;

    public Transform PlayerTransform { get { return _playerTransform; } }

    public static CoinManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowSushi(Vector3 spawnPos, int score, GameObject sushiPrefab)
    {
        //instantiate reward text
        GameObject rewardTxt = Instantiate(_rewardTxtPrefab, _root);
        rewardTxt.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        rewardTxt.gameObject.GetComponent<RewardText>().SetValueText(score);

        StartCoroutine(SushiSpawn(spawnPos, sushiPrefab));
    }
    public void ShowCoin(Vector3 spawnPos, int coinAmount, int score)
    {
        //instantiate reward text
        GameObject rewardTxt = Instantiate(_rewardTxtPrefab, _root);
        rewardTxt.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        rewardTxt.gameObject.GetComponent<RewardText>().SetValueText(score);

        StartCoroutine(SilverCoinSpawn(spawnPos, coinAmount));
    }

    IEnumerator SilverCoinSpawn(Vector3 spawnPos, int coinAmount)
    {
        _audioSource.Stop();
        _audioSource.Play();
        for (int i = 0; i < coinAmount; i++)
        {
            
            Vector3 randomPos = new Vector3(Random.Range(spawnPos.x - 200, spawnPos.x + 200), 
                Random.Range(spawnPos.y - 200, spawnPos.y + 200), spawnPos.z);


            GameObject coinObj = Instantiate(_silverCoinPrefab, _root);
            coinObj.GetComponent<RectTransform>().anchoredPosition = randomPos;

            yield return new WaitForSeconds(_coinInterval);
        }
    }

    IEnumerator SushiSpawn(Vector3 spawnPos, GameObject sushiPrefab)
    {
        _audioSource.Stop();
        _audioSource.Play();

        GameObject sushiObj = Instantiate(sushiPrefab, _root);
        sushiObj.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        yield return new WaitForSeconds(_coinInterval);
    }
}
