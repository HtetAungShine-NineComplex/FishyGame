using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointManager : MonoBehaviour
{
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
    public Vector3 GetRandomSpawnPoint()
    {
        float randomX = 0f;
        float randomY = 0f;

        int border = Random.Range(0, 4);

        float screenX = Screen.width;
        float screenY = Screen.height;

        switch (border)
        {
            case 0: //Top
                randomX = Random.Range(0f, screenX);
                randomY = screenY;
                break;

            case 1: //Bottom
                randomX = Random.Range(0f, screenX);
                randomY = 0f;
                break;

            case 2: //Left
                randomX = 0f;
                randomY = Random.Range(0f, screenY);
                break;

            case 3:
                randomX = screenX;
                randomY = Random.Range(0f, screenY);
                break;
        }

        Vector3 spawnPoint = Camera.main.ScreenToWorldPoint(new Vector3(randomX, randomY, 10f));
        return spawnPoint;

         
    }
    public Vector3 GetRandomEndPoint(Vector3 spawnPoint)
    {
        return spawnPoint - new Vector3(2000f, 0f, 0f);
    }
}
