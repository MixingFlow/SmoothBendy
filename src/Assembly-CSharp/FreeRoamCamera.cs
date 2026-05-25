using TMG.Controls;
using TMG.Core;
using UnityEngine;

public class FreeRoamCamera : TMGMonoBehaviour
{
	private Transform m_CamParent;

	private Transform m_Cam;

	private bool m_IsActive;

	private Quaternion m_TargetRotation;

	private float m_TSpeed;

	private Transform m_FreeRoamCam
	{
		get
		{
			if (!Object.op_Implicit((Object)(object)m_Cam) && Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
			{
				m_Cam = GameManager.Instance.GameCamera.CameraContainer;
				m_CamParent = m_Cam.parent;
			}
			return m_Cam;
		}
	}

	private void Update()
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.Instance.isPaused)
		{
			return;
		}
		if (Input.GetKeyDown((KeyCode)116))
		{
			m_IsActive = !m_IsActive;
			if (m_IsActive)
			{
				((Component)m_FreeRoamCam).transform.SetParent((Transform)null);
				m_FreeRoamCam.eulerAngles = m_FreeRoamCam.eulerAngles;
				GameManager.Instance.Player.gameObject.SetActive(false);
				GameManager.Instance.HideCrosshair();
				m_TSpeed = 0.8f;
			}
			else
			{
				GameManager.Instance.Player.gameObject.SetActive(true);
				GameManager.Instance.Player.GoToAndLookAt(m_FreeRoamCam);
				m_FreeRoamCam.SetParent(m_CamParent);
				m_FreeRoamCam.localPosition = Vector3.zero;
				m_FreeRoamCam.localEulerAngles = Vector3.zero;
				GameManager.Instance.ShowCrosshair();
				m_TSpeed = 0f;
			}
		}
		if (m_IsActive)
		{
			float num = ((!PlayerInput.Run()) ? 1 : 4);
			Transform freeRoamCam = m_FreeRoamCam;
			freeRoamCam.position += m_FreeRoamCam.right * (PlayerInput.MoveX() * num) + m_FreeRoamCam.forward * (PlayerInput.MoveY() * num);
			Transform freeRoamCam2 = m_FreeRoamCam;
			freeRoamCam2.eulerAngles += new Vector3(0f - PlayerInput.LookY(m_TSpeed) * 2f, PlayerInput.LookX(m_TSpeed) * 2f, 0f);
		}
	}
}
