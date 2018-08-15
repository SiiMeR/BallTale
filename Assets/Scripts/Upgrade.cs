using UnityEngine;
using UnityEngine.Events;

public abstract class Upgrade : MonoBehaviour
{
    protected string _description = $"IMPLEMENT DESCRIPTION FOR THIS OBJECT";

    public UnityEvent OnAquire;

    public int Price { get; set; }
    public string Name { get; set; }

    public virtual string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    public Sprite Sprite { get; set; }

    private void Start()
    {
        if (OnAquire == null) OnAquire = new UnityEvent();
    }

    private void Update()
    {
    }
}