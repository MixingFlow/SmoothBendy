using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class PAParticleField : MonoBehaviour
{
	public enum ParticleType
	{
		Billboard,
		Mesh,
		Custom
	}

	public enum SimulationSpace
	{
		World,
		Local,
		LocalWithDelta
	}

	public enum Shape
	{
		Cube,
		Sphere,
		Cylinder
	}

	public enum EdgeMode
	{
		Alpha,
		Scale,
		Both
	}

	public enum MaterialType
	{
		Transparent,
		TransparentLit,
		Additive,
		AdditiveLit,
		CutOff,
		CutOffLit,
		Custom,
		MeshDefault,
		MeshUnlit
	}

	public enum TextureType
	{
		Simple,
		SpriteGrid,
		AnimatedRows
	}

	public enum SoftParticleType
	{
		None,
		NearClipOnly,
		NearClipAndCameraDepth
	}

	public enum TurbulenceType
	{
		None,
		Simplex2D,
		Simplex
	}

	private static readonly string[] builtinShaderNames = new string[9] { "PA/ParticleField/Transparent", "PA/ParticleField/TransparentLit", "PA/ParticleField/Additive", "PA/ParticleField/AdditiveLit", "PA/ParticleField/CutOff", "PA/ParticleField/CutOffLit", "DoNotUse", "PA/ParticleField/MeshDefault", "PA/ParticleField/MeshUnlit" };

	private const int MAX_PARTICLE_COUNT = 16250;

	public bool clearCacheInBuilds;

	private bool isOpenGL;

	[SerializeField]
	private int mSeed = 1234;

	[SerializeField]
	private ParticleType mGeneratorType;

	[SerializeField]
	private Mesh mInputMesh;

	[SerializeField]
	private int mParticleCount = 1200;

	[SerializeField]
	private float mParticleCountMask = 1f;

	[SerializeField]
	private Vector3 mFieldSize = new Vector3(10f, 10f, 10f);

	[SerializeField]
	private Vector3 mEdgeThreshold = Vector3.one;

	[SerializeField]
	private EdgeMode mEdgeMode;

	[SerializeField]
	private SimulationSpace mSimulationSpace;

	[SerializeField]
	private Shape mShape;

	[SerializeField]
	private bool mUseExclusionZones;

	[SerializeField]
	private Transform mExclusionAnchorOverride;

	[SerializeField]
	private Vector2 mParticleSize = new Vector2(0.1f, 0.1f);

	[SerializeField]
	private float mSpeed = 0.1f;

	[SerializeField]
	private Vector3 mSpeedMask = Vector3.one;

	[SerializeField]
	private Color mColor = Color.white;

	[SerializeField]
	private Vector3 mForce = Vector3.zero;

	[SerializeField]
	private bool mCustomFacingDirection;

	[SerializeField]
	private Vector3 mFacingDirection = Vector3.up;

	[SerializeField]
	private bool mCustomUpDirection;

	[SerializeField]
	private Vector3 mUpDirection = new Vector3(0f, 1f, 0f);

	[SerializeField]
	private bool mStretchedBillboard;

	[SerializeField]
	private float mSpeedScaleMultiplier = 10f;

	[SerializeField]
	private bool mSpin;

	[SerializeField]
	private float mSpinSpeed;

	[SerializeField]
	private float mMinSpinSpeed = -1f;

	[SerializeField]
	private bool mCustomRotationAxis;

	[SerializeField]
	private Vector3 mRotationAxis = new Vector3(0f, 1f, 0f);

	[SerializeField]
	private SoftParticleType mSoftParticles = SoftParticleType.NearClipOnly;

	[SerializeField]
	private float mNearFadeDistance = 1f;

	[SerializeField]
	private float mNearFadeOffset;

	[SerializeField]
	private float mSoftness = 0.5f;

	[SerializeField]
	private TurbulenceType mTurbulenceType;

	[SerializeField]
	private float mTurbulenceFrequency = 1f;

	[SerializeField]
	private float mTurbulenceAmplitude = 1f;

	[SerializeField]
	private Vector3 mTurbulenceScale = Vector3.one;

	[SerializeField]
	private Vector3 mTurbulenceOffsetSpeed = new Vector3(0f, 0.25f, 0f);

	[SerializeField]
	private Gradient mColorVariation = new Gradient();

	[SerializeField]
	private float mMinimumSize = 1f;

	[SerializeField]
	private float mMinimumSpeed = 1f;

	[SerializeField]
	private MaterialType mMaterialType;

	[SerializeField]
	private Shader mShader;

	[SerializeField]
	private Texture2D mTexture;

	[SerializeField]
	private Vector2 mPivotOffset = new Vector2(0f, 0f);

	[SerializeField]
	private TextureType mTextureType;

	[SerializeField]
	private int mSpriteColumns = 1;

	[SerializeField]
	private int mSpriteRows = 1;

	[SerializeField]
	private float mFramerate = 16f;

	[SerializeField]
	private float mCutOff = 0.01f;

	[SerializeField]
	private bool mReceiveShadows;

	[SerializeField]
	private ShadowCastingMode mCastShadows;

	private Mesh particleMesh;

	private MeshFilter meshFilter;

	private MeshRenderer meshRenderer;

	[SerializeField]
	private PAParticleMeshGenerator m_MeshGenerator;

	[SerializeField]
	public Material material;

	private Material renderingMaterial;

	private float time;

	private Vector3 speedTime = Vector3.zero;

	private Vector3 forceTime = Vector3.zero;

	private float spinTime;

	private Vector3 turbulenceOffsetTime = Vector3.zero;

	private float frameTime;

	private Vector3 position;

	private Vector3 deltaPosition;

	private Vector3 scale;

	private bool foundExclusionZones;

	private PAExclusionZone[] zones = new PAExclusionZone[3];

	[SerializeField]
	private Material temporarySerializableMaterial;

	public int seed
	{
		get
		{
			return mSeed;
		}
		set
		{
			if (mSeed != value)
			{
				mSeed = value;
				meshIsDirtyMask |= MeshFlags.Seed;
			}
		}
	}

	public ParticleType generatorType
	{
		get
		{
			return mGeneratorType;
		}
		set
		{
			if (mGeneratorType != value)
			{
				mGeneratorType = value;
				meshIsDirtyMask |= MeshFlags.Generator;
			}
		}
	}

	public int particleCount
	{
		get
		{
			return mParticleCount;
		}
		set
		{
			value = ((!Object.op_Implicit((Object)(object)meshGenerator)) ? Mathf.Clamp(value, 0, 16250) : meshGenerator.GetClampedParticleCount(value));
			if (mParticleCount != value)
			{
				mParticleCount = value;
				meshIsDirtyMask |= MeshFlags.Count;
			}
		}
	}

	public float particleCountMask
	{
		get
		{
			return mParticleCountMask;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (mParticleCountMask != value)
			{
				mParticleCountMask = value;
				shaderIsDirty = true;
			}
		}
	}

	public Vector3 fieldSize
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mFieldSize;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mFieldSize != value)
			{
				mFieldSize = value;
				shaderIsDirty = true;
			}
		}
	}

	public Vector3 edgeThreshold
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mEdgeThreshold;
		}
		set
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			value.x = Mathf.Clamp01(value.x);
			value.y = Mathf.Clamp01(value.y);
			value.z = Mathf.Clamp01(value.z);
			if (mEdgeThreshold != value)
			{
				mEdgeThreshold = value;
				shaderIsDirty = true;
			}
		}
	}

	public SimulationSpace simulationSpace
	{
		get
		{
			return mSimulationSpace;
		}
		set
		{
			if (mSimulationSpace != value)
			{
				mSimulationSpace = value;
				shaderIsDirty = true;
			}
		}
	}

	public Shape shape
	{
		get
		{
			return mShape;
		}
		set
		{
			if (mShape != value)
			{
				mShape = value;
				shaderIsDirty = true;
			}
		}
	}

	public EdgeMode edgeMode
	{
		get
		{
			return mEdgeMode;
		}
		set
		{
			if (mEdgeMode != value)
			{
				mEdgeMode = value;
				shaderIsDirty = true;
			}
		}
	}

	public bool useExclusionZones
	{
		get
		{
			return mUseExclusionZones;
		}
		set
		{
			if (mUseExclusionZones != value)
			{
				mUseExclusionZones = value;
				shaderIsDirty = true;
			}
		}
	}

	public Transform exclusionAnchorOverride
	{
		get
		{
			return mExclusionAnchorOverride;
		}
		set
		{
			if ((Object)(object)mExclusionAnchorOverride != (Object)(object)value)
			{
				mExclusionAnchorOverride = value;
			}
		}
	}

	public Color color
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mColor != value)
			{
				mColor = value;
				shaderIsDirty = true;
			}
		}
	}

	public float alpha
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return color.a;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (color.a != value)
			{
				Color val = color;
				val.a = value;
				color = val;
			}
		}
	}

	public float speed
	{
		get
		{
			return mSpeed;
		}
		set
		{
			if (mSpeed != value)
			{
				mSpeed = value;
				shaderIsDirty = true;
			}
		}
	}

	public Vector3 speedMask
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mSpeedMask;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mSpeedMask != value)
			{
				mSpeedMask = value;
				shaderIsDirty = true;
			}
		}
	}

	public Vector2 particleSize
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mParticleSize;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mParticleSize != value)
			{
				mParticleSize = value;
				shaderIsDirty = true;
			}
		}
	}

	public bool stretchedBillboard
	{
		get
		{
			return mStretchedBillboard;
		}
		set
		{
			if (mStretchedBillboard != value)
			{
				mStretchedBillboard = value;
				if (value)
				{
					mCustomUpDirection = false;
					mSpin = false;
				}
				shaderIsDirty = true;
			}
		}
	}

	public float speedScaleMultiplier
	{
		get
		{
			return mSpeedScaleMultiplier;
		}
		set
		{
			if (mSpeedScaleMultiplier != value)
			{
				mSpeedScaleMultiplier = value;
				shaderIsDirty = true;
			}
		}
	}

	public bool spin
	{
		get
		{
			return mSpin;
		}
		set
		{
			if (mSpin != value)
			{
				mSpin = value;
				if (value)
				{
					mStretchedBillboard = false;
					mCustomUpDirection = false;
				}
				shaderIsDirty = true;
			}
		}
	}

	public float spinSpeed
	{
		get
		{
			return mSpinSpeed;
		}
		set
		{
			if (mSpinSpeed != value)
			{
				mSpinSpeed = value;
				shaderIsDirty = true;
			}
		}
	}

	public float minSpinSpeed
	{
		get
		{
			return mMinSpinSpeed;
		}
		set
		{
			value = Mathf.Clamp(value, -1f, 1f);
			if (mMinSpinSpeed != value)
			{
				mMinSpinSpeed = value;
				meshIsDirtyMask |= MeshFlags.Speed;
			}
		}
	}

	public bool customRotationAxis
	{
		get
		{
			return mCustomRotationAxis;
		}
		set
		{
			if (mCustomRotationAxis != value)
			{
				mCustomRotationAxis = value;
				meshIsDirtyMask |= MeshFlags.Speed;
			}
		}
	}

	public Vector3 rotationAxis
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mRotationAxis;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mRotationAxis != value)
			{
				mRotationAxis = value;
				meshIsDirtyMask |= MeshFlags.Speed;
			}
		}
	}

	public bool customFacingDirection
	{
		get
		{
			return mCustomFacingDirection;
		}
		set
		{
			if (mCustomFacingDirection != value)
			{
				mCustomFacingDirection = value;
				shaderIsDirty = true;
			}
		}
	}

	public Vector3 facingDirection
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mFacingDirection;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mFacingDirection != value)
			{
				mFacingDirection = value;
				mStretchedBillboard = false;
				shaderIsDirty = true;
			}
		}
	}

	public bool customUpDirection
	{
		get
		{
			return mCustomUpDirection;
		}
		set
		{
			if (mCustomUpDirection != value)
			{
				mCustomUpDirection = value;
				if (value)
				{
					mSpin = false;
					mStretchedBillboard = false;
				}
				shaderIsDirty = true;
			}
		}
	}

	public Vector3 upDirection
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mUpDirection;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mUpDirection != value)
			{
				mUpDirection = value;
				shaderIsDirty = true;
			}
		}
	}

	public SoftParticleType softParticles
	{
		get
		{
			return mSoftParticles;
		}
		set
		{
			if (value == SoftParticleType.NearClipAndCameraDepth && int.Parse(Application.unityVersion.Split('.')[0]) < 5 && !Application.HasProLicense())
			{
				Debug.Log((object)"Soft particles requires Unity Pro");
				if (mSoftParticles != SoftParticleType.NearClipOnly)
				{
					mSoftParticles = SoftParticleType.NearClipOnly;
					shaderIsDirty = true;
				}
			}
			else if (mSoftParticles != value)
			{
				mSoftParticles = value;
				shaderIsDirty = true;
			}
		}
	}

	public float nearFadeDistance
	{
		get
		{
			return mNearFadeDistance;
		}
		set
		{
			if (mNearFadeDistance != value)
			{
				mNearFadeDistance = value;
				shaderIsDirty = true;
			}
		}
	}

	public float nearFadeOffset
	{
		get
		{
			return mNearFadeOffset;
		}
		set
		{
			if (mNearFadeOffset != value)
			{
				mNearFadeOffset = value;
				shaderIsDirty = true;
			}
		}
	}

	public float softness
	{
		get
		{
			return mSoftness;
		}
		set
		{
			if (mSoftness != value)
			{
				mSoftness = value;
				shaderIsDirty = true;
			}
		}
	}

	public TurbulenceType turbulenceType
	{
		get
		{
			return mTurbulenceType;
		}
		set
		{
			if (mTurbulenceType != value)
			{
				mTurbulenceType = value;
				shaderIsDirty = true;
			}
		}
	}

	public float turbulenceFrequency
	{
		get
		{
			return mTurbulenceFrequency;
		}
		set
		{
			if (mTurbulenceFrequency != value)
			{
				mTurbulenceFrequency = value;
				shaderIsDirty = true;
			}
		}
	}

	public float turbulenceAmplitude
	{
		get
		{
			return mTurbulenceAmplitude;
		}
		set
		{
			if (mTurbulenceAmplitude != value)
			{
				mTurbulenceAmplitude = value;
				shaderIsDirty = true;
			}
		}
	}

	public Vector3 turbulenceScale
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mTurbulenceScale;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mTurbulenceScale != value)
			{
				mTurbulenceScale = value;
				shaderIsDirty = true;
			}
		}
	}

	public Vector3 turbulenceOffsetSpeed
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mTurbulenceOffsetSpeed;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mTurbulenceOffsetSpeed != value)
			{
				mTurbulenceOffsetSpeed = value;
				shaderIsDirty = true;
			}
		}
	}

	public Gradient colorVariation
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			if (mColorVariation == null)
			{
				mColorVariation = new Gradient();
			}
			return mColorVariation;
		}
		set
		{
			if (mColorVariation != value)
			{
				mColorVariation = value;
				meshIsDirtyMask |= MeshFlags.Color;
			}
		}
	}

	public float minimumSize
	{
		get
		{
			return mMinimumSize;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (mMinimumSize != value)
			{
				mMinimumSize = value;
				meshIsDirtyMask |= MeshFlags.Surface;
			}
		}
	}

	public float minimumSpeed
	{
		get
		{
			return mMinimumSpeed;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (mMinimumSpeed != value)
			{
				mMinimumSpeed = value;
				meshIsDirtyMask |= MeshFlags.Speed;
			}
		}
	}

	public Vector3 force
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mForce;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mForce != value)
			{
				mForce = value;
				shaderIsDirty = true;
			}
		}
	}

	public MaterialType materialType
	{
		get
		{
			return mMaterialType;
		}
		set
		{
			if (mMaterialType != value)
			{
				material = null;
				mMaterialType = value;
				if (mMaterialType != MaterialType.Custom)
				{
					shader = Shader.Find(builtinShaderNames[(int)mMaterialType]);
					shaderIsDirty = true;
				}
			}
		}
	}

	public Shader shader
	{
		get
		{
			return mShader;
		}
		private set
		{
			if ((Object)(object)mShader != (Object)(object)value)
			{
				mShader = value;
				shaderIsDirty = true;
			}
		}
	}

	public Texture2D texture
	{
		get
		{
			return mTexture;
		}
		set
		{
			if ((Object)(object)mTexture != (Object)(object)value)
			{
				mTexture = value;
				shaderIsDirty = true;
			}
		}
	}

	public TextureType textureType
	{
		get
		{
			return mTextureType;
		}
		set
		{
			if (mTextureType != value)
			{
				mTextureType = value;
				shaderIsDirty = true;
				meshIsDirtyMask |= MeshFlags.Surface;
			}
		}
	}

	public int spriteColumns
	{
		get
		{
			return mSpriteColumns;
		}
		set
		{
			value = Mathf.Min(value, 1);
			if (mSpriteColumns != value)
			{
				mSpriteColumns = value;
				meshIsDirtyMask |= MeshFlags.Surface;
			}
		}
	}

	public int spriteRows
	{
		get
		{
			return mSpriteRows;
		}
		set
		{
			value = Mathf.Min(value, 1);
			if (mSpriteRows != value)
			{
				mSpriteRows = value;
				meshIsDirtyMask |= MeshFlags.Surface;
			}
		}
	}

	public float framerate
	{
		get
		{
			return mFramerate;
		}
		set
		{
			if (mFramerate != value)
			{
				mFramerate = value;
			}
		}
	}

	public float cutOff
	{
		get
		{
			return mCutOff;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (mCutOff != value)
			{
				mCutOff = value;
				shaderIsDirty = true;
			}
		}
	}

	public Vector2 pivotOffset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mPivotOffset;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mPivotOffset != value)
			{
				mPivotOffset = value;
				meshIsDirtyMask |= MeshFlags.Surface;
			}
		}
	}

	public Mesh inputMesh
	{
		get
		{
			return mInputMesh;
		}
		set
		{
			mInputMesh = value;
			meshIsDirtyMask |= MeshFlags.All;
		}
	}

	public bool receiveShadows
	{
		get
		{
			return mReceiveShadows;
		}
		set
		{
			mReceiveShadows = value;
			if (Object.op_Implicit((Object)(object)meshRenderer))
			{
				((Renderer)meshRenderer).receiveShadows = value;
			}
		}
	}

	public ShadowCastingMode castShadows
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mCastShadows;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			mCastShadows = value;
			if (Object.op_Implicit((Object)(object)meshRenderer))
			{
				((Renderer)meshRenderer).shadowCastingMode = value;
			}
		}
	}

	private PAParticleMeshGenerator meshGenerator
	{
		get
		{
			if ((Object)(object)m_MeshGenerator == (Object)null)
			{
				m_MeshGenerator = ((Component)this).GetComponent<PAParticleMeshGenerator>();
				if ((Object)(object)m_MeshGenerator == (Object)null && generatorType != ParticleType.Custom)
				{
					UpdateGeneratorType(generatorType);
				}
			}
			return m_MeshGenerator;
		}
		set
		{
			m_MeshGenerator = value;
		}
	}

	public MeshFlags meshIsDirtyMask { get; set; }

	public bool shaderIsDirty { get; set; }

	private T GetOrAddComponent<T>() where T : Component
	{
		T val = ((Component)this).GetComponent<T>();
		if (!Object.op_Implicit((Object)(object)val))
		{
			val = ((Component)this).gameObject.AddComponent<T>();
		}
		return val;
	}

	private void GetRenderingComponents()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		meshFilter = GetOrAddComponent<MeshFilter>();
		((Object)meshFilter).hideFlags = (HideFlags)3;
		meshRenderer = GetOrAddComponent<MeshRenderer>();
		((Object)meshRenderer).hideFlags = (HideFlags)3;
		((Renderer)meshRenderer).receiveShadows = mReceiveShadows;
		((Renderer)meshRenderer).shadowCastingMode = mCastShadows;
	}

	private void CreateAssetTypes()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		if (!Object.op_Implicit((Object)(object)particleMesh))
		{
			particleMesh = new Mesh();
			((Object)particleMesh).name = ((Object)((Component)this).gameObject).name + "_PAPF";
		}
		((Object)particleMesh).hideFlags = (HideFlags)63;
		meshFilter.sharedMesh = particleMesh;
		if (!Object.op_Implicit((Object)(object)shader))
		{
			shader = Shader.Find("PA/ParticleField/Transparent");
		}
		renderingMaterial = CreateInstanceMaterial();
		((Renderer)meshRenderer).sharedMaterial = renderingMaterial;
	}

	private Material CreateInstanceMaterial()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		Material val = new Material((materialType != MaterialType.Custom || !Object.op_Implicit((Object)(object)material)) ? shader : material.shader);
		((Object)val).name = ((Object)((Component)this).gameObject).name + " (Instance)" + DateTime.Now.Millisecond;
		((Object)val).hideFlags = (HideFlags)63;
		return val;
	}

	private void UpdateGeneratorType(ParticleType newType)
	{
		if (Object.op_Implicit((Object)(object)m_MeshGenerator))
		{
			if (newType != ParticleType.Custom)
			{
				Object.DestroyImmediate((Object)(object)m_MeshGenerator);
			}
			else
			{
				((Object)m_MeshGenerator).hideFlags = (HideFlags)0;
			}
		}
		switch (newType)
		{
		case ParticleType.Billboard:
			m_MeshGenerator = ((Component)this).gameObject.AddComponent<PABillboardParticle>();
			((Object)m_MeshGenerator).hideFlags = (HideFlags)2;
			break;
		case ParticleType.Mesh:
		{
			PAMeshParticle pAMeshParticle = ((Component)this).gameObject.AddComponent<PAMeshParticle>();
			pAMeshParticle.inputMesh = inputMesh;
			m_MeshGenerator = pAMeshParticle;
			((Object)m_MeshGenerator).hideFlags = (HideFlags)2;
			break;
		}
		}
		if (newType != ParticleType.Custom && materialType != MaterialType.Custom)
		{
			if (newType == ParticleType.Mesh)
			{
				materialType = MaterialType.MeshDefault;
			}
			else
			{
				materialType = MaterialType.Transparent;
			}
			CreateAssetTypes();
		}
		mGeneratorType = newType;
		shaderIsDirty = true;
		meshIsDirtyMask = MeshFlags.All;
	}

	private void SetShaderValues()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		PAPFHelper.GetPropertyIDs();
		if (materialType == MaterialType.Custom && (Object)(object)material != (Object)null)
		{
			if ((Object)(object)renderingMaterial.shader != (Object)(object)material.shader)
			{
				renderingMaterial.shader = material.shader;
			}
			renderingMaterial.CopyPropertiesFromMaterial(material);
		}
		else if ((Object)(object)renderingMaterial.shader != (Object)(object)shader)
		{
			renderingMaterial.shader = shader;
		}
		Vector3 val = speed * speedMask;
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(val.x / fieldSize.x, val.y / fieldSize.y, val.z / fieldSize.z);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector((0f - force.x) / fieldSize.x, (0f - force.y) / fieldSize.y, (0f - force.z) / fieldSize.z);
		renderingMaterial.SetVector(PAPFHelper._DeltaSpeed, Vector4.op_Implicit(val2));
		renderingMaterial.SetVector(PAPFHelper._DeltaForce, Vector4.op_Implicit(val3));
		renderingMaterial.SetVector(PAPFHelper._TurbulenceDeltaOffset, Vector4.op_Implicit(turbulenceOffsetSpeed * (1f / 60f)));
		Vector3 val4 = Vector3.Scale(fieldSize, ((Component)this).transform.lossyScale);
		Vector3 val5 = (Vector3)((simulationSpace != SimulationSpace.LocalWithDelta) ? Vector3.zero : new Vector3(deltaPosition.x / val4.x, deltaPosition.y / val4.y, deltaPosition.z / val4.z));
		renderingMaterial.SetVector(PAPFHelper._DeltaPosition, Vector4.op_Implicit(val5));
		if (materialType != MaterialType.Custom)
		{
			renderingMaterial.SetColor(PAPFHelper._Color, color);
		}
		else if ((Object)(object)material != (Object)null && material.HasProperty(PAPFHelper._Color))
		{
			renderingMaterial.SetColor(PAPFHelper._Color, material.color * color);
		}
		if (materialType != MaterialType.Custom)
		{
			renderingMaterial.SetTexture(PAPFHelper._MainTex, (Texture)(object)texture);
			renderingMaterial.SetFloat(PAPFHelper._CutOff, cutOff);
		}
		if (textureType != TextureType.AnimatedRows)
		{
			renderingMaterial.SetFloat(PAPFHelper._UOffset, 0f);
		}
		renderingMaterial.SetFloat(PAPFHelper._CountMask, particleCountMask);
		renderingMaterial.SetFloat(PAPFHelper._ParticleCount, (float)particleCount);
		renderingMaterial.SetVector(PAPFHelper._FieldSize, Vector4.op_Implicit(Vector3.Scale(fieldSize, (simulationSpace != SimulationSpace.World) ? Vector3.one : ((Component)this).transform.lossyScale)));
		renderingMaterial.SetVector(PAPFHelper._EdgeThreshold, Vector4.op_Implicit(Vector3.one - edgeThreshold));
		renderingMaterial.SetVector(PAPFHelper._InverseEdgeThreshold, Vector4.op_Implicit(new Vector3(1f / edgeThreshold.x, 1f / edgeThreshold.y, 1f / edgeThreshold.z)));
		renderingMaterial.SetVector(PAPFHelper._ParticleSize, Vector4.op_Implicit(particleSize));
		renderingMaterial.SetFloat(PAPFHelper._SpeedScale, (!stretchedBillboard) ? 1f : speedScaleMultiplier);
		Material obj = renderingMaterial;
		int faceDirection = PAPFHelper._FaceDirection;
		Vector3 val7;
		if (customFacingDirection)
		{
			Vector3 val6 = facingDirection;
			val7 = ((Vector3)(ref val6)).normalized + Vector3.right * 0.001f;
		}
		else
		{
			val7 = Vector3.forward;
		}
		obj.SetVector(faceDirection, Vector4.op_Implicit(val7));
		Material obj2 = renderingMaterial;
		int num = PAPFHelper._UpDirection;
		Vector3 val9;
		if (mCustomUpDirection)
		{
			Vector3 val8 = upDirection;
			val9 = ((Vector3)(ref val8)).normalized;
		}
		else
		{
			val9 = Vector3.up;
		}
		obj2.SetVector(num, Vector4.op_Implicit(val9));
		renderingMaterial.SetFloat(PAPFHelper._NearFadeDistance, (softParticles == SoftParticleType.None) ? 0f : nearFadeDistance);
		renderingMaterial.SetFloat(PAPFHelper._NearFadeOffset, (softParticles == SoftParticleType.None) ? 0f : nearFadeOffset);
		renderingMaterial.SetFloat(PAPFHelper._Softness, softness);
		renderingMaterial.SetFloat(PAPFHelper._TurbulenceFrequency, turbulenceFrequency);
		renderingMaterial.SetVector(PAPFHelper._TurbulenceScale, Vector4.op_Implicit(turbulenceScale * turbulenceAmplitude));
		SetFloatKeyword(PAPFHelper._EdgeAlpha, edgeMode == EdgeMode.Alpha || edgeMode == EdgeMode.Both);
		SetFloatKeyword(PAPFHelper._EdgeScale, edgeMode == EdgeMode.Scale || edgeMode == EdgeMode.Both);
		SetFloatKeyword(PAPFHelper._UserFacing, customFacingDirection);
		SetFloatKeyword(PAPFHelper._Editor, !Application.isPlaying);
		SetKeyword("DIRECTIONAL_ON", stretchedBillboard);
		SetKeyword("WORLDSPACE_ON", simulationSpace == SimulationSpace.World);
		SetKeyword("SPIN_ON", spin);
		SetKeyword("TURBULENCE_SIMPLEX2D", turbulenceType == TurbulenceType.Simplex2D);
		SetKeyword("TURBULENCE_SIMPLEX", turbulenceType == TurbulenceType.Simplex);
		SetKeyword("SHAPE_SPHERE", shape == Shape.Sphere);
		SetKeyword("SHAPE_CYLINDER", shape == Shape.Cylinder);
		float num2 = ((generatorType != ParticleType.Mesh) ? Mathf.Max(particleSize.x, particleSize.y) : particleSize.x);
		if (Object.op_Implicit((Object)(object)particleMesh) && Object.op_Implicit((Object)(object)meshGenerator))
		{
			particleMesh.bounds = new Bounds(Vector3.zero, fieldSize + num2 * meshGenerator.GetParticleBaseSize() * 2f * Vector3.one);
		}
	}

	private void SetKeyword(string keyword, bool enable)
	{
		SetMaterialKeyword(keyword, enable, renderingMaterial);
	}

	private static void SetMaterialKeyword(string keyword, bool enable, Material material)
	{
		if (Object.op_Implicit((Object)(object)material))
		{
			if (enable)
			{
				material.EnableKeyword(keyword);
			}
			else
			{
				material.DisableKeyword(keyword);
			}
		}
	}

	private void SetFloatKeyword(string keyword, bool enable)
	{
		renderingMaterial.SetFloat(keyword, (float)(enable ? 1 : 0));
	}

	private void SetFloatKeyword(int keywordID, bool enable)
	{
		renderingMaterial.SetFloat(keywordID, (float)(enable ? 1 : 0));
	}

	public void UpdateParticleField()
	{
		UpdateMesh();
		UpdateShader();
	}

	public void UpdateMesh()
	{
		if ((meshIsDirtyMask & MeshFlags.Generator) != MeshFlags.None)
		{
			UpdateGeneratorType(generatorType);
		}
		if ((Object)(object)meshGenerator != (Object)null)
		{
			meshGenerator.UpdateMesh(particleMesh, this);
		}
		meshIsDirtyMask = MeshFlags.None;
	}

	public void UpdateShader()
	{
		SetShaderValues();
		shaderIsDirty = false;
	}

	private void UpdateAnimationValues()
	{
		if (Application.isPlaying)
		{
			Simulate(Time.deltaTime);
		}
	}

	public void Simulate(float t, bool restart = false)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (restart)
		{
			ResetTimers();
		}
		time += t;
		speedTime += speed * speedMask * t;
		forceTime += force * (0f - t);
		spinTime += spinSpeed * t;
		turbulenceOffsetTime += turbulenceOffsetSpeed * t;
		frameTime += t * framerate;
	}

	private void UpdateExclusionZoneValues()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (useExclusionZones)
		{
			foundExclusionZones = PAExclusionZone.GetExclusionZones(ref zones, (!Object.op_Implicit((Object)(object)exclusionAnchorOverride)) ? ((Component)this).transform.position : exclusionAnchorOverride.position, new Bounds(((Component)this).transform.position, fieldSize), ((Component)this).gameObject.layer);
		}
	}

	private void SetShaderAnimationValues()
	{
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying)
		{
			renderingMaterial.SetVector(PAPFHelper._Speed, Vector4.op_Implicit(new Vector3(speedTime.x / fieldSize.x, speedTime.y / fieldSize.y, speedTime.z / fieldSize.z)));
			renderingMaterial.SetVector(PAPFHelper._Force, Vector4.op_Implicit(new Vector3(forceTime.x / fieldSize.x, forceTime.y / fieldSize.y, forceTime.z / fieldSize.z)));
			renderingMaterial.SetFloat(PAPFHelper._SpinSpeed, spinTime * 0.5f * 3.145f / 180f);
			renderingMaterial.SetFloat(PAPFHelper._TotalTime, time);
			renderingMaterial.SetVector(PAPFHelper._TurbulenceOffset, Vector4.op_Implicit(turbulenceOffsetTime));
			if (textureType == TextureType.AnimatedRows)
			{
				int num = (int)Mathf.Repeat(frameTime, (float)spriteColumns);
				renderingMaterial.SetFloat(PAPFHelper._UOffset, (float)num / (float)spriteColumns);
			}
			return;
		}
		Vector3 val = speed * speedMask;
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(val.x / fieldSize.x, val.y / fieldSize.y, val.z / fieldSize.z);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector((0f - force.x) / fieldSize.x, (0f - force.y) / fieldSize.y, (0f - force.z) / fieldSize.z);
		renderingMaterial.SetVector(PAPFHelper._Speed, Vector4.op_Implicit(val2));
		renderingMaterial.SetVector(PAPFHelper._Force, Vector4.op_Implicit(val3));
		renderingMaterial.SetFloat(PAPFHelper._SpinSpeed, spinSpeed * 0.5f * 3.14159f / 180f);
		renderingMaterial.SetFloat(PAPFHelper._TotalTime, Time.timeSinceLevelLoad);
		renderingMaterial.SetVector(PAPFHelper._TurbulenceOffset, Vector4.op_Implicit(turbulenceOffsetSpeed));
		if (textureType == TextureType.AnimatedRows)
		{
			int num2 = (int)Mathf.Repeat(Time.timeSinceLevelLoad * framerate, (float)spriteColumns);
			renderingMaterial.SetFloat(PAPFHelper._UOffset, (float)num2 / (float)spriteColumns);
		}
	}

	private void SetShaderExclusionZoneValues()
	{
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		if (!useExclusionZones || !foundExclusionZones)
		{
			renderingMaterial.DisableKeyword("EXCLUSION_ON");
			return;
		}
		renderingMaterial.EnableKeyword("EXCLUSION_ON");
		Vector3 val3 = default(Vector3);
		for (int i = 0; i < zones.Length; i++)
		{
			if ((Object)(object)zones[i] != (Object)null)
			{
				Transform transform = ((Component)zones[i]).transform;
				transform.localScale *= 0.5f;
				Matrix4x4 val = ((simulationSpace != SimulationSpace.World) ? (((Component)zones[i]).transform.worldToLocalMatrix * ((Component)this).transform.localToWorldMatrix) : ((Component)zones[i]).transform.worldToLocalMatrix);
				Transform transform2 = ((Component)zones[i]).transform;
				transform2.localScale *= 2f;
				Vector3 val2 = Vector3.Min(zones[i].edgeThreshold, Vector3.one * 0.9999f);
				((Vector3)(ref val3))._002Ector(1f / (1f - val2.x), 1f / (1f - val2.y), 1f / (1f - val2.z));
				renderingMaterial.SetMatrix(PAPFHelper._ExclusionMatrix[i], val);
				renderingMaterial.SetVector(PAPFHelper._ExclusionThreshold[i], Vector4.op_Implicit(val2));
				renderingMaterial.SetVector(PAPFHelper._InverseExclusionThreshold[i], Vector4.op_Implicit(val3));
			}
			else
			{
				renderingMaterial.SetMatrix(PAPFHelper._ExclusionMatrix[i], Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 100000f));
				renderingMaterial.SetVector(PAPFHelper._ExclusionThreshold[i], Vector4.op_Implicit(Vector3.zero));
				renderingMaterial.SetVector(PAPFHelper._InverseExclusionThreshold[i], Vector4.op_Implicit(Vector3.one * float.PositiveInfinity));
			}
		}
	}

	private void Start()
	{
		meshIsDirtyMask = MeshFlags.None;
		isOpenGL = SystemInfo.graphicsDeviceVersion.ToLower().Contains("opengl");
		GetRenderingComponents();
		CreateAssetTypes();
		UpdateParticleField();
	}

	private void OnDisable()
	{
		ResetTimers();
	}

	public void ResetTimers()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		time = 0f;
		spinTime = 0f;
		speedTime = Vector3.zero;
		forceTime = Vector3.zero;
		turbulenceOffsetTime = Vector3.zero;
		frameTime = 0f;
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)this).transform.position != position)
		{
			Vector3 val = ((Component)this).transform.InverseTransformDirection(((Component)this).transform.position - position);
			deltaPosition += val;
			position = ((Component)this).transform.position;
			shaderIsDirty = true;
		}
		if (((Component)this).transform.lossyScale != scale)
		{
			scale = ((Component)this).transform.lossyScale;
			shaderIsDirty = true;
		}
		UpdateAnimationValues();
		UpdateExclusionZoneValues();
		if (meshIsDirtyMask != MeshFlags.None)
		{
			UpdateMesh();
		}
		if (materialType != MaterialType.Custom)
		{
			if (shaderIsDirty)
			{
				UpdateShader();
			}
			SetShaderAnimationValues();
			SetShaderExclusionZoneValues();
		}
	}

	private void OnWillRenderObject()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Invalid comparison between Unknown and I4
		if (Object.op_Implicit((Object)(object)renderingMaterial))
		{
			if (materialType == MaterialType.Custom)
			{
				UpdateShader();
				SetShaderExclusionZoneValues();
				SetShaderAnimationValues();
			}
			if (isOpenGL)
			{
				renderingMaterial.SetFloat(PAPFHelper._NearFadeDistance, (!Camera.current.orthographic) ? mNearFadeDistance : 0f);
			}
			bool flag = softParticles == SoftParticleType.NearClipAndCameraDepth;
			flag &= (int)Camera.current.depthTextureMode != 0;
			flag &= !Application.isMobilePlatform;
			SetKeyword("SOFTPARTICLES_ON", flag);
		}
	}

	private void OnDestroy()
	{
		if (Object.op_Implicit((Object)(object)renderingMaterial))
		{
			Object.DestroyImmediate((Object)(object)renderingMaterial);
		}
		if (Object.op_Implicit((Object)(object)particleMesh))
		{
			Object.DestroyImmediate((Object)(object)particleMesh);
		}
	}

	public int GetMaxCount()
	{
		if ((Object)(object)meshGenerator != (Object)null)
		{
			return meshGenerator.GetMaximumParticleCount();
		}
		return 16250;
	}

	public Bounds GetBounds()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)meshRenderer))
		{
			return ((Renderer)meshRenderer).bounds;
		}
		return new Bounds(((Component)this).transform.position, Vector3.Scale(fieldSize, ((Component)this).transform.lossyScale));
	}

	public static PAParticleField Create(string name)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new GameObject(name).AddComponent<PAParticleField>();
	}

	private void CreateTemporarySerializableMaterial()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		temporarySerializableMaterial = new Material((mMaterialType == MaterialType.Custom) ? material.shader : shader);
		SetMaterialKeyword("DIRECTIONAL_ON", stretchedBillboard, temporarySerializableMaterial);
		SetMaterialKeyword("WORLDSPACE_ON", simulationSpace == SimulationSpace.World, temporarySerializableMaterial);
		SetMaterialKeyword("SPIN_ON", spin, temporarySerializableMaterial);
		SetMaterialKeyword("TURBULENCE_SIMPLEX2D", turbulenceType == TurbulenceType.Simplex2D, temporarySerializableMaterial);
		SetMaterialKeyword("TURBULENCE_SIMPLEX", turbulenceType == TurbulenceType.Simplex, temporarySerializableMaterial);
		SetMaterialKeyword("SOFTPARTICLES_ON", softParticles == SoftParticleType.NearClipAndCameraDepth, temporarySerializableMaterial);
		SetMaterialKeyword("SHAPE_SPHERE", shape == Shape.Sphere, temporarySerializableMaterial);
		SetMaterialKeyword("SHAPE_CYLINDER", shape == Shape.Cylinder, temporarySerializableMaterial);
	}
}
