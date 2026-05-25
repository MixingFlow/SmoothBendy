using System;
using TMG.Controls;
using TMG.Core;
using UnityEngine;

[Serializable]
public class CharacterLook : TMGAbstractDisposable
{
	[SerializeField]
	private float m_Sensitivity = 3f;

	[SerializeField]
	private bool m_ClampVerticalRotation = true;

	[SerializeField]
	private float m_VerticalClamp = 85f;

	private Quaternion m_CharacterTargetRotation;

	private Quaternion m_CameraTargetRotation;

	private bool m_IsRotationInitialized;

	private float m_HorizontalClamp;

	private float m_InitialVerticalClamp;

	private float turnSpeedBoostTimer;

	private float m_InputX;

	private float m_InputY;

	private float smoothedRatio;

	public bool hasHorizontalLock { get; private set; }

	public void Init(Transform character)
	{
		ResetRotation(character);
		m_InitialVerticalClamp = m_VerticalClamp;
	}

	public void Init(Transform character, Transform camera)
	{
		ResetRotation(character, camera);
		m_InitialVerticalClamp = m_VerticalClamp;
	}

	public void ForceRotation(Quaternion rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_CharacterTargetRotation = rotation;
	}

	public void ForceCameraRotation(Quaternion rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_CameraTargetRotation = rotation;
	}

	public void GetInput()
	{
		float num = 0f - PlayerInput.LookY();
		if (GameManager.Instance.HasController)
		{
			float tSpeed = -0.4f;
			if (turnSpeedBoostTimer > 0.3f)
			{
				tSpeed = 0f;
			}
			float num2 = PlayerInput.LookX(tSpeed);
			if (Mathf.Abs(num2) > 0.1f)
			{
				turnSpeedBoostTimer += Time.unscaledDeltaTime;
			}
			else
			{
				turnSpeedBoostTimer = 0f;
			}
			float num3 = Time.unscaledDeltaTime * 60f;
			float num4 = m_Sensitivity * 5f;
			m_InputX = num2 * num4 * num3;
			m_InputY = num * num4 * num3;
		}
		else
		{
			float num2 = PlayerInput.LookX();
			m_InputX = num2 * m_Sensitivity;
			m_InputY = num * m_Sensitivity;
		}
	}

	public void Rotation(Transform character)
	{
		Rotation(character, null, hasGravity: true);
	}

	public void Rotation(Transform character, bool hasGravity)
	{
		Rotation(character, null, hasGravity);
	}

	public void Rotation(Transform character, params Transform[] cameras)
	{
		for (int i = 0; i < cameras.Length; i++)
		{
			if (Object.op_Implicit((Object)(object)cameras[i]))
			{
				Rotation(character, cameras[i], hasGravity: true);
			}
		}
	}

	public void Rotation(Transform character, Transform camera)
	{
		Rotation(character, camera, hasGravity: true);
	}

