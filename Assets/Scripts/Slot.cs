using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TMP_Text _priceText;

    private Upgrade _upgrade;

    public Upgrade Upgrade
    {
        get => _upgrade;
        set
        {
            _upgrade = value;

            if (value != null)
            {
                UpdateValues(value);
                ShowImage();
            }
            else
            {
                Clear();
                HideImage();
            }
        }
    }

    private void UpdateValues(Upgrade upgrade)
    {
        _priceText.text = upgrade.Price.ToString();
        _itemImage.sprite = upgrade.GetComponent<SpriteRenderer>().sprite;
    }

    private void Clear()
    {
        _priceText.text = null;
        _itemImage.sprite = null;
    }

    private void HideImage()
    {
        var color = _itemImage.color;
        color.a = 0;
        _itemImage.color = color;
    }

    private void ShowImage()
    {
        var color = _itemImage.color;
        color.a = 1;
        _itemImage.color = color;
    }


    public bool IsEmpty()
    {
        return Upgrade == null;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(_priceText)}: {_priceText}, {nameof(_upgrade)}: {_upgrade}";
    }
}