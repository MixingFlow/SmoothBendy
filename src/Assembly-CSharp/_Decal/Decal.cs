using UnityEngine;

namespace _Decal;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class Decal : MonoBehaviour
{
	public Material material;

	public Sprite sprite;

	public float maxAngle = 90f;

	public float pushDistance = 0.0001f;

	public LayerMask affectedLayers = LayerMask.op_Implicit(-1);

	[SerializeField]
	private Vector3[] vertices;

	[SerializeField]
	private Vector3[] normals;

	[SerializeField]
	private Vector2[] uv;

	[SerializeField]
	private Vector2[] uv2;

	[SerializeField]
	private int[] triangles;

	public Texture texture => (!Object.op_Implicit((Object)(object)material)) ? null : material.mainTexture;

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			((Behaviour)this).enabled = false;
			DeserializeMeshData();
		}
	}

	private void Start()
	{
		((Component)this).transform.hasChanged = false;
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}

	public GameObject[] ForceBuild()
	{
		return (GameObject[])(object)new GameObject[0];
	}

	private void DeserializeMeshData()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		Mesh sharedMesh = ((Component)this).GetComponent<MeshFilter>().sharedMesh;
		if (!((Object)(object)sharedMesh != (Object)null) && vertices != null && vertices.Length != 0)
		{
			sharedMesh = new Mesh();
			sharedMesh.vertices = vertices;
			sharedMesh.normals = normals;
			sharedMesh.uv = uv;
			sharedMesh.uv2 = uv2;
			sharedMesh.triangles = triangles;
			((Component)this).GetComponent<MeshFilter>().sharedMesh = sharedMesh;
		}
	}

	private void SerializeMeshData()
	{
		Mesh sharedMesh = ((Component)this).GetComponent<MeshFilter>().sharedMesh;
		if ((Object)(object)sharedMesh == (Object)null)
		{
			Debug.LogWarning((object)"Unable to serialize Decal mesh data, as there is no mesh.", (Object)(object)this);
			return;
		}
		vertices = sharedMesh.vertices;
		normals = sharedMesh.normals;
		uv = sharedMesh.uv;
		uv2 = sharedMesh.uv2;
		triangles = sharedMesh.triangles;
	}
}
