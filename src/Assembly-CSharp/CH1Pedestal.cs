using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH1Pedestal : TMGMonoBehaviour
{
	public enum CollectableType
	{
		GEAR,
		WRENCH,
		BOOK,
		DOLL,
		RECORD,
		INKWELL
	}

	[Header("Type")]
	[SerializeField]
	private CollectableType m_CollectableType;

	[SerializeField]
	private Sprite m_MenuSprite;

	[Header("Interactables")]
	[SerializeField]
	private Interactable m_Collectable;

	[SerializeField]
	private Interactable m_Pedestal;

	[Header("GameObject")]
	[SerializeField]
	private GameObject m_Collected;

	[Header("Light")]
	[SerializeField]
	private LightFixtureController m_Light;

	private Sequence m_CollectSequence;

	private AudioClip m_AudioKey;

	private string m_SpriteLookupKey;

	private string m_SpriteListKey;

	private AudioClip m_CollectClip;

	private AudioClip m_LightClip;

	public bool isCollected;

	public Transform Collectable => m_Collectable.transform;

	public Sprite MenuSprite => m_MenuSprite;

	public bool isComplete { get; private set; }

	public event EventHandler OnCollect;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_LightClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Light_Switch_Sammys_Room_01");
	}

	public void Initialize(Transform collectableLocation)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		m_Collectable.transform.position = collectableLocation.position;
		m_Collectable.transform.eulerAngles = collectableLocation.eulerAngles;
		CheckCollectableType();
		isComplete = false;
		m_Pedestal.SetActive(active: false);
		m_Collectable.SetActive(active: false);
		m_Collected.SetActive(false);
		TurnLightOff();
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.CollectablesObjective.IsComplete || IsTypeComplete())
		{
			ForceComplete();
		}
		else if (IsTypeCollected())
		{
			ForceCollected();
		}
	}

	public void Activate()
	{
		m_Collectable.SetActive(active: true);
		m_Collectable.OnInteracted += HandleCollectableOnCollected;
	}

	public void TurnLightOff()
	{
		m_Light.TurnOff();
	}

	private void HandleCollectableOnCollected(object sender, EventArgs e)
	{
		m_Collectable.OnInteracted -= HandleCollectableOnCollected;
		GameManager.Instance.ShowCollectable(CollectableDataVO.Create(m_AudioKey, m_SpriteLookupKey, m_SpriteListKey));
		m_Collectable.Dispose();
		isCollected = true;
		SetTypeCollected();
		this.OnCollect.Send(this);
		m_Pedestal.OnInteracted += HandlePedestalOnInteracted;
		m_Pedestal.SetActive(active: true);
	}

	private void HandlePedestalOnInteracted(object sender, EventArgs e)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		m_Pedestal.OnInteracted -= HandlePedestalOnInteracted;
		m_Pedestal.SetActive(active: false);
		TweenSettingsExtensions.OnComplete<Sequence>(DOCollect(), new TweenCallback(HandleCollectOnComplete));
	}

	private Sequence DOCollect()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		ResetSequence();
		m_Collected.SetActive(true);
		GameManager.Instance.AudioManager.Play(m_CollectClip);
		TweenSettingsExtensions.Insert(m_CollectSequence, 0.1f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Pedestal.transform, -0.15f, 0.75f, false), (Ease)27));
		TweenSettingsExtensions.InsertCallback(m_CollectSequence, 0.6f, new TweenCallback(TurnLightOn));
		return m_CollectSequence;
	}

	private void TurnLightOn()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.AudioManager.PlayAtPosition(m_LightClip, base.transform.position);
		m_Light.TurnOn();
	}

	private void HandleCollectOnComplete()
	{
		isComplete = true;
		SetTypeComplete();
		this.OnComplete.Send(this);
	}

	private void CheckCollectableType()
	{
		if (m_CollectableType == CollectableType.GEAR)
		{
			m_AudioKey = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Gear_Pick_UP_Vanish_01");
			m_SpriteLookupKey = "UI/ChapterOneCollectables/ChapterOneCollectables";
			m_SpriteListKey = "collectable_gear";
			m_CollectClip = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Gear_01");
		}
		else if (m_CollectableType == CollectableType.WRENCH)
		{
			m_AudioKey = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Wrench_Pick_UP_Vanish_01");
			m_SpriteLookupKey = "UI/ChapterOneCollectables/ChapterOneCollectables";
			m_SpriteListKey = "collectable_wrench";
			m_CollectClip = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Wrench_01");
		}
		else if (m_CollectableType == CollectableType.BOOK)
		{
			m_AudioKey = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Book_Pick_Up_Vanish_01");
			m_SpriteLookupKey = "UI/ChapterOneCollectables/ChapterOneCollectables";
			m_SpriteListKey = "collectable_book";
			m_CollectClip = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Book_01");
		}
		else if (m_CollectableType == CollectableType.DOLL)
		{
			m_AudioKey = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Toy_Pick_UP_Vanish_01");
			m_SpriteLookupKey = "UI/ChapterOneCollectables/ChapterOneCollectables";
			m_SpriteListKey = "collectable_doll";
			m_CollectClip = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Toy_01");
		}
		else if (m_CollectableType == CollectableType.RECORD)
		{
			m_AudioKey = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Record_Pick_UP_Vanish_01");
			m_SpriteLookupKey = "UI/ChapterOneCollectables/ChapterOneCollectables";
			m_SpriteListKey = "collectable_record";
			m_CollectClip = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Record_01");
		}
		else if (m_CollectableType == CollectableType.INKWELL)
		{
			m_AudioKey = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Ink_Jar_Pick_UP_Vanish_01");
			m_SpriteLookupKey = "UI/ChapterOneCollectables/ChapterOneCollectables";
			m_SpriteListKey = "collectable_inkwell";
			m_CollectClip = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Ink_Jar_01");
		}
	}

	public void UpdateObjective()
	{
		IsTypeCollected();
	}

	private bool IsTypeCollected()
	{
		if (m_CollectableType == CollectableType.GEAR)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Gear.IsStarted)
			{
				if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
				{
					GameManager.Instance.CurrentObjective.Collected[0] = true;
				}
				return true;
			}
			return false;
		}
		if (m_CollectableType == CollectableType.WRENCH)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Wrench.IsStarted)
			{
				if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
				{
					GameManager.Instance.CurrentObjective.Collected[1] = true;
				}
				return true;
			}
			return false;
		}
		if (m_CollectableType == CollectableType.BOOK)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Book.IsStarted)
			{
				if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
				{
					GameManager.Instance.CurrentObjective.Collected[2] = true;
				}
				return true;
			}
			return false;
		}
		if (m_CollectableType == CollectableType.DOLL)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Doll.IsStarted)
			{
				if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
				{
					GameManager.Instance.CurrentObjective.Collected[3] = true;
				}
				return true;
			}
			return false;
		}
		if (m_CollectableType == CollectableType.RECORD)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Record.IsStarted)
			{
				if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
				{
					GameManager.Instance.CurrentObjective.Collected[4] = true;
				}
				return true;
			}
			return false;
		}
		if (m_CollectableType == CollectableType.INKWELL)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Inkwell.IsStarted)
			{
				if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
				{
					GameManager.Instance.CurrentObjective.Collected[5] = true;
				}
				return true;
			}
			return false;
		}
		return false;
	}

	private bool IsTypeComplete()
	{
		if (m_CollectableType == CollectableType.GEAR)
		{
			return GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Gear.IsComplete;
		}
		if (m_CollectableType == CollectableType.WRENCH)
		{
			return GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Wrench.IsComplete;
		}
		if (m_CollectableType == CollectableType.BOOK)
		{
			return GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Book.IsComplete;
		}
		if (m_CollectableType == CollectableType.DOLL)
		{
			return GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Doll.IsComplete;
		}
		if (m_CollectableType == CollectableType.RECORD)
		{
			return GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Record.IsComplete;
		}
		if (m_CollectableType == CollectableType.INKWELL)
		{
			return GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Inkwell.IsComplete;
		}
		return false;
	}

	private void SetTypeCollected()
	{
		if (m_CollectableType == CollectableType.GEAR)
		{
			if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
			{
				GameManager.Instance.CurrentObjective.Collected[0] = true;
			}
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Gear.IsStarted = true;
		}
		else if (m_CollectableType == CollectableType.WRENCH)
		{
			if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
			{
				GameManager.Instance.CurrentObjective.Collected[1] = true;
			}
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Wrench.IsStarted = true;
		}
		else if (m_CollectableType == CollectableType.BOOK)
		{
			if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
			{
				GameManager.Instance.CurrentObjective.Collected[2] = true;
			}
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Book.IsStarted = true;
		}
		else if (m_CollectableType == CollectableType.DOLL)
		{
			if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
			{
				GameManager.Instance.CurrentObjective.Collected[3] = true;
			}
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Doll.IsStarted = true;
		}
		else if (m_CollectableType == CollectableType.RECORD)
		{
			if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
			{
				GameManager.Instance.CurrentObjective.Collected[4] = true;
			}
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Record.IsStarted = true;
		}
		else
		{
			if (m_CollectableType != CollectableType.INKWELL)
			{
				return;
			}
			if (GameManager.Instance.CurrentObjective != null && GameManager.Instance.CurrentObjective.Collected != null)
			{
				GameManager.Instance.CurrentObjective.Collected[5] = true;
			}
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Inkwell.IsStarted = true;
		}
		GameManager.Instance.GameDataManager.Save();
	}

	private void SetTypeComplete()
	{
		if (m_CollectableType == CollectableType.GEAR)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Gear.IsComplete = true;
		}
		else if (m_CollectableType == CollectableType.WRENCH)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Wrench.IsComplete = true;
		}
		else if (m_CollectableType == CollectableType.BOOK)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Book.IsComplete = true;
		}
		else if (m_CollectableType == CollectableType.DOLL)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Doll.IsComplete = true;
		}
		else if (m_CollectableType == CollectableType.RECORD)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Record.IsComplete = true;
		}
		else if (m_CollectableType == CollectableType.INKWELL)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Inkwell.IsComplete = true;
		}
		GameManager.Instance.GameDataManager.Save();
	}

	private void ForceCollected()
	{
		m_Collectable.Dispose();
		isCollected = true;
		m_Pedestal.OnInteracted += HandlePedestalOnInteracted;
		m_Pedestal.SetActive(active: true);
	}

	private void ForceComplete()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		isCollected = true;
		isComplete = true;
		m_Light.TurnOn();
		m_Collectable.Dispose();
		m_Collected.SetActive(true);
		Transform obj = m_Pedestal.transform;
		obj.position += Vector3.down * 0.15f;
	}

	private void ResetSequence()
	{
		KillSequence();
		m_CollectSequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_CollectSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_CollectSequence, false);
			m_CollectSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		if (Object.op_Implicit((Object)(object)m_Pedestal))
		{
			m_Pedestal.OnInteracted -= HandlePedestalOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_Collectable))
		{
			m_Collectable.OnInteracted -= HandleCollectableOnCollected;
		}
		m_LightClip = null;
		m_CollectClip = null;
		m_AudioKey = null;
		KillSequence();
		base.OnDisposed();
	}
}
