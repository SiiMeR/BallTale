using System.Collections;
using TMPro;
using UnityEngine;

public class SavePoint : Interactable
{

    [SerializeField] private TextMeshProUGUI _saveText;
    [SerializeField] private ParticleSystem _particleSystem;
    
    private Coroutine _displayTextCoroutine;
    
    public override void Interact()
    {
        if (!_notice.activeInHierarchy) return;
        
        var player = FindObjectOfType<Player>();
        player.CurrentHealth = player.MaxHealth;
        
        if(_displayTextCoroutine != null) StopCoroutine(_displayTextCoroutine);
        
        _displayTextCoroutine = StartCoroutine(DisplaySaveGameText());
        
        SaveGameManager.Instance.CreateSaveGame();
        
        AudioManager.Instance.Play("Buy");
        
        _particleSystem.Play();
        
        _notice.SetActive(false);
    }

    private IEnumerator DisplaySaveGameText()
    {
        var timer = 0f;

        var originalTextColor = _saveText.color;
        originalTextColor.a = 1.0f;
        _saveText.color = originalTextColor;

        var startPos = transform.position + Vector3.up;
        var endPos = startPos + Vector3.up * 1.5f;

        while ((timer += Time.deltaTime) < 1.0f)
        {
            var t = timer / 1.0f;
            var sint = Mathf.Sin(t * Mathf.PI * 0.5f);

            _saveText.transform.position = Vector3.Lerp(startPos, endPos, timer / 1.0f);

            var c = _saveText.color;
            c.a = Mathf.Lerp(1.0f, 0.0f, sint);
            _saveText.color = c;

            yield return null;
        }

        var endTextColor = _saveText.color;
        endTextColor.a = 0.0f;
        _saveText.color = endTextColor;
    }
}