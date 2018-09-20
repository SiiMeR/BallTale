public class ShootingUpgrade : Upgrade
{
    public override void Apply()
    {
        var player = FindObjectOfType<Player>();
        player.Upgrades.Add(this);
    }
}