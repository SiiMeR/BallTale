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


    public void TryToShoot(Vector2 direction)
    {
        if (IsOnCooldown()) return;

        Shoot(direction);
    }

    private void Shoot(Vector2 direction)
    {
        AudioManager.Instance.Play("Shot", 0.7f);

        var particle = Instantiate(_shotParticlePrefab, transform.position, Quaternion.identity);

        var shot = particle.GetComponent<Shot>();

        var shotData = new ShotData(direction, _shotSpeed, _maxShotRange);

        shot.ShotData = shotData;

        _shotCoolDownTimer = _shotCoolDown;
    }

    private bool IsOnCooldown()
    {
        return _shotCoolDownTimer > .0f;
    }
}