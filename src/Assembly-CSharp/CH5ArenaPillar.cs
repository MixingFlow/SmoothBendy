using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH5ArenaPillar : TMGMonoBehaviour, IHittable
{
	[SerializeField]
	private GameObject m_Glass;

	[SerializeField]
	private GameObject m_BrokenGlass;

	[SerializeField]
	private GameObject m_Generator;

	[SerializeField]
	private GameObject m_GeneratorBroken;

	[SerializeField]
	private GameObject m_GearsOn;

	[SerializeField]
	private GameObject m_GearsOff;

	[SerializeField]
	private MeshRenderer m_Ink;

	private S13Switch m_audioSwitch;

	public event EventHandler OnHit;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		((Renderer)m_Ink).material.SetFloat("_Cutout", 0f);
		m_GeneratorBroken.SetActive(false);
		m_BrokenGlass.SetActive(false);
		m_Glass.SetActive(true);
		m_GearsOn.SetActive(false);
		m_GearsOff.SetActive(true);
		m_audioSwitch = ((Component)this).GetComponentInChildren<S13Switch>();
		if ((Object)(object)m_audioSwitch == (Object)null)
		{
			Debug.Log((object)"The Arena Pillar can't find an S13Switch in child objects. Ensure the switch is being proxied into the scene.", (Object)(object)base.gameObject);
		}
		m_audioSwitch.Play("closed");
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleZ(m_Generator.transform, 0.755f, 0.1f), (Ease)7), -1, (LoopType)1);
	}

	public void TurnOn()
	{
		m_GearsOn.SetActive(true);
		m_GearsOff.SetActive(false);
		ShortcutExtensions.DOFloat(((Renderer)m_Ink).material, 0.6f, "_Cutout", 1f);
	}

	public void Hit(RaycastHit _hit, WeaponInfo _weaponInfo)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		this.OnHit.Send(this);
		m_GeneratorBroken.SetActive(true);
		m_Generator.SetActive(false);
		m_BrokenGlass.SetActive(true);
		m_Glass.SetActive(false);
		m_GearsOn.SetActive(false);
		m_GearsOff.SetActive(true);
		m_audioSwitch.Play("smash");
		m_audioSwitch.Stop("closed");
		m_audioSwitch.Play("open");
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 1f, 4f, 15, 90f, false, true), (TweenCallback)delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		});
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
