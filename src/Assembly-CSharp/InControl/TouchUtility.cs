using UnityEngine;

namespace InControl;

public static class TouchUtility
{
	public static Vector2 AnchorToViewPoint(TouchControlAnchor touchControlAnchor)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		return (Vector2)(touchControlAnchor switch
		{
			TouchControlAnchor.TopLeft => new Vector2(0f, 1f), 
			TouchControlAnchor.CenterLeft => new Vector2(0f, 0.5f), 
			TouchControlAnchor.BottomLeft => new Vector2(0f, 0f), 
			TouchControlAnchor.TopCenter => new Vector2(0.5f, 1f), 
			TouchControlAnchor.Center => new Vector2(0.5f, 0.5f), 
			TouchControlAnchor.BottomCenter => new Vector2(0.5f, 0f), 
			TouchControlAnchor.TopRight => new Vector2(1f, 1f), 
			TouchControlAnchor.CenterRight => new Vector2(1f, 0.5f), 
			TouchControlAnchor.BottomRight => new Vector2(1f, 0f), 
			_ => Vector2.zero, 
		});
	}

	public static Vector2 RoundVector(Vector2 vector)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
	}
}
