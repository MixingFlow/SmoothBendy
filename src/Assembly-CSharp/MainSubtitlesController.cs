using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using TMG.UI;
using TMPro;
using UnityEngine;

public class MainSubtitlesController : BaseUIController
{
	[Header("TextMeshProUGUIs")]
	[SerializeField]
	private TextMeshProUGUI m_SubtitleText;

	private List<SubtitleDataVO> m_SubtitleQueue = new List<SubtitleDataVO>();

	private Sequence m_SubtitleSequence;

	private float m_SubtitleTimer;

	private float m_SubtitleDuration;

	public override void Init()
	{
		base.Init();
		m_SubtitleText.text = string.Empty;
		m_SubtitleText.alpha = 0f;
	}

	public override void InitController(object _data)
	{
		base.InitController(_data);
	}

	public void Update()
	{
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		if (m_SubtitleQueue.Count > 0)
		{
			m_SubtitleTimer += Time.deltaTime;
			if (m_SubtitleTimer >= m_SubtitleDuration)
			{
				m_SubtitleTimer = 0f;
				m_SubtitleDuration = 0f;
				SubtitleDataVO subtitleDataVO = m_SubtitleQueue[0];
				m_SubtitleDuration = subtitleDataVO.Duration;
				m_SubtitleQueue.RemoveAt(0);
				m_SubtitleText.text = subtitleDataVO.Subtitles;
				m_SubtitleText.alpha = 0f;
				float num = 0.5f;
				float num2 = subtitleDataVO.Duration - ((!subtitleDataVO.IsTrimmed) ? 0f : num);
				KillSubtitleSequence();
				m_SubtitleSequence = DOTween.Sequence();
				TweenSettingsExtensions.Insert(m_SubtitleSequence, 0f, (Tween)(object)m_SubtitleText.DOFade(1f, num));
				TweenSettingsExtensions.Insert(m_SubtitleSequence, num2, (Tween)(object)m_SubtitleText.DOFade(0f, num));
				TweenSettingsExtensions.OnComplete<Sequence>(m_SubtitleSequence, new TweenCallback(HandleSubtitleOnComplete));
			}
		}
	}

	private void HandleSubtitleOnComplete()
	{
		KillSubtitleSequence();
		m_SubtitleText.text = string.Empty;
		m_SubtitleDuration = 0f;
		if (m_SubtitleQueue.Count <= 0)
		{
			Kill();
		}
	}

	public void ShowSubtitles(string subtitles, float duration, bool isTrimmed = false)
	{
		string Translation = subtitles;
		if (LocalizationManager.TryGetTranslation(subtitles, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			subtitles = Translation;
		}
		m_SubtitleQueue.Add(SubtitleDataVO.Create(subtitles, duration, isTrimmed));
	}

	private void KillSubtitleSequence()
	{
		if (m_SubtitleSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_SubtitleSequence, false);
			m_SubtitleSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSubtitleSequence();
		if (m_SubtitleQueue != null)
		{
			for (int num = m_SubtitleQueue.Count - 1; num >= 0; num--)
			{
				m_SubtitleQueue[num].Dispose();
			}
			m_SubtitleQueue.Clear();
			m_SubtitleQueue = null;
		}
		base.OnDisposed();
	}
}
