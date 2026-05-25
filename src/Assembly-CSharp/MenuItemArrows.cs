using System;
using DG.Tweening;
using TMG.Core;
using TMG.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemArrows : TMGMonoBehaviour
{
	[SerializeField]
	private BaseUIButton m_LeftArrow;

	[SerializeField]
	private BaseUIButton m_RightArrow;

	private Sequence m_Sequence;

	private Sprite m_OriginSprite;

	public event EventHandler OnLeft;

	public event EventHandler OnRight;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_OriginSprite = ((Selectable)m_LeftArrow.Button).image.sprite;
		m_LeftArrow.OnClick += HandleLeftArrowOnClick;
		m_RightArrow.OnClick += HandleRightArrowOnClick;
	}

	private void HandleLeftArrowOnClick(object sender, EventArgs e)
	{
		this.OnLeft.Send(this);
	}

	private void HandleRightArrowOnClick(object sender, EventArgs e)
	{
		this.OnRight.Send(this);
	}

	public void TriggerLeft()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		ResetSequence();
		Image image = ((Selectable)m_LeftArrow.Button).image;
		SpriteState spriteState = ((Selectable)m_LeftArrow.Button).spriteState;
		image.sprite = ((SpriteState)(ref spriteState)).pressedSprite;
		this.OnLeft.Send(this);
		TweenSettingsExtensions.InsertCallback(m_Sequence, 0.1f, new TweenCallback(ResetSprites));
	}

	public void TriggerRight()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		ResetSequence();
		Image image = ((Selectable)m_RightArrow.Button).image;
		SpriteState spriteState = ((Selectable)m_RightArrow.Button).spriteState;
		image.sprite = ((SpriteState)(ref spriteState)).pressedSprite;
		this.OnRight.Send(this);
		TweenSettingsExtensions.InsertCallback(m_Sequence, 0.1f, new TweenCallback(ResetSprites));
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
		ResetSprites();
		m_Sequence = DOTween.Sequence();
	}

	private void ResetSprites()
	{
		((Selectable)m_LeftArrow.Button).image.sprite = m_OriginSprite;
		((Selectable)m_RightArrow.Button).image.sprite = m_OriginSprite;
	}

	protected override void OnDisposed()
	{
		KillSequence();
		this.OnLeft = null;
		this.OnRight = null;
		m_OriginSprite = null;
		base.OnDisposed();
	}
}
