using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class DynamicDecals : MonoBehaviour
{
	private static DynamicDecals system;

	public static List<Camera> validCameras = new List<Camera>();

	public SystemPath renderingPath;

	private DynamicDecalSettings settings;

	private bool sceneFocus;

	private bool cameraClipping;

	private Shader normalShader;

	private Material deferredBlit;

	private Material metallic;

	private Material metallicCutout;

	private Material specular;

	private Material specularCutout;

	private Material unlit;

	private Material unlitCutout;

	private Material roughness;

	private Material roughnessCutout;

	private Material normal;

	private Material normalCutout;

	private Material omnidecal;

	private Material omnidecalCutout;

	private Material eraser;

	private Material eraserCutout;

	private Material eraserGrab;

	private Mesh cube;

	private Mesh cameraBlit;

	private bool sortingProjections;

	private bool sort;

	private List<Projection> projections;

	private BoundingSphere[] projectionSpheres;

	private int staticCount;

	private List<ProjectionMask> masks;

	private BoundingSphere[] maskSpheres;

	private Dictionary<MaskValue, Material> MaskMaterials;

	internal Dictionary<Camera, CameraData> cameraData = new Dictionary<Camera, CameraData>();

	private Camera customCamera;

	public Rect FullRect = new Rect(0f, 0f, 1f, 1f);

	private Dictionary<int, ProjectionPool> Pools;

	private bool initialized;

	private RenderTargetIdentifier[] one = (RenderTargetIdentifier[])(object)new RenderTargetIdentifier[1];

	private RenderTargetIdentifier[] two = (RenderTargetIdentifier[])(object)new RenderTargetIdentifier[2];

	private RenderTargetIdentifier[] three = (RenderTargetIdentifier[])(object)new RenderTargetIdentifier[3];

	private RenderTargetIdentifier[] four = (RenderTargetIdentifier[])(object)new RenderTargetIdentifier[4];

	private int[] staticRts;

	private int[] dynamicRts;

	public static DynamicDecals System
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			if ((Object)(object)system == (Object)null)
			{
				GameObject val = new GameObject("Dynamic Decals");
				((Object)val).hideFlags = (HideFlags)23;
				val.AddComponent<DynamicDecals>();
			}
			return system;
		}
	}

	public SystemPath SystemPath => (!Settings.forceForward) ? renderingPath : SystemPath.Forward;

	public DynamicDecalSettings Settings
	{
		get
		{
			if ((Object)(object)settings == (Object)null)
			{
				settings = Resources.Load<DynamicDecalSettings>("Settings");
			}
			if ((Object)(object)settings == (Object)null)
			{
				settings = ScriptableObject.CreateInstance<DynamicDecalSettings>();
			}
			return settings;
		}
	}

	public Shader NormalShader
	{
		get
		{
			if ((Object)(object)normalShader == (Object)null)
			{
				normalShader = Shader.Find("Decal/Internal/DepthTexture/Normal");
			}
			return normalShader;
		}
	}

	public Material Mat_DeferredBlit
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			if ((Object)(object)deferredBlit == (Object)null)
			{
				deferredBlit = new Material(Shader.Find("Decal/Internal/Blit"));
			}
			return deferredBlit;
		}
	}

	public Material Mat_Decal_Metallic
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			if ((Object)(object)metallic == (Object)null)
			{
				Shader val = Shader.Find("Decal/Metallic");
				if (Settings.forceForward)
				{
					val.maximumLOD = 0;
				}
				else
				{
					val.maximumLOD = 1000;
				}
				metallic = new Material(val);
				metallic.DisableKeyword("_AlphaTest");
				metallic.EnableKeyword("_Blend");
			}
			return metallic;
		}
	}

	public Material Mat_Decal_MetallicCutout
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			if ((Object)(object)metallicCutout == (Object)null)
			{
				Shader val = Shader.Find("Decal/Metallic");
				if (Settings.forceForward)
				{
					val.maximumLOD = 0;
				}
				else
				{
					val.maximumLOD = 1000;
				}
				metallicCutout = new Material(val);
				metallicCutout.EnableKeyword("_AlphaTest");
				metallicCutout.DisableKeyword("_Blend");
			}
			return metallicCutout;
		}
	}

	public Material Mat_Decal_Specular
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			if ((Object)(object)specular == (Object)null)
			{
				Shader val = Shader.Find("Decal/Specular");
				if (Settings.forceForward)
				{
					val.maximumLOD = 0;
				}
				else
				{
					val.maximumLOD = 1000;
				}
				specular = new Material(val);
				specular.DisableKeyword("_AlphaTest");
				specular.EnableKeyword("_Blend");
			}
			return specular;
		}
	}

	public Material Mat_Decal_SpecularCutout
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			if ((Object)(object)specularCutout == (Object)null)
			{
				Shader val = Shader.Find("Decal/Specular");
				if (Settings.forceForward)
				{
					val.maximumLOD = 0;
				}
				else
				{
					val.maximumLOD = 1000;
				}
				specularCutout = new Material(val);
				specularCutout.EnableKeyword("_AlphaTest");
				specularCutout.DisableKeyword("_Blend");
			}
			return specularCutout;
		}
	}

	public Material Mat_Decal_Unlit
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			if ((Object)(object)unlit == (Object)null)
			{
				Shader val = Shader.Find("Decal/Unlit");
				if (Settings.forceForward)
				{
					val.maximumLOD = 0;
				}
				else
				{
					val.maximumLOD = 1000;
				}
				unlit = new Material(val);
				unlit.DisableKeyword("_AlphaTest");
				unlit.EnableKeyword("_Blend");
			}
			return unlit;
		}
	}

	public Material Mat_Decal_UnlitCutout
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			if ((Object)(object)unlitCutout == (Object)null)
			{
				Shader val = Shader.Find("Decal/Unlit");
				if (Settings.forceForward)
				{
					val.maximumLOD = 0;
				}
				else
				{
					val.maximumLOD = 1000;
				}
				unlitCutout = new Material(val);
				unlitCutout.EnableKeyword("_AlphaTest");
				unlitCutout.DisableKeyword("_Blend");
			}
			return unlitCutout;
		}
	}

	public Material Mat_Decal_Roughness
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			if ((Object)(object)roughness == (Object)null)
			{
				roughness = new Material(Shader.Find("Decal/Roughness"));
				roughness.DisableKeyword("_AlphaTest");
				roughness.EnableKeyword("_Blend");
			}
			return roughness;
		}
	}

	public Material Mat_Decal_RoughnessCutout
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			if ((Object)(object)roughnessCutout == (Object)null)
			{
				roughnessCutout = new Material(Shader.Find("Decal/Roughness"));
				roughnessCutout.EnableKeyword("_AlphaTest");
				roughnessCutout.DisableKeyword("_Blend");
			}
			return roughnessCutout;
		}
	}

	public Material Mat_Decal_Normal
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			if ((Object)(object)normal == (Object)null)
			{
				normal = new Material(Shader.Find("Decal/Normal"));
				normal.DisableKeyword("_AlphaTest");
				normal.EnableKeyword("_Blend");
			}
			return normal;
		}
	}

	public Material Mat_Decal_NormalCutout
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			if ((Object)(object)normalCutout == (Object)null)
			{
				normalCutout = new Material(Shader.Find("Decal/Normal"));
				normalCutout.EnableKeyword("_AlphaTest");
				normalCutout.DisableKeyword("_Blend");
			}
			return normalCutout;
		}
	}

	public Material Mat_OmniDecal
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			if ((Object)(object)omnidecal == (Object)null)
			{
				Shader val = Shader.Find("Decal/OmniDecal");
				if (Settings.forceForward)
				{
					val.maximumLOD = 0;
				}
				else
				{
					val.maximumLOD = 1000;
				}
				omnidecal = new Material(val);
				omnidecal.DisableKeyword("_AlphaTest");
				omnidecal.EnableKeyword("_Blend");
			}
			return omnidecal;
		}
	}

	public Material Mat_OmniDecalCutout
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			if ((Object)(object)omnidecalCutout == (Object)null)
			{
				Shader val = Shader.Find("Decal/OmniDecal");
				if (Settings.forceForward)
				{
					val.maximumLOD = 0;
				}
				else
				{
					val.maximumLOD = 1000;
				}
				omnidecalCutout = new Material(val);
				omnidecalCutout.EnableKeyword("_AlphaTest");
				omnidecalCutout.DisableKeyword("_Blend");
			}
			return omnidecalCutout;
		}
	}

	public Material Mat_Eraser
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			if ((Object)(object)eraser == (Object)null)
			{
				Shader val = Shader.Find("Decal/Eraser/Read");
				if (Settings.forceForward)
				{
					val.maximumLOD = 0;
				}
				else
				{
					val.maximumLOD = 1000;
				}
				eraser = new Material(val);
				eraser.DisableKeyword("_AlphaTest");
				eraser.EnableKeyword("_Blend");
			}
			return eraser;
		}
	}

	public Material Mat_EraserCutout
	{
		get
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			if ((Object)(object)eraserCutout == (Object)null)
			{
				Shader val = Shader.Find("Decal/Eraser/Read");
				if (Settings.forceForward)
				{
					val.maximumLOD = 0;
				}
				else
				{
					val.maximumLOD = 1000;
				}
				eraserCutout = new Material(val);
				eraserCutout.EnableKeyword("_AlphaTest");
				eraserCutout.DisableKeyword("_Blend");
			}
			return eraserCutout;
		}
	}

	public Material Mat_EraserGrab
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			if ((Object)(object)eraserGrab == (Object)null)
			{
				eraserGrab = new Material(Shader.Find("Decal/Eraser/Write"));
			}
			return eraserGrab;
		}
	}

	public Mesh Cube
	{
		get
		{
			if ((Object)(object)cube == (Object)null)
			{
				cube = Resources.Load<Mesh>("Decal");
			}
			return cube;
		}
	}

	public Mesh CameraBlit
	{
		get
		{
			if ((Object)(object)cameraBlit == (Object)null)
			{
				cameraBlit = Object.Instantiate<Mesh>(Cube);
				ScaleMesh(cameraBlit, 100f);
			}
			return cameraBlit;
		}
	}

	public bool StaticPass => staticCount > 0;

	private Camera CustomCamera
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if ((Object)(object)customCamera == (Object)null)
			{
				GameObject val = new GameObject("Custom Camera");
				customCamera = val.AddComponent<Camera>();
				val.AddComponent<ProjectionBlocker>();
				((Object)((Component)customCamera).gameObject).hideFlags = (HideFlags)61;
				((Behaviour)customCamera).enabled = false;
			}
			return customCamera;
		}
	}

	private void UpdateRenderingPath()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Invalid comparison between Unknown and I4
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		Camera val = null;
		if ((Object)(object)Camera.main != (Object)null)
		{
			val = Camera.main;
		}
		else if ((Object)(object)Camera.current != (Object)null)
		{
			val = Camera.current;
		}
		if (!((Object)(object)val != (Object)null))
		{
			return;
		}
		if ((int)val.actualRenderingPath == 1 || (int)val.actualRenderingPath == 3)
		{
			RenderingPath actualRenderingPath = val.actualRenderingPath;
			if ((int)actualRenderingPath != 1)
			{
				if ((int)actualRenderingPath == 3)
				{
					renderingPath = SystemPath.Deferred;
				}
			}
			else
			{
				renderingPath = SystemPath.Forward;
			}
		}
		else
		{
			Debug.LogWarning((object)"Current Rendering Path not supported! Please use either Forward or Deferred");
		}
	}

	public void RestoreDepthTextureModes()
	{
		for (int i = 0; i < cameraData.Count; i++)
		{
			Camera key = cameraData.ElementAt(i).Key;
			if ((Object)(object)key != (Object)null)
			{
				cameraData.ElementAt(i).Value.RestoreDepthTextureMode(key);
			}
		}
	}

	public void UpdateLODs()
	{
		Shader.Find("Decal/Metallic").maximumLOD = ((!Settings.forceForward) ? 1000 : 0);
		Shader.Find("Decal/Specular").maximumLOD = ((!Settings.forceForward) ? 1000 : 0);
		Shader.Find("Decal/Unlit").maximumLOD = ((!Settings.forceForward) ? 1000 : 0);
		Shader.Find("Decal/OmniDecal").maximumLOD = ((!Settings.forceForward) ? 1000 : 0);
		Shader.Find("Decal/Eraser/Read").maximumLOD = ((!Settings.forceForward) ? 1000 : 0);
	}

	private void ScaleMesh(Mesh Mesh, float Scale)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[Mesh.vertices.Length];
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 val = Mesh.vertices[i];
			val.x *= Scale;
			val.y *= Scale;
			val.z *= Scale;
			array[i] = val;
		}
		Mesh.vertices = array;
		Mesh.RecalculateBounds();
	}

	public void AddProjection(Projection Projection)
	{
		Initialize();
		if (projections == null)
		{
			projections = new List<Projection>();
		}
		projections.Add(Projection);
		if (!sortingProjections)
		{
			((MonoBehaviour)this).StartCoroutine(SortProjections());
		}
		if (((object)Projection).GetType() == typeof(Eraser))
		{
			staticCount++;
		}
	}

	public void RemoveProjection(Projection Projection)
	{
		if (projections == null)
		{
			return;
		}
		if ((Object)(object)Projection == (Object)null)
		{
			for (int num = projections.Count - 1; num >= 0; num--)
			{
				if ((Object)(object)projections[num] == (Object)null)
				{
					projections.RemoveAt(num);
				}
			}
			return;
		}
		if (projections.Remove(Projection) && ((object)Projection).GetType() == typeof(Eraser))
		{
			staticCount = Mathf.Clamp(staticCount - 1, 0, 10000000);
		}
		if (projections.Count == 0)
		{
			if (Application.isPlaying)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
			else
			{
				Object.DestroyImmediate((Object)(object)((Component)this).gameObject, true);
			}
		}
	}

	private IEnumerator SortProjections()
	{
		sortingProjections = true;
		yield return (object)new WaitForEndOfFrame();
		if (projections != null)
		{
			projections = (from x in projections.Distinct()
				orderby x.Priority
				select x).ToList();
		}
		sortingProjections = false;
	}

	public void Sort()
	{
		sort = true;
	}

	private void ReorderProjections()
	{
		if (!sort || renderingPath != SystemPath.Deferred)
		{
			return;
		}
		projections.Sort(delegate(Projection x, Projection y)
		{
			if (x.Priority > y.Priority)
			{
				return 1;
			}
			if (x.Priority < y.Priority)
			{
				return -1;
			}
			if (x.timeID > y.timeID)
			{
				return 1;
			}
			return (x.timeID < y.timeID) ? (-1) : 0;
		});
		sort = false;
	}

	private void UpdateProjections()
	{
		if (projections == null)
		{
			return;
		}
		for (int i = 0; i < projections.Count; i++)
		{
			if (Object.op_Implicit((Object)(object)projections[i]))
			{
				projections[i].UpdateProjection();
			}
		}
	}

	public void AddMask(ProjectionMask Mask)
	{
		if (masks == null)
		{
			masks = new List<ProjectionMask>();
		}
		if (!masks.Contains(Mask))
		{
			masks.Add(Mask);
		}
	}

	public void RemoveMask(ProjectionMask Mask)
	{
		if (masks == null)
		{
			masks = new List<ProjectionMask>();
		}
		masks.Remove(Mask);
	}

	public Material GetMaskMaterial(MaskValue Value)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		if (MaskMaterials == null)
		{
			MaskMaterials = new Dictionary<MaskValue, Material>();
		}
		if (!MaskMaterials.TryGetValue(Value, out var value))
		{
			value = new Material(Shader.Find("Decal/Internal/Mask"));
			value.SetFloat("_Layer1", (float)(Value.layer1 ? 1 : 0));
			value.SetFloat("_Layer2", (float)(Value.layer2 ? 1 : 0));
			value.SetFloat("_Layer3", (float)(Value.layer3 ? 1 : 0));
			value.SetFloat("_Layer4", (float)(Value.layer4 ? 1 : 0));
		}
		return value;
	}

	private void ClearMaskMaterials()
	{
		foreach (Material value in MaskMaterials.Values)
		{
			if (Application.isPlaying)
			{
				Object.Destroy((Object)(object)value);
			}
			else
			{
				Object.DestroyImmediate((Object)(object)value, true);
			}
		}
		MaskMaterials.Clear();
	}

	internal CameraData GetData(Camera Camera)
	{
		CameraData value = null;
		if (!cameraData.TryGetValue(Camera, out value))
		{
			value = new CameraData(Camera);
			cameraData[Camera] = value;
		}
		if (value != null)
		{
			if (!value.enabled && (Object)(object)((Component)Camera).GetComponent<ProjectionBlocker>() == (Object)null)
			{
				value.Initialize(Camera, this);
			}
			else if (value.enabled && (Object)(object)((Component)Camera).GetComponent<ProjectionBlocker>() != (Object)null)
			{
				value.Terminate(Camera);
			}
		}
		return value;
	}

	internal ProjectionPool PoolFromInstance(PoolInstance Instance)
	{
		if (Pools == null)
		{
			Pools = new Dictionary<int, ProjectionPool>();
		}
		if (!Pools.TryGetValue(Instance.id, out var value))
		{
			value = new ProjectionPool(Instance);
			Pools.Add(Instance.id, value);
		}
		return value;
	}

	public ProjectionPool GetPool(string Title)
	{
		for (int i = 0; i < Settings.pools.Length; i++)
		{
			if (settings.pools[i].title == Title)
			{
				return PoolFromInstance(settings.pools[i]);
			}
		}
		Debug.LogWarning((object)("No valid pool with the title : " + Title + " found. Returning default pool"));
		return PoolFromInstance(settings.pools[0]);
	}

	public ProjectionPool GetPool(int ID)
	{
		for (int i = 0; i < Settings.pools.Length; i++)
		{
			if (settings.pools[i].id == ID)
			{
				return PoolFromInstance(settings.pools[i]);
			}
		}
		Debug.LogWarning((object)("No valid pool with the ID : " + ID + " found. Returning default pool"));
		return PoolFromInstance(settings.pools[0]);
	}

	private void Initialize()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		if (!initialized)
		{
			Debug.Log((object)"[DynamicDecals] Initialize()", (Object)(object)this);
			Camera.onPreCull = (CameraCallback)Delegate.Combine((Delegate)(object)Camera.onPreCull, (Delegate)new CameraCallback(CullProjections));
			Camera.onPreRender = (CameraCallback)Delegate.Combine((Delegate)(object)Camera.onPreRender, (Delegate)new CameraCallback(RenderProjections));
			MaskMaterials = new Dictionary<MaskValue, Material>();
			initialized = true;
		}
	}

	private void Terminate()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		if (!initialized)
		{
			return;
		}
		Debug.Log((object)"[DynamicDecals] Terminate()", (Object)(object)this);
		Camera.onPreCull = (CameraCallback)Delegate.Remove((Delegate)(object)Camera.onPreCull, (Delegate)new CameraCallback(CullProjections));
		Camera.onPreRender = (CameraCallback)Delegate.Remove((Delegate)(object)Camera.onPreRender, (Delegate)new CameraCallback(RenderProjections));
		foreach (KeyValuePair<Camera, CameraData> cameraDatum in cameraData)
		{
			cameraDatum.Value.Terminate(cameraDatum.Key);
		}
		cameraData.Clear();
		ClearMaskMaterials();
		initialized = false;
	}

	private void OnEnable()
	{
		if ((Object)(object)system == (Object)null)
		{
			system = this;
		}
		else if ((Object)(object)system != (Object)(object)this)
		{
			if (Application.isPlaying)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
			else
			{
				Object.DestroyImmediate((Object)(object)((Component)this).gameObject, true);
			}
		}
	}

	private void OnDisable()
	{
		Debug.Log((object)"[DynamicDecals] OnDisable()", (Object)(object)this);
		Terminate();
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
	}

	private void LateUpdate()
	{
		if (Pools != null && Pools.Count > 0)
		{
			for (int i = 0; i < Pools.Count; i++)
			{
				Pools.ElementAt(i).Value.Update(Time.deltaTime);
			}
		}
		UpdateRenderingPath();
		ReorderProjections();
		UpdateProjections();
		RequestCullUpdate();
	}

	private void CullProjections(Camera Camera)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)Camera.cameraType == 1 && validCameras != null && !validCameras.Contains(Camera))
		{
			return;
		}
		CameraData data = GetData(Camera);
		if (data != null && data.enabled && (data.sceneCamera || data.previewCamera || ((Behaviour)Camera).isActiveAndEnabled))
		{
			data.UpdateRenderingMethod(Camera, this);
			if (masks != null && masks.Count > 0)
			{
				data.maskCulling.SetBoundingSpheres(maskSpheres);
			}
			if (renderingPath == SystemPath.Deferred && projections != null && projections.Count > 0)
			{
				data.projectionCulling.SetBoundingSpheres(projectionSpheres);
			}
		}
	}

	private void RenderProjections(Camera Camera)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)Camera.cameraType == 1 && validCameras != null && !validCameras.Contains(Camera))
		{
			return;
		}
		CameraData data = GetData(Camera);
		if (data != null && data.enabled && (data.sceneCamera || data.previewCamera || ((Behaviour)Camera).isActiveAndEnabled))
		{
			if (!sceneFocus && data.sceneCamera && Camera.farClipPlane > 100000f)
			{
				Debug.LogWarning((object)"Scene Camera Far Clipping Plane too far out - Projections in the scene view may appear strange or not at all. To fix this, simply focus on an object with a reasonable scale (Select then F key), or scroll in with the mouse wheel. This occurs as Unity sets its far clipping plane absurdly high when focusing large objects, or scrolling out with the mouse-wheel. As the depthbuffer is stored as a value between the near and far clipping planes, when the far clipping plane is set to high the depthbuffer becomes very inprecise. As a result, the position and Uv's of our projections also become very inprecise.");
				sceneFocus = true;
			}
			if (!cameraClipping && !data.sceneCamera && !data.previewCamera && Camera.farClipPlane > 1000000f)
			{
				Debug.LogWarning((object)"Cameras far clipping plane is too high to maintain an accurate Depth Buffer - Projections may appear strange or not at all. You'll also have a host of other issues, z-fighting among your objects etc.");
				cameraClipping = true;
			}
			UpdateMaskBuffer(Camera, data);
			UpdateProjectionBuffer(Camera, data);
			CustomNormals(Camera, data);
		}
	}

	internal void RequestCullUpdate()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		if (masks != null && masks.Count > 0)
		{
			if (maskSpheres == null || maskSpheres.Length < masks.Count)
			{
				maskSpheres = (BoundingSphere[])(object)new BoundingSphere[masks.Count * 2];
			}
			for (int i = 0; i < masks.Count; i++)
			{
				if (((Behaviour)masks[i]).enabled && masks[i].Enabled)
				{
					Bounds bounds = masks[i].Bounds;
					maskSpheres[i].position = ((Bounds)(ref bounds)).center;
					maskSpheres[i].radius = Mathf.Max(((Bounds)(ref bounds)).size.x, Mathf.Max(((Bounds)(ref bounds)).size.y, ((Bounds)(ref bounds)).size.z)) * 1.5f;
				}
			}
		}
		if (renderingPath != SystemPath.Deferred || projections == null || projections.Count <= 0)
		{
			return;
		}
		if (projectionSpheres == null || projectionSpheres.Length < projections.Count)
		{
			projectionSpheres = (BoundingSphere[])(object)new BoundingSphere[projections.Count * 2];
		}
		for (int j = 0; j < projections.Count; j++)
		{
			if (Object.op_Implicit((Object)(object)projections[j]))
			{
				Transform transform = ((Component)projections[j]).transform;
				Vector3 lossyScale = transform.lossyScale;
				projectionSpheres[j].position = transform.position;
				projectionSpheres[j].radius = Mathf.Max(lossyScale.x, Mathf.Max(lossyScale.y, lossyScale.z));
			}
		}
	}

	private void CustomNormals(Camera Camera, CameraData Data)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (Data.customDTM == CustomDepthTextureMode.Normal)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(Camera.pixelWidth, Camera.pixelHeight, 24, (RenderTextureFormat)8);
			CustomCamera.CopyFrom(Camera);
			CustomCamera.targetTexture = temporary;
			CustomCamera.renderingPath = (RenderingPath)1;
			CustomCamera.depthTextureMode = (DepthTextureMode)0;
			CustomCamera.clearFlags = (CameraClearFlags)2;
			CustomCamera.rect = FullRect;
			CustomCamera.RenderWithShader(NormalShader, "RenderType");
			temporary.SetGlobalShaderProperty("_CameraNormalTexture");
			RenderTexture.ReleaseTemporary(temporary);
			Shader.DisableKeyword("_LowPrecision");
			Shader.EnableKeyword("_HighPrecision");
		}
		else
		{
			Shader.DisableKeyword("_HighPrecision");
			Shader.EnableKeyword("_LowPrecision");
		}
	}

	private void UpdateMaskBuffer(Camera Camera, CameraData Data)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		Data.maskBuffer.Clear();
		RenderingPath actualRenderingPath = Camera.actualRenderingPath;
		if ((int)actualRenderingPath != 1)
		{
			if ((int)actualRenderingPath == 3)
			{
				DrawMasksDeferrred(Camera, Data.maskBuffer, Data.maskCulling);
			}
		}
		else
		{
			DrawMasksForward(Camera, Data.maskBuffer, Data.maskCulling);
		}
	}

	private void DrawMasksForward(Camera Camera, CommandBuffer Buffer, CullingGroup MasksCullingGroup)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		int num = Shader.PropertyToID("_MaskBuffer");
		Buffer.GetTemporaryRT(num, -1, -1);
		Buffer.SetRenderTarget(RenderTargetIdentifier.op_Implicit(num), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)1));
		Buffer.ClearRenderTarget(false, true, Color.clear);
		if (masks != null && masks.Count > 0)
		{
			for (int i = 0; i < masks.Count; i++)
			{
				try
				{
					if (MasksCullingGroup.IsVisible(i))
					{
						DrawMask(Camera, Buffer, masks[i]);
					}
				}
				catch (IndexOutOfRangeException)
				{
					DrawMask(Camera, Buffer, masks[i]);
				}
			}
		}
		Buffer.ReleaseTemporaryRT(num);
	}

	private void DrawMasksDeferrred(Camera Camera, CommandBuffer Buffer, CullingGroup MasksCullingGroup)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		int num = Shader.PropertyToID("_MaskBuffer");
		Buffer.GetTemporaryRT(num, -1, -1);
		Buffer.SetRenderTarget(RenderTargetIdentifier.op_Implicit(num), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2));
		Buffer.ClearRenderTarget(false, true, Color.clear);
		if (masks != null && masks.Count > 0)
		{
			for (int i = 0; i < masks.Count; i++)
			{
				try
				{
					if (MasksCullingGroup.IsVisible(i))
					{
						DrawMask(Camera, Buffer, masks[i]);
					}
				}
				catch (IndexOutOfRangeException)
				{
					DrawMask(Camera, Buffer, masks[i]);
				}
			}
		}
		Buffer.ReleaseTemporaryRT(num);
	}

	private void DrawMask(Camera Camera, CommandBuffer Buffer, ProjectionMask Mask)
	{
		if (Mask.Enabled)
		{
			for (int i = 0; i < Mask.SubMeshCount; i++)
			{
				Buffer.DrawRenderer(Mask.Renderer, Mask.Material, i, 0);
			}
		}
	}

	private void UpdateProjectionBuffer(Camera Camera, CameraData Data)
	{
		if (Data.method == RenderingMethod.Deferred)
		{
			Data.projectionBuffer.Clear();
			if (projections == null || projections.Count <= 0)
			{
				return;
			}
			if (staticRts == null)
			{
				staticRts = new int[4];
			}
			if (StaticPass)
			{
				staticRts[0] = Shader.PropertyToID("_StcAlbedo");
				staticRts[1] = Shader.PropertyToID("_StcGloss");
				staticRts[2] = Shader.PropertyToID("_StcNormal");
				staticRts[3] = Shader.PropertyToID("_StcAmbient");
				Data.projectionBuffer.GetTemporaryRT(staticRts[0], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)0);
				Data.projectionBuffer.GetTemporaryRT(staticRts[1], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)0);
				Data.projectionBuffer.GetTemporaryRT(staticRts[2], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)8);
				if (Camera.allowHDR)
				{
					Data.projectionBuffer.GetTemporaryRT(staticRts[3], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)8);
				}
				else
				{
					Data.projectionBuffer.GetTemporaryRT(staticRts[3], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)2);
				}
				MultiChannelFullScreenBlit(Camera, Data.projectionBuffer, staticRts);
			}
			else
			{
				staticRts[2] = Shader.PropertyToID("_StcNormal");
				Data.projectionBuffer.GetTemporaryRT(staticRts[2], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)8);
				StaticNormalFullScreenBlit(Camera, Data.projectionBuffer, staticRts[2]);
			}
			if (dynamicRts == null)
			{
				dynamicRts = new int[4];
			}
			dynamicRts[0] = Shader.PropertyToID("_DynAlbedo");
			dynamicRts[1] = Shader.PropertyToID("_DynGloss");
			dynamicRts[2] = Shader.PropertyToID("_DynNormal");
			dynamicRts[3] = Shader.PropertyToID("_DynAmbient");
			Data.projectionBuffer.GetTemporaryRT(dynamicRts[0], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)0);
			Data.projectionBuffer.GetTemporaryRT(dynamicRts[1], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)0);
			Data.projectionBuffer.GetTemporaryRT(dynamicRts[2], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)8);
			if (Camera.allowHDR)
			{
				Data.projectionBuffer.GetTemporaryRT(dynamicRts[3], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)8);
			}
			else
			{
				Data.projectionBuffer.GetTemporaryRT(dynamicRts[3], -1, -1, 0, (FilterMode)0, (RenderTextureFormat)2);
			}
			for (int i = 0; i < projections.Count; i++)
			{
				if (!Object.op_Implicit((Object)(object)projections[i]))
				{
					continue;
				}
				try
				{
					if (Data.projectionCulling.IsVisible(i) && (Camera.cullingMask & (1 << ((Component)projections[i]).gameObject.layer)) != 0)
					{
						DrawDeferredProjection(Camera, Data.projectionBuffer, projections[i], dynamicRts, i);
						projections[i].SetVisibility(Visible: true);
					}
					else
					{
						projections[i].SetVisibility(Visible: false);
					}
				}
				catch (IndexOutOfRangeException)
				{
					DrawDeferredProjection(Camera, Data.projectionBuffer, projections[i], dynamicRts, i);
				}
			}
			if (StaticPass)
			{
				Data.projectionBuffer.ReleaseTemporaryRT(staticRts[0]);
				Data.projectionBuffer.ReleaseTemporaryRT(staticRts[1]);
				Data.projectionBuffer.ReleaseTemporaryRT(staticRts[2]);
				Data.projectionBuffer.ReleaseTemporaryRT(staticRts[3]);
			}
			else
			{
				Data.projectionBuffer.ReleaseTemporaryRT(staticRts[2]);
			}
			Data.projectionBuffer.ReleaseTemporaryRT(dynamicRts[0]);
			Data.projectionBuffer.ReleaseTemporaryRT(dynamicRts[1]);
			Data.projectionBuffer.ReleaseTemporaryRT(dynamicRts[2]);
			Data.projectionBuffer.ReleaseTemporaryRT(dynamicRts[3]);
		}
		else if (Data.projectionBuffer.sizeInBytes > 0)
		{
			Data.projectionBuffer.Clear();
		}
	}

	private void DrawDeferredProjection(Camera Camera, CommandBuffer Buffer, Projection Projection, int[] Rts, int Index)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)Projection).isActiveAndEnabled && (Object)(object)Projection.RenderMaterial != (Object)null && Projection.DeferredBuffers != null && Projection.DeferredBuffers.Length > 0)
		{
			if (Projection.DeferredPrePass)
			{
				MultiChannelBlit(Camera, Buffer, Projection, Rts);
			}
			if (Camera.allowHDR)
			{
				Buffer.SetRenderTarget(Projection.DeferredHDRTargets, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2));
			}
			else
			{
				Buffer.SetRenderTarget(Projection.DeferredTargets, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2));
			}
			Buffer.DrawMesh(Cube, Projection.RenderMatrix, Projection.RenderMaterial, 0, Projection.DeferredPass, Projection.MaterialProperties);
		}
	}

	public RenderTargetIdentifier[] PassesToTargets(bool[] Channels, bool HDR)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if (Channels[0])
		{
			num += 2;
		}
		if (Channels[1])
		{
			num++;
		}
		if (Channels[2])
		{
			num++;
		}
		RenderTargetIdentifier[] array = null;
		switch (num)
		{
		case 0:
			return null;
		case 1:
			array = one;
			break;
		case 2:
			array = two;
			break;
		case 3:
			array = three;
			break;
		case 4:
			array = four;
			break;
		}
		num = 0;
		if (Channels[0])
		{
			ref RenderTargetIdentifier reference = ref array[num];
			reference = RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)10);
			num++;
		}
		if (Channels[1])
		{
			ref RenderTargetIdentifier reference2 = ref array[num];
			reference2 = RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)11);
			num++;
		}
		if (Channels[2])
		{
			ref RenderTargetIdentifier reference3 = ref array[num];
			reference3 = RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)12);
			num++;
		}
		if (Channels[0])
		{
			if (HDR)
			{
				ref RenderTargetIdentifier reference4 = ref array[num];
				reference4 = RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2);
				num++;
			}
			else
			{
				ref RenderTargetIdentifier reference5 = ref array[num];
				reference5 = RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)13);
				num++;
			}
		}
		return array;
	}

	private void MultiChannelBlit(Camera Camera, CommandBuffer Buffer, Projection Projection, int[] Rts)
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		if (Projection.DeferredBuffers[0] && Projection.DeferredBuffers[1] && Projection.DeferredBuffers[2])
		{
			ref RenderTargetIdentifier reference = ref four[0];
			reference = RenderTargetIdentifier.op_Implicit(Rts[0]);
			ref RenderTargetIdentifier reference2 = ref four[1];
			reference2 = RenderTargetIdentifier.op_Implicit(Rts[1]);
			ref RenderTargetIdentifier reference3 = ref four[2];
			reference3 = RenderTargetIdentifier.op_Implicit(Rts[2]);
			ref RenderTargetIdentifier reference4 = ref four[3];
			reference4 = RenderTargetIdentifier.op_Implicit(Rts[3]);
			DrawPrePass(Camera, Buffer, four, 6, Projection);
		}
		else if (Projection.DeferredBuffers[1] && Projection.DeferredBuffers[2])
		{
			ref RenderTargetIdentifier reference5 = ref two[0];
			reference5 = RenderTargetIdentifier.op_Implicit(Rts[1]);
			ref RenderTargetIdentifier reference6 = ref two[1];
			reference6 = RenderTargetIdentifier.op_Implicit(Rts[2]);
			DrawPrePass(Camera, Buffer, two, 5, Projection);
		}
		else if (Projection.DeferredBuffers[0] && Projection.DeferredBuffers[2])
		{
			ref RenderTargetIdentifier reference7 = ref three[0];
			reference7 = RenderTargetIdentifier.op_Implicit(Rts[0]);
			ref RenderTargetIdentifier reference8 = ref three[1];
			reference8 = RenderTargetIdentifier.op_Implicit(Rts[2]);
			ref RenderTargetIdentifier reference9 = ref three[2];
			reference9 = RenderTargetIdentifier.op_Implicit(Rts[3]);
			DrawPrePass(Camera, Buffer, three, 4, Projection);
		}
		else if (Projection.DeferredBuffers[0] && Projection.DeferredBuffers[1])
		{
			ref RenderTargetIdentifier reference10 = ref three[0];
			reference10 = RenderTargetIdentifier.op_Implicit(Rts[0]);
			ref RenderTargetIdentifier reference11 = ref three[1];
			reference11 = RenderTargetIdentifier.op_Implicit(Rts[1]);
			ref RenderTargetIdentifier reference12 = ref three[2];
			reference12 = RenderTargetIdentifier.op_Implicit(Rts[3]);
			DrawPrePass(Camera, Buffer, three, 3, Projection);
		}
		else if (Projection.DeferredBuffers[2])
		{
			ref RenderTargetIdentifier reference13 = ref one[0];
			reference13 = RenderTargetIdentifier.op_Implicit(Rts[2]);
			DrawPrePass(Camera, Buffer, one, 2, Projection);
		}
		else if (Projection.DeferredBuffers[1])
		{
			ref RenderTargetIdentifier reference14 = ref one[0];
			reference14 = RenderTargetIdentifier.op_Implicit(Rts[1]);
			DrawPrePass(Camera, Buffer, one, 1, Projection);
		}
		else
		{
			ref RenderTargetIdentifier reference15 = ref two[0];
			reference15 = RenderTargetIdentifier.op_Implicit(Rts[0]);
			ref RenderTargetIdentifier reference16 = ref two[1];
			reference16 = RenderTargetIdentifier.op_Implicit(Rts[3]);
			DrawPrePass(Camera, Buffer, two, 0, Projection);
		}
	}

	private void DrawPrePass(Camera Camera, CommandBuffer Buffer, RenderTargetIdentifier[] Rts, int Pass, Projection Projection)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (Rts.Length > 0)
		{
			Buffer.SetRenderTarget(Rts, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2));
			Buffer.DrawMesh(Cube, Projection.RenderMatrix, Mat_DeferredBlit, 0, Pass);
		}
	}

	private void StaticNormalFullScreenBlit(Camera Camera, CommandBuffer Buffer, int Rt)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Buffer.SetRenderTarget(RenderTargetIdentifier.op_Implicit(Rt), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2));
		Buffer.DrawMesh(CameraBlit, ((Component)Camera).transform.localToWorldMatrix, Mat_DeferredBlit, 0, 2);
	}

	private void MultiChannelFullScreenBlit(Camera Camera, CommandBuffer Buffer, int[] Rts)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		ref RenderTargetIdentifier reference = ref four[0];
		reference = RenderTargetIdentifier.op_Implicit(Rts[0]);
		ref RenderTargetIdentifier reference2 = ref four[1];
		reference2 = RenderTargetIdentifier.op_Implicit(Rts[1]);
		ref RenderTargetIdentifier reference3 = ref four[2];
		reference3 = RenderTargetIdentifier.op_Implicit(Rts[2]);
		ref RenderTargetIdentifier reference4 = ref four[3];
		reference4 = RenderTargetIdentifier.op_Implicit(Rts[3]);
		Buffer.SetRenderTarget(four, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2));
		Buffer.DrawMesh(CameraBlit, ((Component)Camera).transform.localToWorldMatrix, Mat_DeferredBlit, 0, 6);
	}
}
