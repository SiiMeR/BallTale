using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int _damage;

    public int Damage
    {
        get => _damage;
        set => _damage = value;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<Player>();

            player.DamagePlayer(Damage);
        }
    }
}