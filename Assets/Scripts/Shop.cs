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


	[SerializeField] private TextMeshProUGUI _shootingText;
	[SerializeField] private TextMeshProUGUI _descriptionText;
	private int _currentSelectionSlot;

	private bool _moveAxisInUse;

	public List<Slot> _slots;
	public Queue<Upgrade> _saleQueue; // holds items that are not yet on sale

	private bool _justBoughtShootingUpgrade;

	private UI ui;


	private void OnDisable()
	{
		if (ui)
		{
			ui.KeepOpen = false;
		}
		
	}

	// Use this for initialization
	protected void Awake ()
	{

		ui = FindObjectOfType<UI>();
		
		_saleQueue = new Queue<Upgrade>();
		_slots = GetComponentsInChildren<Slot>(true).ToList();

		Upgrade healthUpgrade = UpgradeBuilder.Instance.GetHealthUpgrade(10, 10);
		Upgrade healthUpgrade2 = UpgradeBuilder.Instance.GetHealthUpgrade(20, 15);
		Upgrade healthUpgrade3 = UpgradeBuilder.Instance.GetHealthUpgrade(50, 500);
		Upgrade healthUpgrade4 = UpgradeBuilder.Instance.GetHealthUpgrade(100, 1000);

		Upgrade powerUpgrade = UpgradeBuilder.Instance.GetShotUpgrade(50, "Gives the ability to shoot fireballs");
		
		_saleQueue.Enqueue(healthUpgrade);
		_saleQueue.Enqueue(powerUpgrade);
		_saleQueue.Enqueue(healthUpgrade2);
		_saleQueue.Enqueue(healthUpgrade3);
		_saleQueue.Enqueue(healthUpgrade4);
		
		foreach (var slot in _slots)
		{
			
			if (_saleQueue.Count < 1)
			{
				break;
			}

			var upgrade = _saleQueue.Dequeue();
			slot.Upgrade = upgrade;
			_descriptionText.text = upgrade.Description;
		}
	}


	public void RefillSlots(params Upgrade[] upgrades)
	{
		_slots = GetComponentsInChildren<Slot>(true).ToList();
		_saleQueue = new Queue<Upgrade>(upgrades);
		
		
		foreach (var slot in _slots)
		{
			
			if (_saleQueue.Count < 1)
			{
				break;
			}

			var upgrade = _saleQueue.Dequeue();
			slot.Upgrade = upgrade;
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


	public override void FlipDialogue()
	{
		base.FlipDialogue();

		ui.KeepOpen = !ui.KeepOpen;
		
		if (_justBoughtShootingUpgrade)
		{
			StartCoroutine(ShowShootingHelpText());
			_justBoughtShootingUpgrade = false;
		}
	}

	private IEnumerator ShowShootingHelpText()
	{
		var timer = 0f;
		var totaltime = 4f;

		while ((timer += Time.deltaTime) < 0.75f)
		{
			var c = _shootingText.color;

			c.a = Mathf.Lerp(0, 1, timer / 0.75f);

			_shootingText.color = c;
			
			yield return null;
		}
		
		var c2 = _shootingText.color;

		c2.a = 1;

		_shootingText.color = c2;
		
		yield return new WaitForSeconds(totaltime);

		timer = 0;
		
		while ((timer += Time.deltaTime) < 0.75f)
		{
			var c = _shootingText.color;

			c.a = Mathf.Lerp(1,0, timer / 0.75f);

			_shootingText.color = c;
			
			yield return null;
		}
		
		var c3 = _shootingText.color;

		c3.a = 0;

		_shootingText.color = c3;

		
	}

	// Update is called once per frame	
	protected override void Update()
	{
		base.Update();	
		
		if (Math.Abs(Time.timeScale) < 0.01f)
		{
			var input = Input.GetAxisRaw("Horizontal");

			if (input == 0)
			{
				_moveAxisInUse = false;
			}
			
			if(input > 0.1 && !_moveAxisInUse)
			{
				_moveAxisInUse = true;
				StartCoroutine(MoveFrame(false));
			}
			else if (input < -0.1 && !_moveAxisInUse)
			{
				_moveAxisInUse = true;
				StartCoroutine(MoveFrame(true));
			}
			
			
			if (Input.GetButtonDown("Fire3") && !_slots[_currentSelectionSlot].IsEmpty())
			{
				
				var slot = _slots[_currentSelectionSlot];

				if (slot.Upgrade.Price <= FindObjectOfType<Player>().Currency)
				{
					if (slot.Upgrade is SkillUpgrade)
					{
						_justBoughtShootingUpgrade = true;
					}
					
					AudioManager.Instance.Play("Buy", 6f);
					
					slot
						.Upgrade
						.GetComponent<Upgrade>()
						.OnAquire
						.Invoke();

					slot.Upgrade = null;
					_descriptionText.text = "";
					
					FillSlotWithNewItem(slot);
				}

				
			}
		}


	}

	IEnumerator MoveFrame(bool moveLeft)
	{
		float elapsedTime = 0;

		var startPos = _slots[_currentSelectionSlot].transform.position;
			
		_currentSelectionSlot = moveLeft
			? (int) Mathf.Repeat(++_currentSelectionSlot, _slots.Count)
			: (int) Mathf.Repeat(--_currentSelectionSlot, _slots.Count);
		
		var endPos = _slots[_currentSelectionSlot].transform.position;
		
		while ((elapsedTime += Time.unscaledDeltaTime) < FrameMoveTime)
		{
			
			_selectionFrame.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / FrameMoveTime);
			
			yield return null;
			
		}
		
		_selectionFrame.transform.position = _slots[_currentSelectionSlot].transform.position;

		var selectedUpgrade = _slots[_currentSelectionSlot];

		_descriptionText.text = selectedUpgrade.Upgrade ? _slots[_currentSelectionSlot].Upgrade.Description : "";
	
	}

	private void FillSlotWithNewItem(Slot slot)
	{
		if (_saleQueue.Count != 0)
		{
			var upgrade = _saleQueue.Dequeue();
			_descriptionText.text = upgrade.Description;
			slot.Upgrade = upgrade;
		}
	}
}

