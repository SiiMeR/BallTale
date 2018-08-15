﻿public class HealthUpgrade : Upgrade
{
    public override string Description
    {
        get { return $"Increases current and max health by {HealthBonus}"; }
        set { _description = value; }
    }

    public int HealthBonus { get; set; }

    // Use this for initialization
    private void Start()
    {
        OnAquire.AddListener(AddHealth);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void AddHealth()
    {
        var player = FindObjectOfType<Player>();
        player.CurrentHealth += HealthBonus;
        player.MaxHealth += HealthBonus;
        player.Currency -= Price;
    }
}