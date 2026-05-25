using System;
using TMG.Core;
using UnityEngine;

public class Prize_Wheel : TMGMonoBehaviour
{
	[SerializeField]
	private Interactable m_Interaction;

	[SerializeField]
	private float m_PostAngle;

	[SerializeField]
	private float m_SpinSpeed;

	[SerializeField]
	private float m_SpeedLossPerPost;

	[SerializeField]
	private Transform m_WheelParent;

	private Vector3 m_PreviousAngle = Vector3.up;

	private float m_CurrentSpeed;

	private bool m_IsSpinning;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Interaction.OnInteracted += HandleOnInteraction;
	}

	private void HandleOnInteraction(object sender, EventArgs e)
	{
		m_Interaction.OnInteracted -= HandleOnInteraction;
		m_Interaction.SetActive(active: false);
		m_CurrentSpeed = m_SpinSpeed + Random.Range(-2f, 2f);
		m_IsSpinning = true;
	}

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsSpinning)
		{
			if (Vector3.Angle(m_PreviousAngle, m_WheelParent.up) > m_PostAngle)
			{
				m_PreviousAngle = m_WheelParent.up;
				m_CurrentSpeed -= m_SpeedLossPerPost;
			}
			if (m_CurrentSpeed > m_SpeedLossPerPost)
			{
				m_WheelParent.Rotate(0f, 0f, m_CurrentSpeed * Time.deltaTime);
				m_CurrentSpeed -= m_SpeedLossPerPost / 2f * Time.deltaTime;
				return;
			}
			m_IsSpinning = false;
			m_CurrentSpeed = 0f;
			m_Interaction.OnInteracted += HandleOnInteraction;
			m_Interaction.SetActive(active: true);
		}
	}
}
