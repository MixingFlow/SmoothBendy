using System.Collections.Generic;
using DG.Tweening;
using TMG.UI;
using UnityEngine;
using UnityEngine.UI;

public class CH1ConclusionModalController : BaseUIController
{
	[Header("RectTransforms")]
	[SerializeField]
	private List<Image> m_Images;

	public override void InitController(object _data)
	{
		base.InitController(_data);
		for (int i = 0; i < m_Images.Count; i++)
		{
			((Behaviour)m_Images[i]).enabled = false;
		}
	}

	public void ShowImage(int index)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		Image image = m_Images[index];
		((Behaviour)image).enabled = true;
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(((Component)image).transform, 1.05f, 0.25f), (Ease)1);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(image.DOFade(0f, 0.125f), 0.15f), (TweenCallback)delegate
		{
			((Behaviour)image).enabled = false;
		});
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
