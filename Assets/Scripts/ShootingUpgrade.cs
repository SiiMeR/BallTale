public class ShootingUpgrade : Upgrade
{
    public override void Apply(Player player)
    {
        player.Upgrades.Add(this);
    }
}