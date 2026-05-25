using System;
using TMG.Core;
using UnityEngine;

public class CH5ButcherGangSpotter : TMGMonoBehaviour
{
	[SerializeField]
	private ButcherGangAi[] m_ButcherGang;

	private AudioClip m_SpottedClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_SpottedClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_ThereYouAre");
		for (int i = 0; i < m_ButcherGang.Length; i++)
		{
			m_ButcherGang[i].OnSpotted += HandleButcherGangOnSpotted;
		}
	}

	private void HandleButcherGangOnSpotted(object sender, EventArgs e)
	{
		GameManager.Instance.AudioManager.Play(m_SpottedClip);
	}

	protected override void OnDisposed()
	{
		m_SpottedClip = null;
		base.OnDisposed();
	}
}
