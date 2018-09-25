using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    public int Price { get; set; }
    public virtual string Name { get; set; } = "UPGRADE HAS NO NAME SET";
    public virtual string Description { get; set; } = "IMPLEMENT DESCRIPTION FOR THIS OBJECT";
    public Sprite Sprite { get; set; }

    public abstract void Apply(Player player);

    public override string ToString()
    {
        return
            $"{base.ToString()}, {nameof(Price)}: {Price}, {nameof(Name)}: {Name}, {nameof(Description)}: {Description}, {nameof(Sprite)}: {Sprite}";
    }
}