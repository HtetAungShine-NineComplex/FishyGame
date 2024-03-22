using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCrab : Fish
{
    [SerializeField] private PowerUpType _powerUpType;
    [SerializeField] private CannonHandler _cannonHandler; //for player temp
    [SerializeField] private GameObject _cannonObject;

    protected override void Start()
    {
        base.Start();

        _cannonHandler = CanvasInstance.Instance.GetCannonHandler();
    }

    public override void OnDead()
    {
        base.OnDead();

        _cannonHandler.ActiveLaserCannon();
        StartCoroutine(MoveCannonObject());
    }

    private IEnumerator MoveCannonObject()
    {
        Debug.Log("MoveCannonObject");
        float elapsedTime = 0;
        float duration = 2f;
        

        GameObject cannon = Instantiate(_cannonObject, transform.position, Quaternion.identity, CanvasInstance.Instance.GetForegroundSpawn());

        Vector3 startingPosition = cannon.transform.position;
        Vector3 targetPosition = _cannonHandler.transform.position;

        while (elapsedTime < duration)
        {
            cannon.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object ends up exactly at the target position.
        cannon.transform.position = targetPosition;
        Destroy(cannon.gameObject);
    }
}

public enum PowerUpType
{
    Laser,
    Other //temp
}
