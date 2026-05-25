using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class AI_DangerRoom_AI_Controller : MonoBehaviour
{
	[Header("Target Setup")]
	public Transform MoveTowardThis;

	public Animator animatorToUse;

	private int currentAction = 1;

	private bool changeAction;

	private DangerRoom_Zapper.ActivationMode activationMode;

	[HideInInspector]
	public float[] speeds = new float[10];

	[HideInInspector]
	public string[] speedLabels = new string[10];

	private void Update()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyDown((KeyCode)112))
		{
			activationMode = DangerRoom_Zapper.ActivationMode.FROZEN;
		}
		if (currentAction - 1 > animatorToUse.runtimeAnimatorController.animationClips.Length || activationMode != DangerRoom_Zapper.ActivationMode.MOVE)
		{
			return;
		}
		if ((Object)(object)MoveTowardThis != (Object)null)
		{
			Vector3 val = new Vector3(MoveTowardThis.position.x, ((Component)this).transform.position.y, MoveTowardThis.position.z) - ((Component)this).transform.position;
			Quaternion val2 = Quaternion.LookRotation(val);
			((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val2, speeds[currentAction - 1] * Time.deltaTime);
			if (Vector3.Distance(((Component)this).transform.position, MoveTowardThis.position) > 5f)
			{
				((Component)this).transform.Translate(0f, 0f, speeds[currentAction - 1] * Time.deltaTime);
			}
		}
		else
		{
			((Component)this).transform.Translate(0f, 0f, speeds[currentAction - 1] * Time.deltaTime);
		}
	}

	public void SetActivationMode(DangerRoom_Zapper.ActionInfo info)
	{
		if (info.animIndex - 1 <= animatorToUse.runtimeAnimatorController.animationClips.Length || animatorToUse.runtimeAnimatorController.animationClips.Length == 10)
		{
			activationMode = info.sendMode;
			currentAction = info.animIndex;
			animatorToUse.Play(info.animIndex.ToString(), -1, 0f);
			MonoBehaviour.print((object)("SETTING " + ((Object)((Component)this).gameObject).name + " ANIM: " + currentAction + " MODE: " + activationMode));
		}
		else
		{
			activationMode = DangerRoom_Zapper.ActivationMode.STOP;
			currentAction = 1;
			animatorToUse.Play(info.animIndex.ToString());
			MonoBehaviour.print((object)("NO ANIMATION FOR " + ((Object)((Component)this).gameObject).name + " AT: " + info.animIndex));
		}
	}
}
