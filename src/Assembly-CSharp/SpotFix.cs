using UnityEngine;

[ExecuteInEditMode]
public class SpotFix : MonoBehaviour
{
	public Material PostProcessMat;

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit((Texture)(object)src, dest, PostProcessMat);
	}
}
