using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseOptionsMenu : SimpleMenu<PauseOptionsMenu>
{

	public TextMeshProUGUI soundSliderVal;
	public Slider soundSlider;
	
	public TextMeshProUGUI musicSliderVal;
	public Slider musicSlider;
	
	protected override void Awake()
	{
		base.Awake();
		
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnSoundValueChanged()
	{
		var newVal = soundSlider.value * 10;
		soundSliderVal.text = newVal.ToString();
	}
	public void OnMusicValueChanged()
	{
		var newVal = musicSlider.value * 10;
		musicSliderVal.text = newVal.ToString();
	}
	
	

}
