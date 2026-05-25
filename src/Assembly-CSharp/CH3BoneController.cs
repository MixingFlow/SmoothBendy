using System;
using TMG.Core;
using UnityEngine;

public class CH3BoneController : TMGMonoBehaviour
{
	[Header("Interactables")]
	[SerializeField]
	private Interactable m_Bone;

	private AudioClip m_PickupClip;

	private AudioClip m_TakeBoneClip;

	private BorisAi m_Boris => GameManager.Instance.CharacterManager.Boris;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Bone.SetActive(active: false);
		m_PickupClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_genericpickup");
		m_TakeBoneClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_boristakesbone");
	}

	public void Activate()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HasBorisBone)
		{
			m_Bone.gameObject.SetActive(true);
			m_Bone.transform.SetParent(m_Boris.Mouth);
			m_Bone.transform.localPosition = Vector3.zero;
			m_Bone.transform.localEulerAngles = Vector3.zero;
			Dispose();
		}
		else
		{
			m_Bone.SetActive(active: true);
			m_Bone.OnInteracted += HandleBoneOnInteracted;
		}
	}

	private void HandleBoneOnInteracted(object sender, EventArgs e)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		m_Bone.OnInteracted -= HandleBoneOnInteracted;
		m_Bone.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_PickupClip);
		m_Bone.gameObject.SetActive(false);
		m_Bone.transform.SetParent(m_Boris.Mouth);
		m_Bone.transform.localPosition = Vector3.zero;
		m_Bone.transform.localEulerAngles = Vector3.zero;
		m_Boris.Interact.SetActive(active: true);
		m_Boris.Interact.OnInteracted += HandleBorisOnInteracted;
	}

	private void HandleBorisOnInteracted(object sender, EventArgs e)
	{
		m_Boris.Interact.OnInteracted -= HandleBorisOnInteracted;
		m_Boris.Interact.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_TakeBoneClip);
		m_Bone.gameObject.SetActive(true);
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.KNICK_KNACK_PADDYWHACK);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HasBorisBone = true;
		Dispose();
	}

	protected override void OnDisposed()
	{
		m_PickupClip = null;
		m_TakeBoneClip = null;
		if (Object.op_Implicit((Object)(object)m_Boris))
		{
			m_Boris.Interact.OnInteracted -= HandleBorisOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_Bone))
		{
			m_Bone.OnInteracted -= HandleBoneOnInteracted;
		}
		base.OnDisposed();
	}
}
