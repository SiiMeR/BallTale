using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Shop : Interactable
{
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private GameObject _selectionFrame;
    [SerializeField] private TextMeshProUGUI _shootingText;

    public List<Slot> Slots;
    public float FrameMoveTime = 1.0f;
    public Queue<Upgrade> _saleQueue; // holds items that are not yet on sale

    private int _currentSelectionSlot;
    private bool _justBoughtShootingUpgrade;
    private bool currentlyMovingFrame;
    private UI _ui;
    private Player _player;


    private void OnDisable()
    {
        if (_ui) _ui.KeepOpen = false;
    }

    // Use this for initialization
    protected void Awake()
    {
        _player = FindObjectOfType<Player>();
        _ui = FindObjectOfType<UI>();
        _saleQueue = new Queue<Upgrade>();
        Slots = GetComponentsInChildren<Slot>(true).ToList();


        if (PlayerPrefs.GetInt("loadgame") == 0)
        {
            InitShopInventory();
        }
    }
    
    /// <summary>
    /// Fills shop inventory with items when starting a new game.
    /// Should not be called when loading game from an existing savegame.
    /// </summary>
    private void InitShopInventory()
    {
        var healthUpgrade = UpgradeBuilder.Instance.GetHealthUpgrade(10, 10);
        var healthUpgrade2 = UpgradeBuilder.Instance.GetHealthUpgrade(20, 15);
        var healthUpgrade3 = UpgradeBuilder.Instance.GetHealthUpgrade(50, 500);
        var healthUpgrade4 = UpgradeBuilder.Instance.GetHealthUpgrade(100, 1000);

        var powerUpgrade = UpgradeBuilder.Instance.GetShootingUpgrade(50, "Gives the ability to shoot fireballs");

        var initialUpgrades = new List<Upgrade>{healthUpgrade,healthUpgrade2,healthUpgrade3,healthUpgrade4,powerUpgrade};
        
        RefillSlots(initialUpgrades);    
    }

    
    public void RefillSlots(List<Upgrade> upgrades)
    {
        Slots = GetComponentsInChildren<Slot>(true).ToList();

        upgrades.ToList().ForEach(upgrade => _saleQueue.Enqueue(upgrade));

        foreach (var slot in Slots)
        {
            FillSlotWithNewItem(slot);
        }
    }

    public override void Interact()
    {
        base.Interact();

        _ui.KeepOpen = !_ui.KeepOpen;

        if (_selectionFrame.activeInHierarchy)
        {
            var slot = Slots[_currentSelectionSlot];
            _descriptionText.text = slot.IsEmpty() ?  "" : slot.Upgrade.Description;
        }
        
        if (_justBoughtShootingUpgrade)
        {
            StartCoroutine(ShowShootingHelpText());
            _justBoughtShootingUpgrade = false;
        }
    }

    private IEnumerator ShowShootingHelpText()
    {
        var timer = 0f;    
        const float totalTime = 4f;

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

        yield return new WaitForSeconds(totalTime);

        timer = 0;

        while ((timer += Time.deltaTime) < 0.75f)
        {
            var c = _shootingText.color;

            c.a = Mathf.Lerp(1, 0, timer / 0.75f);

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

        if (ApplicationSettings.IsPaused() && _panel.activeInHierarchy) // paused
        {
            HandleSelectorMovement();
            HandleBuying();
        }
    }
   
    private void HandleBuying()
    {
        if (!WantsToBuyItem() || Slots[_currentSelectionSlot].IsEmpty()) return;
        
        var slot = Slots[_currentSelectionSlot];

        var upgradePrice = slot.Upgrade.Price;

        if (upgradePrice > _player.Currency) return;
        
        if (slot.Upgrade is ShootingUpgrade) _justBoughtShootingUpgrade = true; // TODO doesn't work when I will add more upgrades

        BuyItem(slot, upgradePrice);

        FillSlotWithNewItem(slot);
    }

    private void BuyItem(Slot slot, int upgradePrice)
    {
        AudioManager.Instance.Play("Buy", 5f);
        slot.Upgrade.Apply(_player);
        _player.Currency -= upgradePrice;
        slot.Upgrade = null;
        _descriptionText.text = "";
    }

    private static bool WantsToBuyItem()
    {
        return Input.GetButtonDown("Fire3");
    }

    private void HandleSelectorMovement()
    {
        var input = Input.GetAxisRaw("Horizontal");

        if (Math.Abs(input) < float.Epsilon || currentlyMovingFrame) return;
        
        StartCoroutine(MoveFrame(input < 0f));
        currentlyMovingFrame = true;
    }

    private IEnumerator MoveFrame(bool moveLeft)
    {
        float elapsedTime = 0;

        var startPos = Slots[_currentSelectionSlot].transform.position;

        _currentSelectionSlot = moveLeft
            ? (int) Mathf.Repeat(--_currentSelectionSlot, Slots.Count)
            : (int) Mathf.Repeat(++_currentSelectionSlot, Slots.Count);

        var endPos = Slots[_currentSelectionSlot].transform.position;

        while ((elapsedTime += Time.unscaledDeltaTime) < FrameMoveTime)
        {
            currentlyMovingFrame = true;
            
            _selectionFrame.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / FrameMoveTime);

            yield return null;
        }

        _selectionFrame.transform.position = Slots[_currentSelectionSlot].transform.position;

        var selectedUpgrade = Slots[_currentSelectionSlot];

        _descriptionText.text = selectedUpgrade.Upgrade ? Slots[_currentSelectionSlot].Upgrade.Description : "";

        currentlyMovingFrame = false;
    }

    private void FillSlotWithNewItem(Slot slot)
    {
        slot.Upgrade = _saleQueue.Count != 0 ? _saleQueue.Dequeue() : null;
    }
}