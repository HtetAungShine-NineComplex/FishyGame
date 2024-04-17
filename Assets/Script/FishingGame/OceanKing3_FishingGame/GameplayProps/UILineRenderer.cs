using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILineRenderer : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;

    public void SetDir(Vector2 globalStart, Vector2 globalEnd)
    {
        Vector2 start = _rect.parent.InverseTransformPoint(globalStart);
        Vector2 end = _rect.parent.InverseTransformPoint(globalEnd);

        Vector2 direction = end - start;
        float distance = direction.magnitude;
        _rect.sizeDelta = new Vector2(distance, _rect.sizeDelta.y);
        _rect.anchoredPosition = start + (direction / 2);
        _rect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }
}
