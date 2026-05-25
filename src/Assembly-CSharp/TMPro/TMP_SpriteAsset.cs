using System.Collections.Generic;
using UnityEngine;

namespace TMPro;

public class TMP_SpriteAsset : TMP_Asset
{
	public static TMP_SpriteAsset m_defaultSpriteAsset;

	public Texture spriteSheet;

	public List<TMP_Sprite> spriteInfoList;

	private List<Sprite> m_sprites;

	public static TMP_SpriteAsset defaultSpriteAsset
	{
		get
		{
			if ((Object)(object)m_defaultSpriteAsset == (Object)null)
			{
				m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");
			}
			return m_defaultSpriteAsset;
		}
	}

	private void OnEnable()
	{
	}

	private Material GetDefaultSpriteMaterial()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		ShaderUtilities.GetShaderPropertyIDs();
		Shader val = Shader.Find("TextMeshPro/Sprite");
		Material val2 = new Material(val);
		val2.SetTexture(ShaderUtilities.ID_MainTex, spriteSheet);
		((Object)val2).hideFlags = (HideFlags)1;
		return val2;
	}

	public int GetSpriteIndex(int hashCode)
	{
		for (int i = 0; i < spriteInfoList.Count; i++)
		{
			if (spriteInfoList[i].hashCode == hashCode)
			{
				return i;
			}
		}
		return -1;
	}
}
