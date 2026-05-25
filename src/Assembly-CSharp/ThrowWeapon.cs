using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class ThrowWeapon : BaseWeapon
{
	[Header("Gun Options")]
	[SerializeField]
	private ThrowableObject m_ThrowObject;

	[SerializeField]
	private Transform m_HeldPosition;

	private List<ThrowWeaponRespawner> m_ThrowWeaponRespawners = new List<ThrowWeaponRespawner>();

	private S13Switch m_AudioSwitch;

	private ThrowWeaponRespawner m_Respawner;

	private bool m_IsReloading;

	public ThrowableObject ThrowableObject { get; private set; }

	public int CurrentAmmo => m_ThrowWeaponRespawners.Count;

	public event EventHandler OnThrow;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AudioSwitch = ((Component)this).GetComponentInChildren<S13Switch>();
	}

	public void AddAmmo(ThrowWeaponRespawner respawner)
	{
		if (!m_ThrowWeaponRespawners.Contains(respawner))
		{
			m_ThrowWeaponRespawners.Add(respawner);
		}
		m_AudioSwitch.Play("pickup");
	}

	protected override void OnEquip()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		SendOnEquipped();
		if ((Object)(object)m_HeldPosition != (Object)null && (Object)(object)m_WeaponModel != (Object)null)
		{
			m_WeaponModel.transform.SetParent(m_HeldPosition);
			m_WeaponModel.transform.localPosition = Vector3.zero;
			m_WeaponModel.transform.localEulerAngles = Vector3.zero;
		}
	}

	public override void OnAttack()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		m_CanAttack = true;
		if (!m_IsReloading)
		{
			if (CurrentAmmo > 0)
			{
				m_AudioSwitch.Play("toss");
				ThrowableObject = Object.Instantiate<ThrowableObject>(m_ThrowObject, base.transform.position, base.transform.rotation);
				WeaponInfo weaponInfo = ThrowableObject.WeaponInfo;
				weaponInfo.Attacker = GetWeaponInfo().Attacker;
				ThrowableObject.SetRespawner(m_ThrowWeaponRespawners[0]);
				m_ThrowWeaponRespawners.RemoveAt(0);
				ThrowableObject.OnHit += HandleThrowableObjectOnHit;
				ThrowableObject.Initialize(weaponInfo, GameManager.Instance.Player.CharacterController.velocity + ThrowableObject.transform.forward * ThrowableObject.Force + ThrowableObject.transform.up * 7f - ThrowableObject.transform.right);
				ThrowableObject.Throw();
				this.OnThrow.Send(this);
			}
			Reload();
		}
	}

	private void HandleThrowableObjectOnHit(object sender, EventArgs e)
	{
		ThrowableObject throwableObject = sender as ThrowableObject;
		throwableObject.OnHit -= HandleThrowableObjectOnHit;
		throwableObject.HitPlaySound("land");
	}

	private void Reload()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		m_IsReloading = true;
		if (CurrentAmmo > 0)
		{
			Sequence val = DOTween.Sequence();
			TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)ShortcutExtensions.DOLocalMoveY(base.transform, -0.6f, 0f, false));
			TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)ShortcutExtensions.DOLocalMoveZ(base.transform, -0.6f, 0f, false));
			TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)ShortcutExtensions.DOLocalRotate(base.transform, new Vector3(30f, 0f, 0f), 0f, (RotateMode)0));
			TweenSettingsExtensions.Insert(val, 0.75f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(base.transform, 0f, 0.5f, false), (Ease)6));
			TweenSettingsExtensions.Insert(val, 0.75f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(base.transform, 0f, 0.5f, false), (Ease)6));
			TweenSettingsExtensions.Insert(val, 0.75f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(base.transform, Vector3.zero, 0.4f, (RotateMode)0), (Ease)6));
			TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(ReloadOnComplete));
			return;
		}
		GameManager.Instance.Player.UnEquipWeapon();
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.InactiveWeapon))
		{
			GameManager.Instance.Player.WeaponGameObject = GameManager.Instance.Player.InactiveWeapon;
			GameManager.Instance.Player.InactiveWeapon = null;
		}
		else
		{
			GameManager.Instance.Player.WeaponGameObject = null;
		}
		Sequence val2 = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val2, 0f, (Tween)(object)ShortcutExtensions.DOLocalMoveY(base.transform, -0.6f, 0f, false));
		TweenSettingsExtensions.Insert(val2, 0f, (Tween)(object)ShortcutExtensions.DOLocalMoveZ(base.transform, -0.6f, 0f, false));
		TweenSettingsExtensions.Insert(val2, 0f, (Tween)(object)ShortcutExtensions.DOLocalRotate(base.transform, new Vector3(30f, 0f, 0f), 0f, (RotateMode)0));
		TweenSettingsExtensions.InsertCallback(val2, 0.2f, new TweenCallback(base.Dispose));
	}

	private void ReloadOnComplete()
	{
		m_IsReloading = false;
	}

	protected override void OnDisposed()
	{
		m_AudioSwitch = null;
		base.OnDisposed();
	}
}
