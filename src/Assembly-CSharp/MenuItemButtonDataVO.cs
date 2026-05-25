using System;
using TMG.Core;

public class MenuItemButtonDataVO : TMGAbstractDisposable
{
	public string Label { get; private set; }

	public Action Callback { get; private set; }

	public static MenuItemButtonDataVO Create(string label, Action callback)
	{
		MenuItemButtonDataVO menuItemButtonDataVO = new MenuItemButtonDataVO();
		menuItemButtonDataVO.Label = label;
		menuItemButtonDataVO.Callback = callback;
		return menuItemButtonDataVO;
	}

	public void UpdateLabel(string label)
	{
		Label = label;
	}

	public void UpdateCallback(Action callback)
	{
		Callback = callback;
	}

	protected override void OnDisposed()
	{
		Callback = null;
		base.OnDisposed();
	}
}
