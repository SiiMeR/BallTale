public class HealthUpgrade : Upgrade
{
    public override string Description => $"Increases current and max health by {HealthBonus}";

    public int HealthBonus { get; set; }
    
    public override void Apply(Player player)
    {
        player.CurrentHealth += HealthBonus;
        player.MaxHealth += HealthBonus;
        player.Upgrades.Add(this);
    }
}