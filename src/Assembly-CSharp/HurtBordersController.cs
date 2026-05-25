using System;
using System.Collections.Generic;
using DG.Tweening;
using TMG.UI;
using UnityEngine;
using UnityEngine.UI;

public class HurtBordersController : BaseUIController
{
	[Header("Images")]
	[SerializeField]
	private Image m_BlackImage;

	[SerializeField]
	private List<Image> m_Borders;

	[Header("Audio")]
	[SerializeField]
	private AudioClip[] m_HurtClips;

	[SerializeField]
	private AudioClip[] m_DeathClips;

	private int m_HitCount;

	private int m_HitMax;

	private float m_Timer;

	private float m_RelaxTime = 8f;

	private bool m_IsHit;

	private bool m_IsHitMax;

	public event EventHandler OnMaxHit;

	public override void InitController(object _data)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		((Graphic)m_BlackImage).color = new Color(0f, 0f, 0f, 0f);
		m_HitMax = m_Borders.Count;
		for (int i = 0; i < m_HitMax; i++)
		{
			((Behaviour)m_Borders[i]).enabled = false;
		}
		ShuffleAudio(ref m_HurtClips);
		ShuffleAudio(ref m_DeathClips);
	}

	private void Update()
	{
		if (m_IsHit)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer >= m_RelaxTime)
			{
				HideBorder();
			}
		}
	}

	public void ShowBorder(bool isSilent = false)
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		if (m_HitCount >= m_HitMax)
		{
			if (!m_IsHitMax)
			{
				GameManager.Instance.isDead = true;
				m_HitCount--;
				m_IsHitMax = true;
				ShortcutExtensions.DOKill((Component)(object)m_BlackImage, false);
				TweenSettingsExtensions.SetEase<Tweener>(m_BlackImage.DOFade(1f, 0.2f), (Ease)1);
				GameManager.Instance.ShowScreenBlocker(0f);
				GameManager.Instance.HideCrosshair();
				PlayAudio(ref m_DeathClips);
				this.OnMaxHit.Send(this);
			}
			return;
		}
		if (!isSilent)
		{
			PlayAudio(ref m_HurtClips);
		}
		m_IsHit = true;
		m_Timer = 0f;
		Transform camera = GameManager.Instance.GameCamera.transform;
		camera.localPosition = Vector3.zero;
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(camera, 0.75f, 0.35f, 15, 90f, false, true), (TweenCallback)delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			camera.localPosition = Vector3.zero;
		});
		for (int num = 0; num < m_HitMax && num <= m_HitCount; num++)
		{
			Image val = m_Borders[num];
			ShortcutExtensions.DOKill((Component)(object)val, false);
			ShortcutExtensions.DOKill((Component)(object)((Graphic)val).rectTransform, false);
			((Behaviour)val).enabled = true;
			((Graphic)val).color = Color.white;
			((Transform)((Graphic)val).rectTransform).localScale = Vector3.one;
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)((Graphic)val).rectTransform, 1.005f, 0.25f + (float)num * 0.005f), (Ease)7), -1, (LoopType)1);
		}
		m_HitCount++;
	}

	private void HideBorder()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		m_Timer = 0f;
		m_HitCount = 0;
		m_IsHit = false;
		for (int i = 0; i < m_HitMax; i++)
		{
			Image hitBorder = m_Borders[i];
			ShortcutExtensions.DOKill((Component)(object)hitBorder, false);
			ShortcutExtensions.DOKill((Component)(object)((Graphic)hitBorder).rectTransform, false);
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(hitBorder.DOFade(0f, 1.5f), (Ease)1), (TweenCallback)delegate
			{
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				ShortcutExtensions.DOKill((Component)(object)hitBorder, false);
				ShortcutExtensions.DOKill((Component)(object)((Graphic)hitBorder).rectTransform, false);
				((Transform)((Graphic)hitBorder).rectTransform).localScale = Vector3.one;
				((Behaviour)hitBorder).enabled = false;
				((Graphic)hitBorder).color = new Color(1f, 1f, 1f, 0f);
			});
		}
	}

	private AudioObject PlayAudio(ref AudioClip[] audioClips)
	{
		if (audioClips == null || audioClips.Length <= 0)
		{
			return null;
		}
		int num = Random.Range(0, audioClips.Length);
		AudioClip val = audioClips[num];
		AudioObject result = GameManager.Instance.AudioManager.Play(val, AudioObjectType.DIALOGUE);
		audioClips[num] = audioClips[0];
		audioClips[0] = val;
		return result;
	}

	private void ShuffleAudio(ref AudioClip[] audioClips)
	{
		int num = Random.Range(0, audioClips.Length);
		AudioClip val = audioClips[num];
		audioClips[num] = audioClips[0];
		audioClips[0] = val;
	}

	protected override void OnDisposed()
	{
		this.OnMaxHit = null;
		m_HitCount = 0;
		m_DeathClips = null;
		m_HurtClips = null;
		for (int i = 0; i < m_HitMax; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_Borders[i], false);
			ShortcutExtensions.DOKill((Component)(object)((Graphic)m_Borders[i]).rectTransform, false);
		}
		base.OnDisposed();
	}
}
