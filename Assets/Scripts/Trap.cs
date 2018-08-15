using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int _damage;

    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
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