	public void Rotation(Transform character, Transform camera, bool hasGravity)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (!IsNullRotation(m_InputX, m_InputY))
		{
			if (hasGravity)
			{
				m_CharacterTargetRotation *= Quaternion.Euler(0f, m_InputX, 0f);
				if (hasHorizontalLock)
				{
					m_CharacterTargetRotation = ClampRotationYAxis(m_CharacterTargetRotation, 0f - m_HorizontalClamp, m_HorizontalClamp);
				}
				character.localRotation = m_CharacterTargetRotation;
				if (Object.op_Implicit((Object)(object)camera))
				{
					m_CameraTargetRotation *= Quaternion.Euler(m_InputY, 0f, 0f);
					if (m_ClampVerticalRotation)
					{
						m_CameraTargetRotation = ClampRotationXAxis(m_CameraTargetRotation, 0f - m_VerticalClamp, m_VerticalClamp);
					}
					camera.localRotation = m_CameraTargetRotation;
					Vector3 localEulerAngles = camera.localEulerAngles;
					localEulerAngles.z = 0f;
					camera.localEulerAngles = localEulerAngles;
				}
			}
			else
			{
				m_CharacterTargetRotation *= Quaternion.Euler(m_InputY, m_InputX, 0f);
				character.localRotation = m_CharacterTargetRotation;
			}
		}
		else
		{
			character.localRotation = m_CharacterTargetRotation;
			if (Object.op_Implicit((Object)(object)camera))
			{
				camera.localRotation = m_CameraTargetRotation;
			}
		}
		UpdateCursorLock();
	}

	public void SmoothLook(Transform character, Transform camera, Vector3 newDirection)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.LookRotation(newDirection);
		character.localRotation = Quaternion.Lerp(character.rotation, val, 2f * Time.deltaTime);
		Vector3 localEulerAngles = default(Vector3);
		((Vector3)(ref localEulerAngles))._002Ector(0f, character.localEulerAngles.y, 0f);
		character.localEulerAngles = localEulerAngles;
		camera.localRotation = Quaternion.Lerp(camera.rotation, val, 2f * Time.deltaTime);
		Quaternion localRotation = camera.localRotation;
		localRotation = ClampRotationXAxis(localRotation, 0f - m_VerticalClamp, m_VerticalClamp);
		localRotation = ClampRotationYAxis(localRotation, 0f - m_HorizontalClamp, m_HorizontalClamp);
		camera.localRotation = localRotation;
		Vector3 localEulerAngles2 = camera.localEulerAngles;
		localEulerAngles2.z = 0f;
		camera.localEulerAngles = localEulerAngles2;
	}

	public void ResetRotation(Transform character)
	{
		ResetRotation(character, null);
	}

	public void ResetRotation(Transform character, Transform camera)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		m_CharacterTargetRotation = character.localRotation;
		if (Object.op_Implicit((Object)(object)camera))
		{
			m_CameraTargetRotation = camera.localRotation;
		}
	}

	public void ResetVerticalClamp()
	{
		m_VerticalClamp = m_InitialVerticalClamp;
	}

	public void SetVerticalClamp(float clamp)
	{
		m_VerticalClamp = clamp;
	}

	public void SetHorizontalClamp(float clamp)
	{
		m_HorizontalClamp = clamp;
	}

	public void HorizontalClampSetActive(bool active)
	{
		hasHorizontalLock = active;
	}

	public void UpdateCursorLock()
	{
		InternalLockUpdate();
	}

	private void InternalLockUpdate()
	{
		Cursor.lockState = (CursorLockMode)1;
	}

	private bool IsNullRotation(float horizontal, float vertical)
	{
		if (horizontal == 0f && vertical == 0f)
		{
			return true;
		}
		if (!m_IsRotationInitialized)
		{
			m_IsRotationInitialized = true;
			return true;
		}
		return false;
	}

	private Quaternion ClampRotationXAxis(Quaternion _quaternion, float minimum, float maximum)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		_quaternion.x /= _quaternion.w;
		_quaternion.y /= _quaternion.w;
		_quaternion.z /= _quaternion.w;
		_quaternion.w = 1f;
		float num = 114.59156f * Mathf.Atan(_quaternion.x);
		num = Mathf.Clamp(num, minimum, maximum);
		_quaternion.x = Mathf.Tan((float)Math.PI / 360f * num);
		return _quaternion;
	}

	private Quaternion ClampRotationYAxis(Quaternion _quaternion, float minimum, float maximum)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		_quaternion.x /= _quaternion.w;
		_quaternion.y /= _quaternion.w;
		_quaternion.z /= _quaternion.w;
		_quaternion.w = 1f;
		float num = 114.59156f * Mathf.Atan(_quaternion.y);
		num = Mathf.Clamp(num, minimum, maximum);
		_quaternion.y = Mathf.Tan((float)Math.PI / 360f * num);
		return _quaternion;
	}

	public void SetSensitivity(float sensitivity)
	{
		m_Sensitivity = sensitivity * 1.5f;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
