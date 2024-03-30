using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallDragonForAnim : MonoBehaviour
{
    [SerializeField] private RectTransform imageTransform;
    [SerializeField] private float moveSpeed = 1100f;
    [SerializeField] private RectTransform targetTransform;

    private void Update()
    {
        imageTransform.position = Vector3.MoveTowards(imageTransform.position, targetTransform.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(imageTransform.position, targetTransform.position) < 0.01f)
        {
            Destroy(gameObject);
        }      
    }
}
