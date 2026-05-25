using TMG.Core;
using UnityEngine;
using UnityEngine.PostProcessing;

public class GameCamera : TMGMonoBehaviour
{
	[SerializeField]
	private Camera m_WeaponCamera;

	[SerializeField]
	private Transform m_HeadContainer;

	[SerializeField]
	private Transform m_CameraContainer;

	private VolumetricLightRenderer m_VolumetricLighting;

	private CameraDepthOfFieldDistanceSetter m_UnityDOF;

	private GameObject m_ParticleField;

	private float fov = 54f;

	public Transform FreeRoamCam { get; private set; }

	public Transform HeadContainer => m_HeadContainer;

	public Camera WeaponCamera => m_WeaponCamera;

	public Transform CameraContainer => m_CameraContainer;

	public Camera Camera { get; private set; }

	public VisionEffectController VisionEffect { get; private set; }

	public BrightnessImageEffect Brightness { get; private set; }

	public bool VolumetricLighting
	{
		get
		{
			return ((Behaviour)m_VolumetricLighting).enabled;
		}
		set
		{
			((Behaviour)m_VolumetricLighting).enabled = value;
		}
	}

	public CameraDepthOfFieldDistanceSetter UnityDOF => m_UnityDOF;

	public bool DoF
	{
		get
		{
			return ((PostProcessingModel)PostProcessBerhaviourDepth.profile.depthOfField).enabled;
		}
		set
		{
			((PostProcessingModel)PostProcessBerhaviourDepth.profile.depthOfField).enabled = value;
		}
	}

	public bool Dust
	{
		get
		{
			return m_ParticleField.activeSelf;
		}
		set
		{
			m_ParticleField.SetActive(value);
		}
	}

	public PostProcessingBehaviour PostProcessBerhaviourDepth { get; private set; }

	public PostProcessingProfile m_PostProcessingProfileDepth { get; private set; }

	public bool Bloom
	{
		get
		{
			return ((PostProcessingModel)m_PostProcessingProfileDepth.bloom).enabled;
		}
		set
		{
			((PostProcessingModel)m_PostProcessingProfileDepth.bloom).enabled = value;
			((PostProcessingModel)m_PostProcessingProfileColor.bloom).enabled = value;
		}
	}

	public bool AA
	{
		get
		{
			return ((PostProcessingModel)m_PostProcessingProfileDepth.antialiasing).enabled;
		}
		set
		{
			((PostProcessingModel)m_PostProcessingProfileDepth.antialiasing).enabled = value;
		}
	}

	public bool AmbientOcclusion
	{
		get
		{
			return ((PostProcessingModel)m_PostProcessingProfileDepth.ambientOcclusion).enabled;
		}
		set
		{
			((PostProcessingModel)m_PostProcessingProfileDepth.ambientOcclusion).enabled = value;
		}
	}

	public bool Fog
	{
		get
		{
			return ((PostProcessingModel)m_PostProcessingProfileDepth.fog).enabled;
		}
		set
		{
			((PostProcessingModel)m_PostProcessingProfileDepth.fog).enabled = value;
		}
	}

	public PostProcessingBehaviour PostProcessBerhaviourColor { get; private set; }

	public PostProcessingProfile m_PostProcessingProfileColor { get; private set; }

	public bool Grain
	{
		get
		{
			return ((PostProcessingModel)m_PostProcessingProfileColor.grain).enabled;
		}
		set
		{
			((PostProcessingModel)m_PostProcessingProfileColor.grain).enabled = value;
		}
	}

	public bool MotionBlur
	{
		get
		{
			return ((PostProcessingModel)m_PostProcessingProfileColor.motionBlur).enabled;
		}
		set
		{
			((PostProcessingModel)m_PostProcessingProfileColor.motionBlur).enabled = value;
		}
	}

	public float FOV
	{
		get
		{
			return fov;
		}
		set
		{
			fov = value;
		}
	}

