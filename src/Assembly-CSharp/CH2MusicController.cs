using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CH2MusicController : BaseController
{
	[Header("Music")]
	[SerializeField]
	private Transform m_MusicPositionLeft;

	[SerializeField]
	private Transform m_MusicPositionRight;

	[SerializeField]
	private Transform m_MusicPositionForward;

	[SerializeField]
	private Transform m_MusicPositionBack;

	[SerializeField]
	private List<Transform> m_Speakers;

	private AudioObject m_MusicAudioObject;

	private AudioClip m_MusicClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_MusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_Lobby_Jazz_01");
	}

	private void Update()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsActive || !((Object)(object)m_MusicAudioObject == (Object)null))
		{
			Transform val = GameManager.Instance.Player.transform;
			if (val.position.x < m_MusicPositionRight.position.x && val.position.x > m_MusicPositionLeft.position.x)
			{
				Vector3 worldPosition = m_MusicAudioObject.WorldPosition;
				worldPosition.x = val.position.x;
				m_MusicAudioObject.WorldPosition = worldPosition;
			}
			if (val.position.z < m_MusicPositionForward.position.z && val.position.z > m_MusicPositionBack.position.z)
			{
				Vector3 worldPosition2 = m_MusicAudioObject.WorldPosition;
				worldPosition2.z = val.position.z;
				m_MusicAudioObject.WorldPosition = worldPosition2;
			}
		}
	}

	public override void Activate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_MusicAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_MusicClip, m_MusicPositionForward.position);
		m_MusicAudioObject.OnComplete += HandleMusicOnComplete;
		for (int i = 0; i < m_Speakers.Count; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_Speakers[i], false);
			TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_Speakers[i], 1.025f, 0.225f), (Ease)7), -1, (LoopType)1), 2f);
		}
		m_IsActive = true;
	}

	private void HandleMusicOnComplete(object sender, EventArgs e)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		m_MusicAudioObject.OnComplete -= HandleMusicOnComplete;
		ClearLobbyMusic();
		for (int i = 0; i < m_Speakers.Count; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_Speakers[i], false);
			m_Speakers[i].localScale = Vector3.one;
		}
		Dispose();
	}

	private void ClearLobbyMusic()
	{
		if ((Object)(object)m_MusicAudioObject != (Object)null)
		{
			m_MusicAudioObject.Clear();
			m_MusicAudioObject = null;
		}
	}

	protected override void OnDisposed()
	{
		ClearLobbyMusic();
		m_MusicAudioObject = null;
		m_MusicClip = null;
		for (int i = 0; i < m_Speakers.Count; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_Speakers[i], false);
		}
		base.OnDisposed();
	}
}
