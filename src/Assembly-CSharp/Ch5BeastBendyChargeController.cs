using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class Ch5BeastBendyChargeController : TMGMonoBehaviour
{
	[SerializeField]
	private CH5BeastBendyInkPortal[] m_BendyPaths;

	[SerializeField]
	private Transform m_Bendy;

	[SerializeField]
	private float m_BendySpeed;

	[SerializeField]
	private float m_CameraShakeDistance = 30f;

	[SerializeField]
	private Animator m_AnimationController;

	[SerializeField]
	private AnimationClip m_Anim_Charge;

	private int m_CurrentPathIdex;

	private bool m_PathEndParticlesFired;

	private bool m_HasAttackedPlayer;

	private bool m_IsActive;

	private bool m_IsComplete;

	private S13AnimationSwitch m_AudioSwitch;

	public override void InitOnComplete()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Charge).name, "ChargeStomp", 0);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Charge).name, "ChargeStomp", 15);
		m_AudioSwitch = ((Component)this).GetComponentInChildren<S13AnimationSwitch>();
		((Component)m_Bendy).transform.position = m_BendyPaths[0].m_StartPoint.position;
		((Component)m_Bendy).gameObject.SetActive(false);
	}

	public void Activate()
	{
		((Component)m_Bendy).gameObject.SetActive(true);
		m_IsActive = true;
	}

	public void Deactivate()
	{
		m_IsComplete = true;
	}

	private void Update()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsActive)
		{
			return;
		}
		((Component)m_Bendy).transform.position = Vector3.MoveTowards(((Component)m_Bendy).transform.position, m_BendyPaths[m_CurrentPathIdex].m_EndPoint.position, m_BendySpeed * Time.deltaTime);
		if (!m_PathEndParticlesFired)
		{
			if (Vector3.Distance(m_Bendy.position, m_BendyPaths[m_CurrentPathIdex].m_EndPoint.position) < 22f)
			{
				m_BendyPaths[m_CurrentPathIdex].ActivateEndParticles();
				m_PathEndParticlesFired = true;
			}
		}
		else if (Vector3.Distance(m_Bendy.position, m_BendyPaths[m_CurrentPathIdex].m_EndPoint.position) <= 0f)
		{
			if (m_IsComplete)
			{
				m_IsActive = false;
				((Component)m_Bendy).gameObject.SetActive(false);
				return;
			}
			m_CurrentPathIdex++;
			if (m_CurrentPathIdex == m_BendyPaths.Length)
			{
				m_CurrentPathIdex = 0;
			}
			m_PathEndParticlesFired = false;
			m_HasAttackedPlayer = false;
			m_BendyPaths[m_CurrentPathIdex].ActivateStartParticles();
			((Component)m_Bendy).transform.position = m_BendyPaths[m_CurrentPathIdex].m_StartPoint.position;
			((Component)m_Bendy).transform.rotation = m_BendyPaths[m_CurrentPathIdex].m_StartPoint.rotation;
			m_AudioSwitch.PlaySwitch("Roar_Voice");
		}
		if (Physics.CheckSphere(m_Bendy.position, 6f, LayerMask.GetMask(new string[1] { "Player" })) && !m_HasAttackedPlayer)
		{
			AttackTarget();
			m_HasAttackedPlayer = true;
		}
	}

	public void ApplyShake(float shakePower = 0.5f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		float num = Vector3.Distance(m_Bendy.position, GameManager.Instance.Player.transform.position) / m_CameraShakeDistance;
		num = 1f - Mathf.Clamp01(num);
		GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 0.5f, shakePower * num, 15, 90f, false, true), (TweenCallback)delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		});
	}

	public void AttackTarget()
	{
		for (int i = 0; i < 5; i++)
		{
			GameManager.Instance.ShowHurtBorder(isSilent: true);
		}
		GameManager.Instance.ShowHurtBorder();
		S13AudioManager.Instance.InvokeEvent("evt_player_hit_by_boris");
	}
}
