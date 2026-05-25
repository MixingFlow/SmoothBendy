using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5BendyHandChase : TMGMonoBehaviour
{
	[SerializeField]
	private Animator m_BendyHandAnimator;

	[SerializeField]
	private Transform m_BendyHand;

	[SerializeField]
	private AnimationClip m_SlapClip;

	[SerializeField]
	private RiverWaves m_Wave;

	[SerializeField]
	private SkinnedMeshRenderer m_Renderer;

	[SerializeField]
	private float m_HandIdleTime = 10f;

	[SerializeField]
	private float m_HandSlapKillDistance = 7f;

	private Transform m_BendyHandSpawnLocation;

	private Transform m_BendyHandSpawnStartLocation;

	public event EventHandler OnHit;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		AnimationEventUtil.AddEventController(ref m_BendyHandAnimator).SetReference(this);
		AnimationEventUtil.AddAnimationEvent(ref m_BendyHandAnimator, ((Object)m_SlapClip).name, "BendyHandSlap", 25);
		AnimationEventUtil.AddAnimationEvent(ref m_BendyHandAnimator, ((Object)m_SlapClip).name, "BendyHandSlapOnComplete", 59);
		((Renderer)m_Renderer).enabled = false;
	}

	public void SetHandLocation(Transform location, Transform startLocation)
	{
		m_BendyHandSpawnLocation = location;
		m_BendyHandSpawnStartLocation = startLocation;
	}

	public void ActivateHand()
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_BendyHandSpawnLocation))
		{
			m_BendyHand.position = new Vector3(m_BendyHandSpawnStartLocation.position.x, m_BendyHand.position.y, m_BendyHandSpawnStartLocation.position.z);
			m_BendyHand.rotation = m_BendyHandSpawnStartLocation.rotation;
			ShortcutExtensions.DOKill((Component)(object)m_BendyHand, false);
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_BendyHand, new Vector3(m_BendyHandSpawnLocation.position.x, m_BendyHand.position.y, m_BendyHandSpawnLocation.position.z), 8f, false), (Ease)5);
		}
		m_BendyHandAnimator.SetTrigger("Rise");
		GameManager.Instance.AudioManager.Play("Audio/SFX/sfx_BendyHand_Rise");
		m_Wave.AddWave(((Component)m_BendyHand).transform.position, 1.8f, 0.06f, 3f);
		((Renderer)m_Renderer).enabled = true;
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), m_HandIdleTime, (TweenCallback)delegate
		{
			DoSlap();
		});
	}

	public void DoSlap()
	{
		m_BendyHandAnimator.SetTrigger("Slap");
		GameManager.Instance.AudioManager.Play("Audio/SFX/sfx_BendyHand_Slap");
	}

	public void CheckForAndKillPlayer()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		m_Wave.AddWave(((Component)m_BendyHand).transform.position, 1.8f, 0.06f, 3f);
		Vector3 position = GameManager.Instance.Player.transform.position;
		position.y = m_BendyHand.position.y;
		if (Vector3.Distance(m_BendyHand.position, position) < m_HandSlapKillDistance)
		{
			this.OnHit.Send(this);
			GameManager.Instance.AudioManager.Play("Audio/SFX/sfx_boat_destroyed");
		}
	}

	public void BendyHandSlapOnComplete()
	{
		((Renderer)m_Renderer).enabled = false;
	}

	protected override void OnDisposed()
	{
		m_BendyHandSpawnLocation = null;
		ShortcutExtensions.DOKill((Component)(object)m_BendyHand, false);
		base.OnDisposed();
	}
}
