public class SkillUpgrade : Upgrade
{
    
    public override string Description
    {
        get => _description;
        set => _description = value;
    }
    
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