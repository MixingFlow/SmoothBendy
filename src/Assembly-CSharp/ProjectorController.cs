using System.Collections.Generic;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class ProjectorController : TMGMonoBehaviour
{
	[Header("Transforms")]
	[SerializeField]
	private Transform m_AudioPosition;

	[SerializeField]
	private Transform m_MusicPosition;

	[Header("Projector Wheels")]
	[SerializeField]
	private List<Transform> m_ProjectorWheels;

	[Header("Enable GameObjects")]
	[SerializeField]
	private List<GameObject> m_EnableGameObjects;

	[HideInInspector]
	public AudioObject MusicAudioObject;

	[HideInInspector]
	public AudioObject FilmAudioObject;

	private AudioClip m_ProjectorInteract;

	public Transform AudioPosition => m_AudioPosition;

	public Transform MusicPosition => m_MusicPosition;

	public override void Init()
	{
		base.Init();
		SetEnableGameObjectsActive(active: false);
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ProjectorInteract = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Projector_Switch_Turn_On_01");
	}

	public void InitTurnOn()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		int count = m_ProjectorWheels.Count;
		for (int i = 0; i < count; i++)
		{
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_ProjectorWheels[i], new Vector3(0f, 0f, 360f), 1.8f, (RotateMode)3), (Ease)1), -1, (LoopType)0);
		}
		SetEnableGameObjectsActive(active: true);
	}

	public void InitTurnOff()
	{
		int count = m_ProjectorWheels.Count;
		for (int i = 0; i < count; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_ProjectorWheels[i], false);
		}
		SetEnableGameObjectsActive(active: false);
	}

	public void TurnOn(bool isSilent = false)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		int count = m_ProjectorWheels.Count;
		for (int i = 0; i < count; i++)
		{
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_ProjectorWheels[i], new Vector3(0f, 0f, 360f), 1.8f, (RotateMode)3), (Ease)1), -1, (LoopType)0);
		}
		if (!isSilent)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(m_ProjectorInteract, m_AudioPosition.position);
		}
		SetEnableGameObjectsActive(active: true);
	}

	public void TurnOffSilent()
	{
		int count = m_ProjectorWheels.Count;
		for (int i = 0; i < count; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_ProjectorWheels[i], false);
		}
		if ((Object)(object)MusicAudioObject != (Object)null)
		{
			MusicAudioObject.Clear();
			MusicAudioObject = null;
		}
		if ((Object)(object)FilmAudioObject != (Object)null)
		{
			FilmAudioObject.Clear();
			FilmAudioObject = null;
		}
		SetEnableGameObjectsActive(active: false);
	}

	public void TurnOff()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		int count = m_ProjectorWheels.Count;
		for (int i = 0; i < count; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_ProjectorWheels[i], false);
		}
		GameManager.Instance.AudioManager.PlayAtPosition(m_ProjectorInteract, m_AudioPosition.position);
		if ((Object)(object)MusicAudioObject != (Object)null)
		{
			MusicAudioObject.Clear();
			MusicAudioObject = null;
		}
		if ((Object)(object)FilmAudioObject != (Object)null)
		{
			FilmAudioObject.Clear();
			FilmAudioObject = null;
		}
		SetEnableGameObjectsActive(active: false);
	}

	private void SetEnableGameObjectsActive(bool active)
	{
		int count = m_EnableGameObjects.Count;
		for (int i = 0; i < count; i++)
		{
			m_EnableGameObjects[i].SetActive(active);
		}
	}

	protected override void OnDisposed()
	{
		int count = m_ProjectorWheels.Count;
		for (int i = 0; i < count; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_ProjectorWheels[i], false);
		}
		if ((Object)(object)FilmAudioObject != (Object)null)
		{
			FilmAudioObject.Clear();
			FilmAudioObject = null;
		}
		base.OnDisposed();
	}
}
