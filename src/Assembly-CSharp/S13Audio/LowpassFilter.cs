using UnityEngine;

namespace S13Audio;

public class LowpassFilter : AudioEffectBase
{
	private AudioLowPassFilter _unityFilter;

	public override void Attach<T>(T unityFilter)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		object obj = unityFilter;
		if (obj is AudioLowPassFilter)
		{
			_unityFilter = (AudioLowPassFilter)obj;
			UpdateParameters();
		}
	}

	public override void UpdateParameters()
	{
		_unityFilter.cutoffFrequency = cutoffFrequency;
		_unityFilter.lowpassResonanceQ = resonance;
	}
}
