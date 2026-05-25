using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai;

public class AiThoughtMachine : AiThoughtMachineBehaviour
{
	[Header("Debug Options")]
	[SerializeField]
	protected bool m_CanDebug = true;

	private const string m_ThoughtEnterPrefix = "T_Enter";

	private const string m_ThoughtBehaviourPrefix = "T_";

	private List<IEnumerator> m_ThoughtCoroutines;

	private int m_ThoughtCoroutinesIndex;

	private int m_ThoughtIndexCount;

	public bool IsActive { get; private set; }

	public AiThought CurrentThought { get; private set; }

	public AiThought PreviousThought { get; private set; }

	public override void Activate()
	{
		base.Activate();
		IsActive = true;
		InitThoughtMachine();
	}

	private void InitThoughtMachine()
	{
		ClearThoughtCoroutines();
		m_ThoughtCoroutines = new List<IEnumerator>();
		m_ThoughtIndexCount = Enum.GetValues(typeof(AiThought)).Length;
		InitThoughtCoroutines();
		TriggerNextThoughtCoroutine();
	}

	private void InitThoughtCoroutines()
	{
		m_ThoughtCoroutines.Clear();
		m_ThoughtCoroutinesIndex = -1;
		for (int i = 0; i < m_ThoughtIndexCount; i++)
		{
			AiThought aiThought = (AiThought)i;
			string enter = "T_Enter" + aiThought;
			string behaviour = "T_" + aiThought;
			m_ThoughtCoroutines.Add(OnEnterThought(i, enter, behaviour));
		}
		m_ThoughtCoroutines.Add(ResetThoughtCoroutines());
	}

	private void TriggerNextThoughtCoroutine()
	{
		m_ThoughtCoroutinesIndex++;
		((MonoBehaviour)this).StartCoroutine(m_ThoughtCoroutines[m_ThoughtCoroutinesIndex]);
	}

	private IEnumerator Thoughts()
	{
		while (!base.IsDisposed)
		{
			for (int i = 0; i < m_ThoughtCoroutines.Count; i++)
			{
				yield return ((MonoBehaviour)this).StartCoroutine(m_ThoughtCoroutines[i]);
			}
			yield return (object)new WaitForEndOfFrame();
		}
	}

	private IEnumerator OnEnterThought(int _currentThought, string _enter, string _behaviour)
	{
		bool hasEnteredThought = true;
		while (CurrentThought == (AiThought)_currentThought)
		{
			while (GameManager.Instance.isPaused)
			{
				yield return null;
			}
			if (!IsActive || base.IsDisposed)
			{
				yield break;
			}
			if (hasEnteredThought)
			{
				((MonoBehaviour)this).Invoke(_enter, 0f);
				hasEnteredThought = false;
			}
			((MonoBehaviour)this).Invoke(_behaviour, 0f);
			if (CurrentThought == AiThought.Die || _currentThought == 10)
			{
				yield break;
			}
			yield return (object)new WaitForEndOfFrame();
		}
		TriggerNextThoughtCoroutine();
	}

	private IEnumerator ResetThoughtCoroutines()
	{
		InitThoughtCoroutines();
		TriggerNextThoughtCoroutine();
		yield return null;
	}

	public void SetThought(int _index)
	{
		SetThought((AiThought)_index);
	}

	public void SetThought(AiThought _thought)
	{
		if (_thought != CurrentThought)
		{
			PreviousThought = CurrentThought;
			CurrentThought = _thought;
		}
	}

	public void RevertToPreviousThought()
	{
		SetThought(PreviousThought);
	}

	public bool IsInThought(int _index)
	{
		return IsInThought((AiThought)_index);
	}

	public bool IsInThought(AiThought _thought)
	{
		return _thought == CurrentThought;
	}

	public bool WasInThought(int _index)
	{
		return WasInThought((AiThought)_index);
	}

	public bool WasInThought(AiThought _thought)
	{
		return _thought == PreviousThought;
	}

	public void StartStateMachine()
	{
		IsActive = true;
		TriggerNextThoughtCoroutine();
	}

	public void StopStateMachine()
	{
		IsActive = false;
	}

	private void ClearThoughtCoroutines()
	{
		if (m_ThoughtCoroutines != null)
		{
			m_ThoughtCoroutines.Clear();
			m_ThoughtCoroutines = null;
		}
	}
}
