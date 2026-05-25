using UnityEngine;

namespace S13Audio;

public class CrossFade : MonoBehaviour
{
	public AnimationCurve curve;

	public S13AudioSource zoomIn;

	public S13AudioSource zoomOut;

	public void DoCrossFade(float t)
	{
		float num = curve.Evaluate(t);
		Debug.Log((object)num);
		zoomIn.volume = S13AudioUtil.Lin2dB(1f - num);
		zoomOut.volume = S13AudioUtil.Lin2dB(num);
		zoomIn.UpdateParameters();
		zoomOut.UpdateParameters();
	}
}
