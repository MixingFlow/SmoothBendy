using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleCull : MonoBehaviour
{
	[FormerlySerializedAs("overrideMaxDist")]
	public bool overrideCullingDistance;

	[FormerlySerializedAs("lightCullingDistance")]
	[Tooltip("When 'override culling distance' is false, this is set to the far clipping plane of the player camera.")]
	public float cullingDistance = 100f;

	[Header("Debugging")]
	[SerializeField]
	private Transform _cullTransform;

	[SerializeField]
	private Camera playerCamera;

	private HashSet<ParticleSystem> particleSystems = new HashSet<ParticleSystem>();

	private HashSet<Light> _lightSet = new HashSet<Light>();

	private HashSet<Projection> decalList = new HashSet<Projection>();

	private HashSet<ProjectionMask> maskList = new HashSet<ProjectionMask>();

	private HashSet<AudioSource> audioSourceList = new HashSet<AudioSource>();

	private Vector3 lightVector;

	private Vector3 itemPosition;

	public Transform cullTransform => _cullTransform;

	public int totalAudioSources { get; protected set; }

	public int activeAudioSources { get; protected set; }

	private void Awake()
	{
		SetupLights();
		SetupParticleSystems();
		SetupDynamicDecals();
		SetupProjectionMasks();
	}

	private IEnumerator Start()
	{
		int counter = 100;
		playerCamera = null;
		do
		{
			playerCamera = GameManager.Instance.GameCamera.Camera;
			yield return null;
		}
		while ((Object)(object)playerCamera == (Object)null && counter-- > 0);
		if (counter <= 0)
		{
			Debug.LogError((object)Log("Could not find GameManager.Instance.GameCamera...."), (Object)(object)this);
			yield break;
		}
		_cullTransform = ((Component)playerCamera).transform;
		float dist = playerCamera.farClipPlane;
		if (!overrideCullingDistance)
		{
			cullingDistance = dist;
		}
	}

	private void Update()
	{
		if (Object.op_Implicit((Object)(object)_cullTransform))
		{
			CullLights();
			CullParticleSystems();
			CullDecals();
			CullProjectionMasks();
		}
	}

	private void OnDrawGizmosSelected()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		foreach (Light item in _lightSet)
		{
			if (Object.op_Implicit((Object)(object)item))
			{
				Gizmos.color = ((!((Behaviour)item).enabled) ? Color.red : Color.green);
				Gizmos.DrawWireSphere(((Component)item).transform.position, 3f);
			}
		}
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			if (Object.op_Implicit((Object)(object)particleSystem))
			{
				Gizmos.color = ((!particleSystem.isPaused) ? Color.green : Color.red);
				Gizmos.DrawWireCube(((Component)particleSystem).transform.position, Vector3.one * 3f);
			}
		}
	}

	public IEnumerable<Light> GetLightList()
	{
		return _lightSet.ToList();
	}

	public void AddLights(IEnumerable<Light> list)
	{
		_lightSet.AddRange(list);
	}

	public void RemoveLights(IEnumerable<Light> list)
	{
		_lightSet.RemoveRange(list);
	}

	private void CullLights()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Vector3 forward = _cullTransform.forward;
		foreach (Light item in _lightSet)
		{
			if (!Object.op_Implicit((Object)(object)item))
			{
				continue;
			}
			itemPosition = ((Component)item).transform.position;
			lightVector = itemPosition - _cullTransform.position;
			float num = cullingDistance + item.range;
			if (((Vector3)(ref lightVector)).sqrMagnitude > num * num)
			{
				if (((Behaviour)item).enabled)
				{
					((Behaviour)item).enabled = false;
				}
				continue;
			}
			if (Vector3.Dot(lightVector, forward) < 0f)
			{
				Vector3 val = Vector3.Project(lightVector, forward);
				if (((Vector3)(ref val)).sqrMagnitude > item.range * item.range)
				{
					if (((Behaviour)item).enabled)
					{
						((Behaviour)item).enabled = false;
					}
					continue;
				}
			}
			if (!((Behaviour)item).enabled)
			{
				((Behaviour)item).enabled = true;
			}
		}
	}

	private void SetupLights()
	{
		_lightSet.Clear();
		_lightSet.AddRange(Object.FindObjectsOfType<Light>());
		List<Light> list = new List<Light>();
		foreach (Light item in _lightSet)
		{
			if (!((Behaviour)item).isActiveAndEnabled || !((Component)item).gameObject.activeInHierarchy)
			{
				list.Add(item);
			}
			else if (Object.op_Implicit((Object)(object)((Component)item).gameObject.GetComponentInParent<NoLightCullingMarker>()))
			{
				list.Add(item);
			}
		}
		_lightSet.RemoveRange(list);
	}

	private void CullParticleSystems()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = cullingDistance * cullingDistance;
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			if (!Object.op_Implicit((Object)(object)particleSystem))
			{
				continue;
			}
			Vector3 val = _cullTransform.position - ((Component)particleSystem).transform.position;
			if (((Vector3)(ref val)).sqrMagnitude > num)
			{
				if (!particleSystem.isPaused)
				{
					particleSystem.Pause();
				}
			}
			else if (!particleSystem.isPlaying)
			{
				particleSystem.Play();
			}
		}
	}

	private void SetupParticleSystems()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		particleSystems.Clear();
		ParticleSystem[] array = Object.FindObjectsOfType<ParticleSystem>();
		foreach (ParticleSystem val in array)
		{
			MainModule main = val.main;
			if (((MainModule)(ref main)).loop && ((Component)val).gameObject.activeInHierarchy && val.isPlaying)
			{
				ParticleSystemPause component = ((Component)val).GetComponent<ParticleSystemPause>();
				if (!Object.op_Implicit((Object)(object)component))
				{
					particleSystems.Add(val);
				}
			}
		}
	}

	private void CullDecals()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = cullingDistance * cullingDistance;
		foreach (Projection decal in decalList)
		{
			if (!Object.op_Implicit((Object)(object)decal))
			{
				continue;
			}
			Vector3 val = _cullTransform.position - ((Component)decal).transform.position;
			if (((Vector3)(ref val)).sqrMagnitude > num)
			{
				if (((Behaviour)decal).enabled)
				{
					((Behaviour)decal).enabled = false;
				}
			}
			else if (!((Behaviour)decal).enabled)
			{
				((Behaviour)decal).enabled = true;
			}
		}
	}

	private void SetupDynamicDecals()
	{
		decalList.Clear();
		Projection[] array = Object.FindObjectsOfType<Projection>();
		foreach (Projection projection in array)
		{
			if (((Component)projection).gameObject.activeSelf)
			{
				decalList.Add(projection);
			}
		}
	}

	private void CullAudioSources()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = cullingDistance * cullingDistance;
		foreach (AudioSource audioSource in audioSourceList)
		{
			if (!Object.op_Implicit((Object)(object)audioSource))
			{
				continue;
			}
			Vector3 val = _cullTransform.position - ((Component)audioSource).transform.position;
			if (((Vector3)(ref val)).sqrMagnitude > num)
			{
				if (((Component)audioSource).gameObject.activeSelf)
				{
					((Component)audioSource).gameObject.SetActive(false);
				}
			}
			else if (!((Component)audioSource).gameObject.activeSelf)
			{
				((Component)audioSource).gameObject.SetActive(true);
			}
		}
	}

	private void SetupAudioSources()
	{
		audioSourceList.Clear();
		AudioSource[] array = Object.FindObjectsOfType<AudioSource>();
		foreach (AudioSource val in array)
		{
			if (((Component)val).gameObject.activeInHierarchy && !Object.op_Implicit((Object)(object)((Component)val).GetComponent<NoAudioSourceCullingMarker>()))
			{
				audioSourceList.Add(val);
			}
		}
	}

	private void CullProjectionMasks()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = cullingDistance * cullingDistance;
		foreach (ProjectionMask mask in maskList)
		{
			if (!Object.op_Implicit((Object)(object)mask))
			{
				continue;
			}
			Vector3 val = _cullTransform.position - ((Component)mask).transform.position;
			if (((Vector3)(ref val)).sqrMagnitude > num)
			{
				if (((Behaviour)mask).enabled)
				{
					((Behaviour)mask).enabled = false;
				}
			}
			else if (!((Behaviour)mask).enabled)
			{
				((Behaviour)mask).enabled = true;
			}
		}
	}

	private void SetupProjectionMasks()
	{
		maskList.Clear();
		ProjectionMask[] array = Object.FindObjectsOfType<ProjectionMask>();
		foreach (ProjectionMask projectionMask in array)
		{
			if (((Component)projectionMask).gameObject.activeInHierarchy)
			{
				maskList.Add(projectionMask);
			}
		}
	}

	private string Log(string msg)
	{
		return "[SIMPLE CULL] " + msg;
	}
}
