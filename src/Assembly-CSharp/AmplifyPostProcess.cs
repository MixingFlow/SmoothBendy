using UnityEngine;

[ExecuteInEditMode]
public class AmplifyPostProcess : MonoBehaviour
{
	public Material PostProcessMat;

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit((Texture)(object)src, dest, PostProcessMat);
	}
}
