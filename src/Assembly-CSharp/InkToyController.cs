using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class InkToyController : TMGMonoBehaviour
{
	[Header("Transforms")]
	[SerializeField]
	private Transform m_Content;

	[Header("GameObjects")]
	[SerializeField]
	private GameObject[] m_ToyMeshes;

	[SerializeField]
	private GameObject[] m_Blobs;

	[Header("Interactable")]
	[SerializeField]
	private Interactable m_ToyInteractable;

	[Header("Effects")]
	[SerializeField]
	private ParticleSystem m_Particles;

	private int m_CurrentToyIndex;

	public int ToyIndex => m_CurrentToyIndex;

	public override void Init()
	{
		base.Init();
		m_ToyInteractable.OnInteracted += HandleToyOnInteracted;
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_ToyMeshes.Length; i++)
		{
			m_ToyMeshes[i].SetActive(i == GameManager.Instance.GameData.CurrentSaveFile.CH3Data.Toy);
		}
	}

	private void HandleToyOnInteracted(object sender, EventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		m_ToyInteractable.OnInteracted -= HandleToyOnInteracted;
		TweenSettingsExtensions.OnComplete<Sequence>(DOAnimation(), new TweenCallback(OnAnimationComplete));
	}

	private Sequence DOAnimation()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0f;
		float num2 = 0.15f;
		float num3 = num2 * 2f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_Content, new Vector3(2.15f, 0.7f, 2.15f), num2), (Ease)1));
		num += num2;
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(ChangeToBlob));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_Content, new Vector3(0.45f, 1.65f, 0.45f), num2), (Ease)1));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Content, 1f, num3, false), (Ease)6));
		TweenSettingsExtensions.Insert(val, num + num3, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Content, 0f, num3, false), (Ease)5));
		num += num2;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_Content, Vector3.one, num2), (Ease)1));
		num += num2;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_Content, new Vector3(0.7f, 1.2f, 0.7f), num2), (Ease)1));
		num += num2;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_Content, Vector3.one, num2 / 2f), (Ease)1));
		num += num2 / 2f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_Content, new Vector3(1.5f, 0.35f, 1.5f), num2 / 2f), (Ease)1));
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play("Audio/SFX/CH3/SFX_CH3_inktoy");
		});
		num += num2;
		TweenSettingsExtensions.InsertCallback(val, num - 0.05f, new TweenCallback(ChangeToObject));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_Content, Vector3.one, 0.25f), (Ease)1));
		return val;
	}

	private void OnAnimationComplete()
	{
		m_ToyInteractable.OnInteracted += HandleToyOnInteracted;
	}

	public void ChangeToBlob()
	{
		for (int i = 0; i < m_ToyMeshes.Length; i++)
		{
			m_ToyMeshes[i].SetActive(false);
		}
		m_Blobs[(Random.value <= 0.1f) ? 1u : 0u].SetActive(true);
		m_Particles.Emit(10);
	}

	public void ChangeToObject()
	{
		IncreaseIndex();
		ActivateToyObject();
		m_Particles.Emit(10);
	}

	private void IncreaseIndex()
	{
		m_CurrentToyIndex++;
		if (m_CurrentToyIndex == m_ToyMeshes.Length)
		{
			m_CurrentToyIndex = 0;
		}
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.Toy = m_CurrentToyIndex;
	}

	private void ActivateToyObject()
	{
		for (int i = 0; i < m_ToyMeshes.Length; i++)
		{
			m_ToyMeshes[i].SetActive(i == m_CurrentToyIndex);
		}
		for (int j = 0; j < m_Blobs.Length; j++)
		{
			m_Blobs[j].SetActive(false);
		}
	}

	protected override void OnDisposed()
	{
		m_ToyInteractable.OnInteracted -= HandleToyOnInteracted;
		base.OnDisposed();
	}
}
