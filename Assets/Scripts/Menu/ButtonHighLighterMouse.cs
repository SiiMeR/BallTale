using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
 

/// <summary>
/// https://answers.unity.com/questions/1362482/how-to-set-ui-button-as-selected-on-mouse-over.html
/// </summary>
[RequireComponent(typeof(Selectable))]
public class ButtonHighLighterMouse : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!EventSystem.current.alreadySelecting)
			EventSystem.current.SetSelectedGameObject(gameObject);
	}
 
	public void OnDeselect(BaseEventData eventData)
	{
		GetComponent<Selectable>().OnPointerExit(null);
	}
}