using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro;

public static class TMP_MaterialManager
{
	private class FallbackMaterial
	{
		public int baseID;

		public Material baseMaterial;

		public Material fallbackMaterial;

		public int count;
	}

	private class MaskingMaterial
	{
		public Material baseMaterial;

		public Material stencilMaterial;

		public int count;

		public int stencilID;
	}

	private static List<MaskingMaterial> m_materialList = new List<MaskingMaterial>();

	private static Dictionary<long, FallbackMaterial> m_fallbackMaterials = new Dictionary<long, FallbackMaterial>();

	private static Dictionary<int, long> m_fallbackMaterialLookup = new Dictionary<int, long>();

	private static List<long> m_fallbackCleanupList = new List<long>();

	public static Material GetStencilMaterial(Material baseMaterial, int stencilID)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		if (!baseMaterial.HasProperty(ShaderUtilities.ID_StencilID))
		{
			Debug.LogWarning((object)"Selected Shader does not support Stencil Masking. Please select the Distance Field or Mobile Distance Field Shader.");
			return baseMaterial;
		}
		int instanceID = ((Object)baseMaterial).GetInstanceID();
		for (int i = 0; i < m_materialList.Count; i++)
		{
			if (((Object)m_materialList[i].baseMaterial).GetInstanceID() == instanceID && m_materialList[i].stencilID == stencilID)
			{
				m_materialList[i].count++;
				return m_materialList[i].stencilMaterial;
			}
		}
		Material val = new Material(baseMaterial);
		((Object)val).hideFlags = (HideFlags)61;
		val.shaderKeywords = baseMaterial.shaderKeywords;
		ShaderUtilities.GetShaderPropertyIDs();
		val.SetFloat(ShaderUtilities.ID_StencilID, (float)stencilID);
		val.SetFloat(ShaderUtilities.ID_StencilComp, 4f);
		MaskingMaterial maskingMaterial = new MaskingMaterial();
		maskingMaterial.baseMaterial = baseMaterial;
		maskingMaterial.stencilMaterial = val;
		maskingMaterial.stencilID = stencilID;
		maskingMaterial.count = 1;
		m_materialList.Add(maskingMaterial);
		return val;
	}

	public static void ReleaseStencilMaterial(Material stencilMaterial)
	{
		int instanceID = ((Object)stencilMaterial).GetInstanceID();
		for (int i = 0; i < m_materialList.Count; i++)
		{
			if (((Object)m_materialList[i].stencilMaterial).GetInstanceID() == instanceID)
			{
				if (m_materialList[i].count > 1)
				{
					m_materialList[i].count--;
					break;
				}
				Object.DestroyImmediate((Object)(object)m_materialList[i].stencilMaterial);
				m_materialList.RemoveAt(i);
				stencilMaterial = null;
				break;
			}
		}
	}

	public static Material GetBaseMaterial(Material stencilMaterial)
	{
		int num = m_materialList.FindIndex((MaskingMaterial item) => (Object)(object)item.stencilMaterial == (Object)(object)stencilMaterial);
		if (num == -1)
		{
			return null;
		}
		return m_materialList[num].baseMaterial;
	}

	public static Material SetStencil(Material material, int stencilID)
	{
		material.SetFloat(ShaderUtilities.ID_StencilID, (float)stencilID);
		if (stencilID == 0)
		{
			material.SetFloat(ShaderUtilities.ID_StencilComp, 8f);
		}
		else
		{
			material.SetFloat(ShaderUtilities.ID_StencilComp, 4f);
		}
		return material;
	}

	public static void AddMaskingMaterial(Material baseMaterial, Material stencilMaterial, int stencilID)
	{
		int num = m_materialList.FindIndex((MaskingMaterial item) => (Object)(object)item.stencilMaterial == (Object)(object)stencilMaterial);
		if (num == -1)
		{
			MaskingMaterial maskingMaterial = new MaskingMaterial();
			maskingMaterial.baseMaterial = baseMaterial;
			maskingMaterial.stencilMaterial = stencilMaterial;
			maskingMaterial.stencilID = stencilID;
			maskingMaterial.count = 1;
			m_materialList.Add(maskingMaterial);
		}
		else
		{
			stencilMaterial = m_materialList[num].stencilMaterial;
			m_materialList[num].count++;
		}
	}

	public static void RemoveStencilMaterial(Material stencilMaterial)
	{
		int num = m_materialList.FindIndex((MaskingMaterial item) => (Object)(object)item.stencilMaterial == (Object)(object)stencilMaterial);
		if (num != -1)
		{
			m_materialList.RemoveAt(num);
		}
	}

	public static void ReleaseBaseMaterial(Material baseMaterial)
	{
		int num = m_materialList.FindIndex((MaskingMaterial item) => (Object)(object)item.baseMaterial == (Object)(object)baseMaterial);
		if (num == -1)
		{
			Debug.Log((object)("No Masking Material exists for " + ((Object)baseMaterial).name));
		}
		else if (m_materialList[num].count > 1)
		{
			m_materialList[num].count--;
			Debug.Log((object)("Removed (1) reference to " + ((Object)m_materialList[num].stencilMaterial).name + ". There are " + m_materialList[num].count + " references left."));
		}
		else
		{
			Debug.Log((object)("Removed last reference to " + ((Object)m_materialList[num].stencilMaterial).name + " with ID " + ((Object)m_materialList[num].stencilMaterial).GetInstanceID()));
			Object.DestroyImmediate((Object)(object)m_materialList[num].stencilMaterial);
			m_materialList.RemoveAt(num);
		}
	}

	public static void ClearMaterials()
	{
		if (m_materialList.Count() == 0)
		{
			Debug.Log((object)"Material List has already been cleared.");
			return;
		}
		for (int i = 0; i < m_materialList.Count(); i++)
		{
			Material stencilMaterial = m_materialList[i].stencilMaterial;
			Object.DestroyImmediate((Object)(object)stencilMaterial);
			m_materialList.RemoveAt(i);
		}
	}

	public static int GetStencilID(GameObject obj)
	{
		int num = 0;
		List<Mask> list = TMP_ListPool<Mask>.Get();
		obj.GetComponentsInParent<Mask>(false, list);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].MaskEnabled())
			{
				num++;
			}
		}
		TMP_ListPool<Mask>.Release(list);
		return Mathf.Min((1 << num) - 1, 255);
	}

	public static Material GetFallbackMaterial(Material sourceMaterial, Material targetMaterial)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		int instanceID = ((Object)sourceMaterial).GetInstanceID();
		Texture texture = targetMaterial.GetTexture(ShaderUtilities.ID_MainTex);
		int instanceID2 = ((Object)texture).GetInstanceID();
		long num = ((long)instanceID << 32) | (uint)instanceID2;
		if (m_fallbackMaterials.TryGetValue(num, out var value))
		{
			return value.fallbackMaterial;
		}
		Material val = new Material(sourceMaterial);
		((Object)val).hideFlags = (HideFlags)61;
		val.SetTexture(ShaderUtilities.ID_MainTex, texture);
		val.SetFloat(ShaderUtilities.ID_GradientScale, targetMaterial.GetFloat(ShaderUtilities.ID_GradientScale));
		val.SetFloat(ShaderUtilities.ID_TextureWidth, targetMaterial.GetFloat(ShaderUtilities.ID_TextureWidth));
		val.SetFloat(ShaderUtilities.ID_TextureHeight, targetMaterial.GetFloat(ShaderUtilities.ID_TextureHeight));
		val.SetFloat(ShaderUtilities.ID_WeightNormal, targetMaterial.GetFloat(ShaderUtilities.ID_WeightNormal));
		val.SetFloat(ShaderUtilities.ID_WeightBold, targetMaterial.GetFloat(ShaderUtilities.ID_WeightBold));
		value = new FallbackMaterial();
		value.baseID = instanceID;
		value.baseMaterial = sourceMaterial;
		value.fallbackMaterial = val;
		value.count = 0;
		m_fallbackMaterials.Add(num, value);
		m_fallbackMaterialLookup.Add(((Object)val).GetInstanceID(), num);
		return val;
	}

	public static void AddFallbackMaterialReference(Material targetMaterial)
	{
		if (!((Object)(object)targetMaterial == (Object)null))
		{
			int instanceID = ((Object)targetMaterial).GetInstanceID();
			if (m_fallbackMaterialLookup.TryGetValue(instanceID, out var value) && m_fallbackMaterials.TryGetValue(value, out var value2))
			{
				value2.count++;
			}
		}
	}

	public static void RemoveFallbackMaterialReference(Material targetMaterial)
	{
		if ((Object)(object)targetMaterial == (Object)null)
		{
			return;
		}
		int instanceID = ((Object)targetMaterial).GetInstanceID();
		if (m_fallbackMaterialLookup.TryGetValue(instanceID, out var value) && m_fallbackMaterials.TryGetValue(value, out var value2))
		{
			value2.count--;
			if (value2.count < 1)
			{
				m_fallbackCleanupList.Add(value);
			}
		}
	}

	public static void CleanupFallbackMaterials()
	{
		for (int i = 0; i < m_fallbackCleanupList.Count; i++)
		{
			long key = m_fallbackCleanupList[i];
			if (m_fallbackMaterials.TryGetValue(key, out var value) && value.count < 1)
			{
				Material fallbackMaterial = value.fallbackMaterial;
				Object.DestroyImmediate((Object)(object)fallbackMaterial);
				m_fallbackMaterials.Remove(key);
				m_fallbackMaterialLookup.Remove(((Object)fallbackMaterial).GetInstanceID());
				fallbackMaterial = null;
			}
		}
	}

	public static void ReleaseFallbackMaterial(Material fallackMaterial)
	{
		if ((Object)(object)fallackMaterial == (Object)null)
		{
			return;
		}
		int instanceID = ((Object)fallackMaterial).GetInstanceID();
		if (m_fallbackMaterialLookup.TryGetValue(instanceID, out var value) && m_fallbackMaterials.TryGetValue(value, out var value2))
		{
			if (value2.count > 1)
			{
				value2.count--;
				return;
			}
			Object.DestroyImmediate((Object)(object)value2.fallbackMaterial);
			m_fallbackMaterials.Remove(value);
			m_fallbackMaterialLookup.Remove(instanceID);
			fallackMaterial = null;
		}
	}

	public static void CopyMaterialPresetProperties(Material source, Material destination)
	{
		Texture texture = destination.GetTexture(ShaderUtilities.ID_MainTex);
		float num = destination.GetFloat(ShaderUtilities.ID_GradientScale);
		float num2 = destination.GetFloat(ShaderUtilities.ID_TextureWidth);
		float num3 = destination.GetFloat(ShaderUtilities.ID_TextureHeight);
		float num4 = destination.GetFloat(ShaderUtilities.ID_WeightNormal);
		float num5 = destination.GetFloat(ShaderUtilities.ID_WeightBold);
		destination.CopyPropertiesFromMaterial(source);
		destination.shaderKeywords = source.shaderKeywords;
		destination.SetTexture(ShaderUtilities.ID_MainTex, texture);
		destination.SetFloat(ShaderUtilities.ID_GradientScale, num);
		destination.SetFloat(ShaderUtilities.ID_TextureWidth, num2);
		destination.SetFloat(ShaderUtilities.ID_TextureHeight, num3);
		destination.SetFloat(ShaderUtilities.ID_WeightNormal, num4);
		destination.SetFloat(ShaderUtilities.ID_WeightBold, num5);
	}
}
