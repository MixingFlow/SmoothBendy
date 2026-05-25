using System.Collections;
using UnityEngine;

namespace TMPro.Examples;

public class TMP_WarpText : MonoBehaviour
{
	private TMP_Text m_TextComponent;

	public AnimationCurve VertexCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[5]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.25f, 2f),
		new Keyframe(0.5f, 0f),
		new Keyframe(0.75f, 2f),
		new Keyframe(1f, 0f)
	});

	public float AngleMultiplier = 1f;

	public float SpeedMultiplier = 1f;

	public float CurveScale = 1f;

	private void Awake()
	{
		m_TextComponent = ((Component)this).gameObject.GetComponent<TMP_Text>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		((MonoBehaviour)this).StartCoroutine(WarpText());
	}

	private AnimationCurve CopyAnimationCurve(AnimationCurve curve)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		AnimationCurve val = new AnimationCurve();
		val.keys = curve.keys;
		return val;
	}

	private IEnumerator WarpText()
	{
		VertexCurve.preWrapMode = (WrapMode)1;
		VertexCurve.postWrapMode = (WrapMode)1;
		m_TextComponent.havePropertiesChanged = true;
		float old_CurveScale = CurveScale;
		AnimationCurve old_curve = CopyAnimationCurve(VertexCurve);
		bool isComplete = false;
		Vector3 val2 = default(Vector3);
		while (!isComplete)
		{
			if (!m_TextComponent.havePropertiesChanged && old_CurveScale == CurveScale && ((Keyframe)(ref old_curve.keys[1])).value == ((Keyframe)(ref VertexCurve.keys[1])).value)
			{
				yield return null;
				continue;
			}
			old_CurveScale = CurveScale;
			old_curve = CopyAnimationCurve(VertexCurve);
			m_TextComponent.ForceMeshUpdate();
			TMP_TextInfo textInfo = m_TextComponent.textInfo;
			int characterCount = textInfo.characterCount;
			if (characterCount == 0)
			{
				continue;
			}
			Bounds bounds = m_TextComponent.bounds;
			float boundsMinX = ((Bounds)(ref bounds)).min.x;
			Bounds bounds2 = m_TextComponent.bounds;
			float boundsMaxX = ((Bounds)(ref bounds2)).max.x;
			for (int i = 0; i < characterCount; i++)
			{
				if (textInfo.characterInfo[i].isVisible)
				{
					int vertexIndex = textInfo.characterInfo[i].vertexIndex;
					int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
					Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
					Vector3 val = Vector2.op_Implicit(new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, textInfo.characterInfo[i].baseLine));
					ref Vector3 reference = ref vertices[vertexIndex];
					reference += -val;
					ref Vector3 reference2 = ref vertices[vertexIndex + 1];
					reference2 += -val;
					ref Vector3 reference3 = ref vertices[vertexIndex + 2];
					reference3 += -val;
					ref Vector3 reference4 = ref vertices[vertexIndex + 3];
					reference4 += -val;
					float num = (val.x - boundsMinX) / (boundsMaxX - boundsMinX);
					float num2 = num + 0.0001f;
					float num3 = VertexCurve.Evaluate(num) * CurveScale;
					float num4 = VertexCurve.Evaluate(num2) * CurveScale;
					((Vector3)(ref val2))._002Ector(1f, 0f, 0f);
					Vector3 val3 = new Vector3(num2 * (boundsMaxX - boundsMinX) + boundsMinX, num4) - new Vector3(val.x, num3);
					float num5 = Mathf.Acos(Vector3.Dot(val2, ((Vector3)(ref val3)).normalized)) * 57.29578f;
					float num6 = ((!(Vector3.Cross(val2, val3).z > 0f)) ? (360f - num5) : num5);
					Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0f, num3, 0f), Quaternion.Euler(0f, 0f, num6), Vector3.one);
					ref Vector3 reference5 = ref vertices[vertexIndex];
					reference5 = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(vertices[vertexIndex]);
					ref Vector3 reference6 = ref vertices[vertexIndex + 1];
					reference6 = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(vertices[vertexIndex + 1]);
					ref Vector3 reference7 = ref vertices[vertexIndex + 2];
					reference7 = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(vertices[vertexIndex + 2]);
					ref Vector3 reference8 = ref vertices[vertexIndex + 3];
					reference8 = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(vertices[vertexIndex + 3]);
					ref Vector3 reference9 = ref vertices[vertexIndex];
					reference9 += val;
					ref Vector3 reference10 = ref vertices[vertexIndex + 1];
					reference10 += val;
					ref Vector3 reference11 = ref vertices[vertexIndex + 2];
					reference11 += val;
					ref Vector3 reference12 = ref vertices[vertexIndex + 3];
					reference12 += val;
				}
			}
			m_TextComponent.UpdateVertexData();
			isComplete = true;
			yield return (object)new WaitForSeconds(0.025f);
		}
	}
}
