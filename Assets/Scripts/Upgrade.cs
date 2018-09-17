using UnityEngine;
using UnityEngine.Events;

public abstract class Upgrade : MonoBehaviour
{
    public string _description = $"IMPLEMENT DESCRIPTION FOR THIS OBJECT";

    public UnityEvent OnAquire;
    [SerializeField] private Sprite _sprite;


    public int Price { get; set; }
    public string Name { get; set; }

    public virtual string Description
    {
        get => _description;
        set => _description = value;
    }

    public Sprite Sprite
    {
        get => _sprite;
        set => _sprite = value;
    }

    private void Start()
    {
        if (OnAquire == null) OnAquire = new UnityEvent();
    }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(Price)}: {Price}, {nameof(Name)}: {Name}, {nameof(Description)}: {Description}, {nameof(Sprite)}: {Sprite}";
    }
}