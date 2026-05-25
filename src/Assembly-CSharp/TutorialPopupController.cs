using DG.Tweening;
using InControl;
using TMG.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupController : BaseUIController
{
	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private TextMeshProUGUI m_ActionLbl;

	[Header("Controller Bindings")]
	[SerializeField]
	private GameObject m_ButtonImage;

	[SerializeField]
	private TextMeshProUGUI m_ButtonLbl;

	[Header("Keyboard Bindings")]
	[SerializeField]
	private GameObject m_KeyboardImage;

	[SerializeField]
	private TextMeshProUGUI m_KeyboardLbl;

	[Header("Mouse Bindings")]
	[SerializeField]
	private GameObject m_MouseImageLeft;

	[SerializeField]
	private GameObject m_MouseImageRight;

	[SerializeField]
	private GameObject m_MouseImageMiddle;

	[Header("GFX Bindings")]
	[SerializeField]
	private ControllerInputMappingGraphics gfxMappings;

	[SerializeField]
	private MouseAndKeyboardInputMappingGraphics mouseAndKeyboardGfxMappings;

	private TutorialDataVO m_DataVO;

	public override void InitController(object _data)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		m_DataVO = _data as TutorialDataVO;
		if (GameManager.Instance.HasController)
		{
			InputControlType inputTypeForAction = GetInputTypeForAction(m_DataVO.ActionRaw);
			m_ButtonImage.SetActive(true);
			m_ButtonImage.GetComponent<Image>().sprite = gfxMappings.GetSpriteForInputAndPlatform(inputTypeForAction, Application.platform);
			((Component)m_ButtonLbl).gameObject.SetActive(false);
		}
		else if (m_DataVO.ActionRaw == "Tutorial/TUTORIAL_ATTACK")
		{
			m_ButtonImage.SetActive(true);
			((Component)m_ButtonLbl).gameObject.SetActive(false);
			m_ButtonImage.GetComponent<Image>().sprite = mouseAndKeyboardGfxMappings.GetSpriteForInput((KeyCode)323);
		}
		else if (m_DataVO.ActionRaw == "Tutorial/TUTORIAL_SEEING_TOOL")
		{
			m_ButtonImage.SetActive(true);
			((Component)m_ButtonLbl).gameObject.SetActive(false);
			m_ButtonImage.GetComponent<Image>().sprite = mouseAndKeyboardGfxMappings.GetSpriteForInput((KeyCode)324);
		}
		else if (true)
		{
			m_KeyboardImage.SetActive(true);
			if (m_DataVO.ActionRaw == "Tutorial/TUTORIAL_JUMP")
			{
				m_KeyboardLbl.text = "SPACE";
			}
			if (m_DataVO.ActionRaw == "Tutorial/TUTORIAL_RUN")
			{
				m_KeyboardLbl.text = "L-SHIFT";
			}
			if (m_DataVO.ActionRaw == "Tutorial/TUTORIAL_INTERACT")
			{
				m_KeyboardLbl.text = "E";
			}
		}
		else
		{
			int mouseButtonBinding = GetMouseButtonBinding();
			if (mouseButtonBinding == 1)
			{
				m_MouseImageLeft.SetActive(true);
			}
			if (mouseButtonBinding == 2)
			{
				m_MouseImageRight.SetActive(true);
			}
			if (mouseButtonBinding == 3)
			{
				m_MouseImageMiddle.SetActive(true);
			}
		}
		m_ActionLbl.text = m_DataVO.Action;
		m_CanvasGroup.alpha = 0f;
	}

	public override void PlayIn()
	{
		Show();
		PlayInComplete();
	}

	public void Show()
	{
		ShortcutExtensions.DOKill((Component)(object)m_CanvasGroup, false);
		m_CanvasGroup.DOFade(1f, 0.5f);
	}

	public void Hide()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)m_CanvasGroup, false);
		TweenSettingsExtensions.OnComplete<Tweener>(m_CanvasGroup.DOFade(0f, 0.5f), new TweenCallback(base.Kill));
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_CanvasGroup, false);
		base.OnDisposed();
	}

	private int GetMouseButtonBinding()
	{
		return Random.Range(1, 4);
	}

	private InputControlType GetInputTypeForAction(string action)
	{
		if (m_DataVO.ActionRaw == "Tutorial/TUTORIAL_JUMP")
		{
			return InputControlType.Action1;
		}
		if (m_DataVO.ActionRaw == "Tutorial/TUTORIAL_RUN")
		{
			return InputControlType.LeftBumper;
		}
		if (m_DataVO.ActionRaw == "Tutorial/TUTORIAL_INTERACT")
		{
			return InputControlType.Action3;
		}
		if (m_DataVO.ActionRaw == "Tutorial/TUTORIAL_ATTACK")
		{
			return InputControlType.RightTrigger;
		}
		if (m_DataVO.ActionRaw == "Tutorial/TUTORIAL_SEEING_TOOL")
		{
			return InputControlType.LeftTrigger;
		}
		Debug.LogError((object)("Unknown Tutorial action - '" + m_DataVO.ActionRaw + "'"), (Object)(object)this);
		return InputControlType.None;
	}
}
