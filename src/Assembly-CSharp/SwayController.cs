using TMG.Core;
using UnityEngine;

public class SwayController : TMGMonoBehaviour
{
	[SerializeField]
	private SwayProp SwayProp;

	public void EnableSway()
	{
		SwayProp.Sway();
	}

	public void EnableHeavySway(float intensity)
	{
		SwayProp.HeavySway(intensity);
	}

	public void ResetSway()
	{
		SwayProp.ResetSway();
	}

	public void DisableSway()
	{
		SwayProp.Stop();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
