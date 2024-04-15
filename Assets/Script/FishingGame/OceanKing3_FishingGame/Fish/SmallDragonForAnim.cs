using UnityEngine;

public class SmallDragonForAnim : MonoBehaviour
{
    [SerializeField] private RectTransform dragonTransform;
    [SerializeField] private float moveSpeed = 1100f;
    [SerializeField] private RectTransform targetTransform;

    private void Update()
    {
        dragonTransform.position = Vector3.MoveTowards(dragonTransform.position, targetTransform.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(dragonTransform.position, targetTransform.position) < 0.01f)
        {
            Destroy(gameObject);
        }      
    }
}
