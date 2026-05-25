using System.Collections.Generic;
using TMG.Controls;
using TMG.Core;
using UnityEngine;

namespace TMG.UI;

public class UIManager : TMGMonoBehaviour
{
	public Camera Camera { get; private set; }

	public Dictionary<string, UILayer> UILayers { get; private set; }

	public static UIManager Create(object _data = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return new GameObject("[UI MANAGER]").AddComponent<UIManager>();
	}

	public override void Init()
	{
		base.Init();
		Object.DontDestroyOnLoad((Object)(object)base.gameObject);
		GameObject.FindGameObjectWithTag("UIVisualControls").transform.SetParent(base.transform);
		Camera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
		UILayers = new Dictionary<string, UILayer>();
		UILayers.Add("VIEW", UILayer.Create("VIEW", 0f, base.transform));
		UILayers.Add("BORDERS", UILayer.Create("BORDERS", -20f, base.transform));
		UILayers.Add("CROSSHAIR", UILayer.Create("CROSSHAIR", -40f, base.transform));
		UILayers.Add("MODAL", UILayer.Create("MODAL", -80f, base.transform));
		UILayers.Add("PAUSE", UILayer.Create("PAUSE", -260f, base.transform));
		UILayers.Add("OBJECTIVE", UILayer.Create("OBJECTIVE", -120f, base.transform));
		UILayers.Add("NOTIFICATIONS", UILayer.Create("NOTIFICATIONS", -140f, base.transform));
		UILayers.Add("PROMPT", UILayer.Create("PROMPT", -160f, base.transform));
		UILayers.Add("LOADER", UILayer.Create("LOADER", -180f, base.transform));
		UILayers.Add("BLOCKER", UILayer.Create("BLOCKER", -200f, base.transform));
		UILayers.Add("CHAPTERTITLE", UILayer.Create("CHAPTERTITLE", -220f, base.transform));
		UILayers.Add("SUBTITLES", UILayer.Create("SUBTITLES", -240f, base.transform));
		UILayers.Add("ASYNC LOADER", UILayer.Create("ASYNC LOADER", -300f, base.transform));
		UILayers.Add("ERROR", UILayer.Create("ERROR", -800f, base.transform));
	}

	public void Update()
	{
		if (GameManager.Instance.isPauseReady && PlayerInput.Pause())
		{
			if (!GameManager.Instance.isPaused)
			{
				GameManager.Instance.Pause();
			}
			else
			{
				GameManager.Instance.Unpause();
			}
		}
	}

	public T Show<T>(string assetKey, string layer, object data = null)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Object.Instantiate<GameObject>(GameManager.Instance.AssetManager.GetAsset<GameObject>(assetKey));
		UILayer uILayer = UILayers[layer];
		val.transform.position = uILayer.transform.position;
		val.transform.eulerAngles = uILayer.transform.eulerAngles;
		val.transform.localScale = Vector3.one;
		val.transform.SetParent(uILayer.transform);
		T component = val.GetComponent<T>();
		(component as BaseUIController).InitController(data);
		(component as BaseUIController).PlayIn();
		return component;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
