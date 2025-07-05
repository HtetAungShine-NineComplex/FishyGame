using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.WebSocketSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayNetworkManager : MonoBehaviour
{
    public List<PlayerManager> _playerMangers;

    public List<FishSpawnData> fishSpawnDatas;
    public Dictionary<string, FishManager> fishManagerDic;

    private void Awake()
    {
        fishManagerDic = new Dictionary<string, FishManager>();
        GlobalManager.Instance.ExtensionResponse += OnExtensionResponse;

        foreach (FishSpawnData data in fishSpawnDatas)
        {
            fishManagerDic.Add(data.fishName, data.manager);
        }
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

            case "fishSpawn":
                OnFishSpawn(sfsobject);
                break;

            case "stageUpdate":
                OnStageUpdate(sfsobject);
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

        if (userName != GlobalManager.Instance.GetSfsClient().MySelf.Name)
        {
            GetPlayerByName(userName).OnNetworkShoot(z, balance);
        }
    }

    void OnFishSpawn(ISFSObject data)
    {

        string fishType = data.GetUtfString("fishType");
        float normalizedX = data.GetFloat("x");
        float normalizedY = data.GetFloat("y");
        string spawnSide = data.GetUtfString("spawnSide");
        float endX = data.GetFloat("endX");
        float endY = data.GetFloat("endY");

        // Convert normalized spawn position to world position
        Vector3 spawnPoint = SpawnpointManager.Instance.GetSpawnPointOnline(normalizedX, normalizedY, spawnSide);
        Vector3 endPoint = SpawnpointManager.Instance.GetEndPointOnline(endX, endY);

        // Spawn the fish
        //SpawnFish(fishType, spawnPoint, endPoint);
        fishManagerDic[fishType].SpawnFish(spawnPoint, endPoint);
    }

    void OnStageUpdate(ISFSObject data)
    {
        int newStage = data.GetInt("stage");

        Debug.Log("Stage changed to: " + newStage);

        switch (newStage)
        {
            case 1:
                WaveManager.Instance.NormalStage();
                break;

            case 2:
                WaveManager.Instance.BossStage();
                break;

            case 3:
                WaveManager.Instance.BonusStage();
                break;

            default:
                break;
        }
    }
}

[Serializable]
public class FishSpawnData
{
    public string fishName;
    public FishManager manager;
}
