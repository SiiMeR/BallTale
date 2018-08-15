using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighlighter : MonoBehaviour
{
    public Button currentButton;

    private void Update()
    {
        if (!currentButton) return;

        var selectedObj = EventSystem.current.currentSelectedGameObject;

        if (selectedObj == null)
        {
            EventSystem.current.SetSelectedGameObject(currentButton.gameObject);
            return;
        }

        var selectedAsButton = selectedObj.GetComponent<Button>();
        if (selectedAsButton != null && selectedAsButton != currentButton)
        {
            if (selectedAsButton.transform.name != "PauseButton")
                AudioManager.Instance.Play("menuselect");
            HighlightButton(selectedAsButton);
        }


        if (currentButton != null && currentButton != selectedAsButton)
        {
            //	UnHighlightButton(previousButton);
        }

        currentButton = selectedAsButton;
    }

    private void OnDisable()
    {
        if (currentButton != null) UnHighlightButton(currentButton);
    }

    public void HighlightButton(Button butt)
    {
        currentButton = butt;
        butt.Select();
    }

    public void UnHighlightButton(Button butt)
    {
    }
}