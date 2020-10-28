using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVAnimation : MonoBehaviour
{
	[SerializeField]
	private Material tvOnMaterial;
	

	private void OnEnable()
	{
		ResetMaterialOffset();
	}

	private void Update()
	{
		ChangeMaterialOffset();
	}

	private void ChangeMaterialOffset()
	{
		float sin = Mathf.Sin(Time.time) + 1;
		tvOnMaterial.SetTextureOffset("_BaseMap", new Vector2(0, sin));
	}

	private void ResetMaterialOffset()
	{
		tvOnMaterial.SetTextureOffset("_BaseMap", new Vector2(0, 0));
	}
}
