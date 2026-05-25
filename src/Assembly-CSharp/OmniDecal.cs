using UnityEngine;

[ExecuteInEditMode]
public class OmniDecal : Projection
{
	[SerializeField]
	public Texture2D mainTex;

	[SerializeField]
	public Color color = Color.white;

	private bool[] buffers = new bool[4] { true, true, false, true };

	private int _MainTex;

	private int _Color;

	public Texture2D MainTex
	{
		get
		{
			return mainTex;
		}
		set
		{
			mainTex = value;
			UpdateMaterial();
		}
	}

	public Color Color
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return color;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			color = value;
			UpdateMaterial();
		}
	}

	public override Material RenderMaterial => (transparencyType != TransparencyType.Blend) ? DynamicDecals.System.Mat_OmniDecalCutout : DynamicDecals.System.Mat_OmniDecal;

	public override int DeferredPass => 1;

	public override bool DeferredPrePass => transparencyType == TransparencyType.Blend;

	protected override void GrabIds()
	{
		base.GrabIds();
		_MainTex = Shader.PropertyToID("_MainTex");
		_Color = Shader.PropertyToID("_Color");
	}

	protected override void UpdateMaterialProperties()
	{
		base.UpdateMaterialProperties();
		UpdateColor();
	}

	private void UpdateColor()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mainTex != (Object)null)
		{
			materialProperties.SetTexture(_MainTex, (Texture)(object)mainTex);
		}
		Color val = color;
		val.a *= base.AlphaModifier;
		materialProperties.SetColor(_Color, val);
	}

	protected override void UpdateDeferredRendering()
	{
		base.DeferredBuffers = buffers;
		base.UpdateDeferredRendering();
	}

	protected override void UpdateForwardRendering(MeshRenderer Renderer)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		if (((Renderer)Renderer).sharedMaterials.Length != 1)
		{
			((Renderer)Renderer).sharedMaterials = (Material[])(object)new Material[1];
		}
		if (transparencyType == TransparencyType.Blend)
		{
			((Renderer)Renderer).sharedMaterial = new Material(DynamicDecals.System.Mat_OmniDecal);
		}
		else
		{
			((Renderer)Renderer).sharedMaterial = new Material(DynamicDecals.System.Mat_OmniDecalCutout);
		}
		base.UpdateForwardRendering(Renderer);
	}

	public void CopyAllProperties(OmniDecal Target, bool IncludeTextures = true)
	{
		if ((Object)(object)Target != (Object)null)
		{
			CopyBaseProperties(Target);
			CopyProperties(Target, IncludeTextures);
			CopyMaskProperties(Target);
		}
		else
		{
			Debug.LogWarning((object)"No Decal found to copy from");
		}
	}

	public void CopyProperties(OmniDecal Target, bool IncludeTextures = true)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (IncludeTextures)
		{
			MainTex = Target.MainTex;
		}
		Color = Target.Color;
	}
}
