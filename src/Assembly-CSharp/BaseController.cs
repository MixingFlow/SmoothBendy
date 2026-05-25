using System;
using TMG.Core;

public abstract class BaseController : TMGMonoBehaviour
{
	protected bool m_IsActive;

	public bool IsComplete { get; private set; }

	public event EventHandler OnComplete;

	public virtual void Activate()
	{
	}

	public override void Init()
	{
		base.Init();
		m_IsComponent = true;
	}

	protected void SendOnComplete()
	{
		IsComplete = true;
		this.OnComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		base.OnDisposed();
	}
}
