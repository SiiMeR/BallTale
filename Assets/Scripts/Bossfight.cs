using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Deals with all the bureaucracy behind bossfights : making camera static, blocking entrances and so on...
/// </summary>
public class Bossfight : MonoBehaviour
{
    [SerializeField] private GameObject _boss;

    [SerializeField]
    private CinemachineVirtualCamera _bossFightCamera; // The virtual camera that pans the boss fight scene into view

    private bool _fightOn;
    [SerializeField] private List<GameObject> _walls;

    private void Awake()
    {
        _bossFightCamera.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) StartFight();
    }

    private void StartFight()
    {
        if (_fightOn) return;


        StartCoroutine(AudioManager.Instance.FadeToNextMusic("02VortexBoss"));
        _boss.SetActive(true);

        _bossFightCamera.enabled = true;

        StartCoroutine(ActivateWalls());
        _fightOn = true;
    }


    private IEnumerator ActivateWalls()
    {
        yield return new WaitForSeconds(0.1f);

        _walls.ForEach(go => go.SetActive(true));
    }

    public void Endfight()
    {
        if (!_boss || _boss.GetComponent<Boss>().CurrentHealth >= 1) return;

        _bossFightCamera.enabled = false;

        // FIGHTON FALSE TODO
        //	_walls.ForEach(go => go.SetActive(false));
    }
}