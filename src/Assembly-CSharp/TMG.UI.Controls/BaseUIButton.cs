using System;
using TMG.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMG.UI.Controls;

[RequireComponent(typeof(Button))]
public class BaseUIButton : TMGMonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[Header("Image Swap")]
	[SerializeField]
	private bool m_HasImageSwap;

	[SerializeField]
	private GameObject m_ImageActive;

	[SerializeField]
	private GameObject m_ImageHighlight;

	[Header("Image BG")]
	[SerializeField]
	private bool m_HasImageBG;

	[SerializeField]
	private GameObject m_ImageBG;

	private Button m_Button;

	public Button Button
	{
		get
		{
			if ((Object)(object)m_Button == (Object)null)
			{
				m_Button = ((Component)this).GetComponent<Button>();
			}
			return m_Button;
		}
	}

	public event EventHandler OnDown;

	public event EventHandler OnUp;

	public event EventHandler OnClick;

	public event EventHandler OnSelected;

	public event EventHandler OnDeselected;

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public override void Init()
	{
		base.Init();
		if (m_HasImageSwap)
		{
			if (Object.op_Implicit((Object)(object)m_ImageActive))
			{
				m_ImageActive.SetActive(true);
			}
			if (Object.op_Implicit((Object)(object)m_ImageHighlight))
			{
				m_ImageHighlight.SetActive(false);
			}
		}
		if (m_HasImageBG && Object.op_Implicit((Object)(object)m_ImageBG))
		{
			m_ImageBG.SetActive(false);
		}
	}

	public override void OnDisable()
	{
		if (m_HasImageSwap)
		{
			if (Object.op_Implicit((Object)(object)m_ImageActive))
			{
				m_ImageActive.SetActive(true);
			}
			if (Object.op_Implicit((Object)(object)m_ImageHighlight))
			{
				m_ImageHighlight.SetActive(false);
			}
		}
		if (m_HasImageBG && Object.op_Implicit((Object)(object)m_ImageBG))
		{
			m_ImageBG.SetActive(false);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (((Selectable)Button).interactable)
		{
			this.OnDown.Send(this);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (((Selectable)Button).interactable)
		{
			this.OnUp.Send(this);
		}
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (((Selectable)Button).interactable)
		{
			this.OnClick.Send(this);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (((Selectable)Button).interactable)
		{
			this.OnSelected.Send(this);
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if (((Selectable)Button).interactable)
		{
			this.OnDeselected.Send(this);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!((Selectable)Button).interactable)
		{
			return;
		}
		if (m_HasImageSwap)
		{
			if (Object.op_Implicit((Object)(object)m_ImageActive))
			{
				m_ImageActive.SetActive(false);
			}
			if (Object.op_Implicit((Object)(object)m_ImageHighlight))
			{
				m_ImageHighlight.SetActive(true);
			}
		}
		if (m_HasImageBG && Object.op_Implicit((Object)(object)m_ImageBG))
		{
			m_ImageBG.SetActive(true);
		}
		this.OnEnter.Send(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!((Selectable)Button).interactable)
		{
			return;
		}
		if (m_HasImageSwap)
		{
			if (Object.op_Implicit((Object)(object)m_ImageActive))
			{
				m_ImageActive.SetActive(true);
			}
			if (Object.op_Implicit((Object)(object)m_ImageHighlight))
			{
				m_ImageHighlight.SetActive(false);
			}
		}
		if (m_HasImageBG && Object.op_Implicit((Object)(object)m_ImageBG))
		{
			m_ImageBG.SetActive(false);
		}
		this.OnExit.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnDown = null;
		this.OnUp = null;
		this.OnClick = null;
		this.OnSelected = null;
		this.OnDeselected = null;
		base.OnDisposed();
	}
}
