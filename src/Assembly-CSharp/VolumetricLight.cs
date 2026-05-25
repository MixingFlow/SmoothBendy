using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Light))]
public class VolumetricLight : MonoBehaviour
{
	private Light _light;

	private Material _material;

	private CommandBuffer _commandBuffer;

	private CommandBuffer _cascadeShadowCommandBuffer;

	[Range(1f, 64f)]
	public int SampleCount = 8;

	[Range(0f, 1f)]
	public float ScatteringCoef = 0.5f;

	[Range(0f, 0.1f)]
	public float ExtinctionCoef = 0.01f;

	[Range(0f, 1f)]
	public float SkyboxExtinctionCoef = 0.9f;

	[Range(0f, 0.999f)]
	public float MieG = 0.1f;

	public bool HeightFog;

	[Range(0f, 0.5f)]
	public float HeightScale = 0.1f;

	public float GroundLevel;

	public bool Noise;

	public float NoiseScale = 0.015f;

	public float NoiseIntensity = 1f;

	public float NoiseIntensityOffset = 0.3f;

	public Vector2 NoiseVelocity = new Vector2(3f, 3f);

	[Tooltip("")]
	public float MaxRayLength = 400f;

	private Vector4[] _frustumCorners = (Vector4[])(object)new Vector4[4];

	private bool _reversedZ;

	public Light Light => _light;

	public Material VolumetricMaterial => _material;

	public event Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> CustomRenderEvent;

	private void Start()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Invalid comparison between Unknown and I4
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		if ((int)SystemInfo.graphicsDeviceType == 2 || (int)SystemInfo.graphicsDeviceType == 18 || (int)SystemInfo.graphicsDeviceType == 16 || (int)SystemInfo.graphicsDeviceType == 13 || (int)SystemInfo.graphicsDeviceType == 21 || (int)SystemInfo.graphicsDeviceType == 14)
		{
			_reversedZ = true;
		}
		_commandBuffer = new CommandBuffer();
		_commandBuffer.name = "Light Command Buffer";
		_cascadeShadowCommandBuffer = new CommandBuffer();
		_cascadeShadowCommandBuffer.name = "Dir Light Command Buffer";
		_cascadeShadowCommandBuffer.SetGlobalTexture("_CascadeShadowMapTexture", new RenderTargetIdentifier((BuiltinRenderTextureType)1));
		_light = ((Component)this).GetComponent<Light>();
		if ((int)_light.type == 1)
		{
			_light.AddCommandBuffer((LightEvent)2, _commandBuffer);
			_light.AddCommandBuffer((LightEvent)1, _cascadeShadowCommandBuffer);
		}
		else
		{
			_light.AddCommandBuffer((LightEvent)1, _commandBuffer);
		}
		Shader val = Shader.Find("Sandbox/VolumetricLight");
		if ((Object)(object)val == (Object)null)
		{
			throw new Exception("Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		}
		_material = new Material(val);
	}

	private void OnEnable()
	{
		VolumetricLightRenderer.PreRenderEvent += VolumetricLightRenderer_PreRenderEvent;
	}

	private void OnDisable()
	{
		VolumetricLightRenderer.PreRenderEvent -= VolumetricLightRenderer_PreRenderEvent;
	}

	public void OnDestroy()
	{
		Object.Destroy((Object)(object)_material);
	}

	private void VolumetricLightRenderer_PreRenderEvent(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Invalid comparison between Unknown and I4
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Invalid comparison between Unknown and I4
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_light == (Object)null || (Object)(object)((Component)_light).gameObject == (Object)null)
		{
			VolumetricLightRenderer.PreRenderEvent -= VolumetricLightRenderer_PreRenderEvent;
		}
		if (((Component)_light).gameObject.activeInHierarchy && ((Behaviour)_light).enabled)
		{
			_material.SetVector("_CameraForward", Vector4.op_Implicit(((Component)Camera.current).transform.forward));
			_material.SetInt("_SampleCount", SampleCount);
			_material.SetVector("_NoiseVelocity", new Vector4(NoiseVelocity.x, NoiseVelocity.y) * NoiseScale);
			_material.SetVector("_NoiseData", new Vector4(NoiseScale, NoiseIntensity, NoiseIntensityOffset));
			_material.SetVector("_MieG", new Vector4(1f - MieG * MieG, 1f + MieG * MieG, 2f * MieG, 1f / (4f * (float)Math.PI)));
			_material.SetVector("_VolumetricLight", new Vector4(ScatteringCoef, ExtinctionCoef, _light.range, 1f - SkyboxExtinctionCoef));
			_material.SetTexture("_CameraDepthTexture", (Texture)(object)renderer.GetVolumeLightDepthBuffer());
			_material.SetFloat("_ZTest", 8f);
			if (HeightFog)
			{
				_material.EnableKeyword("HEIGHT_FOG");
				_material.SetVector("_HeightFog", new Vector4(GroundLevel, HeightScale));
			}
			else
			{
				_material.DisableKeyword("HEIGHT_FOG");
			}
			if ((int)_light.type == 2)
			{
				SetupPointLight(renderer, viewProj);
			}
			else if ((int)_light.type == 0)
			{
				SetupSpotLight(renderer, viewProj);
			}
			else if ((int)_light.type == 1)
			{
				SetupDirectionalLight(renderer, viewProj);
			}
		}
	}

