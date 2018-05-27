using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Upgrade : MonoBehaviour
{

	public int Price { get; set; }
	public string Name { get; set; }

	public virtual string Description
	{
		get { return _description; }
		set { _description = value; }
	}

	public Sprite Sprite { get; set; }
	
	public UnityEvent OnAquire;
	protected string _description = $"IMPLEMENT DESCRIPTION FOR THIS OBJECT";

	private void Start()
	{
		if (OnAquire == null)
		{
			OnAquire = new UnityEvent();
		}
	}

	private void Update()
	{
		
	}
}
