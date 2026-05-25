using System.Collections.Generic;
using UnityEngine;

namespace TMG.Core;

public class AssetManager : TMGAbstractDisposable
{
	private Dictionary<int, Object> m_Assets = new Dictionary<int, Object>();

	private Dictionary<int, Object[]> m_AssetsLists = new Dictionary<int, Object[]>();

	private Dictionary<int, Object[]> m_SpriteAssets = new Dictionary<int, Object[]>();

	public T GetSprite<T>(string lookupKey, string assetKey) where T : Object
	{
		int hashCode = assetKey.GetHashCode();
		if (!m_SpriteAssets.ContainsKey(hashCode))
		{
			m_SpriteAssets.Add(hashCode, Resources.LoadAll(lookupKey));
		}
		for (int i = 0; i < m_SpriteAssets[hashCode].Length; i++)
		{
			if (m_SpriteAssets[hashCode][i].name == assetKey)
			{
				return (T)(object)m_SpriteAssets[hashCode][i];
			}
		}
		return (T)(object)null;
	}

	public T GetAsset<T>(string assetKey) where T : Object
	{
		int hashCode = assetKey.GetHashCode();
		if (!m_Assets.ContainsKey(hashCode))
		{
			m_Assets.Add(hashCode, (Object)(object)Resources.Load<T>(assetKey));
		}
		return (T)(object)m_Assets[hashCode];
	}

	public T CreateAsset<T>(string assetKey) where T : Component
	{
		return Object.Instantiate<T>(this.GetAsset<T>(assetKey));
	}

	public T[] GetAssets<T>(string assetKey) where T : Object
	{
		int hashCode = assetKey.GetHashCode();
		if (!m_AssetsLists.ContainsKey(hashCode))
		{
			m_AssetsLists.Add(hashCode, (Object[])(object)Resources.LoadAll<T>(assetKey));
		}
		return (T[])(object)m_AssetsLists[hashCode];
	}

	protected override void OnDisposed()
	{
		if (m_Assets != null)
		{
			m_Assets.Clear();
			m_Assets = null;
		}
		if (m_AssetsLists != null)
		{
			m_AssetsLists.Clear();
			m_AssetsLists = null;
		}
		if (m_SpriteAssets != null)
		{
			m_SpriteAssets.Clear();
			m_SpriteAssets = null;
		}
		base.OnDisposed();
	}
}
