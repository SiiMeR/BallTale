using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeleteConfirmationMenu : SimpleMenu<DeleteConfirmationMenu>
{

	public Button noButton;
	
	// Use this for initialization
	void Start () {
		EventSystem.current.SetSelectedGameObject(noButton.gameObject);
	}

	public void DeleteSave()
	{
		SaveGameManager.Instance.DeleteSave();
		Hide();
		
	}
}
