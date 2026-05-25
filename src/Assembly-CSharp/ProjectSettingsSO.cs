using System;
using UnityEngine;

[Serializable]
public class ProjectSettingsSO : ScriptableObject
{
	[SerializeField]
	public bool ShowTriggerEvents;

	[SerializeField]
	public Sprite NormalDecalSprite;

	[SerializeField]
	public Material NormalDecalMaterial;

	[SerializeField]
	public float NormalDecalScale;

	[SerializeField]
	public Texture2D DynamicDecalTexture;

	[SerializeField]
	public Texture2D DynamicDecalNormal;

	[SerializeField]
	public float DynamicDecalScale;

	[SerializeField]
	public bool DynamicDecalViewAngleToggle;

	[SerializeField]
	public Material SimpleQuadMaterial;

	[SerializeField]
	public float SimpleQuadScale;

	[SerializeField]
	public Texture2D SecretDecalTexture;

	[SerializeField]
	public float SecretScale;

	[SerializeField]
	public bool SecretDecalViewAngleToggle;
}
