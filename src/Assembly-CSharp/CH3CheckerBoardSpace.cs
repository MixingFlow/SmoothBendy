using System.Collections.Generic;
using UnityEngine;

public class CH3CheckerBoardSpace
{
	public List<CH3CheckerBoardSpace> ConnectedSpaces;

	public CH3CheckerPiece CheckerPiece;

	public Vector3 Position;

	public int Row;

	public int Column;

	public bool isJumping;

	public bool isEmpty => (Object)(object)CheckerPiece == (Object)null;
}
