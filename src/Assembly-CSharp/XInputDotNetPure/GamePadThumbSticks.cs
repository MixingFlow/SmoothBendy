using UnityEngine;

namespace XInputDotNetPure;

public struct GamePadThumbSticks
{
	public struct StickValue
	{
		private Vector2 vector;

		public float X => vector.x;

		public float Y => vector.y;

		public Vector2 Vector => vector;

		internal StickValue(float x, float y)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			vector = new Vector2(x, y);
		}
	}

	private StickValue left;

	private StickValue right;

	public StickValue Left => left;

	public StickValue Right => right;

	internal GamePadThumbSticks(StickValue left, StickValue right)
	{
		this.left = left;
		this.right = right;
	}
}
