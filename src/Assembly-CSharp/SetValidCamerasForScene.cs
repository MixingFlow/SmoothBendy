using System.Collections.Generic;
using UnityEngine;

public class SetValidCamerasForScene : MonoBehaviour
{
	[SerializeField]
	private bool m_ExecuteOnStart = true;

	[SerializeField]
	private bool m_ClearCameraList = true;

	[SerializeField]
	private List<Camera> m_CamerasToAdd = new List<Camera>();

	[SerializeField]
	private List<Camera> m_CamerasToRemove = new List<Camera>();

	private void Start()
	{
		if (m_ExecuteOnStart)
		{
			Execute();
		}
	}

	private void OnValidate()
	{
		Execute();
	}

	public void Execute()
	{
		if (m_ClearCameraList)
		{
			DynamicDecals.validCameras.Clear();
		}
		foreach (Camera item in m_CamerasToAdd)
		{
			DynamicDecals.validCameras.Add(item);
		}
		foreach (Camera item2 in m_CamerasToRemove)
		{
			do
			{
				DynamicDecals.validCameras.Remove(item2);
			}
			while (DynamicDecals.validCameras.Contains(item2));
		}
	}
}
