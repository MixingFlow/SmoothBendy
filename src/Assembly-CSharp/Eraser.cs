using System;
using UnityEngine;

[ExecuteInEditMode]
public class Eraser : Projection
{
	[SerializeField]
	private Texture2D mainTex;

	[SerializeField]
	private float multiplier = 1f;

	[SerializeField]
	private float projectionLimit = 80f;

	private Material renderMaterial;

	private int deferredPass;

	private bool[] buffers;

	private int _MainTex;

	private int _Multiplier;

	private int _NormalCutoff;

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

	public float AlphaMultiplier
	{
		get
		{
			return multiplier;
		}
		set
		{
			multiplier = Mathf.Clamp01(value);
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

	public override int DeferredPass => 1;

	public override bool DeferredPrePass => transparencyType == TransparencyType.Blend;

	protected override void GrabIds()
	{
		base.GrabIds();
		_MainTex = Shader.PropertyToID("_MainTex");
		_Multiplier = Shader.PropertyToID("_Multiplier");
		_NormalCutoff = Shader.PropertyToID("_NormalCutoff");
	}

	protected override void UpdateMaterialProperties()
	{
		base.UpdateMaterialProperties();
		UpdateShape();
		UpdateProjectionClipping();
	}

	private void UpdateShape()
	{
		if ((Object)(object)mainTex != (Object)null)
		{
			materialProperties.SetTexture(_MainTex, (Texture)(object)mainTex);
		}
		materialProperties.SetFloat(_Multiplier, multiplier * base.AlphaModifier);
	}

	private void UpdateProjectionClipping()
	{
		float num = Mathf.Cos((float)Math.PI / 180f * projectionLimit);
		materialProperties.SetFloat(_NormalCutoff, num);
	}

	protected override void UpdateDeferredRendering()
	{
		if (transparencyType == TransparencyType.Blend)
		{
			renderMaterial = DynamicDecals.System.Mat_Eraser;
		}
		else
		{
			renderMaterial = DynamicDecals.System.Mat_EraserCutout;
		}
		if (buffers == null || buffers.Length != 3)
		{
			buffers = new bool[3];
			buffers[0] = true;
			buffers[1] = true;
			buffers[2] = true;
		}
		base.DeferredBuffers = buffers;
		base.UpdateDeferredRendering();
	}

	protected override void UpdateForwardRendering(MeshRenderer Renderer)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		Material[] array = (Material[])(object)new Material[2];
		if (transparencyType == TransparencyType.Blend)
		{
			array[0] = new Material(DynamicDecals.System.Mat_Eraser);
		}
		else
		{
			array[0] = new Material(DynamicDecals.System.Mat_EraserCutout);
		}
		array[1] = new Material(DynamicDecals.System.Mat_EraserGrab);
		((Renderer)Renderer).sharedMaterials = array;
		base.UpdateForwardRendering(Renderer);
	}

	public void CopyAllProperties(Eraser Target)
	{
		if ((Object)(object)Target != (Object)null)
		{
			CopyBaseProperties(Target);
			ProjectionLimit = Target.ProjectionLimit;
			mainTex = Target.mainTex;
			multiplier = Target.multiplier;
			CopyMaskProperties(Target);
		}
		else
		{
			Debug.LogWarning((object)"No Eraser found to copy from");
		}
	}
}
