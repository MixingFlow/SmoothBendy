using DG.Tweening;
using TMG.UI;
using UnityEngine;
using UnityEngine.UI;

public class MainCrosshairController : BaseUIController
{
	[SerializeField]
	private Image m_Crosshair;

	private float CrosshairScale => GameManager.Instance.PlayerSettings.CrosshairScale;

	private float CrosshairOpacity => GameManager.Instance.PlayerSettings.CrosshairOpacity;

	public override void InitController(object _data)
	{
		base.InitController(_data);
		SetCrosshairAlpha(0f);
	}

	public override void PlayIn()
	{
		Show();
		PlayInComplete();
	}

	public void Show()
	{
		((Component)m_Crosshair).gameObject.SetActive(true);
		ShortcutExtensions.DOKill((Component)(object)m_Crosshair, false);
		SetCrosshairAlpha(CrosshairOpacity);
		SetCrosshairScale(CrosshairScale);
	}

	public void Hide()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)m_Crosshair, false);
		SetCrosshairScale(CrosshairScale);
		TweenSettingsExtensions.OnComplete<Tweener>(m_Crosshair.DOFade(0f, 0.5f), (TweenCallback)delegate
		{
			((Component)m_Crosshair).gameObject.SetActive(false);
		});
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Crosshair, false);
		base.OnDisposed();
	}

	public void FixedUpdate()
	{
		if (((Component)m_Crosshair).gameObject.activeSelf)
		{
			SetCrosshairAlpha(CrosshairOpacity);
			SetCrosshairScale(CrosshairScale);
		}
	}

	private void SetCrosshairAlpha(float alpha)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)m_Crosshair).color = new Color(((Graphic)m_Crosshair).color.r, ((Graphic)m_Crosshair).color.g, ((Graphic)m_Crosshair).color.b, alpha);
	}

	private void SetCrosshairScale(float scale)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		((Component)m_Crosshair).transform.localScale = new Vector3(scale, scale, scale);
	}
}
