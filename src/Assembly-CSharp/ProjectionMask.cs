using UnityEngine;

[ExecuteInEditMode]
public class ProjectionMask : MonoBehaviour
{
	[SerializeField]
	private bool[] layers = new bool[4];

	private Renderer source;

	private int subMeshCount;

	private bool? vertExMotion;

	private Material material;

	private bool marked = true;

	public bool Enabled
	{
		get
		{
			if (Object.op_Implicit((Object)(object)source))
			{
				return source.enabled;
			}
			return false;
		}
	}

	public bool Layer1
	{
		get
		{
			return layers[0];
		}
		set
		{
			layers[0] = value;
			Mark();
		}
	}

	public bool Layer2
	{
		get
		{
			return layers[1];
		}
		set
		{
			layers[1] = value;
			Mark();
		}
	}

	public bool Layer3
	{
		get
		{
			return layers[2];
		}
		set
		{
			layers[2] = value;
			Mark();
		}
	}

	public bool Layer4
	{
		get
		{
			return layers[3];
		}
		set
		{
			layers[3] = value;
			Mark();
		}
	}

	public Renderer Renderer
	{
		get
		{
			if (Object.op_Implicit((Object)(object)source))
			{
				return source;
			}
			return null;
		}
	}

	public int SubMeshCount => subMeshCount;

	public Bounds Bounds => (Bounds)((!Object.op_Implicit((Object)(object)source)) ? default(Bounds) : source.bounds);

	public bool VertExMotion
	{
		get
		{
			if (!vertExMotion.HasValue)
			{
				vertExMotion = false;
			}
			return vertExMotion.Value;
		}
	}

	public Material Material
	{
		get
		{
			if (marked || !Object.op_Implicit((Object)(object)material))
			{
				material = DynamicDecals.System.GetMaskMaterial(new MaskValue(layers[0], layers[1], layers[2], layers[3]));
				marked = false;
			}
			return material;
		}
	}

	public void Mark()
	{
		marked = true;
	}

	private void OnEnable()
	{
		Initialize();
		Register();
	}

	private void OnDisable()
	{
		Deregister();
	}

	private void Initialize()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)((Component)this).GetComponent<MeshRenderer>() != (Object)null)
		{
			source = (Renderer)(object)((Component)this).GetComponent<MeshRenderer>();
			MeshFilter component = ((Component)this).GetComponent<MeshFilter>();
			if ((Object)(object)component != (Object)null)
			{
				Mesh sharedMesh = component.sharedMesh;
				if ((Object)(object)sharedMesh != (Object)null)
				{
					subMeshCount = sharedMesh.subMeshCount;
				}
			}
		}
		if ((Object)(object)((Component)this).GetComponent<SkinnedMeshRenderer>() != (Object)null)
		{
			source = (Renderer)(object)((Component)this).GetComponent<SkinnedMeshRenderer>();
			subMeshCount = ((SkinnedMeshRenderer)source).sharedMesh.subMeshCount;
		}
		if ((Object)(object)((Component)this).GetComponent<SpriteRenderer>() != (Object)null)
		{
			source = (Renderer)(object)((Component)this).GetComponent<SpriteRenderer>();
			subMeshCount = 1;
		}
	}

	private void Register()
	{
		DynamicDecals.System.AddMask(this);
	}

	private void Deregister()
	{
		DynamicDecals.System.RemoveMask(this);
	}
}
