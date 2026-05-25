using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LightGroup : MonoBehaviour
{
	[Header("General")]
	public bool startOff = true;

	[Header("Bounds")]
	[Tooltip("When true, will use the bounds to turn on this volume if the player is contained by any of the child bounds. When false, you will have to manually turn on and off this Light Group, via the LightGroups public API.")]
	public bool useBounds = true;

	[Header("Debugging")]
	[SerializeField]
	private SimpleCull sc;

	[SerializeField]
	private bool isOn = true;

	private HashSet<BoundObject> boundsList = new HashSet<BoundObject>();

	private HashSet<Light> lights = new HashSet<Light>();

	private HashSet<VideoPlayer> videoPlayerList = new HashSet<VideoPlayer>();

	private bool _started;

	private const float BoundsExpansionFactor = 1.5f;

	private void Awake()
	{
		boundsList.Clear();
		boundsList.AddRange(((Component)this).GetComponentsInChildren<BoundObject>());
		isOn = true;
	}

	private IEnumerator Start()
	{
		int counter = 100;
		do
		{
			yield return null;
			sc = Object.FindObjectOfType<SimpleCull>();
		}
		while (!Object.op_Implicit((Object)(object)sc) && counter-- > 0);
		if (counter <= 0)
		{
			Debug.LogError((object)Log("Unable to find instance of SimpleCull..."), (Object)(object)this);
			yield break;
		}
		counter = 100;
		do
		{
			yield return null;
		}
		while (!Object.op_Implicit((Object)(object)sc.cullTransform) && counter-- > 0);
		if (counter <= 0)
		{
			Debug.LogError((object)Log("Unable to find sc.cullTransform"), (Object)(object)this);
			yield break;
		}
		SetupLights(sc.GetLightList());
		SetupVideoPlayers();
		if (startOff)
		{
			ToggleLightGroup(turnOn: false);
		}
		_started = true;
	}

	private void Update()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (_started && useBounds)
		{
			ToggleLightGroup(Contains(sc.cullTransform.position));
		}
	}

	public void OnDrawGizmosSelected()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		foreach (Light light in lights)
		{
			if (Object.op_Implicit((Object)(object)light) && ((Behaviour)light).enabled)
			{
				Gizmos.color = ((!((Behaviour)light).enabled) ? Color.red : Color.green);
				Gizmos.DrawWireSphere(((Component)light).transform.position, 3f);
			}
		}
		if (!Application.isPlaying || !useBounds)
		{
			return;
		}
		Gizmos.color = ((!isOn) ? Color.red : Color.green);
		foreach (BoundObject bounds in boundsList)
		{
			if (Object.op_Implicit((Object)(object)bounds))
			{
				Gizmos.DrawWireCube(((Bounds)(ref bounds.bounds)).center, ((Bounds)(ref bounds.bounds)).size);
			}
		}
	}

	public bool Contains(Vector3 point)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Bounds val = default(Bounds);
		foreach (BoundObject bounds in boundsList)
		{
			((Bounds)(ref val))._002Ector(((Bounds)(ref bounds.bounds)).center, ((Bounds)(ref bounds.bounds)).size * 1.5f);
			if (((Bounds)(ref val)).Contains(point))
			{
				return true;
			}
		}
		return false;
	}

	public void ToggleLightGroup(bool turnOn)
	{
		if (turnOn && !isOn)
		{
			if (Object.op_Implicit((Object)(object)sc))
			{
				sc.AddLights(lights);
			}
			videoPlayerList.ForEach(delegate(VideoPlayer x)
			{
				if (Object.op_Implicit((Object)(object)x) && Object.op_Implicit((Object)(object)((Component)x).gameObject) && !((Component)x).gameObject.activeSelf)
				{
					((Component)x).gameObject.SetActive(true);
				}
			});
		}
		else if (!turnOn && isOn)
		{
			if (Object.op_Implicit((Object)(object)sc))
			{
				sc.RemoveLights(lights);
			}
			lights.ForEach(delegate(Light x)
			{
				if (Object.op_Implicit((Object)(object)x) && ((Behaviour)x).enabled)
				{
					((Behaviour)x).enabled = false;
				}
			});
			videoPlayerList.ForEach(delegate(VideoPlayer x)
			{
				if (Object.op_Implicit((Object)(object)x) && Object.op_Implicit((Object)(object)((Component)x).gameObject) && ((Component)x).gameObject.activeSelf)
				{
					((Component)x).gameObject.SetActive(false);
				}
			});
		}
		isOn = turnOn;
	}

	private void SetupLights(IEnumerable<Light> newLights)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		lights.Clear();
		foreach (Light newLight in newLights)
		{
			if (!Object.op_Implicit((Object)(object)newLight) || Object.op_Implicit((Object)(object)((Component)newLight).GetComponent<NoLightCullingMarker>()))
			{
				continue;
			}
			foreach (BoundObject bounds in boundsList)
			{
				if (bounds.includeLightsInBounds && ((Bounds)(ref bounds.bounds)).Contains(((Component)newLight).transform.position))
				{
					lights.Add(newLight);
				}
			}
		}
	}

	private void SetupVideoPlayers()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		videoPlayerList.Clear();
		VideoPlayer[] array = Object.FindObjectsOfType<VideoPlayer>();
		foreach (VideoPlayer val in array)
		{
			if (!Object.op_Implicit((Object)(object)val) || !((Component)val).gameObject.activeSelf || !val.playOnAwake)
			{
				continue;
			}
			foreach (BoundObject bounds in boundsList)
			{
				if (bounds.includeLightsInBounds && ((Bounds)(ref bounds.bounds)).Contains(((Component)val).transform.position))
				{
					videoPlayerList.Add(val);
				}
			}
		}
	}

	private string Log(string msg)
	{
		return "[LIGHT GROUP (" + ((Object)((Component)this).gameObject).name + ")] " + msg;
	}
}
