using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class InspectorButton : PropertyAttribute
{
	public string buttonName;

	public InspectorButton(string buttonName = null)
	{
		this.buttonName = buttonName;
	}
}
