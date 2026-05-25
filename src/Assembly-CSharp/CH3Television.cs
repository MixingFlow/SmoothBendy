using TMG.Core;
using UnityEngine;
using UnityEngine.Video;

public class CH3Television : TMGMonoBehaviour
{
	[Header("TV Screen")]
	[SerializeField]
	private GameObject m_Screen;

	[SerializeField]
	private VideoPlayer m_VideoClip;

	[SerializeField]
	private Light m_Light;

	[SerializeField]
	private bool m_IsLooping = true;

	public override void Init()
	{
		base.Init();
		TurnOff();
		m_VideoClip.Prepare();
		m_VideoClip.frame = 1L;
	}

	public void Play()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		m_VideoClip.started += new EventHandler(HandleVideoClipStarted);
		m_VideoClip.loopPointReached += new EventHandler(HandleVideoClipLoopPointReached);
		m_VideoClip.Play();
	}

	public void Stop()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		m_VideoClip.started -= new EventHandler(HandleVideoClipStarted);
		m_VideoClip.loopPointReached -= new EventHandler(HandleVideoClipLoopPointReached);
		TurnOff();
		m_VideoClip.Pause();
		m_VideoClip.frame = 1L;
	}

	private void TurnOn()
	{
		((Behaviour)m_Light).enabled = true;
		m_Screen.SetActive(true);
	}

	private void TurnOff()
	{
		((Behaviour)m_Light).enabled = false;
		m_Screen.SetActive(false);
	}

	private void HandleVideoClipStarted(VideoPlayer source)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		m_VideoClip.started -= new EventHandler(HandleVideoClipStarted);
		TurnOn();
	}

	private void HandleVideoClipLoopPointReached(VideoPlayer source)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		m_VideoClip.loopPointReached -= new EventHandler(HandleVideoClipLoopPointReached);
		if (m_IsLooping)
		{
			m_VideoClip.loopPointReached += new EventHandler(HandleVideoClipLoopPointReached);
			m_VideoClip.frame = 1L;
			m_VideoClip.Play();
		}
		else
		{
			TurnOff();
		}
	}

	public override void OnDisable()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		m_VideoClip.started -= new EventHandler(HandleVideoClipStarted);
		m_VideoClip.loopPointReached -= new EventHandler(HandleVideoClipLoopPointReached);
		base.OnDisable();
	}
}
