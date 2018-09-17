using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Shop : Interactable
{
    private int _currentSelectionSlot;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private List<Upgrade> _itemsOnSale;

    private bool _justBoughtShootingUpgrade;

    private bool _moveAxisInUse;
    public Queue<Upgrade> _saleQueue; // holds items that are not yet on sale
    [SerializeField] private GameObject _selectionFrame;


    [SerializeField] private TextMeshProUGUI _shootingText;

    [FormerlySerializedAs("_slots")] public List<Slot> Slots;
    
    public float FrameMoveTime = 1.0f;

    private UI _ui;
    
    private void OnDisable()
    {
        if (_ui) _ui.KeepOpen = false;
    }

    // Use this for initialization
    protected void Awake()
    {
        
        _ui = FindObjectOfType<UI>();

        _saleQueue = new Queue<Upgrade>();
        Slots = GetComponentsInChildren<Slot>(true).ToList();

        if (PlayerPrefs.GetInt("loadgame") == 0)
        {
            InitShopInventory();
        }
    }
    
    /// <summary>
    /// A method to fill shop inventory with items when starting a new game.
    /// Should not be called when loading game from an existing savegame.
    /// </summary>
    private void InitShopInventory()
    {
        var healthUpgrade = UpgradeBuilder.Instance.GetHealthUpgrade(10, 10);
        var healthUpgrade2 = UpgradeBuilder.Instance.GetHealthUpgrade(20, 15);
        var healthUpgrade3 = UpgradeBuilder.Instance.GetHealthUpgrade(50, 500);
        var healthUpgrade4 = UpgradeBuilder.Instance.GetHealthUpgrade(100, 1000);

        var powerUpgrade = UpgradeBuilder.Instance.GetShotUpgrade(50, "Gives the ability to shoot fireballs");

        var initialUpgrades = new List<Upgrade>(){healthUpgrade,healthUpgrade2,healthUpgrade3,healthUpgrade4,powerUpgrade};
        
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

    public override void FlipDialogue()
    {
        base.FlipDialogue();

        _ui.KeepOpen = !_ui.KeepOpen;

        if (_justBoughtShootingUpgrade)
        {
            StartCoroutine(ShowShootingHelpText());
            _justBoughtShootingUpgrade = false;
        }
    }

    private IEnumerator ShowShootingHelpText()
    {
        var timer = 0f;    
        var totalTime = 4f;

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

        if (Math.Abs(Time.timeScale) < 0.01f)
        {
            var input = Input.GetAxisRaw("Horizontal");

            if (input > 0.1 && !_moveAxisInUse)
            {
                _moveAxisInUse = true;
                StartCoroutine(MoveFrame(false));
            }
            else if (input < -0.1 && !_moveAxisInUse)
            {
                _moveAxisInUse = true;
                StartCoroutine(MoveFrame(true));
            }


            if (Input.GetButtonDown("Fire3") && !Slots[_currentSelectionSlot].IsEmpty())
            {
                var slot = Slots[_currentSelectionSlot];

                if (slot.Upgrade.Price <= FindObjectOfType<Player>().Currency)
                {
                    if (slot.Upgrade is SkillUpgrade) _justBoughtShootingUpgrade = true;

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

    private IEnumerator MoveFrame(bool moveLeft)
    {
        float elapsedTime = 0;

        var startPos = Slots[_currentSelectionSlot].transform.position;

        _currentSelectionSlot = moveLeft
            ? (int) Mathf.Repeat(++_currentSelectionSlot, Slots.Count)
            : (int) Mathf.Repeat(--_currentSelectionSlot, Slots.Count);

        var endPos = Slots[_currentSelectionSlot].transform.position;

        while ((elapsedTime += Time.unscaledDeltaTime) < FrameMoveTime)
        {
            _moveAxisInUse = true;
            
            _selectionFrame.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / FrameMoveTime);

            yield return null;
        }

        _selectionFrame.transform.position = Slots[_currentSelectionSlot].transform.position;

        var selectedUpgrade = Slots[_currentSelectionSlot];

        _descriptionText.text = selectedUpgrade.Upgrade ? Slots[_currentSelectionSlot].Upgrade.Description : "";

        _moveAxisInUse = false;
    }

    private void FillSlotWithNewItem(Slot slot)
    {
        
        if (_saleQueue.Count != 0)
        {
            var upgrade = _saleQueue.Dequeue();
            
            _descriptionText.text = upgrade.Description;
            slot.Upgrade = upgrade;
        }
        else
        {
            slot.Upgrade = null;
            _descriptionText.text = 
                Slots.Find(sl => sl.Upgrade != null).Upgrade.Description; // find first slotted upgrade and use that desc. instead
        }
    }
}