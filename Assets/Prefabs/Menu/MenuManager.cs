using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MenuManager : MonoBehaviour {

	private Stack<Menu> _menuStack = new Stack<Menu>();
	
	public static MenuManager Instance { get; private set; }

	public MainMenu MainMenuPrefab;
	public OptionsMenu OptionsMenuPrefab;

	public void OpenMenu(Menu instance)
	{
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

			if (prefab != null) return prefab;
		}

		throw new Exception("Prefab not found that matches type.");
	}
	
	private void Awake()
	{
		Instance = this;

		MainMenu.Show();
	}

	private void OnDestroy()
	{
		Instance = null;
	}

}
