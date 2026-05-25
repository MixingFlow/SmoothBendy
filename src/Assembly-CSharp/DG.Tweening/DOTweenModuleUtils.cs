using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening;

public static class DOTweenModuleUtils
{
	public static class Physics
	{
		public static void SetOrientationOnPath(PathOptions options, Tween t, Quaternion newRot, Transform trans)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (options.isRigidbody)
			{
				((Rigidbody)t.target).rotation = newRot;
			}
			else
			{
				trans.rotation = newRot;
			}
		}

		public static bool HasRigidbody2D(Component target)
		{
			return (Object)(object)target.GetComponent<Rigidbody2D>() != (Object)null;
		}

		public static bool HasRigidbody(Component target)
		{
			return (Object)(object)target.GetComponent<Rigidbody>() != (Object)null;
		}

		public static TweenerCore<Vector3, Path, PathOptions> CreateDOTweenPathTween(MonoBehaviour target, bool tweenRigidbody, bool isLocal, Path path, float duration, PathMode pathMode)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody val = ((!tweenRigidbody) ? null : ((Component)target).GetComponent<Rigidbody>());
			if (tweenRigidbody && (Object)(object)val != (Object)null)
			{
				return (!isLocal) ? val.DOPath(path, duration, pathMode) : val.DOLocalPath(path, duration, pathMode);
			}
			return (!isLocal) ? ShortcutExtensions.DOPath(((Component)target).transform, path, duration, pathMode) : ShortcutExtensions.DOLocalPath(((Component)target).transform, path, duration, pathMode);
		}
	}

	private static bool _initialized;

	public static void Init()
	{
		if (!_initialized)
		{
			_initialized = true;
			DOTweenExternalCommand.SetOrientationOnPath += Physics.SetOrientationOnPath;
		}
	}
}
