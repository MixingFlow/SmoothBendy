using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH4BridgeMachine : TMGMonoBehaviour
{
	[Header("Bridge Machine")]
	[SerializeField]
	private CH4Cart m_Cart;

	[SerializeField]
	private Interactable m_Lever;

	[SerializeField]
	private Interactable m_Railing;

	[SerializeField]
	private Transform m_ExitTransform;

	[SerializeField]
	private Interactable m_MissingGear;

	[SerializeField]
	private GameObject m_ReplaceGear;

	[SerializeField]
	private List<Transform> m_GearsPositive;

	[SerializeField]
	private List<Transform> m_GearsNegative;

	[Header("Audio")]
	[SerializeField]
	private Transform m_StartAudio;

	[SerializeField]
	private Transform m_EndAudio;

	[Header("Options")]
	[SerializeField]
	private bool m_DevTestEnabled;

	private Sequence m_GearSequence;

	private int m_GearDirection = 1;

	private AudioClip m_HenryClip03;

	private AudioClip m_MusicDanglingByAThread;

	private S13Switch m_CartAudioSwitch;

	private S13Switch m_StartAudioSwitch;

	private S13Switch m_EndAudioSwitch;

	public event EventHandler OnReady;

	public event EventHandler OnEnter;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HenryClip03 = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/Henry/DIA_CH4_HENRY_03");
		m_MusicDanglingByAThread = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_DanglingByAThread");
		m_ReplaceGear.SetActive(false);
		m_CartAudioSwitch = ((Component)m_Cart.transform).GetComponentInChildren<S13Switch>();
		m_StartAudioSwitch = ((Component)m_StartAudio).GetComponentInChildren<S13Switch>();
		m_EndAudioSwitch = ((Component)m_EndAudio).GetComponentInChildren<S13Switch>();
	}

	public void ForceComplete()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		((Component)m_MissingGear).GetComponent<Collider>().enabled = false;
		m_MissingGear.gameObject.SetActive(false);
		m_ReplaceGear.SetActive(true);
		Transform obj = m_Lever.transform;
		obj.localEulerAngles += new Vector3(-80f, 0f, 0f);
		m_Cart.ForceComplete();
	}

	public void Activate()
	{
		if (m_DevTestEnabled)
		{
			FinalActivate();
		}
		else
		{
			((Component)m_MissingGear).GetComponent<Collider>().enabled = false;
			m_MissingGear.gameObject.SetActive(false);
			m_Lever.OnInteracted += HandleInitialLeverOnInteracted;
			m_Lever.SetActive(active: true);
		}
		m_Cart.BeginSway();
	}

	public void FinalActivate()
	{
		m_MissingGear.OnInteracted += HandleMissingGearOnInteracted;
		((Component)m_MissingGear).GetComponent<Collider>().enabled = true;
		m_MissingGear.SetActive(active: true);
	}

	public void TurnOffInitialLever()
	{
		m_Lever.OnInteracted -= HandleInitialLeverOnInteracted;
		m_Lever.SetActive(active: false);
		m_Lever.SetSingleInteraction(_isSingle: false);
		m_MissingGear.gameObject.SetActive(true);
		((Component)m_MissingGear).GetComponent<Collider>().enabled = false;
		m_MissingGear.SetActive(active: true);
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_REPAIR_THE_BRIDGE", string.Empty));
		this.OnReady.Send(this);
	}

	private void HandleInitialLeverOnInteracted(object sender, EventArgs e)
	{
		m_Lever.OnInteracted -= HandleInitialLeverOnInteracted;
		m_Lever.SetActive(active: false);
		m_Lever.SetSingleInteraction(_isSingle: false);
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip03, "DIACH4/DIA_CH4_HENRY_03")).OnComplete += HandleInitialLeverDialogueOnComplete;
	}

	private void HandleInitialLeverDialogueOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleInitialLeverDialogueOnComplete;
		m_MissingGear.gameObject.SetActive(true);
		m_MissingGear.SetActive(active: true);
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_REPAIR_THE_BRIDGE", string.Empty, 4f));
		this.OnReady.Send(this);
	}

	private void HandleMissingGearOnInteracted(object sender, EventArgs e)
	{
		m_MissingGear.OnInteracted += HandleMissingGearOnInteracted;
		m_MissingGear.Dispose();
		m_ReplaceGear.SetActive(true);
		m_StartAudioSwitch.Play("gear_fit");
		m_Lever.OnInteracted += HandleLeverOnInteracted;
		m_Lever.SetActive(active: true);
	}

	private void HandleLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		m_Lever.OnInteracted -= HandleLeverOnInteracted;
		m_Lever.SetActive(active: false);
		GameManager.Instance.AudioManager.Play("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Lever.transform, new Vector3(-80f, 0f, 0f), 0.5f, (RotateMode)3), (Ease)27), (TweenCallback)delegate
		{
			m_Cart.OnComplete += HandleCartOnComplete;
			m_Cart.OnStart += HandleCartOnStart;
			m_Cart.OnStop += HandleCartOnStop;
			m_Cart.OnInteracted += HandleCartOnInteracted;
			m_Cart.OnBreakdown += HandleCartOnBreakdown;
			m_Cart.OnSmash += HandleCartOnSmash;
			m_Cart.CallCart();
		});
	}

	private void HandleCartOnSmash(object sender, EventArgs e)
	{
		m_Cart.OnSmash -= HandleCartOnSmash;
		KillGearSequence();
		m_CartAudioSwitch.Play("end");
		m_EndAudioSwitch.Play("stop");
		m_CartAudioSwitch.Stop("rope");
		m_StartAudioSwitch.Stop("running");
		m_EndAudioSwitch.Stop("running");
	}

	private void HandleCartOnBreakdown(object sender, EventArgs e)
	{
		m_Cart.OnBreakdown -= HandleCartOnBreakdown;
		GameManager.Instance.AudioManager.Play(m_MusicDanglingByAThread, AudioObjectType.MUSIC);
		m_CartAudioSwitch.Play("breakdown");
	}

	private void HandleCartOnInteracted(object sender, EventArgs e)
	{
		m_Cart.OnInteracted -= HandleCartOnInteracted;
		this.OnEnter.Send(this);
		m_CartAudioSwitch.Play("enter");
	}

	private void HandleCartOnStart(object sender, EventArgs e)
	{
		m_StartAudioSwitch.Play("start");
		m_StartAudioSwitch.Play("running");
		m_CartAudioSwitch.Play("rope");
		m_EndAudioSwitch.Play("start");
		m_EndAudioSwitch.Play("running");
		RotateGears();
	}

	private void RotateGears()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		ResetGearSequence();
		for (int i = 0; i < m_GearsPositive.Count; i++)
		{
			Transform val = m_GearsPositive[i];
			TweenSettingsExtensions.Insert(m_GearSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, new Vector3(0f, 0f, (float)(36 * m_GearDirection)), 0.5f, (RotateMode)3), (Ease)1));
		}
		for (int j = 0; j < m_GearsNegative.Count; j++)
		{
			Transform val2 = m_GearsNegative[j];
			TweenSettingsExtensions.Insert(m_GearSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val2, new Vector3(0f, 0f, (float)(36 * -m_GearDirection)), 0.5f, (RotateMode)3), (Ease)1));
		}
		TweenSettingsExtensions.OnComplete<Sequence>(m_GearSequence, new TweenCallback(RotateGears));
	}

	private void HandleCartOnStop(object sender, EventArgs e)
	{
		m_StartAudioSwitch.Play("stop");
		m_StartAudioSwitch.Stop("running");
		m_EndAudioSwitch.Play("stop");
		m_EndAudioSwitch.Stop("running");
		StuckGears();
		m_GearDirection *= -1;
	}

	private void StuckGears()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		ResetGearSequence();
		for (int i = 0; i < m_GearsPositive.Count; i++)
		{
			Transform val = m_GearsPositive[i];
			TweenSettingsExtensions.Insert(m_GearSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, new Vector3(0f, 0f, 12f), 0.3f, (RotateMode)3), (Ease)7));
		}
		for (int j = 0; j < m_GearsNegative.Count; j++)
		{
			Transform val2 = m_GearsNegative[j];
			TweenSettingsExtensions.Insert(m_GearSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val2, new Vector3(0f, 0f, -12f), 0.3f, (RotateMode)3), (Ease)7));
		}
		TweenSettingsExtensions.SetLoops<Sequence>(m_GearSequence, -1, (LoopType)1);
	}

	private void KillGearSequence()
	{
		if (m_GearSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_GearSequence, false);
			m_GearSequence = null;
		}
	}

	private void ResetGearSequence()
	{
		KillGearSequence();
		m_GearSequence = DOTween.Sequence();
	}

	private void HandleCartOnComplete(object sender, EventArgs e)
	{
		m_Cart.OnComplete -= HandleCartOnComplete;
		m_Railing.OnInteracted += HandleRailingOnInteracted;
		m_Railing.SetActive(active: true);
	}

	private void HandleRailingOnInteracted(object sender, EventArgs e)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		m_Railing.OnInteracted -= HandleRailingOnInteracted;
		m_Railing.SetActive(active: false);
		m_CartAudioSwitch.Play("exit");
		Transform val = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.Player.transform.SetParent((Transform)null);
		GameManager.Instance.Player.transform.position = m_ExitTransform.position;
		Vector3 zero = Vector3.zero;
		zero.x = m_ExitTransform.eulerAngles.x;
		Vector3 zero2 = Vector3.zero;
		zero2.y = m_ExitTransform.localEulerAngles.y;
		GameManager.Instance.Player.LookRotation(Quaternion.Euler(zero2), Quaternion.Euler(zero));
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, m_ExitTransform.eulerAngles, 2f, (RotateMode)0), (Ease)7);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(val, GameManager.Instance.Player.HeadContainer.position.x, 2f, false), (Ease)7);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(val, GameManager.Instance.Player.HeadContainer.position.y, 2f, false), (Ease)28);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveZ(val, GameManager.Instance.Player.HeadContainer.position.z, 2.1f, false), (Ease)7), (TweenCallback)delegate
		{
			GameManager.Instance.GameCamera.ExitFreeRoamCam();
			this.OnComplete.Send(this);
		});
	}

	protected override void OnDisposed()
	{
		TweenExtensions.Kill((Tween)(object)m_GearSequence, false);
		m_CartAudioSwitch = null;
		m_StartAudioSwitch = null;
		m_EndAudioSwitch = null;
		m_HenryClip03 = null;
		base.OnDisposed();
	}
}
