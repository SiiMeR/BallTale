using UnityEngine.Events;

public class ShootingUpgrade : Upgrade
{
    public override void AddUpgrade()
    {
        var player = FindObjectOfType<Player>();
        player.HasShotUpgrade = true;
    }
}