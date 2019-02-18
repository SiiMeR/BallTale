using UnityEngine;

public class Firearm : MonoBehaviour // TODO build some contract between player and firearm
{
    [SerializeField] private float _maxShotRange = 10f;
    [SerializeField] private float _shotCoolDown = 1.0f;

    private double _shotCoolDownTimer;
    [SerializeField] private GameObject _shotParticlePrefab;
    [SerializeField] private float _shotSpeed = 20f;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (!ApplicationSettings.IsPaused()) _shotCoolDownTimer -= Time.deltaTime;
    }

    public void Shoot(Vector2 direction)
    {
        if (_shotCoolDownTimer > 0.0f)
            return;

        AudioManager.Instance.Play("Shot", 0.7f);

        var particle = Instantiate(_shotParticlePrefab, transform.position, Quaternion.identity);

        var shot = particle.GetComponent<Shot>();

        shot.MoveSpeed = _shotSpeed;

        shot.Direction = direction;

        shot.MaxRange = _maxShotRange;

        _shotCoolDownTimer = _shotCoolDown;
    }
}