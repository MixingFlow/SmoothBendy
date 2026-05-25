using System;
using DG.Tweening;
using UnityEngine;

public class CH1Internecion : BaseController
{
	[SerializeField]
	private GameObject m_Primary;

	[SerializeField]
	private GameObject m_Internecion;

	[SerializeField]
	private EventTrigger m_Trigger;

	[SerializeField]
	private Transform m_StartPosition;

	private string m_InternecionStr = string.Empty;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		string text = "onef";
		string text2 = "our";
		m_InternecionStr = "f" + text2 + text + text2;
		bool internecion = GetInternecion();
		m_Internecion.SetActive(internecion);
		m_Primary.SetActive(!internecion);
		if (internecion)
		{
			m_StartPosition.SetParent((Transform)null);
			m_Trigger.OnEnter += HandleTriggerOnEnter;
			m_Trigger.SetActive(active: true);
		}
	}

	private void HandleTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		m_Trigger.OnEnter -= HandleTriggerOnEnter;
		GameManager.Instance.ShowScreenBlocker(0f);
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(val, 0.1f, (TweenCallback)delegate
		{
			AmplifyPostProcess component = ((Component)GameManager.Instance.GameCamera.WeaponCamera).GetComponent<AmplifyPostProcess>();
			((Behaviour)component).enabled = true;
			GameManager.Instance.Player.GoToAndLookAt(m_StartPosition);
			GameManager.Instance.Player.SetLock(active: true);
		});
		TweenSettingsExtensions.InsertCallback(val, 0.3f, (TweenCallback)delegate
		{
			GameManager.Instance.Player.SetLock(active: false);
			GameManager.Instance.Player.PlayRespawnEffects();
			GameManager.Instance.HideScreenBlocker(0.11f);
		});
		TweenSettingsExtensions.OnComplete<Sequence>(val, (TweenCallback)delegate
		{
			m_Primary.SetActive(true);
			m_Internecion.SetActive(false);
			GameManager.Instance.GameData.CurrentSaveFile.Internecions[0] = 1;
			GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: false, shouldShowSaveIndicator: false);
		});
	}

	private bool GetInternecion()
	{
		if (!GameManager.Instance.GameData.CurrentSaveFile.HasDied && GameManager.Instance.GameData.CurrentSaveFile.Internecions[0] == 0 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[1] == 4 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[2] == 1 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[3] == 4 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[4] == 0 && GameManager.Instance.GameData.CurrentSaveFile.Internecion == m_InternecionStr)
		{
			return true;
		}
		return false;
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_Trigger != (Object)null)
		{
			m_Trigger.OnEnter -= HandleTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
