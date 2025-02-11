using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUIHandler : MonoBehaviour
{
    //name etc.

    [SerializeField] private TMP_Text _nameTxt;
    [SerializeField] private TMP_Text _coinAmountTxt;

    [SerializeField] private PlayerManager _playerManager;

    private void OnEnable()
    {
        _playerManager.CoinAmountChanged += OnCoinAmountChanged;
    }

    private void OnCoinAmountChanged(int  coinAmount)
    {
        _coinAmountTxt.text = coinAmount.ToString();
    }

    public void SetData(string name, int coinAmount)
    {
        _nameTxt.text = name;
        _coinAmountTxt.text = coinAmount.ToString();
    }

    public void ResetData()
    {
        _nameTxt.text = string.Empty;   
        _coinAmountTxt.text = string.Empty;
    }
}
