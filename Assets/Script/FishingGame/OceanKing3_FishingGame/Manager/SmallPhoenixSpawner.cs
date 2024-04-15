using UnityEngine;

public class SmallPhoenixSpawner : MonoBehaviour
{
    [SerializeField]private RectTransform phoenixTransform;
    [SerializeField]private RectTransform targetTransform;
    [SerializeField]private float speed;


    private void Update()
    {
        phoenixTransform.position = Vector3.MoveTowards(phoenixTransform.position, targetTransform.position, speed * Time.deltaTime);
        if (Vector3.Distance(phoenixTransform.position, targetTransform.position) < 0.01f)
        {
            Destroy(gameObject);
        }
    }
}