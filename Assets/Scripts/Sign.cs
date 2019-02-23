using TMPro;
using UnityEngine;

public class Sign : Interactable
{
    [SerializeField] private TMP_Text _paragraphText;
    [SerializeField] private TMP_Text _titleText;

    [SerializeField] [TextArea] private string _text;
    [SerializeField] [TextArea] private string _title;


    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        _titleText.SetText(_title);
        _paragraphText.SetText(_text);
    }

}