using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4HoldingRoomController : BaseController
{
	[Header("Objective: Enter The Vent")]
	[SerializeField]
	private CH3Flashlight m_Flashlight;

	[SerializeField]
	private Transform m_VentCamStartPos;

	[SerializeField]
	private Transform m_VentEntrancePos;

	[SerializeField]
	private GameObject m_LostOnesPack;

	[SerializeField]
	private GameObject m_LostOnesVent;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_LostOnesPack.SetActive(true);
		m_LostOnesVent.SetActive(false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.VentObjective.IsStarted)
		{
			ForceComplete();
			return;
		}
		m_Flashlight.OnInteracted += HandleFlashlightOnInteracted;
		m_Flashlight.Activate();
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH4_save_point_04");
		m_Flashlight.OnInteracted -= HandleFlashlightOnInteracted;
		m_Flashlight.Equip();
		GameManager.Instance.Player.SetVent(active: true);
		RenderSettings.ambientIntensity = 0f;
		m_LostOnesPack.SetActive(false);
		m_LostOnesVent.SetActive(true);
		SendOnComplete();
	}

	private void HandleFlashlightOnInteracted(object sender, EventArgs e)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		m_Flashlight.OnInteracted -= HandleFlashlightOnInteracted;
		GameManager.Instance.Player.SetVent(active: true);
		GameManager.Instance.Player.WeaponGameObject.SetActive(false);
		S13AudioManager.Instance.InvokeEvent("evt_vent_enter");
		Transform val = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.Player.GoToAndLookAt(m_VentEntrancePos);
		Sequence val2 = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val, m_VentCamStartPos.position, 0.6f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, m_VentCamStartPos.eulerAngles, 0.6f, (RotateMode)0), (Ease)7));
		num += 0.85f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(DOTweenUtil.DOAmbientLightColor(0.6f, 0.5f), (Ease)1));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val, GameManager.Instance.GameCamera.HeadContainer.position, 1f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, m_VentEntrancePos.eulerAngles, 1f, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(val2, new TweenCallback(HandleEnterVentOnComplete));
	}

	private void HandleEnterVentOnComplete()
	{
		GameManager.Instance.Player.WeaponGameObject.SetActive(true);
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		m_LostOnesPack.SetActive(false);
		m_LostOnesVent.SetActive(true);
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.VentObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
