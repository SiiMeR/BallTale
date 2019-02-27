using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int _damage;

    private void OnTriggerStay2D(Collider2D other)
    {
        other.GetComponent<IDamageable>()?.Damage(_damage);
    }
}