using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMG.Core;
using UnityEngine;

namespace TMG.AssetBundles;

public class AssetBundleManager : TMGAbstractDisposable
{
	public Dictionary<string, AssetBundle> AssetBundles { get; private set; }

	public AssetBundle LoadedAssetBundle { get; private set; }

	public bool HasAssetBundle { get; private set; }

	public event EventHandler OnLoaded;

	public AssetBundleManager()
	{
		Caching.ClearCache();
		AssetBundles = new Dictionary<string, AssetBundle>();
	}

	public IEnumerator GetAssetBundle(string assetBundleName)
	{
		HasAssetBundle = false;
		AssetBundleCreateRequest assetBundleRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, assetBundleName));
		yield return assetBundleRequest;
		LoadedAssetBundle = assetBundleRequest.assetBundle;
		if ((Object)(object)LoadedAssetBundle == (Object)null)
		{
			HasAssetBundle = false;
		}
		else if (!AssetBundles.ContainsKey(assetBundleName))
		{
			AssetBundles.Add(assetBundleName, LoadedAssetBundle);
			HasAssetBundle = true;
		}
		this.OnLoaded.Send(this);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
