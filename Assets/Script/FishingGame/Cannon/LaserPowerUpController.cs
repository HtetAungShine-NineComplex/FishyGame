using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LaserPowerUpController : MonoBehaviour
{
    [SerializeField] private GameObject _laserBeam;
    [SerializeField] private GameObject _title;
    [SerializeField] private CannonHandler _cannonHandler;

    private bool _shoot = false;

    private void OnEnable()
    {
        _title.SetActive(true);
    }

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
        if(_shoot)
        {
            return;
        }

        _shoot = true;
        StartCoroutine(ShowLaserBeam());
    }

    IEnumerator ShowLaserBeam()
    {
        _laserBeam.SetActive(true);

        yield return new WaitForSeconds(4f);

        _title.SetActive(false);
        _laserBeam.SetActive(false);
        _cannonHandler.SwapWeapon();
        _shoot = false;
        gameObject.SetActive(false);
    }
}
