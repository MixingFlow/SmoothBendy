using System.Collections.Generic;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class GlobalInkEffectManager : TMGAbstractDisposable
{
	private const string BENDY_INK_POWER = "_BendyInkPower";

	private const string BENDY_POSITION = "_BendyPosition";

	private List<Material> m_Effects;

	public void GetEffects()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ResetEffects();
		Material[] array = Resources.FindObjectsOfTypeAll<Material>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].HasProperty("_BendyPosition"))
			{
				ShortcutExtensions.DOKill(array[i], false);
				array[i].SetFloat("_BendyInkPower", 0f);
				array[i].SetVector("_BendyPosition", Vector4.op_Implicit(Vector3.zero));
				m_Effects.Add(array[i]);
			}
		}
	}

	public void SetActive(bool active, bool isInit = false)
	{
		if (m_Effects == null)
		{
			return;
		}
		for (int i = 0; i < m_Effects.Count; i++)
		{
			Material val = m_Effects[i];
			if ((Object)(object)val != (Object)null && val.HasProperty("_BendyInkPower"))
			{
				ShortcutExtensions.DOKill(val, false);
				if (!isInit)
				{
					ShortcutExtensions.DOFloat(val, (float)(active ? 1 : 0), "_BendyInkPower", 1f);
				}
				else
				{
					val.SetFloat("_BendyInkPower", (float)(active ? 1 : 0));
				}
			}
		}
	}

	public void SetPosition(Vector3 position)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Effects.Count; i++)
		{
			Material val = m_Effects[i];
			if ((Object)(object)val != (Object)null && val.HasProperty("_BendyPosition"))
			{
				val.SetVector("_BendyPosition", Vector4.op_Implicit(position));
			}
		}
	}

	private void ResetEffects()
	{
		ClearEffects();
		m_Effects = new List<Material>();
	}

	public void ClearEffects()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (m_Effects == null)
		{
			return;
		}
		for (int i = 0; i < m_Effects.Count; i++)
		{
			Material val = m_Effects[i];
			if ((Object)(object)val != (Object)null)
			{
				ShortcutExtensions.DOKill(val, false);
				if (val.HasProperty("_BendyInkPower"))
				{
					val.SetFloat("_BendyInkPower", 0f);
				}
				if (val.HasProperty("_BendyPosition"))
				{
					val.SetVector("_BendyPosition", Vector4.op_Implicit(Vector3.zero));
				}
			}
		}
		m_Effects.Clear();
		m_Effects = null;
	}

	protected override void OnDisposed()
	{
		ClearEffects();
		base.OnDisposed();
	}
}
