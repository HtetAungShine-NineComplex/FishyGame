using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCrab : Fish
{
    [SerializeField] private PowerUpType _powerUpType;
    [SerializeField] private CannonHandler _cannonHandler; //for player temp
    [SerializeField] private GameObject _cannonObject;
    [SerializeField] private GameObject _powerUpTitle;

    protected override void Start()
    {
        base.Start();

        _cannonHandler = CanvasInstance.Instance.GetCannonHandler();
    }

    public override void OnDead()
    {
        base.OnDead();

        _cannonHandler.ActivePowerUp(_powerUpType);

        StartCoroutine(MoveCannonObjectAndTitle());
    }

    private IEnumerator MoveCannonObjectAndTitle()
    {
        float elapsedTime = 0;
        float duration = 2f;

        float titleElapsedTime = 0;
        float titleDuration = 1.5f;

        GameObject cannon = null;
        if(_cannonObject != null)
        {
            cannon = Instantiate(_cannonObject, transform.position, Quaternion.identity, CanvasInstance.Instance.GetForegroundSpawn());
        }
        else
        {
            Debug.LogWarning("CannonObject is null");
        }

        GameObject title = null;
        if (_powerUpTitle != null)
        {
            title = Instantiate(_powerUpTitle, transform.position, Quaternion.identity, CanvasInstance.Instance.GetForegroundSpawn());
        }
        else
        {
            Debug.LogWarning("PowerUpTitle is null");
        }


        Vector3 startingPositionCannon = new Vector3();
        if (cannon != null)
        {
            startingPositionCannon = cannon.transform.position;
        }
        Vector3 targetPositionCannon = new Vector3();

        if (_powerUpType == PowerUpType.Hammer)
        {
            targetPositionCannon = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }
        else
        {
            targetPositionCannon = _cannonHandler.transform.position;
        }

        Vector3 startingPositionTitle = new Vector3();

        if(title != null)
        {
            startingPositionTitle = title.transform.position;
        }
        else
        {
            startingPositionTitle = Vector3.zero;
        }
        Vector3 targetPositionTitle = new Vector3();

        targetPositionTitle = _cannonHandler.transform.position + new Vector3(0f, 200f, 0f);

        while (elapsedTime < duration )
        {
            if(cannon != null)
            {
                cannon.transform.position = Vector3.Lerp(startingPositionCannon, targetPositionCannon, elapsedTime / duration);
            }
            elapsedTime += Time.deltaTime;
            if(titleElapsedTime < titleDuration && title != null)
            {
                title.transform.position = Vector3.Lerp(startingPositionTitle, targetPositionTitle, titleElapsedTime / titleDuration);
                titleElapsedTime += Time.deltaTime;
            }
            yield return null;
        }



        // Ensure the objects end up exactly at the target positions.
        if(cannon != null)
        {
            cannon.transform.position = targetPositionCannon;
            if(_powerUpType == PowerUpType.Hammer)
            {
                Destroy(cannon.gameObject, 1f);
            }
            else
            {
                Destroy(cannon.gameObject);
            }
        }

        if(title != null)
        {
            title.transform.position = targetPositionTitle;
            Destroy(title.gameObject);
        }
    }
}

public enum PowerUpType
{
    Laser,
    Drill,
    Hammer
}
