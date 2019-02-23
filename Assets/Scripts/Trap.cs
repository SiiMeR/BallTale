using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int _damage;

    private void OnTriggerStay2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        
        if (player)
        {
            player.DamagePlayer(_damage);
        }
    }
}