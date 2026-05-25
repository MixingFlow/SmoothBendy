using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class BasicAnimationController : TMGMonoBehaviour
{
	[SerializeField]
	private List<Animator> m_Animators = new List<Animator>();

	[SerializeField]
	private bool m_IsAuto;

	public override void Init()
	{
		base.Init();
		if (m_IsAuto)
		{
			GetAnimators(base.gameObject);
			Animator[] componentsInChildren = ((Component)this).GetComponentsInChildren<Animator>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].updateMode = (AnimatorUpdateMode)0;
			}
		}
	}

	public void Play()
	{
		for (int i = 0; i < m_Animators.Count; i++)
		{
			m_Animators[i].speed = 1f;
		}
	}

	public void Stop()
	{
		for (int i = 0; i < m_Animators.Count; i++)
		{
			m_Animators[i].speed = 0f;
		}
	}

	private void GetAnimators(GameObject go)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		Animator[] componentsInChildren = ((Component)this).GetComponentsInChildren<Animator>();
		if (componentsInChildren != null)
		{
			Animator[] array = componentsInChildren;
			foreach (Animator item in array)
			{
				if (!m_Animators.Contains(item))
				{
					m_Animators.Add(item);
				}
			}
		}
		foreach (Transform item2 in go.transform)
		{
			Transform val = item2;
			GetAnimators(((Component)val).gameObject);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
