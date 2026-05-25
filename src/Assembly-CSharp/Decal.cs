using System;
using UnityEngine;

[ExecuteInEditMode]
public class Decal : Projection
{
	[SerializeField]
	private DecalType decalType;

	[SerializeField]
	private LightingModel lightingModel = LightingModel.PBR;

	[SerializeField]
	private GlossType glossType;

	[SerializeField]
	private Texture2D shapeTex;

	[SerializeField]
	private float shapeMultiplier = 1f;

	[SerializeField]
	private Texture2D albedoTex;

	[SerializeField]
	private Color albedoColor = Color.grey;

	[SerializeField]
	private Texture2D smoothnessTex;

	[SerializeField]
	private float smoothness = 0.2f;

	[SerializeField]
	private Texture2D specularTex;

	[SerializeField]
	private Color specularColor = Color.white;

	[SerializeField]
	private Texture2D metallicTex;

	[SerializeField]
	private float metallicity = 0.5f;

	[SerializeField]
	private Texture2D normalTex;

	[SerializeField]
	private float normalStrength = 1f;

	[SerializeField]
	private bool emissive;

	[SerializeField]
	private Texture2D emissionTex;

	[SerializeField]
	private Color emissionColor = Color.white;

	[SerializeField]
	private float emissionIntensity = 1f;

	[SerializeField]
	private float projectionLimit = 80f;

	private Material renderMaterial;

	private int deferredPass;

	private bool[] buffers;

	private int _Glossiness;

	private int _GlossTex;

	private int _MainTex;

	private int _Multiplier;

	private int _Color;

	private int _MetallicGlossMap;

	private int _Metallic;

	private int _SpecGlossMap;

	private int _SpecColor;

	private int _BumpMap;

	private int _BumpScale;

	private int _EmissionMap;

	private int _EmissionColor;

	private int _NormalCutoff;

	private int _BumpFlip;

	private bool flipNormals;

	public DecalType DecalType
	{
		get
		{
			return decalType;
		}
		set
		{
			decalType = value;
			ReplaceMaterial();
			UpdateMaterial();
		}
	}

	public LightingModel LightModel
	{
		get
		{
			return lightingModel;
		}
		set
		{
			lightingModel = value;
			ReplaceMaterial();
			UpdateMaterial();
		}
	}

	public GlossType GlossType
	{
		get
		{
			return glossType;
		}
		set
		{
			glossType = value;
			ReplaceMaterial();
			UpdateMaterial();
		}
	}

	public bool Emissive
	{
		get
		{
			return emissive;
		}
		set
		{
			emissive = value;
			UpdateMaterial();
		}
	}

	public Texture2D ShapeMap
	{
		get
		{
			return shapeTex;
		}
		set
		{
			shapeTex = value;
			UpdateMaterial();
		}
	}

	public float ShapeMultiplier
	{
		get
		{
			return shapeMultiplier;
		}
		set
		{
			shapeMultiplier = value;
			UpdateMaterial();
		}
	}

	public Texture2D AlbedoMap
	{
		get
		{
			return albedoTex;
		}
		set
		{
			albedoTex = value;
			UpdateMaterial();
		}
	}

