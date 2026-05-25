using DG.Tweening;
using TMG.Controls;
using TMG.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessScreenController : BaseUIController
{
	[Header("RectTransforms")]
	[SerializeField]
	private RectTransform m_NavContainer;

	[Header("Images")]
	[SerializeField]
	private Image m_BendyHeadImg;

	[SerializeField]
	private Image m_CircleImg;

	[SerializeField]
	private Image m_BlockerImg;

	[SerializeField]
	private Image m_Fader;

	[Header("Text")]
	[SerializeField]
	private TextMeshProUGUI m_HeaderLbl;

	[SerializeField]
	private TextMeshProUGUI m_BrightnessLbl;

	private bool m_IsActive;

	private MenuItemNavInput m_NavInput;

	public override void InitController(object _data)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		Color color = ((Graphic)m_Fader).color;
		color.a = 1f;
		((Graphic)m_Fader).color = color;
		m_NavInput = GameManager.Instance.AssetManager.CreateAsset<MenuItemNavInput>("UI/Elements/UINavInput");
		m_NavInput.Init(NavInputDataVO.Create("A", "MENU/CONTROL_CONTINUE"));
		((Transform)m_NavInput.rectTransform).SetParent((Transform)(object)m_NavContainer);
		m_NavInput.rectTransform.anchoredPosition = Vector2.zero;
	}

	public override void PlayIn()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(m_Fader.DOFade(0f, 0.5f), new TweenCallback(PlayInComplete));
	}

	public override void PlayInComplete()
	{
		m_IsActive = true;
		base.PlayInComplete();
	}

	private void Update()
	{
		if (m_IsActive && PlayerInput.Jump())
		{
			m_IsAwake = false;
			Kill();
		}
	}

	public override void PlayOut()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(m_Fader.DOFade(1f, 0.5f), new TweenCallback(PlayOutComplete));
	}

	protected override void OnDisposed()
	{
		m_NavInput = null;
		base.OnDisposed();
	}
}
