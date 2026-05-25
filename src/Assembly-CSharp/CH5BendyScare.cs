using System;
using Ai;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5BendyScare : TMGMonoBehaviour
{
	[Header("Bendy")]
	[SerializeField]
	private BendySpawnerList m_BendySpawners;

	[SerializeField]
	private BendyAi m_BendyPrefab;

	private AudioClip m_QuietClip;

	private AudioClip m_BendyMusic;

	private AudioObject m_BendyMusicObject;

	public BendyAi Bendy { get; private set; }

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_QuietClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/AliceA/DIA_CH5_ALICEA_QUIET");
		m_BendyMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_WalkingWithTheDemon");
	}

	public void SpawnBendy()
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_QuietClip, "DIACH5/DIA_CH5_ALLISON_QUIET"));
		m_BendyMusicObject = GameManager.Instance.AudioManager.Play(m_BendyMusic, AudioObjectType.MUSIC);
		if (Object.op_Implicit((Object)(object)m_BendyMusicObject))
		{
			m_BendyMusicObject.AudioSource.volume = 0f;
			m_BendyMusicObject.AudioSource.DOFade(1f, 1f);
		}
		Bendy = Object.Instantiate<BendyAi>(m_BendyPrefab);
		BendySpawner bendySpawner = m_BendySpawners.BendySpawners[0];
		BendySpawner bendySpawner2 = m_BendySpawners.BendySpawners[1];
		m_BendySpawners.Use();
		Bendy.transform.position = bendySpawner.Watpoints[0].transform.position;
		Bendy.transform.eulerAngles = bendySpawner.Watpoints[0].transform.eulerAngles;
		Bendy.OnWaypointComplete += HandleBendyOnWaypointComplete;
		Bendy.UpdateWaypointList(bendySpawner2.Watpoints);
		Bendy.SetPassive(IsPassive: true);
	}

	private void HandleBendyOnWaypointComplete(object sender, EventArgs e)
	{
		Bendy.OnWaypointComplete -= HandleBendyOnWaypointComplete;
		m_BendySpawners.Reset();
		if (Object.op_Implicit((Object)(object)Bendy))
		{
			ClearMusic();
			Bendy.Dispose();
			Bendy = null;
		}
	}

	private void ClearMusic()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		if ((Object)(object)m_BendyMusicObject != (Object)null)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_BendyMusicObject.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
			{
				m_BendyMusicObject.Clear();
				m_BendyMusicObject = null;
			});
		}
	}

	protected override void OnDisposed()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		m_QuietClip = null;
		m_BendyMusic = null;
		m_BendySpawners.Reset();
		if (Object.op_Implicit((Object)(object)Bendy))
		{
			Bendy.Dispose();
			Bendy = null;
		}
		if (!((Object)(object)m_BendyMusicObject != (Object)null))
		{
			return;
		}
		TweenSettingsExtensions.OnComplete<Tweener>(m_BendyMusicObject.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
		{
			if ((Object)(object)m_BendyMusicObject != (Object)null)
			{
				m_BendyMusicObject.Clear();
				m_BendyMusicObject = null;
			}
			base.OnDisposed();
		});
	}
}
