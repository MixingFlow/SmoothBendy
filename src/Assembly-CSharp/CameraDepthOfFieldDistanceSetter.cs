using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraDepthOfFieldDistanceSetter : MonoBehaviour
{
	public LayerMask layerMask;

	[Tooltip("How fast does the Depth Of Field effect follow changes to the look distance.")]
	public float smoothTime = 0.5f;

	[Tooltip("How many samples per frame do we take to get the look distance.")]
	public int sampleSize = 5;

	[Tooltip("How big is the sample area for the look distance.")]
	public float sampleSizeMultiplier = 1f;

	protected bool m_manualDOF;

	public float focalDistance;

	[SerializeField]
	private float dofDistanceUpdateThreshold = 0.01f;

	[Header("Debugging")]
	[SerializeField]
	private bool showRaycasts;

	private PostProcessingBehaviour ppb;

	private Settings dofSettings;

	private RaycastHit raycastHit;

	private float currentVelocity;

	private Vector3 previousPosition;

	private Quaternion previousRotation;

	[SerializeField]
	private float previousDepth;

	private Transform nonBobingTransform;

	private const string IGNOREDOF = "IgnoreDoF";

	public bool manualDOF
	{
		get
		{
			return m_manualDOF;
		}
		set
		{
			Debug.Log((object)("<color=red>CDOFDS.manualDOF = " + value + "</color>"), (Object)(object)this);
			m_manualDOF = value;
		}
	}

	private void Start()
	{
		if (sampleSize < 1)
		{
			sampleSize = 1;
		}
		ppb = ((Component)this).GetComponent<PostProcessingBehaviour>();
		if (!Object.op_Implicit((Object)(object)ppb))
		{
			Debug.LogError((object)"Missing PostProcessingBehaviour...", (Object)(object)this);
			return;
		}
		nonBobingTransform = ((Component)this).transform.parent.parent;
		if (((Object)((Component)nonBobingTransform).gameObject).name != "HeadContainer")
		{
			Debug.LogError((object)"Missing the non-bobbing parent 'HeadContainer'", (Object)(object)this);
		}
	}

	private void Update()
	{
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (!((PostProcessingModel)ppb.profile.depthOfField).enabled)
		{
			return;
		}
		if (!manualDOF)
		{
			float num = 0f;
			int num2 = 0;
			for (int i = 0; i < sampleSize; i++)
			{
				Vector3 val = Random.insideUnitSphere * sampleSizeMultiplier;
				if (Physics.Raycast(((Component)this).transform.position + val, ((Component)this).transform.forward, ref raycastHit, float.MaxValue, LayerMask.op_Implicit(layerMask), (QueryTriggerInteraction)1) && !((Component)((RaycastHit)(ref raycastHit)).transform).gameObject.CompareTag("IgnoreDoF"))
				{
					num += ((RaycastHit)(ref raycastHit)).distance;
					num2++;
				}
			}
			if (num2 == 0)
			{
				num = 1000f;
				num2 = 1;
			}
			focalDistance = Mathf.SmoothDamp(focalDistance, num / (float)num2, ref currentVelocity, smoothTime);
		}
		dofSettings = ppb.profile.depthOfField.settings;
		dofSettings.focusDistance = focalDistance;
		ppb.profile.depthOfField.settings = dofSettings;
	}

	private bool HasDepthChanged()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (Physics.Raycast(nonBobingTransform.position, nonBobingTransform.forward, ref raycastHit, float.MaxValue, LayerMask.op_Implicit(layerMask), (QueryTriggerInteraction)1))
		{
			if (Mathf.Abs(previousDepth - ((RaycastHit)(ref raycastHit)).distance) < dofDistanceUpdateThreshold)
			{
				return false;
			}
			previousDepth = ((RaycastHit)(ref raycastHit)).distance;
			return true;
		}
		return true;
	}
}
