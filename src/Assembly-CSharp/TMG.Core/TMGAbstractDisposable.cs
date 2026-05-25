using System;

namespace TMG.Core;

public abstract class TMGAbstractDisposable : IDisposable
{
	public bool IsDisposed { get; private set; }

	public void Dispose()
	{
		if (!IsDisposed)
		{
			OnDisposed();
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	protected virtual void OnDisposed()
	{
	}
}