	public Color AlbedoColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return albedoColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			albedoColor = value;
			UpdateMaterial();
		}
	}

	public Texture2D SmoothnessMap
	{
		get
		{
			return smoothnessTex;
		}
		set
		{
			smoothnessTex = value;
			UpdateMaterial();
		}
	}

	public float Smoothness
	{
		get
		{
			return smoothness;
		}
		set
		{
			smoothness = Mathf.Clamp01(value);
			UpdateMaterial();
		}
	}

	public Texture2D MetallicMap
	{
		get
		{
			return metallicTex;
		}
		set
		{
			metallicTex = value;
			UpdateMaterial();
		}
	}

	public float Metallicity
	{
		get
		{
			return metallicity;
		}
		set
		{
			metallicity = value;
			UpdateMaterial();
		}
	}

	public Texture2D SpecularMap
	{
		get
		{
			return specularTex;
		}
		set
		{
			specularTex = value;
			UpdateMaterial();
		}
	}

	public Color SpecularColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return specularColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			specularColor = value;
			UpdateMaterial();
		}
	}

	public Texture2D NormalMap
	{
		get
		{
			return normalTex;
		}
		set
		{
			normalTex = value;
			UpdateMaterial();
		}
	}

	public float NormalStrength
	{
		get
		{
			return normalStrength;
		}
		set
		{
			normalStrength = Mathf.Clamp(value, 0f, 4f);
			UpdateMaterial();
		}
	}

	public Texture2D EmissionMap
	{
		get
		{
			return emissionTex;
		}
		set
		{
			emissionTex = value;
			UpdateMaterial();
		}
	}

	public Color EmissionColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return emissionColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			emissionColor = value;
			UpdateMaterial();
		}
	}

	public float EmissionIntensity
	{
		get
		{
			return emissionIntensity;
		}
		set
		{
			emissionIntensity = value;
			UpdateMaterial();
		}
	}

	public float ProjectionLimit
	{
		get
		{
			return projectionLimit;
		}
		set
		{
			projectionLimit = Mathf.Clamp(value, 0f, 180f);
			UpdateMaterial();
		}
	}

	public override Material RenderMaterial => renderMaterial;

	public override int DeferredPass => deferredPass;

	public override bool DeferredPrePass
	{
		get
		{
			if (DecalType == DecalType.Roughness)
			{
				return true;
			}
			return transparencyType == TransparencyType.Blend;
		}
	}

	protected override bool RequiresRenderer => decalType == DecalType.Full;

	public override void UpdateProjection()
	{
		base.UpdateProjection();
		UpdateNormalFlip();
	}

	protected override void GrabIds()
	{
		base.GrabIds();
		_Glossiness = Shader.PropertyToID("_Glossiness");
		_GlossTex = Shader.PropertyToID("_GlossTex");
		_MainTex = Shader.PropertyToID("_MainTex");
		_Multiplier = Shader.PropertyToID("_Multiplier");
		_Color = Shader.PropertyToID("_Color");
		_MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
		_Metallic = Shader.PropertyToID("_Metallic");
		_SpecGlossMap = Shader.PropertyToID("_SpecGlossMap");
		_SpecColor = Shader.PropertyToID("_SpecColor");
		_BumpMap = Shader.PropertyToID("_BumpMap");
		_BumpScale = Shader.PropertyToID("_BumpScale");
		_EmissionMap = Shader.PropertyToID("_EmissionMap");
		_EmissionColor = Shader.PropertyToID("_EmissionColor");
		_NormalCutoff = Shader.PropertyToID("_NormalCutoff");
		_BumpFlip = Shader.PropertyToID("_BumpFlip");
	}

	protected override void UpdateMaterialProperties()
	{
		base.UpdateMaterialProperties();
		UpdateProjectionClipping();
		switch (decalType)
		{
		case DecalType.Full:
			if (lightingModel == LightingModel.PBR)
			{
				UpdateGloss();
				UpdateNormal();
				UpdateEmissive();
			}
			UpdateColor();
			break;
		case DecalType.Roughness:
			UpdateShape();
			materialProperties.SetFloat(_Glossiness, smoothness);
			if ((Object)(object)smoothnessTex != (Object)null)
			{
				materialProperties.SetTexture(_GlossTex, (Texture)(object)smoothnessTex);
			}
			break;
		case DecalType.Normal:
			UpdateShape();
			UpdateNormal();
			break;
		}
	}

	private void UpdateShape()
	{
		if ((Object)(object)shapeTex != (Object)null)
		{
			materialProperties.SetTexture(_MainTex, (Texture)(object)shapeTex);
		}
		materialProperties.SetFloat(_Multiplier, shapeMultiplier * base.AlphaModifier);
	}

	private void UpdateColor()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)albedoTex != (Object)null)
		{
			materialProperties.SetTexture(_MainTex, (Texture)(object)albedoTex);
		}
		Color val = albedoColor;
		val.a *= base.AlphaModifier;
		materialProperties.SetColor(_Color, val);
	}

	private void UpdateGloss()
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		materialProperties.SetFloat(_Glossiness, smoothness);
		switch (GlossType)
		{
		case GlossType.Metallic:
			if ((Object)(object)metallicTex != (Object)null)
			{
				materialProperties.SetTexture(_MetallicGlossMap, (Texture)(object)metallicTex);
			}
			else
			{
				materialProperties.SetTexture(_MetallicGlossMap, (Texture)(object)Texture2D.whiteTexture);
			}
			materialProperties.SetFloat(_Metallic, metallicity);
			break;
		case GlossType.Specular:
			if ((Object)(object)specularTex != (Object)null)
			{
				materialProperties.SetTexture(_SpecGlossMap, (Texture)(object)specularTex);
			}
			else
			{
				materialProperties.SetTexture(_SpecGlossMap, (Texture)(object)Texture2D.whiteTexture);
			}
			materialProperties.SetColor(_SpecColor, specularColor);
			break;
		}
	}

	private void UpdateNormal()
	{
		if ((Object)(object)normalTex != (Object)null)
		{
			materialProperties.SetTexture(_BumpMap, (Texture)(object)normalTex);
		}
		materialProperties.SetFloat(_BumpScale, normalStrength);
	}

	private void UpdateEmissive()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (emissive)
		{
			if ((Object)(object)emissionTex != (Object)null)
			{
				materialProperties.SetTexture(_EmissionMap, (Texture)(object)emissionTex);
			}
			materialProperties.SetColor(_EmissionColor, emissionColor * emissionIntensity);
		}
	}

	private void UpdateProjectionClipping()
	{
		float num = Mathf.Cos((float)Math.PI / 180f * projectionLimit);
		materialProperties.SetFloat(_NormalCutoff, num);
	}

	private void UpdateNormalFlip()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)this).transform;
		Vector3 localScale = transform.localScale;
		if (transform.hasChanged)
		{
			bool flag = false;
			if (Mathf.Sign(localScale.x) == -1f)
			{
				flag = !flag;
			}
			if (Mathf.Sign(localScale.y) == -1f)
			{
				flag = !flag;
			}
			if (Mathf.Sign(localScale.z) == -1f)
			{
				flag = !flag;
			}
			if (flipNormals != flag)
			{
				flipNormals = flag;
				materialProperties.SetFloat(_BumpFlip, (float)(flipNormals ? 1 : 0));
				UpdateMaterial();
			}
		}
	}

	protected override void UpdateDeferredRendering()
	{
		if (buffers == null || buffers.Length != 3)
		{
			buffers = new bool[3];
		}
		switch (decalType)
		{
		case DecalType.Full:
			if (lightingModel == LightingModel.Unlit)
			{
				if (base.TransparencyType == TransparencyType.Blend)
				{
					renderMaterial = DynamicDecals.System.Mat_Decal_Unlit;
				}
				else
				{
					renderMaterial = DynamicDecals.System.Mat_Decal_UnlitCutout;
				}
				buffers[0] = true;
				buffers[1] = true;
				buffers[2] = false;
				deferredPass = 1;
				break;
			}
			if (GlossType == GlossType.Metallic)
			{
				if (base.TransparencyType == TransparencyType.Blend)
				{
					renderMaterial = DynamicDecals.System.Mat_Decal_Metallic;
				}
				else
				{
					renderMaterial = DynamicDecals.System.Mat_Decal_MetallicCutout;
				}
			}
			else if (base.TransparencyType == TransparencyType.Blend)
			{
				renderMaterial = DynamicDecals.System.Mat_Decal_Specular;
			}
			else
			{
				renderMaterial = DynamicDecals.System.Mat_Decal_SpecularCutout;
			}
			buffers[0] = true;
			buffers[1] = true;
			buffers[2] = true;
			deferredPass = 2;
			break;
		case DecalType.Roughness:
			if (base.TransparencyType == TransparencyType.Blend)
			{
				renderMaterial = DynamicDecals.System.Mat_Decal_Roughness;
			}
			else
			{
				renderMaterial = DynamicDecals.System.Mat_Decal_RoughnessCutout;
			}
			buffers[0] = false;
			buffers[1] = true;
			buffers[2] = false;
			deferredPass = 0;
			break;
		case DecalType.Normal:
			if (base.TransparencyType == TransparencyType.Blend)
			{
				renderMaterial = DynamicDecals.System.Mat_Decal_Normal;
			}
			else
			{
				renderMaterial = DynamicDecals.System.Mat_Decal_NormalCutout;
			}
			buffers[0] = false;
			buffers[1] = false;
			buffers[2] = true;
			deferredPass = 0;
			break;
		}
		base.DeferredBuffers = buffers;
		base.UpdateDeferredRendering();
	}

	protected override void UpdateForwardRendering(MeshRenderer Renderer)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		if (((Renderer)Renderer).sharedMaterials.Length != 1)
		{
			((Renderer)Renderer).sharedMaterials = (Material[])(object)new Material[1];
		}
		if (lightingModel == LightingModel.Unlit)
		{
			if (transparencyType == TransparencyType.Blend)
			{
				((Renderer)Renderer).sharedMaterial = new Material(DynamicDecals.System.Mat_Decal_Unlit);
			}
			else
			{
				((Renderer)Renderer).sharedMaterial = new Material(DynamicDecals.System.Mat_Decal_UnlitCutout);
			}
		}
		else
		{
			switch (glossType)
			{
			case GlossType.Metallic:
				if (transparencyType == TransparencyType.Blend)
				{
					((Renderer)Renderer).sharedMaterial = new Material(DynamicDecals.System.Mat_Decal_Metallic);
				}
				else
				{
					((Renderer)Renderer).sharedMaterial = new Material(DynamicDecals.System.Mat_Decal_MetallicCutout);
				}
				break;
			case GlossType.Specular:
				if (transparencyType == TransparencyType.Blend)
				{
					((Renderer)Renderer).sharedMaterial = new Material(DynamicDecals.System.Mat_Decal_Specular);
				}
				else
				{
					((Renderer)Renderer).sharedMaterial = new Material(DynamicDecals.System.Mat_Decal_SpecularCutout);
				}
				break;
			}
		}
		base.UpdateForwardRendering(Renderer);
	}

	public void CopyAllProperties(Decal Target, bool IncludeTextures = true)
	{
		if ((Object)(object)Target != (Object)null)
		{
			CopyBaseProperties(Target);
			ProjectionLimit = Target.ProjectionLimit;
			DecalType = Target.DecalType;
			LightModel = Target.LightModel;
			if (IncludeTextures)
			{
				ShapeMap = Target.ShapeMap;
			}
			ShapeMultiplier = Target.ShapeMultiplier;
			CopyAlbedoProperties(Target, IncludeTextures);
			CopyGlossProperties(Target, IncludeTextures);
			CopyNormalProperties(Target, IncludeTextures);
			CopyEmissiveProperties(Target, IncludeTextures);
			CopyMaskProperties(Target);
		}
		else
		{
			Debug.LogWarning((object)"No Decal found to copy from");
		}
	}

	public void CopyAlbedoProperties(Decal Target, bool IncludeTextures = true)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (IncludeTextures)
		{
			AlbedoMap = Target.AlbedoMap;
		}
		AlbedoColor = Target.AlbedoColor;
	}

	public void CopyGlossProperties(Decal Target, bool IncludeTextures = true)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Smoothness = Target.Smoothness;
		GlossType = Target.GlossType;
		if (IncludeTextures)
		{
			MetallicMap = Target.MetallicMap;
		}
		if (IncludeTextures)
		{
			MetallicMap = Target.MetallicMap;
		}
		Metallicity = Target.Metallicity;
		if (IncludeTextures)
		{
			SpecularMap = Target.SpecularMap;
		}
		SpecularColor = Target.SpecularColor;
	}

	public void CopyNormalProperties(Decal Target, bool IncludeTextures = true)
	{
		if (IncludeTextures)
		{
			NormalMap = Target.NormalMap;
		}
		NormalStrength = Target.NormalStrength;
	}

	public void CopyEmissiveProperties(Decal Target, bool IncludeTextures = true)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Emissive = Target.Emissive;
		if (Emissive)
		{
			if (IncludeTextures)
			{
				EmissionMap = Target.EmissionMap;
			}
			EmissionColor = Target.EmissionColor;
			EmissionIntensity = Target.EmissionIntensity;
		}
	}
}
