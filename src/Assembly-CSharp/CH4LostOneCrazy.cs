using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH4LostOneCrazy : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject m_AudioProxy;

	[SerializeField]
	private Transform m_LostOneModel;

	[SerializeField]
	private Animator m_Animator;

	[SerializeField]
	private Transform[] m_InitialWaypoints;

	[SerializeField]
	private Transform[] m_FinalWaypoints;

	[SerializeField]
	private AnimationClip m_WalkClip;

	private Sequence m_Sequence;

	private S13Switch m_AudioSwitch;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		AnimationEventUtil.AddAnimationEvent(ref m_Animator, ((Object)m_WalkClip).name, "Footstep", 0);
		AnimationEventUtil.AddAnimationEvent(ref m_Animator, ((Object)m_WalkClip).name, "Footstep", 25);
		((Component)m_LostOneModel).gameObject.SetActive(false);
		m_AudioSwitch = m_AudioProxy.GetComponentInChildren<S13Switch>();
	}

	public void Activate()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Expected O, but got Unknown
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		ResetSequence();
		((Component)m_LostOneModel).gameObject.SetActive(true);
		m_AudioSwitch.Play("open");
		float num = 0f;
		float num2 = 7f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(SetWalkAnimation));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_LostOneModel, m_InitialWaypoints[0].position, num2, false), (Ease)1));
		num += num2;
		num2 = 1.5f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num - 3f, new TweenCallback(PlayAudio));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_LostOneModel, m_InitialWaypoints[1].position, num2, false), (Ease)1));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_LostOneModel, m_InitialWaypoints[1].eulerAngles, num2, (RotateMode)0), (Ease)1));
		num += num2;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(SetIdleAnimation));
		num += 6f;
		num2 = 2f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(SetWalkAnimation));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_LostOneModel, m_FinalWaypoints[0].position, num2, false), (Ease)1));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_LostOneModel, m_FinalWaypoints[0].eulerAngles, num2, (RotateMode)0), (Ease)7));
		num += num2;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_LostOneModel, m_FinalWaypoints[1].position, 11f, false), (Ease)1));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_LostOneModel, m_FinalWaypoints[1].eulerAngles, 1f, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.InsertCallback(m_Sequence, num + 8f, (TweenCallback)delegate
		{
			m_AudioSwitch.Play("close");
		});
		TweenSettingsExtensions.OnComplete<Sequence>(m_Sequence, new TweenCallback(OnComplete));
	}

	private void SetIdleAnimation()
	{
		m_Animator.SetTrigger("Idle");
	}

	private void SetWalkAnimation()
	{
		m_Animator.SetTrigger("Walk");
	}

	private void PlayAudio()
	{
		m_AudioSwitch.Play("dialogue");
	}

	public void PlayFootsteps()
	{
		m_AudioSwitch.Play("footsteps");
	}

	private void OnComplete()
	{
		Dispose();
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
		m_AudioSwitch = null;
		base.OnDisposed();
	}
}
