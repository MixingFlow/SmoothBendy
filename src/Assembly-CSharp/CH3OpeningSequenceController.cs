using DG.Tweening;
using UnityEngine;

public class CH3OpeningSequenceController : BaseController
{
	[SerializeField]
	private Transform m_BedPosition;

	[SerializeField]
	private Transform m_SitPosition;

	[SerializeField]
	private Transform m_PosterPosition;

	private Transform m_GameCamTransform;

	private Sequence m_Sequence;

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.SafehouseObjective.IsComplete)
		{
			ForceComplete();
		}
		else
		{
			InternalActivate();
		}
	}

	private void InternalActivate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		m_GameCamTransform = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		m_GameCamTransform.position = m_BedPosition.position;
		m_GameCamTransform.eulerAngles = m_BedPosition.eulerAngles;
		TweenSettingsExtensions.OnComplete<Sequence>(DOSequence(), new TweenCallback(SequenceOnComplete));
	}

	private void ForceComplete()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.Player.transform.position = GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PlayerPosition.GetVector();
		GameManager.Instance.Player.LookRotation(Quaternion.Euler(GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PlayerRotation.GetVector()));
		GameManager.Instance.HideScreenBlocker(1f, 0.5f);
		GameManager.Instance.Player.SetLock(active: false);
		GameManager.Instance.UnlockPause();
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.ShowCrosshair();
		SendOnComplete();
	}

	private Sequence DOSequence()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		ResetSequence();
		float num = 0f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowChapterTitle("MENU/CH3_LABEL", "MENU/CH3_TITLE");
		});
		num += 1f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(1f);
		});
		num += 4f;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_GameCamTransform, m_SitPosition.position, 1.75f, false), (Ease)7));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_GameCamTransform, m_SitPosition.eulerAngles, 1.75f, (RotateMode)0), (Ease)7));
		num += 2f;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_GameCamTransform, m_PosterPosition.position, 2f, false), (Ease)7));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_GameCamTransform, m_PosterPosition.eulerAngles, 2f, (RotateMode)0), (Ease)7));
		num += 3.5f;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_GameCamTransform, GameManager.Instance.Player.transform.eulerAngles, 2f, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_GameCamTransform, GameManager.Instance.Player.HeadContainer.position, 3.5f, false), (Ease)7));
		return m_Sequence;
	}

	private void SequenceOnComplete()
	{
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		GameManager.Instance.Player.SetLock(active: false);
		GameManager.Instance.UnlockPause();
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.ShowCrosshair();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_01", string.Empty, 4f));
		SendOnComplete();
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		base.OnDisposed();
	}
}
