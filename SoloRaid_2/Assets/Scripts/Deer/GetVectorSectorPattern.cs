using UnityEngine;

public class GetVectorSectorPattern : MonoBehaviour
{
    [SerializeField] private Vector3 Rotation;

    public Quaternion GetVector(Transform playerTransform)
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;   
        transform.rotation = Quaternion.LookRotation(direction);
        return transform.rotation;
    }
}
