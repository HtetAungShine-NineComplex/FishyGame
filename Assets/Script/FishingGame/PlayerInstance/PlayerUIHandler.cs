using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUIHandler : MonoBehaviour
{
    //name etc.

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
}
