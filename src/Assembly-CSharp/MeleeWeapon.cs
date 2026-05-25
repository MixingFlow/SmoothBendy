using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MeleeWeapon : BaseWeapon
{
	[Serializable]
	public class WeaponAnimation
	{
		public Vector3 StartRotation;

		public Vector3 EndRotation;

		public Vector3 StartPosition;

		public Vector3 EndPosition;
	}

	[Header("Weapon Animations")]
	[SerializeField]
	private List<WeaponAnimation> m_WeaponAnimations;

	[Header("New Swing Properties")]
	[SerializeField]
	private float m_AttackTimecode;

	[SerializeField]
	private List<AnimationClip> m_AnimationClips;

	[SerializeField]
	private float m_Padding = 16f;

	private int m_PreviousAnimationIndex;

	private int m_AnimationIndex;

	private int m_AnimationCount;

	protected override void OnInteracted()
	{
	}

	protected override void OnEquip()
	{
		SendOnEquipped();
	}

	public override void OnAttack()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Sequence>(DOAttack(), new TweenCallback(HandleSwingComplete));
	}

	private Sequence DOAttack()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		ResetAttackSequence();
		GetAnimationIndex();
		((Component)GameManager.Instance.Player.WeaponParent).GetComponentInParent<Animator>().Play(((Object)m_AnimationClips[m_AnimationIndex]).name);
		TweenSettingsExtensions.InsertCallback(m_AttackSequence, 0f, new TweenCallback(HandleSwingBegin));
		TweenSettingsExtensions.InsertCallback(m_AttackSequence, m_AttackTimecode, new TweenCallback(HandleSwingHit));
		TweenSettingsExtensions.InsertCallback(m_AttackSequence, (m_AnimationClips[m_AnimationIndex].length + m_Padding) / 30f, new TweenCallback(HandleSwingEnd));
		return m_AttackSequence;
	}

	private void GetAnimationIndex()
	{
		m_AnimationIndex = Random.Range(0, 2);
		if (m_AnimationIndex == m_PreviousAnimationIndex)
		{
			m_AnimationCount++;
			if (m_AnimationCount > 1)
			{
				m_AnimationCount = 0;
				if (m_AnimationIndex == 0)
				{
					m_AnimationIndex = 1;
				}
				else
				{
					m_AnimationIndex = 0;
				}
			}
		}
		else
		{
			m_AnimationCount = 0;
		}
		m_PreviousAnimationIndex = m_AnimationIndex;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
