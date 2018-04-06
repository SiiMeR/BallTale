using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {
	
	[SerializeField] private int _damage;

	public int Damage
	{
		get { return _damage; }
		set { _damage = value; }
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
