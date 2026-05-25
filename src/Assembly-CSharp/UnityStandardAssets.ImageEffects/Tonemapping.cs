using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Color Adjustments/Tonemapping")]
public class Tonemapping : VPostEffectsBase
{
	public enum TonemapperType
	{
		SimpleReinhard,
		UserCurve,
		Hable,
		Photographic,
		OptimizedHejiDawson,
		AdaptiveReinhard,
		AdaptiveReinhardAutoWhite
	}

	public enum AdaptiveTexSize
	{
		Square16 = 0x10,
		Square32 = 0x20,
		Square64 = 0x40,
		Square128 = 0x80,
		Square256 = 0x100,
		Square512 = 0x200,
		Square1024 = 0x400
	}

	public TonemapperType type = TonemapperType.Photographic;

	public AdaptiveTexSize adaptiveTextureSize = AdaptiveTexSize.Square256;

	public AnimationCurve remapCurve;

	private Texture2D curveTex;

	public float exposureAdjustment = 1.5f;

	public float middleGrey = 0.4f;

	public float white = 2f;

	public float adaptionSpeed = 1.5f;

	public Shader tonemapper;

	public bool validRenderTextureFormat = true;

	private Material tonemapMaterial;

	private RenderTexture rt;

	private RenderTextureFormat rtFormat = (RenderTextureFormat)2;

