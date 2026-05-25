using System;
using TMG.Core;
using UnityEngine;

[Serializable]
public class CameraMovements : TMGAbstractDisposable
{
	[SerializeField]
	private bool m_Active = true;

	[SerializeField]
	private float m_SwaySpeed = 0.6f;

	[SerializeField]
	private float m_BaseSwayAmount = 1.5f;

	[SerializeField]
	private float m_TrackingSwayAmount = 1.5f;

	[SerializeField]
	private float m_TrackingBias;

	[SerializeField]
	private float m_FollowSpeed = 1f;

	private Transform m_ActualCamera;

	private Transform m_Target;

	private Quaternion m_OriginalRotation;

	private Vector3 m_FollowVelocity;

	private Vector3 m_FollowAngles;

	private Vector2 m_RotationRange;

	public void Init(Transform parent, Transform cameraContainer)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (m_Active)
		{
			m_ActualCamera = cameraContainer;
			m_OriginalRotation = m_ActualCamera.localRotation;
			m_Target = new GameObject("Forward Camera Target").transform;
			m_Target.SetParent(parent);
			m_Target.localPosition = Vector3.forward;
			m_Target.localEulerAngles = Vector3.zero;
		}
	}

	public void Sway(Transform cameraContainer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		m_ActualCamera.localRotation = m_OriginalRotation;
		if (m_Active && GameManager.Instance.PlayerSettings.ViewSwaying)
		{
			Vector3 val = cameraContainer.InverseTransformPoint(m_Target.position);
			float num = Mathf.Atan2(val.x, val.z) * 57.29578f;
			num = Mathf.Clamp(num, -10.5f, 10.5f);
			m_ActualCamera.localRotation = m_OriginalRotation * Quaternion.Euler(0f, num, 0f);
			val = cameraContainer.InverseTransformPoint(m_Target.position);
			float num2 = Mathf.Atan2(val.y, val.z) * 57.29578f;
			num2 = Mathf.Clamp(num2, -10.5f, 10.5f);
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(m_FollowAngles.x + Mathf.DeltaAngle(m_FollowAngles.x, num2), m_FollowAngles.y + Mathf.DeltaAngle(m_FollowAngles.y, num));
			m_FollowAngles = Vector3.SmoothDamp(m_FollowAngles, val2, ref m_FollowVelocity, m_FollowSpeed);
			m_ActualCamera.localRotation = m_OriginalRotation * Quaternion.Euler(0f - m_FollowAngles.x, m_FollowAngles.y, 0f);
			float num3 = Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed) - 0.5f;
			float num4 = Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed + 100f) - 0.5f;
			num3 *= m_BaseSwayAmount;
			num4 *= m_BaseSwayAmount;
			float num5 = Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed) - 0.5f + m_TrackingBias;
			float num6 = Mathf.PerlinNoise(0f, Time.time * m_SwaySpeed + 100f) - 0.5f + m_TrackingBias;
			num5 *= (0f - m_TrackingSwayAmount) * m_FollowVelocity.x;
			num6 *= m_TrackingSwayAmount * m_FollowVelocity.y;
			float num7 = num3 + num5;
			float num8 = num4 + num6;
			m_ActualCamera.Rotate(num7, num8, (0f - num8) * 0.5f);
		}
	}

	protected override void OnDisposed()
	{
		m_ActualCamera = null;
		m_Target = null;
		base.OnDisposed();
	}
}
