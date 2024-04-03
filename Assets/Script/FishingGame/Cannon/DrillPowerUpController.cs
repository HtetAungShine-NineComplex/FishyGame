using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillPowerUpController : MonoBehaviour
{
    [SerializeField] private CannonHandler _cannonHandler;
    //[SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _drillPrefab;
    [SerializeField] private Transform _shootPoint;

    private bool _shoot = false;

    private void Update()
    {
        if (_shoot)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            SyncRotation();
            Shoot();
        }
    }

    private void SyncRotation()
    {

        Vector2 mousePosition = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasInstance.Instance.GetMainCanvas().transform as RectTransform,
            mousePosition, CanvasInstance.Instance.GetMainCanvas().worldCamera, out Vector2 localMousePosition);
        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void Shoot()
    {
        if (_shoot)
        {
            return;
        }

        _shoot = true;
        /*_audioSource.Stop();
        _audioSource.Play();*/

        StartCoroutine(ShootDrill());
    }

    IEnumerator ShootDrill()
    {
        GameObject bulletObj = Instantiate(_drillPrefab, _shootPoint.position, _shootPoint.rotation,
            CanvasInstance.Instance.GetMainCanvas().transform);

        yield return new WaitForSeconds(5f);

        _cannonHandler.SwapWeapon();
        _shoot = false;
        gameObject.SetActive(false);
    }
}
