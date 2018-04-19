using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Shop : Interactable
{
	public float FrameMoveTime = 1.0f;
	
	[SerializeField] private List<Upgrade> _itemsOnSale;
	[SerializeField] private GameObject _selectionFrame;
	
	private List<Slot> _slots;
	private Queue<Upgrade> _saleQueue; // holds items that are not yet on sale
	private int _currentSelectionSlot;
	
	// Use this for initialization
	protected  override void Start () {
		base.Start();
		
		_saleQueue = new Queue<Upgrade>();
		_slots = GetComponentsInChildren<Slot>(true).ToList();

		Upgrade healthUpgrade = UpgradeBuilder.Instance.GetHealthUpgrade(10, 10);
		Upgrade healthUpgrade2 = UpgradeBuilder.Instance.GetHealthUpgrade(10, 15);
		Upgrade healthUpgrade3 = UpgradeBuilder.Instance.GetHealthUpgrade(50, 500);
		Upgrade healthUpgrade4 = UpgradeBuilder.Instance.GetHealthUpgrade(100, 1000);
		
		_saleQueue.Enqueue(healthUpgrade);
		_saleQueue.Enqueue(healthUpgrade2);
		_saleQueue.Enqueue(healthUpgrade3);
		_saleQueue.Enqueue(healthUpgrade4);
		
		foreach (var slot in _slots)
		{
			if (_saleQueue.Count < 1)
			{
				break;
			}
			slot.Upgrade = _saleQueue.Dequeue();
		}
	}

	
	private void AddItemToSlot(Upgrade item)
	{
		var freeSlots = _slots.FirstOrDefault(slot => slot.IsEmpty());

		if (freeSlots != null)
		{
			freeSlots.Upgrade = item;
		}
	}
	
	// Update is called once per frame	
	protected override void Update()
	{
		base.Update();

		if (Math.Abs(Time.timeScale) < 0.01f)
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				StartCoroutine(MoveFrame(true));
				
			} 
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				StartCoroutine(MoveFrame(false));
				
			}
			
			
			//_selectionFrame.transform.position = _slots[_currentSelectionSlot].transform.position;

			if (Input.GetKeyDown(KeyCode.C) && !_slots[_currentSelectionSlot].IsEmpty())
			{
				var slot = _slots[_currentSelectionSlot];
			
				if (slot.Upgrade.Price <= FindObjectOfType<Player>().Currency)
				{
					slot
						.Upgrade
						.GetComponent<HealthUpgrade>()
						.OnAquire
						.Invoke();

					slot.Upgrade = null;
				}

				FillSlotWithNewItem(slot);
			}
		}


	}

	IEnumerator MoveFrame(bool moveLeft)
	{
		float elapsedTime = 0;

		Vector3 startPos = _slots[_currentSelectionSlot].transform.position;

		_currentSelectionSlot = moveLeft
			? (int) Mathf.Repeat(++_currentSelectionSlot, _slots.Count)
			: (int) Mathf.Repeat(--_currentSelectionSlot, _slots.Count);
		
		Vector3 endPos = _slots[_currentSelectionSlot].transform.position;
		
		while ((elapsedTime += Time.unscaledDeltaTime) < FrameMoveTime)
		{
			
			_selectionFrame.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / FrameMoveTime);
			
			yield return null;
		}
		
		_selectionFrame.transform.position = _slots[_currentSelectionSlot].transform.position;
		
		
	}

	private void FillSlotWithNewItem(Slot slot)
	{
		if (_saleQueue.Count != 0)
		{
			slot.Upgrade = _saleQueue.Dequeue();
		}
	}
}

