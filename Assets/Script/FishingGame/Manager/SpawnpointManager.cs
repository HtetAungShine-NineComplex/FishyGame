using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnPosition
{
    Top,
    Bottom,
    Left,
    Right
}

public class SpawnpointManager : MonoBehaviour
{
    float randomX = 0f;
    float randomY = 0f;

    float screenX = Screen.width;
    float screenY = Screen.height;

    private SpawnPosition spawnPosition;

    private float _offsetX;
    private float _offsetY;

    public static SpawnpointManager Instance;
    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        _offsetX = (screenX * 0.2f);
        _offsetY = (screenY * 0.2f);
    }

    public Vector3 GetRandomSpawnPoint()
    {
        int border = Random.Range(0, 4);

        switch (border)
        {
            case 0: //Top
                randomX = Random.Range(0f + (screenX * 0.1f), screenX - (screenX * 0.1f));
                randomY = screenY + _offsetY;
                spawnPosition = SpawnPosition.Top;
                break;

            case 1: //Bottom
                randomX = Random.Range(0f + (screenX * 0.1f), screenX - (screenX * 0.1f));
                randomY = 0f - _offsetY;
                spawnPosition = SpawnPosition.Bottom;
                break;

            case 2: //Left
                randomX = 0f - _offsetX;
                randomY = Random.Range(0f + (screenY * 0.1f), screenY - (screenY * 0.1f));
                spawnPosition = SpawnPosition.Left;
                break;

            case 3:
                randomX = screenX + _offsetX;
                randomY = Random.Range(0f + (screenY * 0.1f), screenY - (screenY * 0.1f));
                spawnPosition = SpawnPosition.Right;
                break;
        }

        //Vector3 spawnPoint = Camera.main.ScreenToWorldPoint(new Vector3(randomX, randomY, 10f));

        Vector3 spawnPoint = new Vector3(randomX, randomY, 0);
        return spawnPoint;

         
    }
    public Vector3 GetRandomEndPoint(Vector3 spawnPoint)
    {

        switch (spawnPosition)
        {
            case SpawnPosition.Top:
                return spawnPoint - new Vector3(0f, screenY + (_offsetY * 2), 0f);

            case SpawnPosition.Bottom:
                return spawnPoint - new Vector3(0f, -screenY - (_offsetY * 2), 0f);

            case SpawnPosition.Left:
                return spawnPoint - new Vector3(-screenX - (_offsetX * 2), 0f, 0f);

            case SpawnPosition.Right:
                return spawnPoint - new Vector3(screenX + (_offsetX * 2), 0f, 0f);

            default:
                return spawnPoint - new Vector3(0f, 0f, 0f);
        }

    }
}
