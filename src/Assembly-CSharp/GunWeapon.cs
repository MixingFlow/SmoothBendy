using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GunWeapon : BaseWeapon
{
	[Header("Gun Options")]
	[SerializeField]
	private ParticleSystem m_Bullets;

	[SerializeField]
	private ParticleSystem m_Sparks;

	[SerializeField]
	private List<Light> m_Lights;

	[SerializeField]
	private float m_FireRate = 0.12f;

	[SerializeField]
	private int m_ClipMax = 18;

	[SerializeField]
	private int m_ReloadMax = 3;

	private float m_FireRateNext;

	private int m_ReloadCount;

	private int m_ClipCount;

	private bool m_IsReloading;

	protected override void OnInteracted()
	{
		base.OnInteracted();
		for (int i = 0; i < m_Lights.Count; i++)
		{
			((Behaviour)m_Lights[i]).enabled = false;
		}
		m_ClipCount = m_ClipMax;
		m_FireRateNext = Time.time + m_FireRate + 0.1f;
	}

	protected override void OnEquip()
	{
		SendOnEquipped();
	}

	public override void OnAttack()
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		m_CanAttack = true;
		if (m_IsReloading || !(Time.time > m_FireRateNext))
		{
			return;
		}
		m_FireRateNext = Time.time + m_FireRate;
		if (m_ClipCount <= 0)
		{
			m_ReloadCount++;
			if (m_ReloadCount > m_ReloadMax)
			{
				Reload();
			}
			else
			{
				GameManager.Instance.AudioManager.Play("Audio/SFX/Gun/SFX_Ink_Gun_Empty");
			}
		}
		else
		{
			m_Bullets.Emit(1);
			m_Sparks.Emit(5);
			for (int i = 0; i < m_Lights.Count; i++)
			{
				((Behaviour)m_Lights[i]).enabled = true;
			}
			GameManager.Instance.AudioManager.Play("Audio/SFX/Gun/SFX_Ink_Shoot");
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
			TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 0.15f, 0.15f, 5, 90f, false, false), (TweenCallback)delegate
			{
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				for (int j = 0; j < m_Lights.Count; j++)
				{
					((Behaviour)m_Lights[j]).enabled = false;
				}
				GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
			});
		}
		m_ClipCount--;
	}

	private void Reload()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		m_IsReloading = true;
		GameManager.Instance.AudioManager.Play("Audio/SFX/Gun/SFX_Ink_Gun_Reload");
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)ShortcutExtensions.DOLocalMoveY(base.transform, -0.25f, 0.4f, false));
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)ShortcutExtensions.DOLocalMoveZ(base.transform, -0.25f, 0.4f, false));
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)ShortcutExtensions.DOLocalRotate(base.transform, new Vector3(25f, 0f, 0f), 0.4f, (RotateMode)0));
		TweenSettingsExtensions.Insert(val, 0.4f, (Tween)(object)ShortcutExtensions.DOLocalMoveY(base.transform, 0f, 0.4f, false));
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)ShortcutExtensions.DOLocalMoveZ(base.transform, 0f, 0.4f, false));
		TweenSettingsExtensions.Insert(val, 0.4f, (Tween)(object)ShortcutExtensions.DOLocalRotate(base.transform, Vector3.zero, 0.4f, (RotateMode)0));
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(ReloadOnComplete));
	}

	private void ReloadOnComplete()
	{
		m_IsReloading = false;
		m_ReloadCount = 0;
		m_ClipCount = m_ClipMax;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
