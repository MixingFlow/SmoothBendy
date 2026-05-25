using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public abstract class Projection : MonoBehaviour
{
	[SerializeField]
	private int priority;

	[SerializeField]
	protected TransparencyType transparencyType;

	[SerializeField]
	private float cutoff = 0.2f;

	[SerializeField]
	private MaskMethod maskMethod;

	[SerializeField]
	private bool[] masks = new bool[4];

	private Color MaskLayers;

	private PoolItem poolItem;

	private float scaleModifier = 1f;

	private float alphaModifier = 1f;

	private Visibility visibility;

	protected Transform forwardRenderer;

	private RenderTargetIdentifier[] deferredTargets;

	private RenderTargetIdentifier[] deferredHDRTargets;

	private int _Cutoff;

	private int _MaskBase;

	private int _MaskLayers;

	private bool replaceDeferred = true;

	private bool replaceForward = true;

	private bool materialUpdated;

	private bool materialApplied;

	protected MaterialPropertyBlock materialProperties;

	public int Priority
	{
		get
		{
			return priority;
		}
		set
		{
			priority = value;
			Reprioritise();
		}
	}

	public TransparencyType TransparencyType
	{
		get
		{
			return transparencyType;
		}
		set
		{
			this.transparencyType = value;
			TransparencyType transparencyType = this.transparencyType;
			if (transparencyType != TransparencyType.Blend && transparencyType == TransparencyType.Cutout)
			{
				cutoff = 0.2f;
			}
			ReplaceMaterial();
			UpdateMaterial();
		}
	}

	public float AlphaCutoff
	{
		get
		{
			return cutoff;
		}
		set
		{
			cutoff = Mathf.Clamp01(value);
			UpdateMaterial();
		}
	}

	public MaskMethod MaskMethod
	{
		get
		{
			return maskMethod;
		}
		set
		{
			maskMethod = value;
			UpdateMaterial();
		}
	}

	public bool MaskLayer1
	{
		get
		{
			return masks[0];
		}
		set
		{
			masks[0] = value;
			UpdateMaterial();
		}
	}

	public bool MaskLayer2
	{
		get
		{
			return masks[1];
		}
		set
		{
			masks[1] = value;
			UpdateMaterial();
		}
	}

	public bool MaskLayer3
	{
		get
		{
			return masks[2];
		}
		set
		{
			masks[2] = value;
			UpdateMaterial();
		}
	}

	public bool MaskLayer4
	{
		get
		{
			return masks[3];
		}
		set
		{
			masks[3] = value;
			UpdateMaterial();
		}
	}

	public PoolItem PoolItem
	{
		get
		{
			return poolItem;
		}
		set
		{
			poolItem = value;
		}
	}

	public float ScaleModifier
	{
		get
		{
			return scaleModifier;
		}
		set
		{
			scaleModifier = value;
		}
	}

	public float AlphaModifier
	{
		get
		{
			return alphaModifier;
		}
		set
		{
			alphaModifier = Mathf.Clamp01(value);
			UpdateMaterial();
		}
	}

	public bool Visible
	{
		get
		{
			if (Object.op_Implicit((Object)(object)this) && DynamicDecals.System.SystemPath != SystemPath.Deferred)
			{
				MeshRenderer component = ((Component)this).GetComponent<MeshRenderer>();
				if ((Object)(object)component != (Object)null)
				{
					return ((Renderer)component).isVisible;
				}
				return true;
			}
			return visibility switch
			{
				Visibility.Visible => true, 
				Visibility.NotVisible => false, 
				_ => true, 
			};
		}
	}

	public Matrix4x4 RenderMatrix
	{
		get
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (scaleModifier == 1f)
			{
				return ((Component)this).transform.localToWorldMatrix;
			}
			return ((Component)this).transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(scaleModifier, scaleModifier, scaleModifier));
		}
	}

	public abstract Material RenderMaterial { get; }

	public abstract int DeferredPass { get; }

	public abstract bool DeferredPrePass { get; }

	public bool[] DeferredBuffers { get; set; }

	public RenderTargetIdentifier[] DeferredTargets
	{
		get
		{
			if (deferredTargets == null || deferredTargets.Length < 1)
			{
				deferredTargets = (RenderTargetIdentifier[])DynamicDecals.System.PassesToTargets(DeferredBuffers, HDR: false).Clone();
			}
			return deferredTargets;
		}
		set
		{
			deferredTargets = value;
		}
	}

	public RenderTargetIdentifier[] DeferredHDRTargets
	{
		get
		{
			if (deferredHDRTargets == null || deferredHDRTargets.Length < 1)
			{
				deferredHDRTargets = (RenderTargetIdentifier[])DynamicDecals.System.PassesToTargets(DeferredBuffers, HDR: true).Clone();
			}
			return deferredHDRTargets;
		}
		set
		{
			deferredHDRTargets = value;
		}
	}

	public float timeID { get; private set; }

	public MaterialPropertyBlock MaterialProperties
	{
		get
		{
			if (!materialUpdated)
			{
				UpdateMaterialProperties();
			}
			materialUpdated = true;
			return materialProperties;
		}
	}

	protected virtual bool RequiresRenderer => true;

	protected virtual void GrabIds()
	{
		_Cutoff = Shader.PropertyToID("_Cutoff");
		_MaskBase = Shader.PropertyToID("_MaskBase");
		_MaskLayers = Shader.PropertyToID("_MaskLayers");
	}

	private void Start()
	{
		Initialize();
	}

	private void OnEnable()
	{
		if (!Application.isPlaying)
		{
			Initialize();
		}
	}

	private void OnDisable()
	{
		if (!Application.isPlaying)
		{
			DestroyRenderer(ForceDestroy: false);
			Deregister();
		}
	}

	private void Initialize()
	{
		GrabIds();
		UpdateMaterialImmeditately();
		UpdateRenderer();
		Register();
	}

	private void Register()
	{
		timeID = Time.timeSinceLevelLoad;
		DynamicDecals.System.AddProjection(this);
	}

	private void Deregister()
	{
		DynamicDecals.System.RemoveProjection(this);
	}

	public void Reprioritise()
	{
		Reprioritise(DelayedSort: false);
	}

	public void Reprioritise(bool DelayedSort)
	{
		if (DelayedSort)
		{
			DynamicDecals.System.Sort();
			return;
		}
		Deregister();
		Register();
	}

	public virtual void UpdateProjection()
	{
		if (((Behaviour)this).enabled)
		{
			visibility = Visibility.Unknown;
			if (DynamicDecals.System.SystemPath == SystemPath.Forward && RequiresRenderer)
			{
				UpdateRenderer();
			}
			else
			{
				DestroyRenderer();
			}
		}
	}

	public void ReplaceMaterial()
	{
		replaceDeferred = true;
		replaceForward = true;
	}

	public void UpdateMaterialImmeditately()
	{
		UpdateMaterialProperties();
		materialUpdated = true;
		materialApplied = false;
	}

	public void UpdateMaterial()
	{
		materialUpdated = false;
		materialApplied = false;
	}

	protected virtual void UpdateMaterialProperties()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		if (materialProperties == null)
		{
			materialProperties = new MaterialPropertyBlock();
		}
		else
		{
			materialProperties.Clear();
		}
		UpdateTransparency();
		UpdateMasking();
		if (replaceDeferred)
		{
			UpdateDeferredRendering();
			replaceDeferred = false;
		}
	}

	private void UpdateTransparency()
	{
		materialProperties.SetFloat(_Cutoff, cutoff);
	}

	private void UpdateMasking()
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		switch (maskMethod)
		{
		case MaskMethod.DrawOnEverythingExcept:
			materialProperties.SetFloat(_MaskBase, 1f);
			MaskLayers.r = ((!masks[0]) ? 0.5f : 0f);
			MaskLayers.g = ((!masks[1]) ? 0.5f : 0f);
			MaskLayers.b = ((!masks[2]) ? 0.5f : 0f);
			MaskLayers.a = ((!masks[3]) ? 0.5f : 0f);
			materialProperties.SetVector(_MaskLayers, Color.op_Implicit(MaskLayers));
			break;
		case MaskMethod.OnlyDrawOn:
			materialProperties.SetFloat(_MaskBase, 0f);
			MaskLayers.r = ((!masks[0]) ? 0.5f : 1f);
			MaskLayers.g = ((!masks[1]) ? 0.5f : 1f);
			MaskLayers.b = ((!masks[2]) ? 0.5f : 1f);
			MaskLayers.a = ((!masks[3]) ? 0.5f : 1f);
			materialProperties.SetVector(_MaskLayers, Color.op_Implicit(MaskLayers));
			break;
		}
	}

	protected virtual void UpdateDeferredRendering()
	{
		DeferredTargets = (RenderTargetIdentifier[])DynamicDecals.System.PassesToTargets(DeferredBuffers, HDR: false).Clone();
		DeferredHDRTargets = (RenderTargetIdentifier[])DynamicDecals.System.PassesToTargets(DeferredBuffers, HDR: true).Clone();
	}

	protected virtual void UpdateForwardRendering(MeshRenderer Renderer)
	{
		((Renderer)Renderer).sharedMaterial.renderQueue = 2455 + Priority;
	}

	private void UpdateRenderer()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		MeshRenderer val = null;
		if (!Object.op_Implicit((Object)(object)forwardRenderer))
		{
			foreach (Transform item in ((Component)this).transform)
			{
				Transform val2 = item;
				if (((Object)val2).name == "Forward Renderer")
				{
					forwardRenderer = val2;
				}
			}
			if (!Object.op_Implicit((Object)(object)forwardRenderer))
			{
				forwardRenderer = new GameObject("Forward Renderer").transform;
				forwardRenderer.SetParent(((Component)this).transform, false);
				((Component)forwardRenderer).gameObject.layer = ((Component)this).gameObject.layer;
				((Object)((Component)forwardRenderer).gameObject).hideFlags = (HideFlags)61;
				MeshFilter val3 = ((Component)forwardRenderer).gameObject.AddComponent<MeshFilter>();
				val3.mesh = DynamicDecals.System.Cube;
				val = ((Component)forwardRenderer).gameObject.AddComponent<MeshRenderer>();
				((Renderer)val).shadowCastingMode = (ShadowCastingMode)0;
				UpdateMaterial();
			}
		}
		if ((Object)(object)val == (Object)null)
		{
			val = ((Component)forwardRenderer).GetComponent<MeshRenderer>();
		}
		if (replaceForward || (Object)(object)((Renderer)val).sharedMaterial == (Object)null)
		{
			DestroyMaterials(val);
			UpdateForwardRendering(val);
			replaceForward = false;
		}
		if (!materialApplied)
		{
			((Renderer)val).SetPropertyBlock(MaterialProperties);
			materialApplied = true;
		}
		float num = Mathf.Clamp(scaleModifier, 1E-08f, 1E+09f);
		forwardRenderer.localScale = new Vector3(num, num, num);
	}

	private void DestroyRenderer(bool ForceDestroy = true)
	{
		if (Object.op_Implicit((Object)(object)forwardRenderer) && Application.isPlaying)
		{
			DestroyMaterials(((Component)forwardRenderer).GetComponent<MeshRenderer>());
			Object.Destroy((Object)(object)((Component)forwardRenderer).gameObject);
		}
	}

	private void DestroyMaterials(MeshRenderer Renderer)
	{
		if (((Renderer)Renderer).sharedMaterials == null || ((Renderer)Renderer).sharedMaterials.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < ((Renderer)Renderer).sharedMaterials.Length; i++)
		{
			if (Application.isPlaying)
			{
				Object.Destroy((Object)(object)((Renderer)Renderer).sharedMaterials[i]);
			}
			else
			{
				Object.DestroyImmediate((Object)(object)((Renderer)Renderer).sharedMaterials[i], true);
			}
		}
	}

	public void SetVisibility(bool Visible)
	{
		if (!Visible && visibility == Visibility.Unknown)
		{
			visibility = Visibility.NotVisible;
		}
		if (Visible)
		{
			visibility = Visibility.Visible;
		}
	}

	public void Fade(FadeMethod Method, float InDuration, float Delay, float OutDuration)
	{
		if (poolItem != null)
		{
			poolItem.Fade(Method, InDuration, Delay, OutDuration);
			float num = 1f;
			num = ((!(InDuration > 0f)) ? 1f : 0f);
			if (Method == FadeMethod.Alpha || Method == FadeMethod.Both)
			{
				AlphaModifier = num;
			}
			if (Method == FadeMethod.Scale || Method == FadeMethod.Both)
			{
				ScaleModifier = num;
			}
			UpdateMaterialImmeditately();
		}
	}

	public void Culled(CullMethod Method, float Duration)
	{
		if (poolItem != null)
		{
			poolItem.Culled(Method, Duration);
		}
	}

	public void Return()
	{
		if (poolItem != null)
		{
			poolItem.Return();
		}
	}

	public float CheckIntersecting(Vector3 Point)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.InverseTransformPoint(Point);
		return Mathf.Clamp01(2f * (0.5f - Mathf.Max(new float[3]
		{
			Mathf.Abs(val.x),
			Mathf.Abs(val.y),
			Mathf.Abs(val.z)
		})));
	}

	public void CopyBaseProperties(Projection Target)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Priority = Target.Priority;
		((Component)this).transform.localScale = ((Component)Target).transform.localScale;
		TransparencyType = Target.TransparencyType;
		AlphaCutoff = Target.AlphaCutoff;
	}

	public void CopyMaskProperties(Projection Target)
	{
		MaskMethod = Target.MaskMethod;
		MaskLayer1 = Target.MaskLayer1;
		MaskLayer2 = Target.MaskLayer2;
		MaskLayer3 = Target.MaskLayer3;
		MaskLayer4 = Target.MaskLayer4;
	}
}
