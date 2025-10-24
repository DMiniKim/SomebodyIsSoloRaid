using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform target;
    Vector3 offset;
    float smoothSpeed = 5f;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = new Vector3(0,0,0);
    }
    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        
    }

}
