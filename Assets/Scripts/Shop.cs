using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : Interactable
{
	[SerializeField] private List<Upgrade> _itemsOnSale;
	[SerializeField] private List<GameObject> _slots;
	[SerializeField] private GameObject _selectionFrame;


	private int _currentSelectionSlot;
	
	// Use this for initialization
	protected  override void Start () {
		base.Start();
		

//		_itemsOnSale.Add(new Upgrade());
//		titleText.SetText(Title);
	//	paragraphText.SetText(Text);


		GameObject healthUpgrade = UpgradeBuilder.Instance.GetHealthUpgrade(10, 100);
		
		_slots[0].GetComponent<Slot>().SetSlotValues(
			healthUpgrade
			);

		GameObject healthUpgrade2 = UpgradeBuilder.Instance.GetHealthUpgrade(10, 150);
		
		_slots[1].GetComponent<Slot>().SetSlotValues(
			healthUpgrade2
			);
		
		GameObject healthUpgrade3 = UpgradeBuilder.Instance.GetHealthUpgrade(50, 150);
		
		_slots[2].GetComponent<Slot>().SetSlotValues(
			healthUpgrade3
		);
	}
	
	// Update is called once per frame	
	protected override void Update()
	{
		base.Update();

		if (Math.Abs(Time.timeScale) < 0.01f)
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				_currentSelectionSlot = (int) Mathf.Repeat(++_currentSelectionSlot, _slots.Count);

			} 
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				_currentSelectionSlot = (int) Mathf.Repeat(--_currentSelectionSlot, _slots.Count);
			}
		
			_selectionFrame.transform.position = _slots[_currentSelectionSlot].transform.position;

			if (Input.GetKeyDown(KeyCode.C))
			{
				_slots[_currentSelectionSlot]
					.GetComponent<Slot>()
					.Upgrade
					.GetComponent<HealthUpgrade>()
					.OnAquire
					.Invoke();
				
				
				
				
				//Destroy(_slots[_currentSelectionSlot]);
			}
		}

		print(_currentSelectionSlot);
	}

	
}

