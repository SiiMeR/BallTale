using BayatGames.SaveGameFree;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeleteConfirmationMenu : SimpleMenu<DeleteConfirmationMenu>
{
    public Button noButton;

    // Use this for initialization
    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(noButton.gameObject);
    }

    public void DeleteSave()
    {
        SaveGame.Delete("player.txt");
        SaveGame.Delete("shop.txt");
        Hide();
    }
}