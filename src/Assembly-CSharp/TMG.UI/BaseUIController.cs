using System;
using TMG.Core;
using UnityEngine;

namespace TMG.UI;

public class BaseUIController : TMGMonoBehaviour
{
	private RectTransform m_RectTransform;

	public RectTransform rectTransform
	{
		get
		{
			if ((Object)(object)m_RectTransform == (Object)null)
			{
				m_RectTransform = ((Component)this).GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	public event EventHandler OnPlayInComplete;

	public event EventHandler OnPlayOutComplete;

	public virtual void InitController(object _data)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Canvas component = ((Component)this).GetComponent<Canvas>();
		Camera val = (component.worldCamera = GameManager.Instance.UIManager.Camera);
		component.pixelPerfect = false;
		component.planeDistance = Math.Abs(((Component)component).transform.parent.position.z - ((Component)val).transform.position.z);
		component.sortingOrder = (int)(0f - component.planeDistance);
	}

	public virtual void PlayIn()
	{
		PlayInComplete();
	}

	public virtual void PlayInComplete()
	{
		this.OnPlayInComplete.Send(this);
	}

	public void Kill()
	{
		PlayOut();
	}

	public virtual void PlayOut()
	{
		PlayOutComplete();
	}

	public virtual void PlayOutComplete()
	{
		this.OnPlayOutComplete.Send(this);
		Dispose();
	}
}
