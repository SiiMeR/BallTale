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
                _priceText.text = value.Price.ToString();
                _itemImage.sprite = value.GetComponent<SpriteRenderer>().sprite;

                var color = _itemImage.color;
                color.a = 1;
                _itemImage.color = color;
            }
            else
            {
                _priceText.text = null;
                _itemImage.sprite = null;

                var color = _itemImage.color;
                color.a = 0;
                _itemImage.color = color;
            }
        }
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