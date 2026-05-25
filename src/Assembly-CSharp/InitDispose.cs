using System.Collections;
using TMG.Core;
using UnityEngine;

public class InitDispose : TMGMonoBehaviour
{
	[SerializeField]
	private bool m_OnStart;

	public override void Init()
	{
		base.Init();
		if (!base.IsDisposed && !m_OnStart)
		{
			((MonoBehaviour)this).StartCoroutine(ActualDispose());
		}
	}

	public override void InitOnComplete()
	{
		if (!base.IsDisposed && m_OnStart)
		{
			((MonoBehaviour)this).StartCoroutine(ActualDispose());
		}
	}

	private IEnumerator ActualDispose()
	{
		yield return (object)new WaitForEndOfFrame();
		Dispose();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
