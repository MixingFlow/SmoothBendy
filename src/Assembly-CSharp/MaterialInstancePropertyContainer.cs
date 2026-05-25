using System;
using UnityEngine;

[Serializable]
public class MaterialInstancePropertyContainer
{
	[SerializeField]
	public string Name;

	[SerializeField]
	public MaterialInstanceController.PropertyContainerType PropertyType;

	[SerializeField]
	public Texture2D NewTexture;

	[SerializeField]
	public float NewFloat;

	[SerializeField]
	public Color NewColor;

	public MaterialInstancePropertyContainer(MaterialInstanceController.PropertyContainerType _propertyType, string _name)
	{
		PropertyType = _propertyType;
		Name = _name;
	}
}
