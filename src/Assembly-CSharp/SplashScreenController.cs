using DG.Tweening;
using TMG.UI;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreenController : BaseUIController
{
	[SerializeField]
	private Image Logo;

	private AudioSource m_AudioSource;

	public override void InitController(object _data)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		m_AudioSource = ((Component)this).GetComponent<AudioSource>();
		((Graphic)Logo).color = new Color(((Graphic)Logo).color.r, ((Graphic)Logo).color.g, ((Graphic)Logo).color.b, 0f);
		ShowTMGLogo();
	}

	private void ShowTMGLogo()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(val, 1f, (TweenCallback)delegate
		{
			m_AudioSource.Play();
		});
		TweenSettingsExtensions.Insert(val, 1f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)((Graphic)Logo).rectTransform, 0.4f, 5f), (Ease)1));
		TweenSettingsExtensions.Insert(val, 1f, (Tween)(object)Logo.DOFade(1f, 1f));
		TweenSettingsExtensions.Insert(val, 4f, (Tween)(object)Logo.DOFade(0f, 1f));
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(ShowTMGLogoOnComplete));
	}

	private void ShowTMGLogoOnComplete()
	{
		PlayOut();
	}

	private void ShowRTLogo()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(ShowRTLogoOnComplete));
	}

	private void ShowRTLogoOnComplete()
	{
		ShowVSEvilLogo();
	}

	private void ShowVSEvilLogo()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(ShowVSEvilLogoOnComplete));
	}

	private void ShowVSEvilLogoOnComplete()
	{
		PlayOut();
	}

	protected override void OnDisposed()
	{
		m_AudioSource = null;
		base.OnDisposed();
	}
}
