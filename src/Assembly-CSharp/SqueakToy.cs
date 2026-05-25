using System;
using DG.Tweening;
using UnityEngine;

public class SqueakToy : Interactable
{
	[Header("ID")]
	[SerializeField]
	private int ID = -1;

	private AudioClip m_SqueakClip;

	private AudioObject m_SqueakAudio;

	private static ItemIDManager m_idManager;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_SqueakClip = GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/SFX/Collectables/SFX_Toy_01");
	}

	public override void OnInteract()
	{
		base.OnInteract();
		m_Active = false;
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(base.transform, 0.9f, 0.15f), (Ease)1));
		TweenSettingsExtensions.Insert(val, 0.15f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(base.transform, 1f, 0.15f), (Ease)27));
		PlaySound();
	}

	private void HandleInteractOnComplete(object sender, EventArgs e)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		base.transform.localScale = Vector3.one;
		m_Active = true;
	}

	private void PlaySound()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_SqueakAudio = GameManager.Instance.AudioManager.PlayAtPosition(m_SqueakClip, base.transform.position);
		m_SqueakAudio.OnComplete += HandleInteractOnComplete;
	}

	public int SetID(int CurrentIDCount)
	{
		if (ID != -1)
		{
			return ID;
		}
		return ID = ++CurrentIDCount;
	}

	public int GetID()
	{
		if (!Object.op_Implicit((Object)(object)m_idManager))
		{
			m_idManager = Object.FindObjectOfType<ItemIDManager>();
		}
		if (Object.op_Implicit((Object)(object)m_idManager))
		{
			int itemId = m_idManager.GetItemId(base.gameObject);
			if (itemId != -1)
			{
				return itemId;
			}
		}
		if (ID != -1)
		{
			return ID;
		}
		return -1;
	}

	public void ResetID()
	{
		ID = -1;
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		if ((Object)(object)m_SqueakAudio != (Object)null)
		{
			m_SqueakAudio.Clear();
			m_SqueakAudio = null;
		}
		m_SqueakClip = null;
		base.OnDisposed();
	}
}
