using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossRewardShower : MonoBehaviour
{
    [SerializeField] private TMP_Text _amountTxt;

    public void ShowReward(int amount)
    {
        gameObject.SetActive(true);
        _amountTxt.text = amount.ToString();
        StartCoroutine(AmountShower());
    }

    IEnumerator AmountShower()
    {
        

        yield return new WaitForSeconds(6f);
        gameObject.SetActive(false);
        yield break;
    }
}
