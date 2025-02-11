using Sfs2X.Core;
using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayNetworkManager : MonoBehaviour
{
    public List<PlayerManager> _playerMangers;

    private void Awake()
    {
        GlobalManager.Instance.ExtensionResponse += OnExtensionResponse;
    }

    private void Start()
    {
        GlobalManager.Instance.RequestJoinRoom();
    }

    private void OnDestroy()
    {
        GlobalManager.Instance.ExtensionResponse -= OnExtensionResponse;
    }


    private void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string)evt.Params["cmd"];
        ISFSObject sfsobject = (SFSObject)evt.Params["params"];

        Debug.Log("Event Received : " + cmd);

        switch (cmd)
        {
            case "roomPlayerList":
                OnRoomPlayerList(sfsobject); break;

            case "shoot":
                OnShoot(sfsobject);
                break;

            default:
                break;
        }
    }

    private PlayerManager GetPlayerByName(string name)
    {
        foreach (PlayerManager player in _playerMangers)
        {
            if (player.playerName == name)
            {
                return player;
            }
        }

        return null;
    }

    private void OnRoomPlayerList(ISFSObject obj)
    {
        ISFSArray sFSArray = obj.GetSFSArray("userArray");

        for (int i = 0; i < sFSArray.Size(); i++)
        {
            ISFSObject data = sFSArray.GetSFSObject(i);

            string userName = data.GetUtfString("userName");
            int balance = data.GetInt("balance");

            _playerMangers[i].SetPlayerData(userName, balance, userName == GlobalManager.Instance.GetSfsClient().MySelf.Name);
        }
    }

    private void OnShoot(ISFSObject obj)
    {

        string userName = obj.GetUtfString("userName");
        int balance = obj.GetInt("balance");
        float z = obj.GetFloat("zRotation");

        if(userName != GlobalManager.Instance.GetSfsClient().MySelf.Name)
        {
            GetPlayerByName(userName).OnNetworkShoot(z, balance);
        }
    }
}
