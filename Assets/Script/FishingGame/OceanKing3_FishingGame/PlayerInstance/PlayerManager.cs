using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private int _initialAmount; //for testing
    [SerializeField] private CannonHandler _cannonHandler; //for testing

    public event Action<int> CoinAmountChanged;

    private int _playerCoin; //total coin amount from user for this match(api)

    private PlayerUIHandler _uiHandler;

    public string playerName;

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

    private void Awake()
    {
        _uiHandler = GetComponent<PlayerUIHandler>();
    }

    private void Start()
    {
        //PlayerCoin = _initialAmount;
        _uiHandler.ResetData();
        _cannonHandler.cannonController.ToggleControl(false);
    }

    public void UseCoin(int useAmount)
    {
        PlayerCoin -= useAmount;
    }

    public void AddCoin(int addAmount)
    {
        PlayerCoin += addAmount;
    }

    public void SetPlayerData(string name, int amount, bool isMe)
    {
        _uiHandler.SetData(name, amount);

        playerName = name;
        PlayerCoin = amount;

        _cannonHandler.cannonController.ToggleControl(isMe);
    }

    public void OnNetworkShoot(float rot, int amt)
    {
        PlayerCoin = amt;
        _cannonHandler.OnNetworkShoot(rot);
    }
}
