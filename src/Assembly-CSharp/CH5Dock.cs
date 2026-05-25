using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH5Dock : BaseController
{
	[Header("Boat Locations")]
	[SerializeField]
	private Transform m_Boat;

	[SerializeField]
	private EventTrigger m_ObjectiveTrigger;

	[Header("Launch 1")]
	[SerializeField]
	private Transform m_L1StartLocation;

	[SerializeField]
	private Transform m_L1MidLocation;

	[SerializeField]
	private Transform m_L1EndLocation;

	[Header("Launch 2")]
	[SerializeField]
	private Transform m_L2MidLocation;

	[SerializeField]
	private Transform m_L2EndLocation;

	[Header("Brakes")]
	[SerializeField]
	private CH5ChuteBrakes m_ChuteBrakes;

	[SerializeField]
	private GameObject m_InitialBoatCollider;

	[Header("Tom & Alice Boat")]
	[SerializeField]
	private Transform m_OtherBoat;

	[SerializeField]
	private Transform m_OtherBoatEndLocation;

	private int m_CurrentLocation;

	private int m_LocationMax = 2;

	public override void InitOnComplete()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((Component)m_OtherBoat).gameObject.SetActive(false);
		m_ObjectiveTrigger.SetActive(active: false);
		m_Boat.position = m_L1StartLocation.position;
		m_Boat.eulerAngles = m_L1StartLocation.eulerAngles;
		base.InitOnComplete();
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.DockObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.DockObjective.IsStarted)
		{
			ForceStart();
		}
		else
		{
			InternalActivate();
		}
	}

	private void InternalActivate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		m_ObjectiveTrigger.OnEnter += HandleObjectiveTriggerOnEnter;
		m_ObjectiveTrigger.SetActive(active: true);
		m_ChuteBrakes.OnGo += HandleChuteBrakesOnGo;
		m_ChuteBrakes.Activate();
		((Component)m_OtherBoat).gameObject.SetActive(true);
		S13AudioManager.Instance.InvokeEvent("evt_ch5_boat_in_distance");
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_OtherBoat, m_OtherBoatEndLocation.position, 15f, false), (Ease)5);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_OtherBoat, m_OtherBoatEndLocation.eulerAngles, 15f, (RotateMode)0), (Ease)5), new TweenCallback(OtherBoatOnComplete));
	}

	private void ForceStart()
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_LAUNCH_THE_BARGE", string.Empty));
		m_ChuteBrakes.OnGo += HandleChuteBrakesOnGo;
		m_ChuteBrakes.Activate();
	}

	private void ForceComplete()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		m_Boat.position = m_L2EndLocation.position;
		m_Boat.eulerAngles = m_L2EndLocation.eulerAngles;
		m_ChuteBrakes.AllowShimmer(_active: false);
		m_ChuteBrakes.ForceRemoveEffects();
		m_ChuteBrakes.Activate();
		m_InitialBoatCollider.SetActive(false);
		SendOnComplete();
	}

	private void HandleObjectiveTriggerOnEnter(object sender, EventArgs e)
	{
		m_ObjectiveTrigger.OnEnter -= HandleObjectiveTriggerOnEnter;
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_LAUNCH_THE_BARGE", string.Empty, 4f));
	}

	private void OtherBoatOnComplete()
	{
		((Component)m_OtherBoat).gameObject.SetActive(false);
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.DockObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleChuteBrakesOnGo(object sender, EventArgs e)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		m_CurrentLocation++;
		if (m_CurrentLocation <= m_LocationMax)
		{
			Sequence val = DOTween.Sequence();
			if (m_CurrentLocation == 1)
			{
				S13AudioManager.Instance.InvokeEvent("evt_boat_chute_slide1");
				TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Boat, m_L1MidLocation.position, 2f, false), (Ease)5));
				TweenSettingsExtensions.Insert(val, 2f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Boat, m_L1EndLocation.position, 3f, false), (Ease)1));
				TweenSettingsExtensions.Insert(val, 2f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_Boat, m_L1EndLocation.eulerAngles, 0.6f, (RotateMode)0), (Ease)1));
			}
			else if (m_CurrentLocation == 2)
			{
				S13AudioManager.Instance.InvokeEvent("evt_boat_chute_slide2");
				TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Boat, m_L2MidLocation.position, 4f, false), (Ease)5));
				TweenSettingsExtensions.Insert(val, 4f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Boat, m_L2EndLocation.position, 1f, false), (Ease)6));
				TweenSettingsExtensions.Insert(val, 4f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_Boat, m_L2EndLocation.eulerAngles, 1.75f, (RotateMode)0), (Ease)6));
				TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(BoatOnComplete));
			}
		}
	}

	private void BoatOnComplete()
	{
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.DockObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		m_InitialBoatCollider.SetActive(false);
		m_ChuteBrakes.OnGo -= HandleChuteBrakesOnGo;
		m_ChuteBrakes.AllowShimmer(_active: false);
		m_ChuteBrakes.ForceRemoveEffects();
		m_ChuteBrakes.Activate();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_ChuteBrakes))
		{
			m_ChuteBrakes.OnGo -= HandleChuteBrakesOnGo;
		}
		base.OnDisposed();
	}
}
