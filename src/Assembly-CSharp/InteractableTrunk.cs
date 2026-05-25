using DG.Tweening;
using UnityEngine;

public class InteractableTrunk : Interactable
{
	[Header("Trunk Options")]
	[SerializeField]
	private Transform m_Lid;

	[SerializeField]
	private Collider m_TriggerCollider;

	[SerializeField]
	private AudioClip m_AudioClip;

	public override void Init()
	{
		base.Init();
		SetActive(m_Active);
	}

	public override void OnInteract()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		base.OnInteract();
		SetActive(active: false);
		GameManager.Instance.AudioManager.PlayAtPosition(m_AudioClip, base.transform.position);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Lid, new Vector3(0f, 0f, -105f), 1f, (RotateMode)3), (Ease)27);
		m_TriggerCollider.enabled = false;
	}

	public void ForceOpen()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		SetActive(active: false);
		m_Lid.localEulerAngles = new Vector3(0f, 0f, -105f);
		m_TriggerCollider.enabled = false;
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Lid, false);
		base.OnDisposed();
	}
}
