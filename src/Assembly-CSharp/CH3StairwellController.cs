using System.Collections.Generic;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH3StairwellController : TMGMonoBehaviour
{
	[SerializeField]
	private List<BaseDoorController> m_DoorControllers;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_DoorControllers.Count; i++)
		{
			m_DoorControllers[i].Lock();
		}
	}

	public void CloseAllFloors()
	{
		for (int i = 0; i < m_DoorControllers.Count; i++)
		{
			m_DoorControllers[i].Unlock();
			m_DoorControllers[i].Close();
			m_DoorControllers[i].Lock();
		}
	}

	public void OpenAllFloors()
	{
		for (int i = 0; i < m_DoorControllers.Count; i++)
		{
			m_DoorControllers[i].Unlock();
			m_DoorControllers[i].Open(0f, (Ease)1, 145f);
		}
	}

	public void UnlockAllFloors()
	{
		for (int i = 0; i < m_DoorControllers.Count; i++)
		{
			m_DoorControllers[i].Unlock();
		}
	}

	public void LockAllFloors()
	{
		for (int i = 0; i < m_DoorControllers.Count; i++)
		{
			m_DoorControllers[i].Close();
			m_DoorControllers[i].Lock();
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
