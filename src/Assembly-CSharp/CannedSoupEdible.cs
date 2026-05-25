using System.Collections.Generic;
using UnityEngine;

public class CannedSoupEdible : Interactable
{
	[Header("ID")]
	[SerializeField]
	private int ID = -1;

	[Header("Sounds")]
	[SerializeField]
	private List<AudioClip> m_AudioClips;

	private static ItemIDManager m_idManager;

	public override void OnInteract()
	{
		PlayEatSound();
		Dispose();
	}

	public void PlayEatSound()
	{
		GameManager.Instance.Heal();
		if (m_AudioClips != null && m_AudioClips.Count > 0)
		{
			int index = Random.Range(0, m_AudioClips.Count);
			GameManager.Instance.AudioManager.Play(m_AudioClips[index]);
		}
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
}
