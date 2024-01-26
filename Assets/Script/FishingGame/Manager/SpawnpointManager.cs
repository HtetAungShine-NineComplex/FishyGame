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
        _offsetX = (screenX * 0.3f);
        _offsetY = (screenY * 0.3f);
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

    public Vector3 GetRandomSpawnPointVertical()
    {
        int border = Random.Range(2, 4);

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

    public SpawnPosition GetSpawnPosition()
    {
        return spawnPosition;
    }

    public Vector3 GetControlPoint(Vector3 spawnPoint, Vector3 endPoint, float dist)
    {
        // Calculate the midpoint between the spawn and end points
        Vector3 midPoint = (spawnPoint + endPoint) / 2f;

        // Calculate a direction perpendicular to the line between the spawn and end points
        Vector3 perpDirection = new Vector3(-(endPoint.y - spawnPoint.y), endPoint.x - spawnPoint.x, 0f).normalized;

        // Move the control point a certain distance away from the midpoint in the perpendicular direction
        float controlPointDistance = dist; // Adjust this value to change the curvature of the path
        Vector3 controlPoint = midPoint + perpDirection * controlPointDistance;

        return controlPoint;
    }

    public Vector3 GetRandomEndPoint(Vector3 spawnPoint, SpawnPosition spawnPos)
    {
        float endY = 0;
        float endX = 0;
        float lastY = 0;
        float lastX = 0;
        Vector3 endPoint = Vector3.zero;

        float randomX = Random.Range(0f + (screenX * 0.1f), screenX - (screenX * 0.1f));
        float randomY = Random.Range(0f + (screenY * 0.1f), screenY - (screenY * 0.1f));



        switch (spawnPos)
        {
            case SpawnPosition.Top:
                endY = screenY + (_offsetY * 2);
                lastY = spawnPoint.y - endY;
                endPoint = new Vector3(randomX, lastY, 0f);

                return endPoint;

            case SpawnPosition.Bottom:
                endY = -screenY - (_offsetY * 2);
                lastY = spawnPoint.y - endY;
                endPoint = new Vector3(randomX, lastY, 0f);

                return endPoint;

                //return spawnPoint - new Vector3(0f, -screenY - (_offsetY * 2), 0f);

            case SpawnPosition.Left:
                endX = -screenX - (_offsetX * 2);
                lastX = spawnPoint.x - endX;
                endPoint = new Vector3(lastX, randomY, 0f);

                return endPoint;

                //return spawnPoint - new Vector3(-screenX - (_offsetX * 2), 0f, 0f);

            case SpawnPosition.Right:
                endX = screenX + (_offsetX * 2);
                lastX = spawnPoint.x - endX;
                endPoint = new Vector3(lastX, randomY, 0f);

                return endPoint;

                //return spawnPoint - new Vector3(screenX + (_offsetX * 2), 0f, 0f);

            default:
                return spawnPoint - new Vector3(0f, 0f, 0f);
        }

    }
}
