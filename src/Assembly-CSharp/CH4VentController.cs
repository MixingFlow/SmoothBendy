using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4VentController : BaseController
{
	[Header("Jumpscare:")]
	[SerializeField]
	private EventTrigger m_BendyJumpscareTrigger;

	[SerializeField]
	private CH4BendyVent m_Bendy;

	[Header("Objective: Exit The Vent")]
	[SerializeField]
	private Transform m_VentExitPos;

	[SerializeField]
	private EventTrigger m_VentExitTrigger;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_VentExitTrigger.SetActive(active: false);
		m_BendyJumpscareTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.VentObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		m_BendyJumpscareTrigger.OnEnter += HandleBendyJumpscareTriggerOnEnter;
		m_BendyJumpscareTrigger.SetActive(active: true);
		m_VentExitTrigger.OnEnter += HandleVentExitTriggerOnEnter;
		m_VentExitTrigger.SetActive(active: true);
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH4_save_point_05");
		RenderSettings.ambientIntensity = 1f;
		GameManager.Instance.Player.WeaponGameObject.SetActive(true);
		GameObject weaponGameObject = GameManager.Instance.Player.WeaponGameObject;
		GameManager.Instance.Player.UnEquipWeapon();
		GameManager.Instance.Player.SetVent(active: false);
		Object.Destroy((Object)(object)weaponGameObject);
		SendOnComplete();
	}

	private void HandleBendyJumpscareTriggerOnEnter(object sender, EventArgs e)
	{
		m_BendyJumpscareTrigger.OnEnter -= HandleBendyJumpscareTriggerOnEnter;
		m_Bendy.Activate();
	}

	private void HandleVentExitTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		m_VentExitTrigger.OnEnter -= HandleVentExitTriggerOnEnter;
		S13AudioManager.Instance.InvokeEvent("evt_vent_exit");
		Transform val = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		Sequence val2 = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, m_VentExitPos.eulerAngles, 1f, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(DOTweenUtil.DOAmbientLightColor(1f, 0.5f), (Ease)1));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(GameManager.Instance.Player.WeaponGameObject.transform, -1f, 0.5f, false), (Ease)7));
		num += 1f;
		TweenSettingsExtensions.InsertCallback(val2, num, (TweenCallback)delegate
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.Player.transform.position = m_VentExitPos.position;
			GameManager.Instance.Player.transform.eulerAngles = m_VentExitPos.eulerAngles;
			GameObject weaponGameObject = GameManager.Instance.Player.WeaponGameObject;
			if (Object.op_Implicit((Object)(object)weaponGameObject))
			{
				weaponGameObject.SetActive(false);
			}
		});
		num += 0.1f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val, m_VentExitPos.position + GameManager.Instance.GameCamera.HeadContainer.localPosition, 0.6f, false), (Ease)5));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, m_VentExitPos.eulerAngles, 0.6f, (RotateMode)0), (Ease)7));
		num += 0.6f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val, m_VentExitPos.position + Vector3.up, 0.1f, false), (Ease)6));
		num += 0.1f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val, m_VentExitPos.position + GameManager.Instance.GameCamera.HeadContainer.localPosition, 0.4f, false), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(val2, new TweenCallback(HandleExitVentOnComplete));
	}

	private void HandleExitVentOnComplete()
	{
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		GameManager.Instance.Player.SetVent(active: false);
		GameObject weaponGameObject = GameManager.Instance.Player.WeaponGameObject;
		if (Object.op_Implicit((Object)(object)weaponGameObject))
		{
			Object.Destroy((Object)(object)weaponGameObject);
		}
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.VentObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
