using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class InkMachineLoopController : TMGMonoBehaviour
{
	private const float CHAIN_HEIGHT = 75f;

	[SerializeField]
	private Transform m_InkMachine;

	[SerializeField]
	private List<Transform> m_Chains;

	[SerializeField]
	private List<Transform> m_SideChains;

	private Vector3 m_TopPosition;

	private Vector3 m_BottomPosition;

	private Sequence m_Sequence;

	private Transform m_LowestChain;

	private Transform m_HighestChain;

	private float m_MoveSpeed = 10f;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if (m_Chains.Count > 0)
		{
			m_LowestChain = m_Chains[0];
		}
		if (m_SideChains.Count > 0)
		{
			m_HighestChain = m_SideChains[0];
		}
	}

	public void Activate()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		ResetSequence();
		S13AudioManager.Instance.InvokeEvent("evt_ink_machine_passby_start");
		for (int i = 0; i < m_Chains.Count; i++)
		{
			Transform val = m_Chains[i];
			Vector3 val2 = val.localPosition - new Vector3(0f, 75f, 0f);
			TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(val, val2, m_MoveSpeed, false), (Ease)1));
		}
		for (int j = 0; j < m_SideChains.Count; j++)
		{
			Transform val3 = m_SideChains[j];
			Vector3 val4 = val3.localPosition + new Vector3(0f, 75f, 0f);
			TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(val3, val4, m_MoveSpeed, false), (Ease)1));
		}
		Vector3 val5 = m_InkMachine.localPosition - new Vector3(0f, 75f, 0f);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_InkMachine, val5, m_MoveSpeed, false), (Ease)1), (TweenCallback)delegate
		{
			ShortcutExtensions.DOKill((Component)(object)m_InkMachine, false);
			((Component)m_InkMachine).gameObject.SetActive(false);
		});
		TweenSettingsExtensions.OnComplete<Sequence>(m_Sequence, new TweenCallback(InitialTweenOnComplete));
	}

	private void InitialTweenOnComplete()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		m_TopPosition = m_Chains[m_Chains.Count - 1].localPosition;
		m_BottomPosition = m_SideChains[0].localPosition;
		RunChainSequence();
	}

	private void RunChainSequence()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Sequence>(DOChains(), new TweenCallback(RunChainSequence));
	}

	private Sequence DOChains()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		ResetSequence();
		int index = m_SideChains.Count - 1;
		m_HighestChain = m_SideChains[index];
		m_SideChains.RemoveAt(index);
		m_SideChains.Insert(0, m_HighestChain);
		m_LowestChain = m_Chains[0];
		m_Chains.RemoveAt(0);
		m_Chains.Add(m_LowestChain);
		for (int i = 0; i < m_Chains.Count; i++)
		{
			Transform val = m_Chains[i];
			Vector3 val2 = val.localPosition - new Vector3(0f, 75f, 0f);
			TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(val, val2, m_MoveSpeed, false), (Ease)1));
		}
		for (int j = 0; j < m_SideChains.Count; j++)
		{
			Transform val3 = m_SideChains[j];
			Vector3 val4 = val3.localPosition + new Vector3(0f, 75f, 0f);
			TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(val3, val4, m_MoveSpeed, false), (Ease)1));
		}
		TweenSettingsExtensions.InsertCallback(m_Sequence, m_MoveSpeed, (TweenCallback)delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			m_LowestChain.localPosition = m_TopPosition;
			m_HighestChain.localPosition = m_BottomPosition;
		});
		return m_Sequence;
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	protected override void OnDisposed()
	{
		KillSequence();
		base.OnDisposed();
	}
}
