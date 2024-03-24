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

        _cannonHandler.ActiveLaserCannon();
        StartCoroutine(MoveCannonObjectAndTitle());
    }

    private IEnumerator MoveCannonObjectAndTitle()
    {
        float elapsedTime = 0;
        float duration = 2f;

        float titleElapsedTime = 0;
        float titleDuration = 1.5f;

        GameObject cannon = Instantiate(_cannonObject, transform.position, Quaternion.identity, CanvasInstance.Instance.GetForegroundSpawn());
        GameObject title = Instantiate(_powerUpTitle, transform.position, Quaternion.identity, CanvasInstance.Instance.GetMidGroundSpawn());

        Vector3 startingPositionCannon = cannon.transform.position;
        Vector3 targetPositionCannon = _cannonHandler.transform.position;

        Vector3 startingPositionTitle = title.transform.position;
        Vector3 targetPositionTitle = _cannonHandler.transform.position + new Vector3(0f, 250f, 0f);

        while (elapsedTime < duration)
        {
            cannon.transform.position = Vector3.Lerp(startingPositionCannon, targetPositionCannon, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            if(titleElapsedTime < titleDuration)
            {
                title.transform.position = Vector3.Lerp(startingPositionTitle, targetPositionTitle, titleElapsedTime / titleDuration);
                titleElapsedTime += Time.deltaTime;
            }
            yield return null;
        }



        // Ensure the objects end up exactly at the target positions.
        cannon.transform.position = targetPositionCannon;
        title.transform.position = targetPositionTitle;
        Destroy(cannon.gameObject);
        Destroy(title.gameObject);
    }
}

public enum PowerUpType
{
    Laser,
    Other //temp
}
