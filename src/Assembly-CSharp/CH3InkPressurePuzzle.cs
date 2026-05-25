using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH3InkPressurePuzzle : TMGMonoBehaviour
{
	[Serializable]
	public class InkValve
	{
		public Transform Ink;

		public Interactable Valve;

		[HideInInspector]
		public int Direction;

		[HideInInspector]
		public int Index;
	}

	private float[] m_InkPositions = new float[10] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f };

	[HideInInspector]
	public int ID;

	[HideInInspector]
	public bool isComplete;

	[SerializeField]
	private Light m_Light;

	[SerializeField]
	private Interactable m_Door;

	[SerializeField]
	private List<InkValve> m_InkValves;

	[SerializeField]
	private Interactable m_Collectable;

	[SerializeField]
	private ParticleSystem m_SparkParticles;

	private AudioClip[] m_ValveClips;

	private AudioClip m_PowerCorePickupClip;

	private AudioClip m_PuzzlePanelClip;

	private AudioClip m_PuzzleSolvedClip;

	private AudioClip m_PuzzleInkRiseClip;

	private bool m_HasSearcher;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ValveClips = GameManager.Instance.GetAudioClips("Audio/SFX/CH3/Valves/");
		m_PuzzlePanelClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_valvepaneldooropen");
		m_PowerCorePickupClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_valvepanelcorepickup");
		m_PuzzleSolvedClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_valvepuzzle_allinkvalvesaligned");
		m_PuzzleInkRiseClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_valvepuzzle_inkrisinginpipes");
		m_Door.SetActive(active: false);
		for (int i = 0; i < m_InkValves.Count; i++)
		{
			m_InkValves[i].Valve.SetActive(active: false);
			m_InkValves[i].Direction = ((!(Random.value < 0.5f)) ? 1 : (-1));
		}
	}

	public void Deactivate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)m_Light).enabled = false;
		m_Collectable.gameObject.SetActive(false);
		Transform obj = m_Door.transform;
		obj.localEulerAngles += new Vector3(0f, 50f, 0f);
		for (int i = 0; i < m_InkValves.Count; i++)
		{
			m_InkValves[i].Ink.localScale = new Vector3(1f, 0.5f, 1f);
		}
	}

	public void Activate()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)m_Light).enabled = true;
		m_Collectable.gameObject.SetActive(true);
		for (int i = 0; i < m_InkValves.Count; i++)
		{
			int num = Random.Range(0, m_InkPositions.Length);
			m_InkValves[i].Index = num;
			m_InkValves[i].Ink.localScale = new Vector3(1f, m_InkPositions[num], 1f);
		}
	}

	public void ActivateInteraction()
	{
		for (int i = 0; i < m_InkValves.Count; i++)
		{
			InkValve inkValve = m_InkValves[i];
			if (inkValve.Index <= 0)
			{
				inkValve.Direction = 1;
			}
			else if (inkValve.Index >= m_InkPositions.Length - 1)
			{
				inkValve.Direction = -1;
			}
			inkValve.Valve.SetActive(active: true);
			inkValve.Valve.OnInteracted += HandleValveOnInteracted;
		}
	}

	private void DisableInteraction()
	{
		for (int i = 0; i < m_InkValves.Count; i++)
		{
			InkValve inkValve = m_InkValves[i];
			inkValve.Valve.SetActive(active: false);
			inkValve.Valve.OnInteracted -= HandleValveOnInteracted;
		}
	}

	private void HandleValveOnInteracted(object sender, EventArgs e)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Interactable obj = (Interactable)sender;
		if (!m_HasSearcher && Random.value < 0.2f)
		{
			m_HasSearcher = true;
			SearcherAi searcherAi = GameManager.Instance.AssetManager.CreateAsset<SearcherAi>("GamePlay/Characters/Ai_Searcher");
			Vector3 position = GameManager.Instance.Player.transform.position;
			position += GameManager.Instance.Player.transform.forward * -5f;
			float y = position.y;
			Bounds bounds = ((Collider)GameManager.Instance.Player.CharacterController).bounds;
			position.y = y - ((Bounds)(ref bounds)).extents.y;
			searcherAi.transform.position = position;
		}
		PlayAudio(ref m_ValveClips);
		for (int i = 0; i < m_InkValves.Count; i++)
		{
			InkValve inkValve = m_InkValves[i];
			if (((object)inkValve.Valve).Equals((object)obj))
			{
				inkValve.Valve.SetActive(active: false);
				inkValve.Valve.OnInteracted -= HandleValveOnInteracted;
				RotateValve(inkValve);
				break;
			}
		}
	}

	private void RotateValve(InkValve inkValve)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		float num = 0f;
		float num2 = 1f;
		float num3 = ((inkValve.Direction >= 0) ? (-180f) : 180f);
		inkValve.Index += inkValve.Direction;
		float endValue = m_InkPositions[inkValve.Index];
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(inkValve.Valve.transform, new Vector3(num3, 0f, 0f), num2, (RotateMode)3), (Ease)7));
		num += num2 / 2f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_PuzzleInkRiseClip);
		});
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(inkValve.Ink, endValue, num2), (Ease)6));
		TweenSettingsExtensions.OnComplete<Sequence>(val, (TweenCallback)delegate
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = inkValve.Ink.localScale;
			localScale.y = endValue;
			inkValve.Ink.localScale = localScale;
			if (!CheckStatus())
			{
				if (inkValve.Index <= 0)
				{
					inkValve.Direction = 1;
				}
				else if (inkValve.Index >= m_InkPositions.Length - 1)
				{
					inkValve.Direction = -1;
				}
				inkValve.Valve.SetActive(active: true);
				inkValve.Valve.OnInteracted += HandleValveOnInteracted;
			}
			else
			{
				DisableInteraction();
				((Behaviour)m_Light).enabled = false;
				isComplete = true;
				GameManager.Instance.AudioManager.Play(m_PuzzleSolvedClip);
				m_Door.SetActive(active: true);
				m_Door.OnInteracted += HandleDoorOnInteracted;
			}
		});
	}

	private bool CheckStatus()
	{
		bool result = true;
		for (int i = 0; i < m_InkValves.Count; i++)
		{
			if (m_InkValves[i].Index != 4)
			{
				result = false;
			}
		}
		return result;
	}

	private void HandleDoorOnInteracted(object sender, EventArgs e)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		m_Door.OnInteracted -= HandleDoorOnInteracted;
		m_Door.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_PuzzlePanelClip);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Door.transform, new Vector3(0f, 50f, 0f), 0.5f, (RotateMode)3), (Ease)6), (TweenCallback)delegate
		{
			m_Collectable.SetActive(active: true);
			m_Collectable.OnInteracted += HandleCollectableOnInteracted;
		});
	}

	private void HandleCollectableOnInteracted(object sender, EventArgs e)
	{
		m_Collectable.OnInteracted -= HandleCollectableOnInteracted;
		m_Collectable.Dispose();
		GameManager.Instance.AudioManager.Play(m_PowerCorePickupClip);
		m_SparkParticles.Emit(10);
		this.OnComplete.Send(this);
	}

	private AudioObject PlayAudio(ref AudioClip[] audioClips, bool is2D = false)
	{
		if (audioClips == null || audioClips.Length <= 0)
		{
			return null;
		}
		int num = Random.Range(0, audioClips.Length);
		AudioClip val = audioClips[num];
		AudioObject audioObject = null;
		audioObject = GameManager.Instance.AudioManager.Play(val);
		audioClips[num] = audioClips[0];
		audioClips[0] = val;
		return audioObject;
	}

	public void ForceComplete()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		DisableInteraction();
		if (Object.op_Implicit((Object)(object)m_Collectable))
		{
			m_Collectable.gameObject.SetActive(false);
		}
		m_Door.OnInteracted -= HandleDoorOnInteracted;
		m_Door.SetActive(active: false);
		m_Door.transform.localEulerAngles = new Vector3(0f, 50f, 0f);
		for (int i = 0; i < m_InkValves.Count; i++)
		{
			m_InkValves[i].Ink.localScale = new Vector3(1f, 0.5f, 1f);
		}
		isComplete = true;
		((Behaviour)m_Light).enabled = false;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
