using DG.Tweening;
using UnityEngine;

public class TestScene_InkMachineShore : MonoBehaviour
{
	[SerializeField]
	private Animator m_Animator;

	private AudioClip[] m_AliceClip;

	public void Start()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		m_AliceClip = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/AliceA/GiantInkMachine");
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 2f, new TweenCallback(Go));
	}

	private void Go()
	{
		m_Animator.SetTrigger("Go");
		for (int i = 0; i < m_AliceClip.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceClip[i], SubtitleConstants.DIA_CH5_ALISONA_GIANT_INK_MACHINE[i], isTrimmed: true));
		}
	}
}
