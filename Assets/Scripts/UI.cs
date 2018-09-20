using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private bool _isFaded;

    private bool _keepOpen;


    [SerializeField] private TextMeshProUGUI currency;
    [SerializeField] private CanvasGroup currencyCanvas;
    public float CurrencyFadeCD = 3.0f;

    public float CurrencyFadeTime = 1.0f;

    [SerializeField] private Image health;
    [SerializeField] private CanvasGroup healthCanvas;

    private int lastCurrency;

    private Player player;
    public float Timer;

    public bool KeepOpen
    {
        get { return _keepOpen; }
        set
        {
            if (value)
            {
                currencyCanvas.alpha = 1;
                Timer = 0;
                _isFaded = false;
            }
            else
            {
                StartCoroutine(FadeOut());
                Timer = 0;
                _isFaded = true;
            }

            _keepOpen = value;
        }
    }

    // Use this for initialization
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (lastCurrency != player.Currency) // changed
        {
            StopCoroutine(FadeOut());
            Timer = 0;
            _isFaded = false;
            currencyCanvas.alpha = 1;
            StartCoroutine(SmoothChangeValue(lastCurrency, player.Currency, 0.5f, currency));
        }
        else if ((Timer += Time.unscaledDeltaTime) > CurrencyFadeCD && !_isFaded && !KeepOpen)
        {
            Timer = 0;
            StartCoroutine(FadeOut());
        }

        health.fillAmount = (float) player.CurrentHealth / player.MaxHealth;


        lastCurrency = player.Currency;
    }

    private IEnumerator SmoothChangeValue(float start, float end, float time, TMP_Text text)
    {
        var timer = 0f;

        while ((timer += Time.unscaledDeltaTime) < time)
        {
            var lerp = (int) Mathf.LerpUnclamped(start, end, timer / time);
            text.SetText(lerp.ToString());
            yield return null;
        }

        text.SetText(end.ToString());
    }

    private IEnumerator FadeOut()
    {
        var timer = 0f;

        while ((timer += Time.unscaledDeltaTime) < CurrencyFadeTime)
        {
            Timer = 0;

            currencyCanvas.alpha = Mathf.Lerp(1, 0, timer / CurrencyFadeTime);

            yield return null;
        }

        _isFaded = true;
        currencyCanvas.alpha = 0;
    }
}