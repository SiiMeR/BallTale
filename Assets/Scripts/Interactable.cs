using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private bool _isCollidingWPlayer;

    [SerializeField] private GameObject _notice;
    [SerializeField] private GameObject _panel;
    [SerializeField] private bool _pauseOnInteract;


    // Use this for initialization
    protected virtual void Start()
    {
        _notice.SetActive(false);
        _panel.SetActive(false);
    }

    // Update is called once per frame	
    protected virtual void Update()
    {
        if (Input.GetButtonDown("Fire2") && _isCollidingWPlayer) Interact();
    }

    public virtual void Interact()
    {
        if (_pauseOnInteract) Time.timeScale = _panel.activeInHierarchy ? 1.0f : 0.0f;
        _panel.SetActive(!_panel.activeInHierarchy);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _notice.SetActive(true);
            _isCollidingWPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _notice.SetActive(false);
            _panel.SetActive(false);
            _isCollidingWPlayer = false;
        }
    }
}