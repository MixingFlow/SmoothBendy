using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

[Serializable]
public class CameraFOV : TMGAbstractDisposable
{
	[SerializeField]
	private bool m_Active = true;

	[SerializeField]
	private float m_BaseFOV = 55f;

	[SerializeField]
	private float m_RunFOV = 70f;

	[SerializeField]
	private float m_TransitionSpeed = 0.2f;

	private Camera[] m_Cameras;

	private float FoVChanged;

	public float FOV => GameManager.Instance.PlayerSettings.FoV;

	public void Init(params Camera[] cameras)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		m_Cameras = cameras;
		SetFOV(GameManager.Instance.PlayerSettings.FoV);
		DOTween.useSmoothDeltaTime = true;
		DOTween.defaultUpdateType = (UpdateType)0;
	}

	public void UpdateVOD(float magnitude, float speed, bool isRunning)
	{
		SetFOV(GameManager.Instance.PlayerSettings.FoV);
	}

	public void SetFOV(float value)
	{
		for (int i = 0; i < m_Cameras.Length; i++)
		{
			m_Cameras[i].fieldOfView = GameManager.Instance.PlayerSettings.FoV;
		}
	}

	public void DOFov(float value, float duration)
	{
	}

	public void SetActiveFOV(bool active)
	{
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
