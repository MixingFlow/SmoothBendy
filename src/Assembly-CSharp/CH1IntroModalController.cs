using DG.Tweening;
using TMG.Controls;
using TMG.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CH1IntroModalController : BaseUIController
{
	[Header("RectTransfrm")]
	[SerializeField]
	private RectTransform m_Visuals;

	[Header("Image")]
	[SerializeField]
	private Image m_Blocker;

	[SerializeField]
	private Image m_Paper;

	[SerializeField]
	private Image m_LightFlickerImage;

	[SerializeField]
	private Image m_BlackOut;

	[Header("TextMeshPro")]
	[SerializeField]
	private TextMeshProUGUI m_Message;

	[SerializeField]
	private TextMeshProUGUI m_ContinueLbl;

	[Header("Touch Controls")]
	[SerializeField]
	private GameObject m_TouchArea;

	private bool m_CanContinue;

	public override void InitController(object _data)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		m_TouchArea.SetActive(false);
		((Transform)m_Visuals).localScale = Vector3.one * 1.5f;
		m_ContinueLbl.alpha = 0f;
		((Graphic)m_BlackOut).color = new Color(((Graphic)m_BlackOut).color.r, ((Graphic)m_BlackOut).color.g, ((Graphic)m_BlackOut).color.b, 1f);
	}

	public override void PlayIn()
	{
		m_BlackOut.DOFade(0f, 1f);
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_LightFlickerImage.DOFade(0.1f, 0.1f), (Ease)1), -1, (LoopType)1);
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_Visuals, 1.55f, 10f), (Ease)7), -1, (LoopType)1);
		base.PlayIn();
	}

	public override void PlayInComplete()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(m_ContinueLbl.DOFade(0.7f, 0.5f), 2f), (TweenCallback)delegate
		{
			m_TouchArea.SetActive(true);
			m_CanContinue = true;
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_ContinueLbl.DOFade(0.5f, 0.25f), (Ease)7), -1, (LoopType)1);
		});
		base.PlayInComplete();
	}

	private void Update()
	{
		if (!base.IsDisposed && m_CanContinue && PlayerInput.Any())
		{
			BeginPlayOut();
		}
	}

	public void BeginPlayOut()
	{
		m_CanContinue = false;
		m_TouchArea.SetActive(false);
		Kill();
	}

	public override void PlayOut()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)m_ContinueLbl, false);
		m_ContinueLbl.DOFade(0f, 0.5f);
		TweenSettingsExtensions.OnComplete<Tweener>(m_BlackOut.DOFade(1f, 0.5f), new TweenCallback(PlayOutComplete));
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Visuals, false);
		ShortcutExtensions.DOKill((Component)(object)m_BlackOut, false);
		ShortcutExtensions.DOKill((Component)(object)m_LightFlickerImage, false);
		base.OnDisposed();
	}
}
