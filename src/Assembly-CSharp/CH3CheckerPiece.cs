using TMG.Core;
using UnityEngine;

public class CH3CheckerPiece : TMGMonoBehaviour
{
	public CH3CheckerBoardSpace CheckerBoardSpace;

	public CheckerPieceColor PieceColor;

	private GameObject m_CheckerPiece;

	private GameObject m_PromotedCheckerPiece;

	public bool isPromoted { get; private set; }

	public bool isCaptured { get; private set; }

	public void Initialize(CH3CheckerBoardSpace checkerBoardSpace, Transform parent, bool _isWhite)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		CheckerBoardSpace = checkerBoardSpace;
		PieceColor = ((!_isWhite) ? CheckerPieceColor.BLACK : CheckerPieceColor.WHITE);
		base.transform.SetParent(parent);
		base.transform.localPosition = CheckerBoardSpace.Position;
		base.transform.localEulerAngles = Vector3.zero;
		m_CheckerPiece = GameObject.CreatePrimitive((PrimitiveType)3);
		m_CheckerPiece.transform.SetParent(base.transform);
		m_CheckerPiece.transform.localPosition = Vector3.zero;
		m_CheckerPiece.transform.localEulerAngles = Vector3.zero;
		m_CheckerPiece.transform.localScale = new Vector3(0.2f, 0.04f, 0.2f);
		((Renderer)m_CheckerPiece.GetComponent<MeshRenderer>()).material.color = ((!_isWhite) ? Color.black : Color.yellow);
	}

	public void Capture(float delay)
	{
		isCaptured = true;
		CheckerBoardSpace.CheckerPiece = null;
		Object.Destroy((Object)(object)m_CheckerPiece, delay);
	}

	public void Promote()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!isPromoted)
		{
			isPromoted = true;
			Transform obj = m_CheckerPiece.transform;
			obj.localScale += new Vector3(0f, 0.05f, 0f);
			((Renderer)m_CheckerPiece.GetComponent<MeshRenderer>()).material.color = ((PieceColor != CheckerPieceColor.WHITE) ? Color.red : Color.green);
		}
	}

	public void Reset(CH3CheckerBoardSpace originCheckerBoardSpace)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		CheckerBoardSpace = originCheckerBoardSpace;
		base.transform.localPosition = CheckerBoardSpace.Position;
		m_CheckerPiece.SetActive(true);
		isCaptured = false;
		isPromoted = false;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
