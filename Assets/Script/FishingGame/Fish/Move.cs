using UnityEngine;
public class Move : MonoBehaviour
{
    [SerializeField] private FishSO fishSO;
    
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
       
    }
    
    public void MoveFish(Vector3 startPoint_T, Vector3 destoryPoint_T)
    {
        elapsedTime += Time.deltaTime;
        float completePercent = elapsedTime / desiredDuration;
        transform.position = Vector2.Lerp(startPoint_T, destoryPoint_T, curve.Evaluate(completePercent));
        //transform.position = Vector2.MoveTowards(transform.position, destoryPoint_T.position, speed * Time.deltaTime);
        transform.up = (destoryPoint_T - transform.position).normalized;

        if (transform.position == destoryPoint_T)
            Destroy(this.gameObject);
    }
}