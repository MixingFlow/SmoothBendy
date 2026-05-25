using DG.Tweening;
using TMG.UI;
using UnityEngine;
using UnityEngine.UI;

public class CollectableModalController : BaseUIController
{
	[Header("Image")]
	[SerializeField]
	private Image m_Collectable;

	private CollectableDataVO m_DataVO;

	public override void InitController(object _data)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		m_DataVO = (CollectableDataVO)_data;
		m_Collectable.sprite = GameManager.Instance.AssetManager.GetSprite<Sprite>(m_DataVO.SpriteLookup.Lookup, m_DataVO.SpriteLookup.Sprite);
		if ((Object)(object)m_Collectable.sprite == (Object)null)
		{
			Kill();
			return;
		}
		((Graphic)m_Collectable).SetNativeSize();
		((Transform)((Graphic)m_Collectable).rectTransform).localScale = Vector3.zero;
		((Graphic)m_Collectable).color = new Color(((Graphic)m_Collectable).color.r, ((Graphic)m_Collectable).color.g, ((Graphic)m_Collectable).color.b, 0f);
	}

	public override void PlayIn()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		if ((Object)(object)m_DataVO.AudioClip != (Object)null)
		{
			GameManager.Instance.AudioManager.Play(m_DataVO.AudioClip);
		}
		else if (!string.IsNullOrEmpty(m_DataVO.AudioClipString))
		{
			GameManager.Instance.AudioManager.Play(m_DataVO.AudioClipString);
		}
		m_Collectable.DOFade(1f, 0.5f);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)((Graphic)m_Collectable).rectTransform, 1f, 0.5f), (Ease)27), new TweenCallback(PlayInComplete));
	}

	public override void PlayInComplete()
	{
		Kill();
	}

	public void Hide()
	{
		Kill();
	}

	public override void PlayOut()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		if ((Object)(object)m_Collectable.sprite != (Object)null)
		{
			Sequence val = DOTween.Sequence();
			TweenSettingsExtensions.Insert(val, 1.5f, (Tween)(object)m_Collectable.DOFade(0f, 0.5f));
			TweenSettingsExtensions.Insert(val, 1.5f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)((Graphic)m_Collectable).rectTransform, 0f, 0.5f), (Ease)26));
			TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(PlayOutComplete));
		}
		else
		{
			PlayOutComplete();
		}
	}

	public override void PlayOutComplete()
	{
		base.PlayOutComplete();
	}

	protected override void OnDisposed()
	{
		m_DataVO = null;
		ShortcutExtensions.DOKill((Component)(object)((Graphic)m_Collectable).rectTransform, false);
		ShortcutExtensions.DOKill((Component)(object)m_Collectable, false);
		base.OnDisposed();
	}
}
