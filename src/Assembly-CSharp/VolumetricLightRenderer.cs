using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class VolumetricLightRenderer : MonoBehaviour
{
	public enum VolumtericResolution
	{
		Full,
		Half,
		Quarter
	}

	private static Mesh _pointLightMesh;

	private static Mesh _spotLightMesh;

	private static Material _lightMaterial;

	private Camera _camera;

	private CommandBuffer _preLightPass;

	private Matrix4x4 _viewProj;

	private Material _blitAddMaterial;

	private Material _bilateralBlurMaterial;

	private RenderTexture _volumeLightTexture;

	private RenderTexture _halfVolumeLightTexture;

	private RenderTexture _quarterVolumeLightTexture;

	private static Texture _defaultSpotCookie;

	private RenderTexture _halfDepthBuffer;

	private RenderTexture _quarterDepthBuffer;

	private VolumtericResolution _currentResolution = VolumtericResolution.Half;

	private Texture2D _ditheringTexture;

	private Texture3D _noiseTexture;

	public VolumtericResolution Resolution = VolumtericResolution.Half;

	public Texture DefaultSpotCookie;

	public CommandBuffer GlobalCommandBuffer => _preLightPass;

	public static event Action<VolumetricLightRenderer, Matrix4x4> PreRenderEvent;

	public static Material GetLightMaterial()
	{
		return _lightMaterial;
	}

	public static Mesh GetPointLightMesh()
	{
		return _pointLightMesh;
	}

	public static Mesh GetSpotLightMesh()
	{
		return _spotLightMesh;
	}

	public RenderTexture GetVolumeLightBuffer()
	{
		if (Resolution == VolumtericResolution.Quarter)
		{
			return _quarterVolumeLightTexture;
		}
		if (Resolution == VolumtericResolution.Half)
		{
			return _halfVolumeLightTexture;
		}
		return _volumeLightTexture;
	}

	public RenderTexture GetVolumeLightDepthBuffer()
	{
		if (Resolution == VolumtericResolution.Quarter)
		{
			return _quarterDepthBuffer;
		}
		if (Resolution == VolumtericResolution.Half)
		{
			return _halfDepthBuffer;
		}
		return null;
	}

	public static Texture GetDefaultSpotCookie()
	{
		return _defaultSpotCookie;
	}

	private void Awake()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		_camera = ((Component)this).GetComponent<Camera>();
		if ((int)_camera.actualRenderingPath == 1)
		{
			_camera.depthTextureMode = (DepthTextureMode)1;
		}
		_currentResolution = Resolution;
		Shader val = Shader.Find("Hidden/BlitAdd");
		if ((Object)(object)val == (Object)null)
		{
			throw new Exception("Critical Error: \"Hidden/BlitAdd\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		}
		_blitAddMaterial = new Material(val);
		val = Shader.Find("Hidden/BilateralBlur");
		if ((Object)(object)val == (Object)null)
		{
			throw new Exception("Critical Error: \"Hidden/BilateralBlur\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		}
		_bilateralBlurMaterial = new Material(val);
		_preLightPass = new CommandBuffer();
		_preLightPass.name = "PreLight";
		ChangeResolution();
		if ((Object)(object)_pointLightMesh == (Object)null)
		{
			GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)0);
			_pointLightMesh = val2.GetComponent<MeshFilter>().sharedMesh;
			Object.Destroy((Object)(object)val2);
		}
		if ((Object)(object)_spotLightMesh == (Object)null)
		{
			_spotLightMesh = CreateSpotLightMesh();
		}
		if ((Object)(object)_lightMaterial == (Object)null)
		{
			val = Shader.Find("Sandbox/VolumetricLight");
			if ((Object)(object)val == (Object)null)
			{
				throw new Exception("Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
			}
			_lightMaterial = new Material(val);
		}
		if ((Object)(object)_defaultSpotCookie == (Object)null)
		{
			_defaultSpotCookie = DefaultSpotCookie;
		}
		LoadNoise3dTexture();
		GenerateDitherTexture();
	}

	private void OnEnable()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Invalid comparison between Unknown and I4
		if (!Shader.Find("Sandbox/VolumetricLight").isSupported || !Shader.Find("Hidden/BilateralBlur").isSupported || !Shader.Find("Hidden/BlitAdd").isSupported)
		{
			Debug.LogWarning((object)"Volumetric Light Shader is not supported!");
			((Behaviour)this).enabled = false;
		}
		else if ((int)_camera.actualRenderingPath == 1)
		{
			_camera.AddCommandBuffer((CameraEvent)1, _preLightPass);
		}
		else
		{
			_camera.AddCommandBuffer((CameraEvent)6, _preLightPass);
		}
	}

	private void OnDisable()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)_camera.actualRenderingPath == 1)
		{
			_camera.RemoveCommandBuffer((CameraEvent)1, _preLightPass);
		}
		else
		{
			_camera.RemoveCommandBuffer((CameraEvent)6, _preLightPass);
		}
	}

	private void ChangeResolution()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		int pixelWidth = _camera.pixelWidth;
		int pixelHeight = _camera.pixelHeight;
		if ((Object)(object)_volumeLightTexture != (Object)null)
		{
			Object.Destroy((Object)(object)_volumeLightTexture);
		}
		_volumeLightTexture = new RenderTexture(pixelWidth, pixelHeight, 0, (RenderTextureFormat)2);
		((Object)_volumeLightTexture).name = "VolumeLightBuffer";
		((Texture)_volumeLightTexture).filterMode = (FilterMode)1;
		if ((Object)(object)_halfDepthBuffer != (Object)null)
		{
			Object.Destroy((Object)(object)_halfDepthBuffer);
		}
		if ((Object)(object)_halfVolumeLightTexture != (Object)null)
		{
			Object.Destroy((Object)(object)_halfVolumeLightTexture);
		}
		if (Resolution == VolumtericResolution.Half || Resolution == VolumtericResolution.Quarter)
		{
			_halfVolumeLightTexture = new RenderTexture(pixelWidth / 2, pixelHeight / 2, 0, (RenderTextureFormat)2);
			((Object)_halfVolumeLightTexture).name = "VolumeLightBufferHalf";
			((Texture)_halfVolumeLightTexture).filterMode = (FilterMode)1;
			_halfDepthBuffer = new RenderTexture(pixelWidth / 2, pixelHeight / 2, 0, (RenderTextureFormat)14);
			((Object)_halfDepthBuffer).name = "VolumeLightHalfDepth";
			_halfDepthBuffer.Create();
			((Texture)_halfDepthBuffer).filterMode = (FilterMode)0;
		}
		if ((Object)(object)_quarterVolumeLightTexture != (Object)null)
		{
			Object.Destroy((Object)(object)_quarterVolumeLightTexture);
		}
		if ((Object)(object)_quarterDepthBuffer != (Object)null)
		{
			Object.Destroy((Object)(object)_quarterDepthBuffer);
		}
		if (Resolution == VolumtericResolution.Quarter)
		{
			_quarterVolumeLightTexture = new RenderTexture(pixelWidth / 4, pixelHeight / 4, 0, (RenderTextureFormat)2);
			((Object)_quarterVolumeLightTexture).name = "VolumeLightBufferQuarter";
			((Texture)_quarterVolumeLightTexture).filterMode = (FilterMode)1;
			_quarterDepthBuffer = new RenderTexture(pixelWidth / 4, pixelHeight / 4, 0, (RenderTextureFormat)14);
			((Object)_quarterDepthBuffer).name = "VolumeLightQuarterDepth";
			_quarterDepthBuffer.Create();
			((Texture)_quarterDepthBuffer).filterMode = (FilterMode)0;
		}
	}

	public void OnPreRender()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 val = Matrix4x4.Perspective(_camera.fieldOfView, _camera.aspect, 0.01f, _camera.farClipPlane);
		val = GL.GetGPUProjectionMatrix(val, true);
		_viewProj = val * _camera.worldToCameraMatrix;
		_preLightPass.Clear();
		bool flag = SystemInfo.graphicsShaderLevel > 40;
		if (Resolution == VolumtericResolution.Quarter)
		{
			Texture val2 = null;
			_preLightPass.Blit(val2, RenderTargetIdentifier.op_Implicit((Texture)(object)_halfDepthBuffer), _bilateralBlurMaterial, (!flag) ? 10 : 4);
			_preLightPass.Blit(val2, RenderTargetIdentifier.op_Implicit((Texture)(object)_quarterDepthBuffer), _bilateralBlurMaterial, (!flag) ? 11 : 6);
			_preLightPass.SetRenderTarget(RenderTargetIdentifier.op_Implicit((Texture)(object)_quarterVolumeLightTexture));
		}
		else if (Resolution == VolumtericResolution.Half)
		{
			Texture val3 = null;
			_preLightPass.Blit(val3, RenderTargetIdentifier.op_Implicit((Texture)(object)_halfDepthBuffer), _bilateralBlurMaterial, (!flag) ? 10 : 4);
			_preLightPass.SetRenderTarget(RenderTargetIdentifier.op_Implicit((Texture)(object)_halfVolumeLightTexture));
		}
		else
		{
			_preLightPass.SetRenderTarget(RenderTargetIdentifier.op_Implicit((Texture)(object)_volumeLightTexture));
		}
		_preLightPass.ClearRenderTarget(false, true, new Color(0f, 0f, 0f, 1f));
		UpdateMaterialParameters();
		if (VolumetricLightRenderer.PreRenderEvent != null)
		{
			VolumetricLightRenderer.PreRenderEvent(this, _viewProj);
		}
	}

	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (Resolution == VolumtericResolution.Quarter)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(((Texture)_quarterDepthBuffer).width, ((Texture)_quarterDepthBuffer).height, 0, (RenderTextureFormat)2);
			((Texture)temporary).filterMode = (FilterMode)1;
			Graphics.Blit((Texture)(object)_quarterVolumeLightTexture, temporary, _bilateralBlurMaterial, 8);
			Graphics.Blit((Texture)(object)temporary, _quarterVolumeLightTexture, _bilateralBlurMaterial, 9);
			Graphics.Blit((Texture)(object)_quarterVolumeLightTexture, _volumeLightTexture, _bilateralBlurMaterial, 7);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (Resolution == VolumtericResolution.Half)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(((Texture)_halfVolumeLightTexture).width, ((Texture)_halfVolumeLightTexture).height, 0, (RenderTextureFormat)2);
			((Texture)temporary2).filterMode = (FilterMode)1;
			Graphics.Blit((Texture)(object)_halfVolumeLightTexture, temporary2, _bilateralBlurMaterial, 2);
			Graphics.Blit((Texture)(object)temporary2, _halfVolumeLightTexture, _bilateralBlurMaterial, 3);
			Graphics.Blit((Texture)(object)_halfVolumeLightTexture, _volumeLightTexture, _bilateralBlurMaterial, 5);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			RenderTexture temporary3 = RenderTexture.GetTemporary(((Texture)_volumeLightTexture).width, ((Texture)_volumeLightTexture).height, 0, (RenderTextureFormat)2);
			((Texture)temporary3).filterMode = (FilterMode)1;
			Graphics.Blit((Texture)(object)_volumeLightTexture, temporary3, _bilateralBlurMaterial, 0);
			Graphics.Blit((Texture)(object)temporary3, _volumeLightTexture, _bilateralBlurMaterial, 1);
			RenderTexture.ReleaseTemporary(temporary3);
		}
		_blitAddMaterial.SetTexture("_Source", (Texture)(object)source);
		Graphics.Blit((Texture)(object)_volumeLightTexture, destination, _blitAddMaterial, 0);
	}

	private void UpdateMaterialParameters()
	{
		_bilateralBlurMaterial.SetTexture("_HalfResDepthBuffer", (Texture)(object)_halfDepthBuffer);
		_bilateralBlurMaterial.SetTexture("_HalfResColor", (Texture)(object)_halfVolumeLightTexture);
		_bilateralBlurMaterial.SetTexture("_QuarterResDepthBuffer", (Texture)(object)_quarterDepthBuffer);
		_bilateralBlurMaterial.SetTexture("_QuarterResColor", (Texture)(object)_quarterVolumeLightTexture);
		Shader.SetGlobalTexture("_DitherTexture", (Texture)(object)_ditheringTexture);
		Shader.SetGlobalTexture("_NoiseTexture", (Texture)(object)_noiseTexture);
	}

	private void Update()
	{
		if (_currentResolution != Resolution)
		{
			_currentResolution = Resolution;
			ChangeResolution();
		}
		if (((Texture)_volumeLightTexture).width != _camera.pixelWidth || ((Texture)_volumeLightTexture).height != _camera.pixelHeight)
		{
			ChangeResolution();
		}
	}

	private void LoadNoise3dTexture()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		Object obj = Resources.Load("NoiseVolume");
		TextAsset val = (TextAsset)(object)((obj is TextAsset) ? obj : null);
		byte[] bytes = val.bytes;
		uint num = BitConverter.ToUInt32(val.bytes, 12);
		uint num2 = BitConverter.ToUInt32(val.bytes, 16);
		uint num3 = BitConverter.ToUInt32(val.bytes, 20);
		uint num4 = BitConverter.ToUInt32(val.bytes, 24);
		uint num5 = BitConverter.ToUInt32(val.bytes, 80);
		uint num6 = BitConverter.ToUInt32(val.bytes, 88);
		if (num6 == 0)
		{
			num6 = num3 / num2 * 8;
		}
		_noiseTexture = new Texture3D((int)num2, (int)num, (int)num4, (TextureFormat)4, false);
		((Object)_noiseTexture).name = "3D Noise";
		Color[] array = (Color[])(object)new Color[num2 * num * num4];
		uint num7 = 128u;
		if (val.bytes[84] == 68 && val.bytes[85] == 88 && val.bytes[86] == 49 && val.bytes[87] == 48 && (num5 & 4) != 0)
		{
			uint num8 = BitConverter.ToUInt32(val.bytes, (int)num7);
			if (num8 >= 60 && num8 <= 65)
			{
				num6 = 8u;
			}
			else if (num8 >= 48 && num8 <= 52)
			{
				num6 = 16u;
			}
			else if (num8 >= 27 && num8 <= 32)
			{
				num6 = 32u;
			}
			num7 += 20;
		}
		uint num9 = num6 / 8;
		num3 = (num2 * num6 + 7) / 8;
		for (int i = 0; i < num4; i++)
		{
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < num2; k++)
				{
					float num10 = (float)(int)bytes[num7 + k * num9] / 255f;
					ref Color reference = ref array[k + j * num2 + i * num2 * num];
					reference = new Color(num10, num10, num10, num10);
				}
				num7 += num3;
			}
		}
		_noiseTexture.SetPixels(array);
		_noiseTexture.Apply();
	}

	private void GenerateDitherTexture()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Unknown result type (might be due to invalid IL or missing references)
		//IL_0773: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)_ditheringTexture != (Object)null))
		{
			int num = 8;
			_ditheringTexture = new Texture2D(num, num, (TextureFormat)1, false, true);
			((Texture)_ditheringTexture).filterMode = (FilterMode)0;
			Color32[] array = (Color32[])(object)new Color32[num * num];
			int num2 = 0;
			byte b = 3;
			ref Color32 reference = ref array[num2++];
			reference = new Color32(b, b, b, b);
			b = 192;
			ref Color32 reference2 = ref array[num2++];
			reference2 = new Color32(b, b, b, b);
			b = 51;
			ref Color32 reference3 = ref array[num2++];
			reference3 = new Color32(b, b, b, b);
			b = 239;
			ref Color32 reference4 = ref array[num2++];
			reference4 = new Color32(b, b, b, b);
			b = 15;
			ref Color32 reference5 = ref array[num2++];
			reference5 = new Color32(b, b, b, b);
			b = 204;
			ref Color32 reference6 = ref array[num2++];
			reference6 = new Color32(b, b, b, b);
			b = 62;
			ref Color32 reference7 = ref array[num2++];
			reference7 = new Color32(b, b, b, b);
			b = 251;
			ref Color32 reference8 = ref array[num2++];
			reference8 = new Color32(b, b, b, b);
			b = 129;
			ref Color32 reference9 = ref array[num2++];
			reference9 = new Color32(b, b, b, b);
			b = 66;
			ref Color32 reference10 = ref array[num2++];
			reference10 = new Color32(b, b, b, b);
			b = 176;
			ref Color32 reference11 = ref array[num2++];
			reference11 = new Color32(b, b, b, b);
			b = 113;
			ref Color32 reference12 = ref array[num2++];
			reference12 = new Color32(b, b, b, b);
			b = 141;
			ref Color32 reference13 = ref array[num2++];
			reference13 = new Color32(b, b, b, b);
			b = 78;
			ref Color32 reference14 = ref array[num2++];
			reference14 = new Color32(b, b, b, b);
			b = 188;
			ref Color32 reference15 = ref array[num2++];
			reference15 = new Color32(b, b, b, b);
			b = 125;
			ref Color32 reference16 = ref array[num2++];
			reference16 = new Color32(b, b, b, b);
			b = 35;
			ref Color32 reference17 = ref array[num2++];
			reference17 = new Color32(b, b, b, b);
			b = 223;
			ref Color32 reference18 = ref array[num2++];
			reference18 = new Color32(b, b, b, b);
			b = 19;
			ref Color32 reference19 = ref array[num2++];
			reference19 = new Color32(b, b, b, b);
			b = 207;
			ref Color32 reference20 = ref array[num2++];
			reference20 = new Color32(b, b, b, b);
			b = 47;
			ref Color32 reference21 = ref array[num2++];
			reference21 = new Color32(b, b, b, b);
			b = 235;
			ref Color32 reference22 = ref array[num2++];
			reference22 = new Color32(b, b, b, b);
			b = 31;
			ref Color32 reference23 = ref array[num2++];
			reference23 = new Color32(b, b, b, b);
			b = 219;
			ref Color32 reference24 = ref array[num2++];
			reference24 = new Color32(b, b, b, b);
			b = 160;
			ref Color32 reference25 = ref array[num2++];
			reference25 = new Color32(b, b, b, b);
			b = 98;
			ref Color32 reference26 = ref array[num2++];
			reference26 = new Color32(b, b, b, b);
			b = 145;
			ref Color32 reference27 = ref array[num2++];
			reference27 = new Color32(b, b, b, b);
			b = 82;
			ref Color32 reference28 = ref array[num2++];
			reference28 = new Color32(b, b, b, b);
			b = 172;
			ref Color32 reference29 = ref array[num2++];
			reference29 = new Color32(b, b, b, b);
			b = 109;
			ref Color32 reference30 = ref array[num2++];
			reference30 = new Color32(b, b, b, b);
			b = 156;
			ref Color32 reference31 = ref array[num2++];
			reference31 = new Color32(b, b, b, b);
			b = 94;
			ref Color32 reference32 = ref array[num2++];
			reference32 = new Color32(b, b, b, b);
			b = 11;
			ref Color32 reference33 = ref array[num2++];
			reference33 = new Color32(b, b, b, b);
			b = 200;
			ref Color32 reference34 = ref array[num2++];
			reference34 = new Color32(b, b, b, b);
			b = 58;
			ref Color32 reference35 = ref array[num2++];
			reference35 = new Color32(b, b, b, b);
			b = 247;
			ref Color32 reference36 = ref array[num2++];
			reference36 = new Color32(b, b, b, b);
			b = 7;
			ref Color32 reference37 = ref array[num2++];
			reference37 = new Color32(b, b, b, b);
			b = 196;
			ref Color32 reference38 = ref array[num2++];
			reference38 = new Color32(b, b, b, b);
			b = 54;
			ref Color32 reference39 = ref array[num2++];
			reference39 = new Color32(b, b, b, b);
			b = 243;
			ref Color32 reference40 = ref array[num2++];
			reference40 = new Color32(b, b, b, b);
			b = 137;
			ref Color32 reference41 = ref array[num2++];
			reference41 = new Color32(b, b, b, b);
			b = 74;
			ref Color32 reference42 = ref array[num2++];
			reference42 = new Color32(b, b, b, b);
			b = 184;
			ref Color32 reference43 = ref array[num2++];
			reference43 = new Color32(b, b, b, b);
			b = 121;
			ref Color32 reference44 = ref array[num2++];
			reference44 = new Color32(b, b, b, b);
			b = 133;
			ref Color32 reference45 = ref array[num2++];
			reference45 = new Color32(b, b, b, b);
			b = 70;
			ref Color32 reference46 = ref array[num2++];
			reference46 = new Color32(b, b, b, b);
			b = 180;
			ref Color32 reference47 = ref array[num2++];
			reference47 = new Color32(b, b, b, b);
			b = 117;
			ref Color32 reference48 = ref array[num2++];
			reference48 = new Color32(b, b, b, b);
			b = 43;
			ref Color32 reference49 = ref array[num2++];
			reference49 = new Color32(b, b, b, b);
			b = 231;
			ref Color32 reference50 = ref array[num2++];
			reference50 = new Color32(b, b, b, b);
			b = 27;
			ref Color32 reference51 = ref array[num2++];
			reference51 = new Color32(b, b, b, b);
			b = 215;
			ref Color32 reference52 = ref array[num2++];
			reference52 = new Color32(b, b, b, b);
			b = 39;
			ref Color32 reference53 = ref array[num2++];
			reference53 = new Color32(b, b, b, b);
			b = 227;
			ref Color32 reference54 = ref array[num2++];
			reference54 = new Color32(b, b, b, b);
			b = 23;
			ref Color32 reference55 = ref array[num2++];
			reference55 = new Color32(b, b, b, b);
			b = 211;
			ref Color32 reference56 = ref array[num2++];
			reference56 = new Color32(b, b, b, b);
			b = 168;
			ref Color32 reference57 = ref array[num2++];
			reference57 = new Color32(b, b, b, b);
			b = 105;
			ref Color32 reference58 = ref array[num2++];
			reference58 = new Color32(b, b, b, b);
			b = 153;
			ref Color32 reference59 = ref array[num2++];
			reference59 = new Color32(b, b, b, b);
			b = 90;
			ref Color32 reference60 = ref array[num2++];
			reference60 = new Color32(b, b, b, b);
			b = 164;
			ref Color32 reference61 = ref array[num2++];
			reference61 = new Color32(b, b, b, b);
			b = 102;
			ref Color32 reference62 = ref array[num2++];
			reference62 = new Color32(b, b, b, b);
			b = 149;
			ref Color32 reference63 = ref array[num2++];
			reference63 = new Color32(b, b, b, b);
			b = 86;
			ref Color32 reference64 = ref array[num2++];
			reference64 = new Color32(b, b, b, b);
			_ditheringTexture.SetPixels32(array);
			_ditheringTexture.Apply();
		}
	}

	private Mesh CreateSpotLightMesh()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		Mesh val = new Mesh();
		Vector3[] array = (Vector3[])(object)new Vector3[50];
		Color32[] array2 = (Color32[])(object)new Color32[50];
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0f, 0f, 0f);
		ref Vector3 reference2 = ref array[1];
		reference2 = new Vector3(0f, 0f, 1f);
		float num = 0f;
		float num2 = (float)Math.PI / 8f;
		float num3 = 0.9f;
		for (int i = 0; i < 16; i++)
		{
			ref Vector3 reference3 = ref array[i + 2];
			reference3 = new Vector3((0f - Mathf.Cos(num)) * num3, Mathf.Sin(num) * num3, num3);
			ref Color32 reference4 = ref array2[i + 2];
			reference4 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			ref Vector3 reference5 = ref array[i + 2 + 16];
			reference5 = new Vector3(0f - Mathf.Cos(num), Mathf.Sin(num), 1f);
			ref Color32 reference6 = ref array2[i + 2 + 16];
			reference6 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)0);
			ref Vector3 reference7 = ref array[i + 2 + 32];
			reference7 = new Vector3((0f - Mathf.Cos(num)) * num3, Mathf.Sin(num) * num3, 1f);
			ref Color32 reference8 = ref array2[i + 2 + 32];
			reference8 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			num += num2;
		}
		val.vertices = array;
		val.colors32 = array2;
		int[] array3 = new int[288];
		int num4 = 0;
		for (int j = 2; j < 17; j++)
		{
			array3[num4++] = 0;
			array3[num4++] = j;
			array3[num4++] = j + 1;
		}
		array3[num4++] = 0;
		array3[num4++] = 17;
		array3[num4++] = 2;
		for (int k = 2; k < 17; k++)
		{
			array3[num4++] = k;
			array3[num4++] = k + 16;
			array3[num4++] = k + 1;
			array3[num4++] = k + 1;
			array3[num4++] = k + 16;
			array3[num4++] = k + 16 + 1;
		}
		array3[num4++] = 2;
		array3[num4++] = 17;
		array3[num4++] = 18;
		array3[num4++] = 18;
		array3[num4++] = 17;
		array3[num4++] = 33;
		for (int l = 18; l < 33; l++)
		{
			array3[num4++] = l;
			array3[num4++] = l + 16;
			array3[num4++] = l + 1;
			array3[num4++] = l + 1;
			array3[num4++] = l + 16;
			array3[num4++] = l + 16 + 1;
		}
		array3[num4++] = 18;
		array3[num4++] = 33;
		array3[num4++] = 34;
		array3[num4++] = 34;
		array3[num4++] = 33;
		array3[num4++] = 49;
		for (int m = 34; m < 49; m++)
		{
			array3[num4++] = 1;
			array3[num4++] = m + 1;
			array3[num4++] = m;
		}
		array3[num4++] = 1;
		array3[num4++] = 34;
		array3[num4++] = 49;
		val.triangles = array3;
		val.RecalculateBounds();
		return val;
	}
}
