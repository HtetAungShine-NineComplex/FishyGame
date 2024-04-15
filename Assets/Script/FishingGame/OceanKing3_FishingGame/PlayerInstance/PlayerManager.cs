using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private int _initialAmount; //for testing

    public event Action<int> CoinAmountChanged;

    private int _playerCoin; //total coin amount from user for this match(api)

    public int PlayerCoin 
    {  
        get { return _playerCoin; } 
        set 
        { 
            _playerCoin = value;

            if(_playerCoin <= 0)
                _playerCoin = 0;

            CoinAmountChanged?.Invoke(_playerCoin);
        }
    }

    private void Start()
    {
        PlayerCoin = _initialAmount;
    }

    public void UseCoin(int useAmount)
    {
        PlayerCoin -= useAmount;
    }

    public void AddCoin(int addAmount)
    {
        PlayerCoin += addAmount;
    }
}
