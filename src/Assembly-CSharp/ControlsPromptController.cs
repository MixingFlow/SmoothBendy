using System;
using TMG.UI.Controls;
using UnityEngine;

public class ControlsPromptController : AbstractPromptController
{
	[Header("Buttons")]
	[SerializeField]
	private BaseUIButton m_BackBtn;

	[SerializeField]
	private BaseUIButton m_BackBGBtn;

	public override void InitController(object _data)
	{
		base.InitController(_data);
	}

	public override void PlayInComplete()
	{
		AddListeners();
		base.PlayInComplete();
	}

	private void HandleBackBtnOnClick(object sender, EventArgs e)
	{
		RemoveListeners();
		Kill();
	}

	private void AddListeners()
	{
		m_BackBtn.OnClick += HandleBackBtnOnClick;
		m_BackBGBtn.OnClick += HandleBackBtnOnClick;
	}

	private void RemoveListeners()
	{
		m_BackBtn.OnClick -= HandleBackBtnOnClick;
		m_BackBGBtn.OnClick -= HandleBackBtnOnClick;
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
