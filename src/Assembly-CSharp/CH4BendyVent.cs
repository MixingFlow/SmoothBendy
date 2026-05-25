using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH4BendyVent : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject m_Bendy;

	[SerializeField]
	private Animator m_Animator;

	[SerializeField]
	private AnimationClip m_AnimationClip;

	[SerializeField]
	private Transform m_InkPuddle;

	[SerializeField]
	private Material m_InkWall;

	[SerializeField]
	private AudioClip m_FingerClip;

	private AudioClip m_SearcherScareClip;

	private AudioClip m_InkFlowClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_SearcherScareClip = GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/SFX/SFX_ChapterOneBendyAppears");
		m_InkFlowClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Ink_Flood_AMB_01");
		m_InkWall.SetFloat("_Cutout", 0f);
		m_Bendy.SetActive(false);
		AnimationEventUtil.AddAnimationEvent(ref m_Animator, ((Object)m_AnimationClip).name, "OnComplete", 400);
	}

	public void Activate()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected O, but got Unknown
		ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 0.7f, 0.7f, 15, 90f, false, true);
		m_Bendy.SetActive(true);
		GameManager.Instance.AudioManager.Play(m_SearcherScareClip, AudioObjectType.MUSIC);
		GameManager.Instance.InkEffectManager.SetActive(active: true);
		GameManager.Instance.InkEffectManager.SetPosition(m_Bendy.transform.position);
		Sequence val = DOTween.Sequence();
		float num = 3f;
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(m_InkWall, 0.6f, "_Cutout", 10f), (Ease)7));
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(((Component)m_InkPuddle).transform, new Vector3(10f, 10f, 1f), 10f), (Ease)7));
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_FingerClip);
		});
		TweenSettingsExtensions.InsertCallback(val, 5f, (TweenCallback)delegate
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.AudioManager.PlayAtPosition(m_InkFlowClip, m_Bendy.transform.position, AudioObjectType.SOUND_EFFECT, -1);
		});
	}

	public void OnComplete()
	{
		m_Bendy.SetActive(false);
		GameManager.Instance.InkEffectManager.SetActive(active: false);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
