using System;
using TMG.Core;
using UnityEngine;
using UnityEngine.Video;

public class CH5TheEndCredits : TMGMonoBehaviour
{
	[SerializeField]
	private VideoPlayer m_VideoPlayer;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		base.InitOnComplete();
		GameManager.Instance.LockPause();
		m_VideoPlayer.loopPointReached += new EventHandler(HandleVideoPlayerOnComplete);
	}

	private void HandleVideoPlayerOnComplete(VideoPlayer source)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		m_VideoPlayer.loopPointReached -= new EventHandler(HandleVideoPlayerOnComplete);
		GameManager.Instance.ShowScreenBlocker();
		this.OnComplete.Send(this);
		Dispose();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
