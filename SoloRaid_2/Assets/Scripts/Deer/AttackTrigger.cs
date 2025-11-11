using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    [SerializeField]float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMove>().TakeDamage(damage);
            Debug.Log(other);
        }
        else if (other.gameObject.CompareTag("Boss"))
        {
            other.gameObject.GetComponent<BossAi>().Hit(damage);
            Debug.Log(other);
        }
        else
        {
            return;
        }
    }
    

}
