using System;
using TMG.Core;
using UnityEngine;

public class CH3BendyClock : TMGMonoBehaviour
{
	private const float m_HoursToDegrees = 30f;

	private const float m_MinutesToDegrees = 6f;

	[SerializeField]
	private Transform m_HourHand;

	[SerializeField]
	private Transform m_MinuteHand;

	private void Update()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		DateTime now = DateTime.Now;
		m_HourHand.localRotation = Quaternion.Euler((float)now.Hour * 30f, 0f, 0f);
		m_MinuteHand.localRotation = Quaternion.Euler((float)now.Minute * 6f, 0f, 0f);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
