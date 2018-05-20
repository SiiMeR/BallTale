using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{

	public Shader _shader;

	private Material _mat;
	// Use this for initialization
	void Start () {
		_mat = new Material(_shader);

		_mat.hideFlags = HideFlags.HideAndDontSave;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, _mat);
	}
}