	public override bool CheckResources()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		CheckSupport(needDepth: false, needHdr: true);
		tonemapMaterial = CheckShaderAndCreateMaterial(tonemapper, tonemapMaterial);
		if (!Object.op_Implicit((Object)(object)curveTex) && type == TonemapperType.UserCurve)
		{
			curveTex = new Texture2D(256, 1, (TextureFormat)5, false, true);
			((Texture)curveTex).filterMode = (FilterMode)1;
			((Texture)curveTex).wrapMode = (TextureWrapMode)1;
			((Object)curveTex).hideFlags = (HideFlags)52;
		}
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	public float UpdateCurve()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		if (remapCurve.keys.Length < 1)
		{
			remapCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
			{
				new Keyframe(0f, 0f),
				new Keyframe(2f, 1f)
			});
		}
		if (remapCurve != null)
		{
			if (remapCurve.length > 0)
			{
				Keyframe val = remapCurve[remapCurve.length - 1];
				num = ((Keyframe)(ref val)).time;
			}
			for (float num2 = 0f; num2 <= 1f; num2 += 0.003921569f)
			{
				float num3 = remapCurve.Evaluate(num2 * 1f * num);
				curveTex.SetPixel((int)Mathf.Floor(num2 * 255f), 0, new Color(num3, num3, num3));
			}
			curveTex.Apply();
		}
		return 1f / num;
	}

	private void OnDisable()
	{
		if (Object.op_Implicit((Object)(object)rt))
		{
			Object.DestroyImmediate((Object)(object)rt);
			rt = null;
		}
		if (Object.op_Implicit((Object)(object)tonemapMaterial))
		{
			Object.DestroyImmediate((Object)(object)tonemapMaterial);
			tonemapMaterial = null;
		}
		if (Object.op_Implicit((Object)(object)curveTex))
		{
			Object.DestroyImmediate((Object)(object)curveTex);
			curveTex = null;
		}
	}

	private bool CreateInternalRenderTexture()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		if (Object.op_Implicit((Object)(object)rt))
		{
			return false;
		}
		rtFormat = (RenderTextureFormat)((!SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)13)) ? 2 : 13);
		rt = new RenderTexture(1, 1, 0, rtFormat);
		((Object)rt).hideFlags = (HideFlags)52;
		return true;
	}

	[ImageEffectTransformsToLDR]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckResources())
		{
			Graphics.Blit((Texture)(object)source, destination);
			return;
		}
		exposureAdjustment = ((!(exposureAdjustment < 0.001f)) ? exposureAdjustment : 0.001f);
		if (type == TonemapperType.UserCurve)
		{
			float num = UpdateCurve();
			tonemapMaterial.SetFloat("_RangeScale", num);
			tonemapMaterial.SetTexture("_Curve", (Texture)(object)curveTex);
			Graphics.Blit((Texture)(object)source, destination, tonemapMaterial, 4);
			return;
		}
		if (type == TonemapperType.SimpleReinhard)
		{
			tonemapMaterial.SetFloat("_ExposureAdjustment", exposureAdjustment);
			Graphics.Blit((Texture)(object)source, destination, tonemapMaterial, 6);
			return;
		}
		if (type == TonemapperType.Hable)
		{
			tonemapMaterial.SetFloat("_ExposureAdjustment", exposureAdjustment);
			Graphics.Blit((Texture)(object)source, destination, tonemapMaterial, 5);
			return;
		}
		if (type == TonemapperType.Photographic)
		{
			tonemapMaterial.SetFloat("_ExposureAdjustment", exposureAdjustment);
			Graphics.Blit((Texture)(object)source, destination, tonemapMaterial, 8);
			return;
		}
		if (type == TonemapperType.OptimizedHejiDawson)
		{
			tonemapMaterial.SetFloat("_ExposureAdjustment", 0.5f * exposureAdjustment);
			Graphics.Blit((Texture)(object)source, destination, tonemapMaterial, 7);
			return;
		}
		bool flag = CreateInternalRenderTexture();
		RenderTexture temporary = RenderTexture.GetTemporary((int)adaptiveTextureSize, (int)adaptiveTextureSize, 0, rtFormat);
		Graphics.Blit((Texture)(object)source, temporary);
		int num2 = (int)Mathf.Log((float)((Texture)temporary).width * 1f, 2f);
		int num3 = 2;
		RenderTexture[] array = (RenderTexture[])(object)new RenderTexture[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = RenderTexture.GetTemporary(((Texture)temporary).width / num3, ((Texture)temporary).width / num3, 0, rtFormat);
			num3 *= 2;
		}
		RenderTexture val = array[num2 - 1];
		Graphics.Blit((Texture)(object)temporary, array[0], tonemapMaterial, 1);
		if (type == TonemapperType.AdaptiveReinhardAutoWhite)
		{
			for (int j = 0; j < num2 - 1; j++)
			{
				Graphics.Blit((Texture)(object)array[j], array[j + 1], tonemapMaterial, 9);
				val = array[j + 1];
			}
		}
		else if (type == TonemapperType.AdaptiveReinhard)
		{
			for (int k = 0; k < num2 - 1; k++)
			{
				Graphics.Blit((Texture)(object)array[k], array[k + 1]);
				val = array[k + 1];
			}
		}
		adaptionSpeed = ((!(adaptionSpeed < 0.001f)) ? adaptionSpeed : 0.001f);
		tonemapMaterial.SetFloat("_AdaptionSpeed", adaptionSpeed);
		rt.MarkRestoreExpected();
		Graphics.Blit((Texture)(object)val, rt, tonemapMaterial, (!flag) ? 2 : 3);
		middleGrey = ((!(middleGrey < 0.001f)) ? middleGrey : 0.001f);
		tonemapMaterial.SetVector("_HdrParams", new Vector4(middleGrey, middleGrey, middleGrey, white * white));
		tonemapMaterial.SetTexture("_SmallTex", (Texture)(object)rt);
		if (type == TonemapperType.AdaptiveReinhard)
		{
			Graphics.Blit((Texture)(object)source, destination, tonemapMaterial, 0);
		}
		else if (type == TonemapperType.AdaptiveReinhardAutoWhite)
		{
			Graphics.Blit((Texture)(object)source, destination, tonemapMaterial, 10);
		}
		else
		{
			Debug.LogError((object)"No valid adaptive tonemapper type found!");
			Graphics.Blit((Texture)(object)source, destination);
		}
		for (int l = 0; l < num2; l++)
		{
			RenderTexture.ReleaseTemporary(array[l]);
		}
		RenderTexture.ReleaseTemporary(temporary);
	}
}
