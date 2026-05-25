using TMG.Core;
using UnityEngine;

public class CH5BoneController : TMGMonoBehaviour
{
	private CH5Bone[] m_Bones;

	public override void Init()
	{
		base.Init();
		m_Bones = Resources.FindObjectsOfTypeAll<CH5Bone>();
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_Bones.Length; i++)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data != null)
			{
				((Component)m_Bones[i]).gameObject.SetActive(GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HasBorisBone);
			}
			else
			{
				((Component)m_Bones[i]).gameObject.SetActive(false);
			}
		}
	}
}
