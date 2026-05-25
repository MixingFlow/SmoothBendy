using UnityEngine;

public class BoundObject : MonoBehaviour
{
	[SerializeField]
	private bool _includeLightsInBounds = true;

	[HideInInspector]
	public Bounds bounds = default(Bounds);

	public bool includeLightsInBounds => _includeLightsInBounds;

	private void OnValidate()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		((Bounds)(ref bounds)).center = ((Component)this).transform.position;
		((Bounds)(ref bounds)).size = ((Component)this).transform.localScale;
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isPlaying)
		{
			Gizmos.color = ((!includeLightsInBounds) ? Color.cyan : Color.yellow);
			Gizmos.DrawWireCube(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).size);
		}
	}
}
