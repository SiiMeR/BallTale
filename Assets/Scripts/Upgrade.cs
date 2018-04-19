using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Upgrade : MonoBehaviour
{

	public int Price { get; set; }
	public string Name { get; set; }
	public Sprite Sprite { get; set; }

	public UnityEvent OnAquire;
	
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
