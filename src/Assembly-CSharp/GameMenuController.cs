using System;
using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using InControl;
using S13Audio;
using TMG.Controls;
using TMG.UI.Controls;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : AbstractGameMenuController
{
	private enum PauseMenu
	{
		PAUSE,
		OPTIONS,
		QUITTING
	}

	private enum MenuItems
	{
		RESUME,
		OPTIONS,
		QUIT
	}

	[Header("Canvas")]
	[SerializeField]
	private CanvasGroup m_BlackoutCanvas;

	[Header("TextMeshPro")]
	[SerializeField]
	private TextMeshProUGUI m_ObjectiveHeaderLbl;

	[SerializeField]
	private TextMeshProUGUI m_ObjectiveMessageLbl;

	[SerializeField]
	private TextMeshProUGUI m_ObjectiveTipLbl;

	[Header("Objectives")]
	[SerializeField]
	private RectTransform m_ItemCounterContainer;

	[SerializeField]
	private Image m_ItemCountImage;

	[SerializeField]
	private TextMeshProUGUI m_ItemCountLbl;

	[SerializeField]
	private RectTransform m_ItemListContainer;

	[SerializeField]
	private Image m_ItemListImageCopy;

	[Header("RectTransforms")]
	[SerializeField]
	private List<RectTransform> m_InkFlow_01;

	[SerializeField]
	private List<RectTransform> m_InkFlow_02;

	[Header("Pause Buttons")]
	[SerializeField]
	private MenuItemButton m_ResumeBtn;

	[SerializeField]
	private MenuItemButton m_OptionsBtn;

	[SerializeField]
	private MenuItemButton m_QuitBtn;

	[Header("Quit Prompt")]
	[SerializeField]
	private GameObject m_QuitPromptBlocker;

	[SerializeField]
	private RectTransform m_QuitPromptRect;

	[SerializeField]
	private CanvasGroup m_QuitPrompt;

	[SerializeField]
	private BaseUIButton m_QuitOKBtn;

	[SerializeField]
	private BaseUIButton m_QuitCancelBtn;

	private List<MenuItemButton> m_MenuItemButtons = new List<MenuItemButton>();

	private OptionsMenuController m_OptionsMenuController;

	private PauseMenu m_CurrentMenu;

	private Sequence m_InkFlowSequence_01;

	private Sequence m_InkFlowSequence_02;

	private int m_SelectedIndex;

	private int m_IndexMax = 3;

	private float m_SelectTimeNext;

	private float m_SelectTimeRate = 0.15f;

	public bool IsQuitting { get; private set; }

	public override void InitController(object _data)
	{
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		ObjectiveDataVO currentObjective = GameManager.Instance.CurrentObjective;
		if (currentObjective != null)
		{
			m_ObjectiveHeaderLbl.text = LocalizationManager.GetTermTranslation("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true);
			m_ObjectiveMessageLbl.text = currentObjective.Objective;
			if (string.IsNullOrEmpty(currentObjective.Tip))
			{
				((Component)m_ObjectiveTipLbl).gameObject.SetActive(false);
				m_ObjectiveTipLbl.text = currentObjective.Tip;
			}
			else
			{
				((Component)m_ObjectiveTipLbl).gameObject.SetActive(true);
				m_ObjectiveTipLbl.text = currentObjective.Tip;
			}
			if ((Object)(object)currentObjective.Item != (Object)null)
			{
				((Component)m_ItemListContainer).gameObject.SetActive(false);
				((Component)m_ItemCounterContainer).gameObject.SetActive(true);
				m_ItemCountLbl.text = currentObjective.ItemCounter.ToString();
				m_ItemCountImage.sprite = currentObjective.Item;
			}
			else if (currentObjective.Items != null && currentObjective.Items.Count > 0)
			{
				((Component)m_ItemCounterContainer).gameObject.SetActive(false);
				((Component)m_ItemListContainer).gameObject.SetActive(true);
				for (int i = 0; i < currentObjective.Items.Count; i++)
				{
					Sprite sprite = currentObjective.Items[i];
					Image val = Object.Instantiate<Image>(m_ItemListImageCopy);
					((Transform)((Graphic)val).rectTransform).SetParent((Transform)(object)m_ItemListContainer);
					((Transform)((Graphic)val).rectTransform).localPosition = Vector3.zero;
					((Transform)((Graphic)val).rectTransform).localEulerAngles = Vector3.zero;
					((Transform)((Graphic)val).rectTransform).localScale = Vector3.one;
					val.sprite = sprite;
					((Graphic)val).color = ((!currentObjective.Collected[i]) ? new Color(0.5f, 0.5f, 0.5f, 0.75f) : new Color(1f, 1f, 1f, 1f));
				}
				((Component)m_ItemListImageCopy).gameObject.SetActive(false);
			}
			else
			{
				((Component)m_ItemCounterContainer).gameObject.SetActive(false);
				((Component)m_ItemListContainer).gameObject.SetActive(false);
			}
		}
		else
		{
			((Component)m_ItemCounterContainer).gameObject.SetActive(false);
			((Component)m_ItemListContainer).gameObject.SetActive(false);
		}
		m_QuitPromptBlocker.SetActive(false);
		((Component)m_QuitPrompt).gameObject.SetActive(false);
		m_BlackoutCanvas.alpha = 0f;
		((Component)m_BlackoutCanvas).gameObject.SetActive(false);
		SetupMenu();
		SetNavigationInput(hasSelect: true, hasBack: false);
		if (GameManager.Instance.HasController)
		{
			m_ResumeBtn.Button.OnPointerEnter(null);
			m_OptionsBtn.Button.OnPointerExit(null);
			m_QuitBtn.Button.OnPointerExit(null);
		}
		CheckForController();
		CheckInitialInput();
		InputManager.OnDeviceAttached += HandleInputManagerOnDeviceAttached;
		InputManager.OnDeviceDetached += HandleInputManagerOnDeviceDetached;
		GameManager.Instance.PauseGame();
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		SetInkLoop(0, 2f);
		SetInkLoop(1, 4f);
	}

	private void SetupMenu()
	{
		GetButtonData(ref m_ResumeBtn, "MENU/MENU_RESUME", CheckSelection);
		GetButtonData(ref m_OptionsBtn, "MENU/MENU_OPTIONS", CheckSelection);
		GetButtonData(ref m_QuitBtn, "MENU/MENU_QUIT", CheckSelection);
	}

	private void GetButtonData(ref MenuItemButton button, string title, Action callback)
	{
		button.Init(MenuItemButtonDataVO.Create(title, callback));
		button.OnSelected += HandleMenuItemOnSelected;
		m_MenuItemButtons.Add(button);
	}

	private void CheckInitialInput()
	{
		if (GameManager.Instance.HasController)
		{
			Cursor.lockState = (CursorLockMode)1;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = (CursorLockMode)0;
			Cursor.visible = true;
		}
	}

	private void HandleInputManagerOnDeviceAttached(InputDevice obj)
	{
		CheckForController();
	}

	private void HandleInputManagerOnDeviceDetached(InputDevice obj)
	{
		CheckForController();
		for (int i = 0; i < m_MenuItemButtons.Count; i++)
		{
			m_MenuItemButtons[i].Button.OnPointerExit(null);
		}
	}

	private void CheckForController()
	{
	}

	public void SetNavigationInput(bool hasSelect, bool hasBack)
	{
	}

	private void HandleMenuItemOnSelected(object sender, EventArgs e)
	{
		for (int i = 0; i < m_MenuItemButtons.Count; i++)
		{
			MenuItemButton menuItemButton = m_MenuItemButtons[i];
			if (((object)menuItemButton).Equals(sender))
			{
				m_SelectedIndex = i;
			}
			else
			{
				menuItemButton.Deselect();
			}
		}
	}

	private void SetInkLoop(int index, float speed)
	{
		switch (index)
		{
		case 0:
			LoopInk(index, ref m_InkFlow_01, speed);
			break;
		case 1:
			LoopInk(index, ref m_InkFlow_02, speed);
			break;
		}
	}

	private void LoopInk(int index, ref List<RectTransform> inkFlows, float speed)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		RectTransform val = inkFlows[inkFlows.Count - 1];
		val.anchoredPosition = inkFlows[0].anchoredPosition + new Vector2(0f, 1024f);
		inkFlows.RemoveAt(inkFlows.Count - 1);
		inkFlows.Insert(0, val);
		if (index == 0)
		{
			ResetInkFlowSequence_01();
		}
		else
		{
			ResetInkFlowSequence_02();
		}
		Sequence val2 = ((index != 0) ? m_InkFlowSequence_02 : m_InkFlowSequence_01);
		TweenSettingsExtensions.SetUpdate<Sequence>(val2, (UpdateType)0);
		for (int i = 0; i < inkFlows.Count; i++)
		{
			RectTransform target = inkFlows[i];
			TweenSettingsExtensions.Insert(val2, 0f, (Tween)(object)TweenSettingsExtensions.SetRelative<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(target.DOAnchorPosY(-1024f, speed), (Ease)1), true));
		}
		TweenSettingsExtensions.OnComplete<Sequence>(val2, (TweenCallback)delegate
		{
			SetInkLoop(index, speed);
		});
	}

	private void ResetInkFlowSequence_01()
	{
		KillInkFlowSequence_01();
		m_InkFlowSequence_01 = DOTween.Sequence();
	}

	private void KillInkFlowSequence_01()
	{
		if (m_InkFlowSequence_01 != null)
		{
			TweenExtensions.Kill((Tween)(object)m_InkFlowSequence_01, false);
			m_InkFlowSequence_01 = null;
		}
	}

	private void ResetInkFlowSequence_02()
	{
		KillInkFlowSequence_02();
		m_InkFlowSequence_02 = DOTween.Sequence();
	}

	private void KillInkFlowSequence_02()
	{
		if (m_InkFlowSequence_02 != null)
		{
			TweenExtensions.Kill((Tween)(object)m_InkFlowSequence_02, false);
			m_InkFlowSequence_02 = null;
		}
	}

	private void Update()
	{
		if (!GameManager.Instance.HasController || m_CurrentMenu == PauseMenu.OPTIONS)
		{
			return;
		}
		if (m_CurrentMenu == PauseMenu.QUITTING)
		{
			if (PlayerInput.Jump())
			{
				Quit();
			}
			if (PlayerInput.BackOnPressed())
			{
				CancelQuit();
			}
			return;
		}
		float axis = 0f;
		if (InputUtil.GetInputY(out axis))
		{
			if (Time.unscaledTime > m_SelectTimeNext)
			{
				m_SelectTimeNext = Time.unscaledTime + m_SelectTimeRate;
				if (axis > 0.3f)
				{
					m_SelectedIndex--;
				}
				else if (axis < -0.3f)
				{
					m_SelectedIndex++;
				}
				if (m_SelectedIndex >= m_IndexMax)
				{
					m_SelectedIndex = 0;
				}
				else if (m_SelectedIndex < 0)
				{
					m_SelectedIndex = m_IndexMax - 1;
				}
				CheckActiveMenuItemButtons();
			}
			if (axis > -0.01f && axis < 0.01f)
			{
				m_SelectTimeNext = 0f;
			}
		}
		if (PlayerInput.Jump())
		{
			CheckSelection();
		}
		if (PlayerInput.BackOnPressed())
		{
			GameManager.Instance.Unpause();
		}
	}

	private void CheckActiveMenuItemButtons()
	{
		if (m_MenuItemButtons == null)
		{
			return;
		}
		for (int i = 0; i < m_MenuItemButtons.Count; i++)
		{
			if (i == m_SelectedIndex)
			{
				m_MenuItemButtons[i].Button.OnPointerEnter(null);
			}
			else
			{
				m_MenuItemButtons[i].Button.OnPointerExit(null);
			}
		}
	}

	private void CheckSelection()
	{
		switch (m_SelectedIndex)
		{
		case 0:
			GameManager.Instance.Unpause();
			break;
		case 1:
			ShowOptionsMenu();
			break;
		case 2:
			ShowQuitPrompt();
			break;
		}
	}

	private void ShowOptionsMenu()
	{
		m_CurrentMenu = PauseMenu.OPTIONS;
		m_OptionsMenuController = GameManager.Instance.UIManager.Show<OptionsMenuController>("UI/Menus/OptionsMenuController", "PAUSE");
		m_OptionsMenuController.OnPlayOutComplete += HandleOptionsMenuControllerOnPlayOutComplete;
		((Component)m_Visuals).gameObject.SetActive(false);
	}

	private void HandleOptionsMenuControllerOnPlayOutComplete(object sender, EventArgs e)
	{
		m_OptionsMenuController.OnPlayOutComplete -= HandleOptionsMenuControllerOnPlayOutComplete;
		m_OptionsMenuController = null;
		((Component)m_Visuals).gameObject.SetActive(true);
		m_CurrentMenu = PauseMenu.PAUSE;
	}

	private void ShowQuitPrompt()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_game_at_quit_prompt");
		m_CurrentMenu = PauseMenu.QUITTING;
		m_QuitPromptBlocker.SetActive(true);
		((Component)m_QuitPrompt).gameObject.SetActive(true);
		m_QuitPrompt.alpha = 0f;
		TweenSettingsExtensions.SetUpdate<Tweener>(m_QuitPrompt.DOFade(1f, 0.5f), (UpdateType)0, true);
		((Transform)m_QuitPromptRect).localScale = Vector3.one * 0.8f;
		TweenSettingsExtensions.SetUpdate<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_QuitPromptRect, 1f, 0.5f), (UpdateType)0, true);
		IsQuitting = true;
		m_QuitOKBtn.OnClick += HandleQuitOKBtnOnClick;
		m_QuitCancelBtn.OnClick += HandleQuitCancelBtnOnClick;
	}

	private void HandleQuitOKBtnOnClick(object sender, EventArgs e)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		m_QuitOKBtn.OnClick -= HandleQuitOKBtnOnClick;
		m_QuitCancelBtn.OnClick -= HandleQuitCancelBtnOnClick;
		((Component)m_BlackoutCanvas).gameObject.SetActive(true);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetUpdate<Tweener>(m_BlackoutCanvas.DOFade(1f, 1f), (UpdateType)0, true), new TweenCallback(Quit));
	}

	private void Quit()
	{
		((Behaviour)GameManager.Instance.GameCamera.Camera).enabled = false;
		m_BlackoutCanvas.alpha = 1f;
		GameManager.Instance.ShowScreenBlocker(0f);
		((Behaviour)GameManager.Instance.UIManager.Camera).enabled = false;
		SceneManager.LoadScene("Reset");
		Kill();
	}

	private void HandleQuitCancelBtnOnClick(object sender, EventArgs e)
	{
		m_QuitOKBtn.OnClick -= HandleQuitOKBtnOnClick;
		m_QuitCancelBtn.OnClick -= HandleQuitCancelBtnOnClick;
		CancelQuit();
	}

	private void CancelQuit()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		m_CurrentMenu = PauseMenu.PAUSE;
		IsQuitting = false;
		TweenSettingsExtensions.SetUpdate<Tweener>(m_QuitPrompt.DOFade(0f, 0.4f), (UpdateType)0, true);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetUpdate<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_QuitPromptRect, 0.7f, 0.5f), (UpdateType)0, true), (TweenCallback)delegate
		{
			m_QuitPromptBlocker.SetActive(false);
			((Component)m_QuitPrompt).gameObject.SetActive(false);
		});
	}

	public void CloseMenu()
	{
		if (!IsQuitting)
		{
			Kill();
		}
	}

	public override void PlayOut()
	{
		PlayOutComplete();
	}

	public override void PlayOutComplete()
	{
		if (Object.op_Implicit((Object)(object)m_OptionsMenuController))
		{
			m_OptionsMenuController.Kill();
		}
		GameManager.Instance.Unpause();
		Cursor.visible = false;
		Cursor.lockState = (CursorLockMode)1;
		GameManager.Instance.UnpauseGame();
		base.PlayOutComplete();
	}

	protected override void OnDisposed()
	{
		KillInkFlowSequence_01();
		KillInkFlowSequence_02();
		m_OptionsMenuController = null;
		if (m_MenuItemButtons != null)
		{
			m_MenuItemButtons.Clear();
			m_MenuItemButtons = null;
		}
		base.OnDisposed();
	}
}
