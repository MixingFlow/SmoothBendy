using System.Collections;
using I2.Loc;
using TMG.Core;
using UnityEngine;

public abstract class AbstractChapterController : TMGMonoBehaviour
{
	protected class LocalizationSupport : ILocalizationParamsManager
	{
		public string GetParameterValue(string Param)
		{
			if (Param != null && Param == "N")
			{
				return "\n";
			}
			return null;
		}
	}

	[Header("< Culling Settings >")]
	[SerializeField]
	private bool m_overrideCullingDistance;

	[SerializeField]
	private float m_cullingDistance;

	[SerializeField]
	private GameObject simpleCullSupportPrefab;

	protected static ILocalizationParamsManager localizationParamsManager;

	protected void StartChapter()
	{
		((MonoBehaviour)this).StartCoroutine(LoadChapter());
	}

	private IEnumerator LoadChapter()
	{
		yield return (object)new WaitForSeconds(1f);
		yield return (object)new WaitForEndOfFrame();
		SetupSimpleCulling();
		InitializeChapter();
	}

	public abstract void InitializeChapter();

	private void SetupSimpleCulling()
	{
		if (m_overrideCullingDistance)
		{
			SetupCullingOverrideDistance(m_cullingDistance);
		}
		if (Object.op_Implicit((Object)(object)simpleCullSupportPrefab))
		{
			Object.Instantiate<GameObject>(simpleCullSupportPrefab);
		}
	}

	public void SetupCullingOverrideDistance(float newDistance)
	{
		SimpleCull simpleCull = Object.FindObjectOfType<SimpleCull>();
		if (Object.op_Implicit((Object)(object)simpleCull))
		{
			simpleCull.overrideCullingDistance = true;
			simpleCull.cullingDistance = newDistance;
		}
		else
		{
			Debug.LogError((object)("[" + ((object)this).GetType().ToString() + "] Unable to find SimpleCull instance..."), (Object)(object)this);
		}
	}

	public override void Init()
	{
		base.Init();
		if (localizationParamsManager == null)
		{
			localizationParamsManager = new LocalizationSupport();
			if (!LocalizationManager.ParamManagers.Contains(localizationParamsManager))
			{
				Debug.Log((object)"<color=green>-- Adding Localization Support object for Globals replacement --</color>", (Object)(object)this);
				LocalizationManager.ParamManagers.Add(localizationParamsManager);
				LocalizationManager.LocalizeAll(Force: true);
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
