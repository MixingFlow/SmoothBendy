using System;
using System.Collections;
using TMG.Core;
using UnityEngine;

[Serializable]
public class FirstPersonHeadBob : TMGAbstractDisposable
{
	[SerializeField]
	private bool m_Active = true;

	[SerializeField]
	private float m_HorizontalBobRange = 0.15f;

	[SerializeField]
	private float m_VerticalBobRange = 0.15f;

	[SerializeField]
	private AnimationCurve m_Bobcurve;

	[SerializeField]
	private float m_VerticaltoHorizontalRatio = 2f;

	[Header("Jump Bob")]
	[SerializeField]
	private bool m_EnableJumpBob = true;

	[SerializeField]
	private float m_JumpBobDuration = 0.2f;

	[SerializeField]
	private float m_JumpBobAmount = 0.1f;

	private Transform m_CameraContainer;

	private Vector3 m_OriginalCameraPosition;

	private Vector3 m_CurrentCameraPosition;

	private float m_CyclePositionX;

	private float m_CyclePositionY;

	private float m_StepInterval;

	private float m_Time;

	private bool m_IsCrawling;

	public bool isActive => m_Active;

	public float JumpOffset { get; private set; }

	public void Init(Transform cameraContainer, float stepInterval)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (m_Active)
		{
			m_CameraContainer = cameraContainer;
			m_StepInterval = stepInterval;
			m_OriginalCameraPosition = m_CameraContainer.localPosition;
			m_IsCrawling = false;
			Keyframe val = m_Bobcurve[m_Bobcurve.length - 1];
			m_Time = ((Keyframe)(ref val)).time;
		}
	}

	public void UpdateCameraPosition(float magnitude, float speed)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Active || !GameManager.Instance.PlayerSettings.ViewBobbing)
		{
			m_CameraContainer.localPosition = m_OriginalCameraPosition;
			return;
		}
		Vector3 val;
		if (magnitude > 0.05f)
		{
			m_CameraContainer.localPosition = DoHeadBob(magnitude + speed);
			val = m_CameraContainer.localPosition;
			val.y = m_CameraContainer.localPosition.y - JumpOffset;
			m_CurrentCameraPosition = val;
		}
		else
		{
			val = Vector3.Lerp(m_CameraContainer.localPosition, m_OriginalCameraPosition, 0.2f);
			val.y -= JumpOffset;
		}
		m_CameraContainer.localPosition = val;
	}

	private Vector3 DoHeadBob(float speed)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Active)
		{
			return Vector3.zero;
		}
		float num = m_OriginalCameraPosition.x + m_Bobcurve.Evaluate(m_CyclePositionX) * m_HorizontalBobRange;
		float num2 = m_OriginalCameraPosition.y + m_Bobcurve.Evaluate(m_CyclePositionY) * m_VerticalBobRange;
		if (m_IsCrawling)
		{
			num = (m_OriginalCameraPosition.z + m_Bobcurve.Evaluate(m_CyclePositionX) * m_HorizontalBobRange) * 3f;
			num2 = m_OriginalCameraPosition.y + m_Bobcurve.Evaluate(m_CyclePositionY) * m_VerticalBobRange / 2f;
		}
		m_CyclePositionX += speed * Time.deltaTime / m_StepInterval;
		m_CyclePositionY += speed * Time.deltaTime / m_StepInterval * m_VerticaltoHorizontalRatio;
		if (m_CyclePositionX > m_Time)
		{
			m_CyclePositionX -= m_Time;
		}
		if (m_CyclePositionY > m_Time)
		{
			m_CyclePositionY -= m_Time;
		}
		return (!m_IsCrawling) ? new Vector3(num, num2, 0f) : new Vector3(num / 2f, num2, num);
	}

	private Vector3 DoHeadReset()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Active)
		{
			return Vector3.zero;
		}
		float num = Time.deltaTime * 1f;
		Vector3 originalCameraPosition = m_OriginalCameraPosition;
		originalCameraPosition.x -= JumpOffset;
		originalCameraPosition.y -= JumpOffset;
		float num2 = num / Vector3.Distance(m_CurrentCameraPosition, originalCameraPosition);
		return Vector3.Lerp(m_CurrentCameraPosition, originalCameraPosition, num2);
	}

	public IEnumerator DoJumpBob()
	{
		if (m_EnableJumpBob && GameManager.Instance.PlayerSettings.ViewBobbing)
		{
			float time = 0f;
			while (time < m_JumpBobDuration)
			{
				JumpOffset = Mathf.Lerp(0f, m_JumpBobAmount, time / m_JumpBobDuration);
				time += Time.deltaTime;
				yield return (object)new WaitForFixedUpdate();
			}
			time = 0f;
			while (time < m_JumpBobDuration)
			{
				JumpOffset = Mathf.Lerp(m_JumpBobAmount, 0f, time / m_JumpBobDuration);
				time += Time.deltaTime;
				yield return (object)new WaitForFixedUpdate();
			}
			JumpOffset = 0f;
		}
	}

	public void SetActive(bool active)
	{
		m_Active = active;
		m_EnableJumpBob = active;
	}

	public void CrawlSetActive(bool active)
	{
		m_IsCrawling = active;
	}

	protected override void OnDisposed()
	{
		m_CameraContainer = null;
		base.OnDisposed();
	}
}
