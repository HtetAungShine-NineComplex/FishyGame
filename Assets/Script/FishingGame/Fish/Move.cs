using UnityEngine;
public class Move : MonoBehaviour
{
    [SerializeField] private FishSO fishSO;
    
    private float speed;
    private AnimationCurve curve;

    float elapsedTime;
    float desiredDuration = 7f;

    private Vector3 _startPoint;
    private Vector3 _endPoint;

    [SerializeField] private FishHealth _health;

    private void Start()
    {
        speed = fishSO.speed;
        curve = fishSO.SpeedCurve;
    }
    private void Update()
    {
       if(_startPoint != null && _endPoint != null)
        {
            MoveFish(_startPoint, _endPoint);
        }
    }

    public void SetPoints(Vector3 startPoint_T, Vector3 destoryPoint_T)
    {
        _startPoint = startPoint_T;
        _endPoint = destoryPoint_T;
    }
    
    public void MoveFish(Vector3 startPoint, Vector3 destoryPoint)
    {
        if(_health != null && _health._isDead)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        float completePercent = elapsedTime / desiredDuration;
        transform.position = Vector2.Lerp(startPoint, destoryPoint, curve.Evaluate(completePercent * speed));
        //transform.position = Vector2.MoveTowards(transform.position, destoryPoint_T.position, speed * Time.deltaTime);
        transform.up = (destoryPoint - transform.position).normalized;

        float distance = Vector3.Distance(transform.position, destoryPoint);

        if (distance <= 0.1f)
            Destroy(this.gameObject);
    }
}