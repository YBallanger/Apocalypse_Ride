using UnityEngine;

public class MissileCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
            other.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }
}