	public override void Init()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		FreeRoamCam = new GameObject("FreeRoamCam").transform;
		Camera = ((Component)this).GetComponent<Camera>();
		((Behaviour)Camera).enabled = false;
		VisionEffect = ((Component)this).GetComponent<VisionEffectController>();
		Brightness = ((Component)WeaponCamera).GetComponent<BrightnessImageEffect>();
		GameManager.Instance.GameCamera = this;
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ParticleField = ((Component)((Component)GameManager.Instance.Player.HeadContainer).GetComponentInChildren<PAParticleField>()).gameObject;
		Dust = GameManager.Instance.PlayerSettings.DustParticles;
		m_VolumetricLighting = ((Component)this).GetComponent<VolumetricLightRenderer>();
		VolumetricLighting = GameManager.Instance.PlayerSettings.VolumetricLighting;
		m_UnityDOF = ((Component)this).GetComponent<CameraDepthOfFieldDistanceSetter>();
		PostProcessBerhaviourDepth = ((Component)this).GetComponent<PostProcessingBehaviour>();
		m_PostProcessingProfileDepth = PostProcessBerhaviourDepth.profile;
		PostProcessBerhaviourColor = ((Component)WeaponCamera).GetComponent<PostProcessingBehaviour>();
		m_PostProcessingProfileColor = PostProcessBerhaviourColor.profile;
		Bloom = GameManager.Instance.PlayerSettings.Bloom;
		AmbientOcclusion = GameManager.Instance.PlayerSettings.AmbientOcclusion;
		MotionBlur = GameManager.Instance.PlayerSettings.MotionBlur;
		Grain = GameManager.Instance.PlayerSettings.Grain;
		Fog = GameManager.Instance.PlayerSettings.Fog;
		AA = GameManager.Instance.PlayerSettings.AA;
		DoF = GameManager.Instance.PlayerSettings.DoF;
		((Behaviour)Camera).enabled = true;
	}

	public Transform InitializeFreeRoamCam()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.Player.EnableSeeingTool(active: false);
		GameManager.Instance.Player.SetLockedMovement(active: true);
		Transform freeRoamCam = GameManager.Instance.GameCamera.FreeRoamCam;
		freeRoamCam.position = GameManager.Instance.GameCamera.CameraContainer.position;
		freeRoamCam.rotation = GameManager.Instance.GameCamera.CameraContainer.rotation;
		GameManager.Instance.GameCamera.CameraContainer.SetParent(freeRoamCam);
		GameManager.Instance.GameCamera.CameraContainer.localScale = Vector3.one;
		return freeRoamCam;
	}

	public void ExitFreeRoamCam()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.zero;
		zero.x = FreeRoamCam.eulerAngles.x;
		Vector3 zero2 = Vector3.zero;
		zero2.y = FreeRoamCam.localEulerAngles.y;
		GameManager.Instance.GameCamera.CameraContainer.SetParent(GameManager.Instance.GameCamera.HeadContainer);
		GameManager.Instance.GameCamera.CameraContainer.localPosition = Vector3.zero;
		GameManager.Instance.GameCamera.CameraContainer.localEulerAngles = Vector3.zero;
		GameManager.Instance.GameCamera.CameraContainer.localScale = Vector3.one;
		GameManager.Instance.Player.LookRotation(Quaternion.Euler(zero2), Quaternion.Euler(zero));
		GameManager.Instance.Player.SetLockedMovement(active: false);
		GameManager.Instance.GameCamera.FreeRoamCam.SetParent((Transform)null);
		GameManager.Instance.Player.EnableSeeingTool(active: true);
	}

	protected override void OnDisposed()
	{
		Camera = null;
		m_WeaponCamera = null;
		m_UnityDOF = null;
		m_VolumetricLighting = null;
		PostProcessBerhaviourDepth = null;
		PostProcessBerhaviourColor = null;
		base.OnDisposed();
	}
}
