public class HealthUpgrade : Upgrade
{
    public override string Description => $"Increases current and max health by {HealthBonus}";

    public int HealthBonus { get; set; }
    
    public override void Apply()
    {
        var player = FindObjectOfType<Player>();
        player.MaxHealth += HealthBonus;
        player.Upgrades.Add(this);
    }
}