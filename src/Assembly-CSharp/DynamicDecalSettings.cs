using UnityEngine;

public class DynamicDecalSettings : ScriptableObject
{
	public PoolInstance[] pools;

	public string[] layerNames;

	public bool highPrecision;

	public bool forceForward;

	public DynamicDecalSettings()
	{
		pools = new PoolInstance[1]
		{
			new PoolInstance("Default", null)
		};
		layerNames = new string[4] { "Layer 1", "Layer 2", "Layer 3", "Layer 4" };
		highPrecision = false;
		forceForward = false;
	}
}
