using System;
using UnityEngine.Events;

public class HealthUpgrade : Upgrade
{
    public override string Description => $"Increases current and max health by {HealthBonus}";

    public int HealthBonus { get; set; }
    
    public override void AddUpgrade()
    {
        var player = FindObjectOfType<Player>();
        player.CurrentHealth += HealthBonus;
        player.MaxHealth += HealthBonus;
    }
}