	private void Update()
	{
		_commandBuffer.Clear();
	}

	private void SetupPointLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if (!IsCameraInPointLightBounds())
		{
			num = 2;
		}
		_material.SetPass(num);
		Mesh pointLightMesh = VolumetricLightRenderer.GetPointLightMesh();
		float num2 = _light.range * 2f;
		Matrix4x4 val = Matrix4x4.TRS(((Component)this).transform.position, ((Component)_light).transform.rotation, new Vector3(num2, num2, num2));
		_material.SetMatrix("_WorldViewProj", viewProj * val);
		_material.SetMatrix("_WorldView", Camera.current.worldToCameraMatrix * val);
		if (Noise)
		{
			_material.EnableKeyword("NOISE");
		}
		else
		{
			_material.DisableKeyword("NOISE");
		}
		_material.SetVector("_LightPos", new Vector4(((Component)_light).transform.position.x, ((Component)_light).transform.position.y, ((Component)_light).transform.position.z, 1f / (_light.range * _light.range)));
		_material.SetColor("_LightColor", _light.color * _light.intensity);
		if ((Object)(object)_light.cookie == (Object)null)
		{
			_material.EnableKeyword("POINT");
			_material.DisableKeyword("POINT_COOKIE");
		}
		else
		{
			Matrix4x4 val2 = Matrix4x4.TRS(((Component)_light).transform.position, ((Component)_light).transform.rotation, Vector3.one);
			Matrix4x4 inverse = ((Matrix4x4)(ref val2)).inverse;
			_material.SetMatrix("_MyLightMatrix0", inverse);
			_material.EnableKeyword("POINT_COOKIE");
			_material.DisableKeyword("POINT");
			_material.SetTexture("_LightTexture0", _light.cookie);
		}
		bool flag = false;
		Vector3 val3 = ((Component)_light).transform.position - ((Component)Camera.current).transform.position;
		if (((Vector3)(ref val3)).magnitude >= QualitySettings.shadowDistance)
		{
			flag = true;
		}
		if ((int)_light.shadows != 0 && !flag)
		{
			_material.EnableKeyword("SHADOWS_CUBE");
			_commandBuffer.SetGlobalTexture("_ShadowMapTexture", RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)1));
			_commandBuffer.SetRenderTarget(RenderTargetIdentifier.op_Implicit((Texture)(object)renderer.GetVolumeLightBuffer()));
			_commandBuffer.DrawMesh(pointLightMesh, val, _material, 0, num);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, _commandBuffer, viewProj);
			}
		}
		else
		{
			_material.DisableKeyword("SHADOWS_CUBE");
			renderer.GlobalCommandBuffer.DrawMesh(pointLightMesh, val, _material, 0, num);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
			}
		}
	}

	private void SetupSpotLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		int num = 1;
		if (!IsCameraInSpotLightBounds())
		{
			num = 3;
		}
		Mesh spotLightMesh = VolumetricLightRenderer.GetSpotLightMesh();
		float range = _light.range;
		float num2 = Mathf.Tan((_light.spotAngle + 1f) * 0.5f * ((float)Math.PI / 180f)) * _light.range;
		Matrix4x4 val = Matrix4x4.TRS(((Component)this).transform.position, ((Component)this).transform.rotation, new Vector3(num2, num2, range));
		Matrix4x4 val2 = Matrix4x4.TRS(((Component)_light).transform.position, ((Component)_light).transform.rotation, Vector3.one);
		Matrix4x4 inverse = ((Matrix4x4)(ref val2)).inverse;
		Matrix4x4 val3 = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0f), Quaternion.identity, new Vector3(-0.5f, -0.5f, 1f));
		Matrix4x4 val4 = Matrix4x4.Perspective(_light.spotAngle, 1f, 0f, 1f);
		_material.SetMatrix("_MyLightMatrix0", val3 * val4 * inverse);
		_material.SetMatrix("_WorldViewProj", viewProj * val);
		_material.SetVector("_LightPos", new Vector4(((Component)_light).transform.position.x, ((Component)_light).transform.position.y, ((Component)_light).transform.position.z, 1f / (_light.range * _light.range)));
		_material.SetVector("_LightColor", Color.op_Implicit(_light.color * _light.intensity));
		Vector3 position = ((Component)this).transform.position;
		Vector3 forward = ((Component)this).transform.forward;
		Vector3 val5 = position + forward * _light.range;
		float num3 = 0f - Vector3.Dot(val5, forward);
		_material.SetFloat("_PlaneD", num3);
		_material.SetFloat("_CosAngle", Mathf.Cos((_light.spotAngle + 1f) * 0.5f * ((float)Math.PI / 180f)));
		_material.SetVector("_ConeApex", new Vector4(position.x, position.y, position.z));
		_material.SetVector("_ConeAxis", new Vector4(forward.x, forward.y, forward.z));
		_material.EnableKeyword("SPOT");
		if (Noise)
		{
			_material.EnableKeyword("NOISE");
		}
		else
		{
			_material.DisableKeyword("NOISE");
		}
		if ((Object)(object)_light.cookie == (Object)null)
		{
			_material.SetTexture("_LightTexture0", VolumetricLightRenderer.GetDefaultSpotCookie());
		}
		else
		{
			_material.SetTexture("_LightTexture0", _light.cookie);
		}
		bool flag = false;
		Vector3 val6 = ((Component)_light).transform.position - ((Component)Camera.current).transform.position;
		if (((Vector3)(ref val6)).magnitude >= QualitySettings.shadowDistance)
		{
			flag = true;
		}
		if ((int)_light.shadows != 0 && !flag)
		{
			val3 = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
			val4 = ((!_reversedZ) ? Matrix4x4.Perspective(_light.spotAngle, 1f, _light.shadowNearPlane, _light.range) : Matrix4x4.Perspective(_light.spotAngle, 1f, _light.range, _light.shadowNearPlane));
			Matrix4x4 val7 = val3 * val4;
			((Matrix4x4)(ref val7))[0, 2] = ((Matrix4x4)(ref val7))[0, 2] * -1f;
			((Matrix4x4)(ref val7))[1, 2] = ((Matrix4x4)(ref val7))[1, 2] * -1f;
			((Matrix4x4)(ref val7))[2, 2] = ((Matrix4x4)(ref val7))[2, 2] * -1f;
			((Matrix4x4)(ref val7))[3, 2] = ((Matrix4x4)(ref val7))[3, 2] * -1f;
			_material.SetMatrix("_MyWorld2Shadow", val7 * inverse);
			_material.SetMatrix("_WorldView", val7 * inverse);
			_material.EnableKeyword("SHADOWS_DEPTH");
			_commandBuffer.SetGlobalTexture("_ShadowMapTexture", RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)1));
			_commandBuffer.SetRenderTarget(RenderTargetIdentifier.op_Implicit((Texture)(object)renderer.GetVolumeLightBuffer()));
			_commandBuffer.DrawMesh(spotLightMesh, val, _material, 0, num);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, _commandBuffer, viewProj);
			}
		}
		else
		{
			_material.DisableKeyword("SHADOWS_DEPTH");
			renderer.GlobalCommandBuffer.DrawMesh(spotLightMesh, val, _material, 0, num);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
			}
		}
	}

	private void SetupDirectionalLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		int num = 4;
		_material.SetPass(num);
		if (Noise)
		{
			_material.EnableKeyword("NOISE");
		}
		else
		{
			_material.DisableKeyword("NOISE");
		}
		_material.SetVector("_LightDir", new Vector4(((Component)_light).transform.forward.x, ((Component)_light).transform.forward.y, ((Component)_light).transform.forward.z, 1f / (_light.range * _light.range)));
		_material.SetVector("_LightColor", Color.op_Implicit(_light.color * _light.intensity));
		_material.SetFloat("_MaxRayLength", MaxRayLength);
		if ((Object)(object)_light.cookie == (Object)null)
		{
			_material.EnableKeyword("DIRECTIONAL");
			_material.DisableKeyword("DIRECTIONAL_COOKIE");
		}
		else
		{
			_material.EnableKeyword("DIRECTIONAL_COOKIE");
			_material.DisableKeyword("DIRECTIONAL");
			_material.SetTexture("_LightTexture0", _light.cookie);
		}
		ref Vector4 reference = ref _frustumCorners[0];
		reference = Vector4.op_Implicit(Camera.current.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.current.farClipPlane)));
		ref Vector4 reference2 = ref _frustumCorners[2];
		reference2 = Vector4.op_Implicit(Camera.current.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.current.farClipPlane)));
		ref Vector4 reference3 = ref _frustumCorners[3];
		reference3 = Vector4.op_Implicit(Camera.current.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.current.farClipPlane)));
		ref Vector4 reference4 = ref _frustumCorners[1];
		reference4 = Vector4.op_Implicit(Camera.current.ViewportToWorldPoint(new Vector3(1f, 0f, Camera.current.farClipPlane)));
		_material.SetVectorArray("_FrustumCorners", _frustumCorners);
		Texture val = null;
		if ((int)_light.shadows != 0)
		{
			_material.EnableKeyword("SHADOWS_DEPTH");
			_commandBuffer.Blit(val, RenderTargetIdentifier.op_Implicit((Texture)(object)renderer.GetVolumeLightBuffer()), _material, num);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, _commandBuffer, viewProj);
			}
		}
		else
		{
			_material.DisableKeyword("SHADOWS_DEPTH");
			renderer.GlobalCommandBuffer.Blit(val, RenderTargetIdentifier.op_Implicit((Texture)(object)renderer.GetVolumeLightBuffer()), _material, num);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
			}
		}
	}

	private bool IsCameraInPointLightBounds()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)_light).transform.position - ((Component)Camera.current).transform.position;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		float num = _light.range + 1f;
		if (sqrMagnitude < num * num)
		{
			return true;
		}
		return false;
	}

	private bool IsCameraInSpotLightBounds()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Dot(((Component)_light).transform.forward, ((Component)Camera.current).transform.position - ((Component)_light).transform.position);
		float num2 = _light.range + 1f;
		if (num > num2)
		{
			return false;
		}
		Vector3 forward = ((Component)this).transform.forward;
		Vector3 val = ((Component)Camera.current).transform.position - ((Component)_light).transform.position;
		float num3 = Vector3.Dot(forward, ((Vector3)(ref val)).normalized);
		if (Mathf.Acos(num3) * 57.29578f > (_light.spotAngle + 3f) * 0.5f)
		{
			return false;
		}
		return true;
	}
}
