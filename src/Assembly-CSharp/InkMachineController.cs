using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class InkMachineController : TMGMonoBehaviour
{
	[Header("GameObjects")]
	[SerializeField]
	private GameObject m_ActiveGameObjects;

	[Header("Transforms")]
	[SerializeField]
	private InkMachineRunningController m_InkMachine;

	[SerializeField]
	private Transform m_Chains;

	[Header("Particles")]
	[SerializeField]
	private List<ParticleSystem> m_SteamJets;

	[Header("Audio")]
	[SerializeField]
	private Transform m_AudioProxy;

	private S13Switch m_AudioSwitch;

	private AudioClip m_StressPipesClip;

	private bool m_IsInkMachineActive;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_StressPipesClip = GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/SFX/SFX_Pipes_Stress_01");
		m_ActiveGameObjects.SetActive(false);
		m_AudioSwitch = ((Component)m_AudioProxy).GetComponentInChildren<S13Switch>();
	}

	public void ShowSteam()
	{
		S13AudioManager.Instance.PlayAudio("sfx_bert_steam_bursts");
		for (int i = 0; i < m_SteamJets.Count; i++)
		{
			m_SteamJets[i].Play();
		}
	}

	public void TurnOn()
	{
		GameManager.Instance.AudioManager.Play(m_StressPipesClip);
		m_InkMachine.Activate();
		m_AudioSwitch.Play("run");
		m_AudioSwitch.Play("flow");
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetRelative<Tweener>(ShortcutExtensions.DOLocalMoveY(m_InkMachine.transform, 0.15f, 0.5f, false)), (Ease)7), -1, (LoopType)1);
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetRelative<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Chains, -0.15f, 0.5f, false)), (Ease)7), -1, (LoopType)1);
		m_ActiveGameObjects.SetActive(true);
	}

	public void TurnOff()
	{
		ShortcutExtensions.DOKill((Component)(object)m_InkMachine.transform, false);
		ShortcutExtensions.DOKill((Component)(object)m_Chains, false);
	}

	protected override void OnDisposed()
	{
		TurnOff();
		m_StressPipesClip = null;
		m_AudioSwitch = null;
		base.OnDisposed();
	}
}
