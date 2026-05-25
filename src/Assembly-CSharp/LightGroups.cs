using System.Collections.Generic;
using UnityEngine;

public class LightGroups : MonoBehaviour
{
	public List<LightGroup> lightGroups;

	private void OnValidate()
	{
		lightGroups.Clear();
		lightGroups.AddRange(((Component)this).GetComponentsInChildren<LightGroup>());
	}

	public void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		foreach (LightGroup lightGroup in lightGroups)
		{
			if (Object.op_Implicit((Object)(object)lightGroup))
			{
				lightGroup.OnDrawGizmosSelected();
			}
		}
	}

	public void TurnLightGroupOn(string groupName)
	{
		ToggleLightGroup(groupName);
	}

	public void TrunLightGroupOff(string groupName)
	{
		ToggleLightGroup(groupName, turnOn: false);
	}

	public void TurnLightGroupOn(GameObject groupParentObject)
	{
		ToggleLightGroup(groupParentObject);
	}

	public void TurnLightGroupOff(GameObject groupParentObject)
	{
		ToggleLightGroup(groupParentObject, turnOn: false);
	}

	private void ToggleLightGroup(string groupName, bool turnOn = true)
	{
		foreach (LightGroup lightGroup in lightGroups)
		{
			if (((Object)lightGroup).name.ToLower() == groupName.ToLower())
			{
				lightGroup.ToggleLightGroup(turnOn);
				break;
			}
		}
	}

	private void ToggleLightGroup(GameObject groupParent, bool turnOn = true)
	{
		foreach (LightGroup lightGroup in lightGroups)
		{
			if ((Object)(object)((Component)lightGroup).gameObject == (Object)(object)groupParent)
			{
				lightGroup.ToggleLightGroup(turnOn);
				break;
			}
		}
	}
}
