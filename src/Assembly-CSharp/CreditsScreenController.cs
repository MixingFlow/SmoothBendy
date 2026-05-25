using DG.Tweening;
using TMG.Controls;
using TMG.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsScreenController : BaseUIController
{
	[Header("RectTransforms")]
	[SerializeField]
	private RectTransform m_Visuals;

	[SerializeField]
	private RectTransform m_Content;

	[SerializeField]
	private RectTransform m_BendyFace;

	[Header("Scroll View")]
	[SerializeField]
	private ScrollRect m_ScrollView;

	[Header("CanvasGroups")]
	[SerializeField]
	private CanvasGroup m_CreditsCanvasGroup;

	[SerializeField]
	private CanvasGroup m_UnlockCanvasGroup;

	[Header("GameObjects")]
	[SerializeField]
	private GameObject[] m_DisableForPC;

	[SerializeField]
	private GameObject m_Keyboard;

	[SerializeField]
	private GameObject m_Controller;

	[Header("Images")]
	[SerializeField]
	private Image m_BlackOutImage;

	private bool m_IsQuitting;

	private bool m_CanQuit;

	private bool m_CanActuallyQuit;

	public AudioObject CreditsMusic { get; private set; }

	public override void InitController(object _data)
	{
		base.InitController(_data);
		for (int i = 0; i < m_DisableForPC.Length; i++)
		{
			m_DisableForPC[i].SetActive(false);
		}
		GameManager.Instance.AudioManager.ListenerSetActive(active: true);
		GameManager.Instance.LockPause();
		GameManager.Instance.KillCrosshair();
		GameManager.Instance.HideScreenBlocker(0f);
		m_CreditsCanvasGroup.alpha = 0f;
		m_UnlockCanvasGroup.alpha = 0f;
		((Component)m_UnlockCanvasGroup).gameObject.SetActive(false);
		m_ScrollView.verticalNormalizedPosition = 1f;
		((Behaviour)m_BlackOutImage).enabled = true;
		m_Controller.SetActive(GameManager.Instance.HasController);
		m_Keyboard.SetActive(!GameManager.Instance.HasController);
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		Initialize();
	}

	private void Initialize()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		CreditsMusic = GameManager.Instance.AudioManager.Play("Audio/MUS/CH4/MUS_DeathOfAFriend", AudioObjectType.MUSIC, -1);
		TweenSettingsExtensions.OnComplete<Sequence>(DOCredits(), new TweenCallback(ActualComplete));
	}

	private void Update()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		if (!m_CanQuit || !PlayerInput.Any())
		{
			return;
		}
		if (m_CanActuallyQuit || GameManager.Instance.GameData.CurrentSaveFile.CompleteCount > 1)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_BlackOutImage.DOFade(1f, 0f), (TweenCallback)delegate
			{
				SceneManager.LoadScene("Reset");
			});
		}
		else if (!m_IsQuitting)
		{
			m_IsQuitting = true;
			TweenSettingsExtensions.OnComplete<Tweener>(CreditsMusic.AudioSource.DOFade(0f, 3f), (TweenCallback)delegate
			{
				CreditsMusic.Clear();
				CreditsMusic = null;
			});
			ActualComplete();
		}
	}

	private void ActualComplete()
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		if (GameManager.Instance.GameData.CurrentSaveFile.CompleteCount > 1)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_BlackOutImage.DOFade(1f, 0f), (TweenCallback)delegate
			{
				SceneManager.LoadScene("Reset");
			});
			return;
		}
		((Component)m_ScrollView).gameObject.SetActive(false);
		((Component)m_UnlockCanvasGroup).gameObject.SetActive(true);
		ShortcutExtensions.DOKill((Component)(object)m_UnlockCanvasGroup, false);
		TweenSettingsExtensions.OnComplete<Tweener>(m_UnlockCanvasGroup.DOFade(1f, 2f), (TweenCallback)delegate
		{
			m_CanActuallyQuit = true;
		});
	}

	private Sequence DOCredits()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		float num = 1f;
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)m_BlackOutImage.DOFade(0f, 1f));
		num += 1f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)m_CreditsCanvasGroup.DOFade(1f, 1f));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(m_ScrollView.DOVerticalNormalizedPos(0f, 110f), (Ease)1));
		TweenSettingsExtensions.InsertCallback(val, num + 10f, (TweenCallback)delegate
		{
			m_CanQuit = true;
		});
		num += 115f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(m_CreditsCanvasGroup.DOFade(0f, 3f), (Ease)1));
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			TweenSettingsExtensions.OnComplete<Tweener>(CreditsMusic.AudioSource.DOFade(0f, 3f), (TweenCallback)delegate
			{
				CreditsMusic.Clear();
				CreditsMusic = null;
			});
		});
		return val;
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)CreditsMusic != (Object)null)
		{
			CreditsMusic.Clear();
			CreditsMusic = null;
		}
		base.OnDisposed();
	}
}
