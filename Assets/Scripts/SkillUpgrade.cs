public class SkillUpgrade : Upgrade
{
    // Use this for initialization
    private void Start()
    {
        OnAquire.AddListener(AddShootingAbility);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void AddShootingAbility()
    {
        var player = FindObjectOfType<Player>().GetComponent<Player>();
        player.HasShotUpgrade = true;
        player.Currency -= Price;
    }
}