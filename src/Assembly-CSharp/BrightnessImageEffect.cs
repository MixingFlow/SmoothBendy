using UnityEngine;

[ExecuteInEditMode]
public class BrightnessImageEffect : MonoBehaviour
{
	public Material PostProcessMat;

	public void SetBrightness(float _ammount)
	{
		PostProcessMat.SetFloat("_Brightness", _ammount);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit((Texture)(object)src, dest, PostProcessMat);
	}
}
