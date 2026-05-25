using TMG.Core;
using UnityEngine;

public class CH5FinaleCamera : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_HeadContainer;

	[SerializeField]
	private Transform m_CameraContainer;

	[SerializeField]
	private CameraMovements m_CameraMovement;

	private bool m_IsInitialized;

	public void Initialize()
	{
		m_CameraMovement.Init(m_HeadContainer, m_CameraContainer);
		GameManager.Instance.Player.SetCameraSway(active: false);
		m_IsInitialized = true;
	}

	private void LateUpdate()
	{
		if (m_IsInitialized)
		{
			m_CameraMovement.Sway(m_HeadContainer);
		}
	}
}
