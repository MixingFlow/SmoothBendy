using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class Interactable : TMGMonoBehaviour
{
	public class MaterialData
	{
		public MeshRenderer MeshRenderer;

		public List<Material> Materials = new List<Material>();
	}

	private const string SHADER_HIGHLIGHT_STRENGTH = "_HighlightStrength";

	private const string SHADER_HIGHLIGHT_BOOL = "_Highlight";

	private const string SHADER_SHIMMER_BOOL = "_Shimmer";

	private const string SHADER_HIGHLIGHT_POWER = "_HighlightPower";

	[Header("Interact Options")]
	[SerializeField]
	protected bool m_Active;

	[SerializeField]
	private bool m_EnableSingleInteraction;

	[Header("Highlight Options")]
	[SerializeField]
	private bool m_EnableHighlight = true;

	[SerializeField]
	private bool m_AllowShimmer;

	[SerializeField]
	private List<MeshRenderer> m_HighlightMeshRenderers;

	protected List<MaterialData> m_MaterialDatas = new List<MaterialData>();

	private bool m_HasInteractedOnce;

	private bool m_IsShimmering;

	private bool m_AllEffectsOn;

	public bool isSingleInteraction => m_EnableSingleInteraction;

	public bool isInteracted { get; private set; }

	public bool isHighlightEnabled => m_EnableHighlight;

	public bool isHighlighted { get; private set; }

	public event EventHandler OnInteracted;

	public override void Init()
	{
		if (!m_EnableHighlight && !m_AllowShimmer)
		{
			return;
		}
		if (m_HighlightMeshRenderers.Count <= 0)
		{
			MeshRenderer component = ((Component)this).GetComponent<MeshRenderer>();
			if ((Object)(object)component != (Object)null)
			{
				m_HighlightMeshRenderers.Add(component);
			}
		}
		for (int i = 0; i < m_HighlightMeshRenderers.Count; i++)
		{
			MaterialData materialData = new MaterialData();
			materialData.MeshRenderer = m_HighlightMeshRenderers[i];
			materialData.Materials = ((Renderer)m_HighlightMeshRenderers[i]).materials.ToList();
			for (int j = 0; j < materialData.Materials.Count; j++)
			{
				materialData.Materials[j].SetFloat("_HighlightStrength", 0f);
				materialData.Materials[j].SetFloat("_Highlight", 0f);
				materialData.Materials[j].SetFloat("_Shimmer", 0f);
			}
			m_MaterialDatas.Add(materialData);
		}
	}

	private void Update()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!m_AllowShimmer || !m_EnableHighlight || !Object.op_Implicit((Object)(object)GameManager.Instance.Player))
		{
			return;
		}
		if (m_Active)
		{
			if (!isSingleInteraction || !m_HasInteractedOnce)
			{
				m_AllEffectsOn = true;
				bool flag = Vector3.SqrMagnitude(GameManager.Instance.Player.transform.position - base.transform.position) < 225f;
				if (m_IsShimmering != flag)
				{
					SetShimmerActiveState(flag);
				}
			}
		}
		else
		{
			TurnOfAllEFfects();
		}
	}

	public void AllowShimmer(bool _active)
	{
		m_AllowShimmer = _active;
	}

	public void ResetInteraction()
	{
		SetActive(active: true);
		m_HasInteractedOnce = false;
		isInteracted = false;
	}

	public void SetSingleInteraction(bool _isSingle)
	{
		m_EnableSingleInteraction = _isSingle;
	}

	private void SetShimmerActiveState(bool _active)
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Expected O, but got Unknown
		if (m_MaterialDatas == null || !m_AllowShimmer)
		{
			return;
		}
		m_IsShimmering = _active;
		float boolToFloat = ((!_active) ? 0f : 1f);
		for (int i = 0; i < m_MaterialDatas.Count; i++)
		{
			MaterialData materialData = m_MaterialDatas[i];
			for (int j = 0; j < materialData.Materials.Count; j++)
			{
				Material mat = materialData.Materials[j];
				if (!((Object)(object)mat != (Object)null))
				{
					continue;
				}
				if (m_IsShimmering)
				{
					ShortcutExtensions.DOKill(mat, false);
					if (mat.HasProperty("_Shimmer"))
					{
						mat.SetFloat("_Shimmer", boolToFloat);
					}
					if (mat.HasProperty("_HighlightPower"))
					{
						ShortcutExtensions.DOFloat(mat, 1f, "_HighlightPower", 0.5f);
					}
					continue;
				}
				ShortcutExtensions.DOKill(mat, false);
				if (!mat.HasProperty("_HighlightPower"))
				{
					continue;
				}
				TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOFloat(mat, 0f, "_HighlightPower", 0.5f), (TweenCallback)delegate
				{
					if (mat.HasProperty("_Shimmer"))
					{
						mat.SetFloat("_Shimmer", boolToFloat);
					}
				});
			}
		}
	}

	public void TurnOfAllEFfects()
	{
		if (!m_EnableHighlight || base.IsDisposed || !m_AllEffectsOn)
		{
			return;
		}
		m_IsShimmering = false;
		m_AllEffectsOn = false;
		for (int i = 0; i < m_MaterialDatas.Count; i++)
		{
			MaterialData materialData = m_MaterialDatas[i];
			for (int j = 0; j < materialData.Materials.Count; j++)
			{
				Material val = materialData.Materials[j];
				ShortcutExtensions.DOKill(val, false);
				val.SetFloat("_Shimmer", 0f);
				val.SetFloat("_Highlight", 0f);
				val.SetFloat("_HighlightPower", 0f);
				val.SetFloat("_HighlightStrength", 0f);
			}
		}
	}

	public void ForceRemoveEffects()
	{
		if (base.IsDisposed)
		{
			return;
		}
		m_IsShimmering = false;
		m_AllEffectsOn = false;
		for (int i = 0; i < m_MaterialDatas.Count; i++)
		{
			MaterialData materialData = m_MaterialDatas[i];
			for (int j = 0; j < materialData.Materials.Count; j++)
			{
				Material val = materialData.Materials[j];
				ShortcutExtensions.DOKill(val, false);
				val.SetFloat("_Shimmer", 0f);
				val.SetFloat("_Highlight", 0f);
				val.SetFloat("_HighlightPower", 0f);
				val.SetFloat("_HighlightStrength", 0f);
			}
		}
	}

	public void InteractEnter()
	{
		if (!m_Active || (m_EnableSingleInteraction && isInteracted))
		{
			return;
		}
		m_IsShimmering = false;
		if (m_EnableHighlight && !isHighlighted)
		{
			for (int i = 0; i < m_MaterialDatas.Count; i++)
			{
				MaterialData materialData = m_MaterialDatas[i];
				for (int j = 0; j < materialData.Materials.Count; j++)
				{
					materialData.Materials[j].SetFloat("_HighlightStrength", 0.05f);
					ShortcutExtensions.DOKill(materialData.Materials[j], false);
					TweenSettingsExtensions.SetLoops<Tweener>(ShortcutExtensions.DOFloat(materialData.Materials[j], 0.09f, "_HighlightStrength", 0.25f), -1, (LoopType)1);
					materialData.Materials[j].SetFloat("_Highlight", 1f);
				}
			}
			isHighlighted = true;
		}
		OnInteractEnter();
	}

	public virtual void OnInteractEnter()
	{
	}

	public void Interact()
	{
		if (m_Active && (!m_EnableSingleInteraction || !isInteracted))
		{
			if (m_EnableSingleInteraction)
			{
				isInteracted = true;
			}
			ExitHighlight();
			SetShimmerActiveState(_active: false);
			OnInteract();
			if (m_EnableSingleInteraction)
			{
				TurnOfAllEFfects();
			}
			m_HasInteractedOnce = true;
			this.OnInteracted.Send(this);
		}
	}

	public virtual void OnInteract()
	{
	}

	public void InteractExit()
	{
		if (m_Active && (!m_EnableSingleInteraction || !isInteracted))
		{
			if (m_EnableHighlight && isHighlighted)
			{
				ExitHighlight();
			}
			OnInteractExit();
		}
	}

	private void ExitHighlight()
	{
		if (!m_EnableHighlight || m_MaterialDatas == null)
		{
			return;
		}
		for (int i = 0; i < m_MaterialDatas.Count; i++)
		{
			MaterialData materialData = m_MaterialDatas[i];
			for (int j = 0; j < materialData.Materials.Count; j++)
			{
				ShortcutExtensions.DOKill(materialData.Materials[j], false);
				materialData.Materials[j].SetFloat("_HighlightStrength", 0f);
				materialData.Materials[j].SetFloat("_Highlight", 0f);
			}
		}
		isHighlighted = false;
		m_IsShimmering = false;
	}

	public virtual void OnInteractExit()
	{
	}

	public void SetActive(bool active)
	{
		m_Active = active;
		SetShimmerActiveState(m_Active);
		if (!m_Active)
		{
			ExitHighlight();
		}
	}

	protected override void OnDisposed()
	{
		if (m_MaterialDatas != null)
		{
			for (int i = 0; i < m_MaterialDatas.Count; i++)
			{
				MaterialData materialData = m_MaterialDatas[i];
				for (int j = 0; j < materialData.Materials.Count; j++)
				{
					ShortcutExtensions.DOKill(materialData.Materials[j], false);
				}
			}
			m_MaterialDatas.Clear();
			m_MaterialDatas = null;
		}
		base.OnDisposed();
	}
}
