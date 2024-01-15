using UnityEngine;
public class Move : MonoBehaviour
{
    [SerializeField] private FishSO fishSO;
    [SerializeField] private Transform destoryPoint_T;
    [SerializeField] private Transform startPoint_T;
    
    private float speed;
    private AnimationCurve curve;

    float elapsedTime;
    float desiredDuration = 7f;
    private void Start()
    {
        speed = fishSO.speed;
        curve = fishSO.SpeedCurve;
    }
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float completePercent = elapsedTime / desiredDuration;
        transform.position = Vector2.Lerp(startPoint_T.position, destoryPoint_T.position, curve.Evaluate(completePercent));
        //transform.position = Vector2.MoveTowards(transform.position, destoryPoint_T.position, speed * Time.deltaTime);
        transform.up = (destoryPoint_T.position - transform.position).normalized;

        if (transform.position == destoryPoint_T.position)
            Destroy(this.gameObject);
    }
    
}