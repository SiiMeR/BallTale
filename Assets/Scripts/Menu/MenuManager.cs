using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	private Stack<Menu> _menuStack = new Stack<Menu>();

	private static MenuManager _instance;
	public static MenuManager Instance
	{
		get
		{
			return _instance;
		}
		private set
		{
			
		}
	}

	public MainMenu MainMenuPrefab;
	public OptionsMenu OptionsMenuPrefab;
	public CreditsMenu CreditsMenuPrefab;

	public PauseOptionsMenu PauseOptionsMenuPrefab;
//	public ContinueMenu ContinueMenuPrefab;
	public PauseMenu PauseMenuPrefab;

	private ButtonHighlighter _buttonHighlighter;
	
	public void OpenMenu(Menu instance)
	{
		SelectFirstButton(instance);
		
		
		// De-activate top menu
		if (_menuStack.Count > 0)
		{
			if (instance.DisableMenusUnderneath)
			{
				foreach (var menu in _menuStack)
				{
					menu.gameObject.SetActive(false);

					if (menu.DisableMenusUnderneath)
						break;
				}
			}

			var topCanvas = instance.GetComponent<Canvas>();
			var previousCanvas = _menuStack.Peek().GetComponent<Canvas>();
			topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
		}

		_menuStack.Push(instance);
	}

	private void SelectFirstButton(Menu instance)
	{
		var firstButton = instance.gameObject.GetComponentInChildren<Button>(true);
		_buttonHighlighter.HighlightButton(firstButton);
	}

	public void CloseMenu(Menu menu)
	{	
		
		if (_menuStack.Count == 0)
		{
			Debug.LogError("No menus open that can be closed. Type: " + menu.GetType());
			return;
		}

		if (_menuStack.Peek() != menu)
		{
			Debug.LogError(menu.GetType() + " cannot be closed because it is not on top of the stack.");
			return;
		}

		CloseTopMenu();
		
	}

	public void CloseTopMenu()
	{
		var instance = _menuStack.Pop();

		
		if (instance.DestroyWhenClosed)
		{
			Destroy(instance.gameObject);
		}
		else
		{
			instance.gameObject.SetActive(false);
		}
		
		foreach (var menu in _menuStack)
		{
			menu.gameObject.SetActive(true);

			if (menu.DisableMenusUnderneath)
			{
				break;
			}
		}

		if (_menuStack.Count > 0)
		{
			SelectFirstButton(_menuStack.Peek());
		}
		

	}

	public void CreateInstance<T>() where T : Menu
	{
		var prefab = GetPrefab<T>();

		Instantiate(prefab, transform);
	}
	
	
	private T GetPrefab<T>() where T : Menu
	{
		var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		
		foreach (var field in fields)
		{
			var prefab = field.GetValue(this) as T;

			if (prefab != null)
			{
				
				// set all button colors
				var prefabButtons = prefab.gameObject.GetComponentsInChildren<Button>().ToList();
				
				prefabButtons.ForEach(button =>
				{
					var colors = button.colors;
					
					
					colors.highlightedColor = Color.green;

					button.colors = colors;
				});
				
			//	Debug.Log("MenuManager - successfully set highlight colors on buttons");
				
				return prefab;
			}
		}

		throw new Exception("Prefab not found that matches type.");
	}
	
	private void Awake()
	{
		
		if (!_instance)
		{
			_instance = this;
		}

		_buttonHighlighter = GetComponent<ButtonHighlighter>();
//		AudioManager.instance.Play("01Peaceful");

		if (SceneManager.GetActiveScene().name == "Menu")
		{
			MainMenu.Show();
		}
		
	}

	private void OnDestroy()
	{
		Instance = null;
	}

}
