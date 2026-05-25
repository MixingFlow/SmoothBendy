using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CH3CheckersController : BaseController
{
	private const int BOARD_ARRAY = 8;

	private const int WHITE_MOVE_DIRECTION = 1;

	private const int BLACK_MOVE_DICECTION = -1;

	private const float BOARD_START_POSITION = 1.05f;

	private const float BOARD_POSITION_PADDING = 0.3f;

	private const float PIECE_MOVE_SPEED = 0.25f;

	private const float PIECE_MOVE_SPEED_HALF = 0.125f;

	[SerializeField]
	private Transform m_BoardSpaceParent;

	private Dictionary<int, List<CH3CheckerBoardSpace>> m_BoardSpaces = new Dictionary<int, List<CH3CheckerBoardSpace>>();

	private List<CH3CheckerPiece> m_WhitePieces = new List<CH3CheckerPiece>();

	private List<CH3CheckerPiece> m_BlackPieces = new List<CH3CheckerPiece>();

	private bool m_IsPlaying;

	private bool m_IsWhiteTurn = true;

	private int m_ActivePieceIndex;

	private int _capture;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		InitBoard();
	}

	private void Update()
	{
		if (!m_IsPlaying && Input.GetKeyDown((KeyCode)112))
		{
			m_IsPlaying = true;
			Play();
		}
	}

	private void InitBoard()
	{
		List<CH3CheckerBoardSpace> newBoardSpaces = GetNewBoardSpaces();
		bool flag = false;
		int num = 0;
		for (int i = 0; i < newBoardSpaces.Count; i++)
		{
			CH3CheckerBoardSpace cH3CheckerBoardSpace = newBoardSpaces[i];
			cH3CheckerBoardSpace.Column = num;
			if (flag)
			{
				if (cH3CheckerBoardSpace.Row < 3 || cH3CheckerBoardSpace.Row > 4)
				{
					bool flag2 = cH3CheckerBoardSpace.Row < 3;
					cH3CheckerBoardSpace.CheckerPiece = CreateCheckerPiece(cH3CheckerBoardSpace, flag2);
					((!flag2) ? m_BlackPieces : m_WhitePieces).Add(cH3CheckerBoardSpace.CheckerPiece);
				}
				if (!m_BoardSpaces.ContainsKey(cH3CheckerBoardSpace.Row))
				{
					m_BoardSpaces.Add(cH3CheckerBoardSpace.Row, new List<CH3CheckerBoardSpace>());
				}
				m_BoardSpaces[cH3CheckerBoardSpace.Row].Add(cH3CheckerBoardSpace);
			}
			flag = !flag;
			num++;
			if (num >= 8)
			{
				num = 0;
				flag = !flag;
			}
		}
		GenerateConnectedSpaces();
	}

	private CH3CheckerBoardSpace CreateCheckerBoardSpace(Vector3 position, int row, int column)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		CH3CheckerBoardSpace cH3CheckerBoardSpace = new CH3CheckerBoardSpace();
		cH3CheckerBoardSpace.Position = position;
		cH3CheckerBoardSpace.Row = row;
		cH3CheckerBoardSpace.Column = column;
		return cH3CheckerBoardSpace;
	}

	private CH3CheckerPiece CreateCheckerPiece(CH3CheckerBoardSpace boardSpace, bool isWhite)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		CH3CheckerPiece cH3CheckerPiece = new GameObject("CheckerPiece_" + ((!isWhite) ? "Black" : "White")).AddComponent<CH3CheckerPiece>();
		cH3CheckerPiece.Initialize(boardSpace, m_BoardSpaceParent, isWhite);
		return cH3CheckerPiece;
	}

	private List<CH3CheckerBoardSpace> GetNewBoardSpaces()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		List<CH3CheckerBoardSpace> list = new List<CH3CheckerBoardSpace>();
		Vector3 position = default(Vector3);
		((Vector3)(ref position))._002Ector(1.05f, 0f, -1.05f);
		for (int i = 0; i < 8; i++)
		{
			position.x = 1.05f - (float)i * 0.3f;
			position.z = -1.05f;
			for (int j = 0; j < 8; j++)
			{
				list.Add(CreateCheckerBoardSpace(position, i, j));
				position.z += 0.3f;
			}
		}
		return list;
	}

	private void GenerateConnectedSpaces()
	{
		foreach (int key in m_BoardSpaces.Keys)
		{
			for (int i = 0; i < m_BoardSpaces[key].Count; i++)
			{
				int num = key - 1;
				int num2 = key + 1;
				int num3 = m_BoardSpaces[key][i].Column - 1;
				int num4 = m_BoardSpaces[key][i].Column + 1;
				List<CH3CheckerBoardSpace> list = new List<CH3CheckerBoardSpace>();
				if (num >= 0)
				{
					if (num3 >= 0)
					{
						foreach (CH3CheckerBoardSpace item in m_BoardSpaces[num])
						{
							if (item.Column == num3)
							{
								list.Add(item);
							}
						}
					}
					if (num4 < 8)
					{
						foreach (CH3CheckerBoardSpace item2 in m_BoardSpaces[num])
						{
							if (item2.Column == num4)
							{
								list.Add(item2);
							}
						}
					}
				}
				if (num2 < 8)
				{
					if (num3 >= 0)
					{
						foreach (CH3CheckerBoardSpace item3 in m_BoardSpaces[num2])
						{
							if (item3.Column == num3)
							{
								list.Add(item3);
							}
						}
					}
					if (num4 < 8)
					{
						foreach (CH3CheckerBoardSpace item4 in m_BoardSpaces[num2])
						{
							if (item4.Column == num4)
							{
								list.Add(item4);
							}
						}
					}
				}
				m_BoardSpaces[key][i].ConnectedSpaces = list;
			}
		}
	}

	private void Play()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		m_IsWhiteTurn = !m_IsWhiteTurn;
		TweenSettingsExtensions.OnComplete<Sequence>(PlayNextMove(), new TweenCallback(TurnOnComplete));
	}

	private void TurnOnComplete()
	{
		_capture = 0;
		Play();
	}

	private Dictionary<bool, List<List<CH3CheckerBoardSpace>>> GetPotentialMoves()
	{
		Dictionary<bool, List<List<CH3CheckerBoardSpace>>> dictionary = new Dictionary<bool, List<List<CH3CheckerBoardSpace>>>();
		((!m_IsWhiteTurn) ? m_BlackPieces : m_WhitePieces).Shuffle();
		for (int i = 0; i < ((!m_IsWhiteTurn) ? m_BlackPieces : m_WhitePieces).Count; i++)
		{
			CH3CheckerPiece cH3CheckerPiece = ((!m_IsWhiteTurn) ? m_BlackPieces : m_WhitePieces)[i];
			if (cH3CheckerPiece.isCaptured)
			{
				continue;
			}
			List<List<CH3CheckerBoardSpace>> list = new List<List<CH3CheckerBoardSpace>>();
			list = GetPotentialBoardSpaces(cH3CheckerPiece.CheckerBoardSpace);
			if (list.Count <= 0)
			{
				continue;
			}
			for (int j = 0; j < list.Count; j++)
			{
				list[j].Insert(0, cH3CheckerPiece.CheckerBoardSpace);
			}
			List<List<CH3CheckerBoardSpace>> list2 = new List<List<CH3CheckerBoardSpace>>();
			List<List<CH3CheckerBoardSpace>> list3 = new List<List<CH3CheckerBoardSpace>>();
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k][1].isJumping)
				{
					list3.Add(list[k]);
				}
				else
				{
					list2.Add(list[k]);
				}
			}
			if (list2.Count > 0)
			{
				if (!dictionary.ContainsKey(key: false))
				{
					dictionary.Add(key: false, new List<List<CH3CheckerBoardSpace>>());
				}
				foreach (List<CH3CheckerBoardSpace> item in list2)
				{
					dictionary[false].Add(item);
				}
			}
			if (list3.Count <= 0)
			{
				continue;
			}
			if (!dictionary.ContainsKey(key: true))
			{
				dictionary.Add(key: true, new List<List<CH3CheckerBoardSpace>>());
			}
			foreach (List<CH3CheckerBoardSpace> item2 in list3)
			{
				dictionary[true].Add(item2);
			}
		}
		return dictionary;
	}

	private List<List<CH3CheckerBoardSpace>> GetPotentialBoardSpaces(CH3CheckerBoardSpace startingBoardSpace)
	{
		List<List<CH3CheckerBoardSpace>> list = new List<List<CH3CheckerBoardSpace>>();
		int num = startingBoardSpace.Row + ((startingBoardSpace.CheckerPiece.PieceColor == CheckerPieceColor.WHITE) ? 1 : (-1));
		if (!startingBoardSpace.CheckerPiece.isPromoted && (!m_BoardSpaces.ContainsKey(num) || num < 0 || num >= 8))
		{
			return list;
		}
		if (startingBoardSpace.ConnectedSpaces.Count <= 0)
		{
			return list;
		}
		for (int i = 0; i < startingBoardSpace.ConnectedSpaces.Count; i++)
		{
			if (startingBoardSpace.CheckerPiece.isPromoted)
			{
				if (!startingBoardSpace.ConnectedSpaces[i].isEmpty)
				{
					if (startingBoardSpace.ConnectedSpaces[i].CheckerPiece.PieceColor == startingBoardSpace.CheckerPiece.PieceColor)
					{
						continue;
					}
					int row = startingBoardSpace.ConnectedSpaces[i].Row + ((startingBoardSpace.ConnectedSpaces[i].Row > startingBoardSpace.Row) ? 1 : (-1));
					int column = startingBoardSpace.ConnectedSpaces[i].Column + ((startingBoardSpace.ConnectedSpaces[i].Column > startingBoardSpace.Column) ? 1 : (-1));
					List<CH3CheckerBoardSpace> list2 = new List<CH3CheckerBoardSpace>();
					CH3CheckerBoardSpace cH3CheckerBoardSpace = CheckJumpBoardSpace(startingBoardSpace.ConnectedSpaces[i], column, row);
					if (cH3CheckerBoardSpace == null)
					{
						continue;
					}
					cH3CheckerBoardSpace.isJumping = true;
					list2.Add(cH3CheckerBoardSpace);
					bool flag = true;
					while (flag)
					{
						int num2 = 0;
						foreach (CH3CheckerBoardSpace connectedSpace in cH3CheckerBoardSpace.ConnectedSpaces)
						{
							if (!connectedSpace.isEmpty && connectedSpace.CheckerPiece.PieceColor != startingBoardSpace.CheckerPiece.PieceColor)
							{
								row = connectedSpace.Row + ((connectedSpace.Row > cH3CheckerBoardSpace.Row) ? 1 : (-1));
								column = connectedSpace.Column + ((connectedSpace.Column > cH3CheckerBoardSpace.Column) ? 1 : (-1));
								CH3CheckerBoardSpace cH3CheckerBoardSpace2 = CheckJumpBoardSpace(connectedSpace, column, row);
								if (cH3CheckerBoardSpace2 != null)
								{
									cH3CheckerBoardSpace2.isJumping = true;
									list2.Add(cH3CheckerBoardSpace2);
									cH3CheckerBoardSpace = cH3CheckerBoardSpace2;
									break;
								}
								flag = false;
							}
							else
							{
								num2++;
								if (num2 >= cH3CheckerBoardSpace.ConnectedSpaces.Count)
								{
									flag = false;
								}
							}
						}
					}
					list.Add(list2);
				}
				else
				{
					List<CH3CheckerBoardSpace> list3 = new List<CH3CheckerBoardSpace>();
					list3.Add(startingBoardSpace.ConnectedSpaces[i]);
					list.Add(list3);
				}
			}
			else
			{
				if (startingBoardSpace.ConnectedSpaces[i].Row != num)
				{
					continue;
				}
				if (!startingBoardSpace.ConnectedSpaces[i].isEmpty)
				{
					if (startingBoardSpace.ConnectedSpaces[i].CheckerPiece.PieceColor != startingBoardSpace.CheckerPiece.PieceColor)
					{
						int row2 = startingBoardSpace.ConnectedSpaces[i].Row + ((startingBoardSpace.ConnectedSpaces[i].Row > startingBoardSpace.Row) ? 1 : (-1));
						int column2 = startingBoardSpace.ConnectedSpaces[i].Column + ((startingBoardSpace.ConnectedSpaces[i].Column > startingBoardSpace.Column) ? 1 : (-1));
						List<CH3CheckerBoardSpace> list4 = new List<CH3CheckerBoardSpace>();
						CH3CheckerBoardSpace cH3CheckerBoardSpace3 = CheckJumpBoardSpace(startingBoardSpace.ConnectedSpaces[i], column2, row2);
						if (cH3CheckerBoardSpace3 != null)
						{
							cH3CheckerBoardSpace3.isJumping = true;
							list4.Add(cH3CheckerBoardSpace3);
							list.Add(list4);
						}
					}
				}
				else
				{
					List<CH3CheckerBoardSpace> list5 = new List<CH3CheckerBoardSpace>();
					list5.Add(startingBoardSpace.ConnectedSpaces[i]);
					list.Add(list5);
				}
			}
		}
		list.Shuffle();
		return list;
	}

	private CH3CheckerBoardSpace CheckJumpBoardSpace(CH3CheckerBoardSpace jumpedBoardSpace, int column, int row)
	{
		CH3CheckerBoardSpace result = null;
		for (int i = 0; i < jumpedBoardSpace.ConnectedSpaces.Count; i++)
		{
			if (jumpedBoardSpace.ConnectedSpaces[i].Column == column && jumpedBoardSpace.ConnectedSpaces[i].Row == row && jumpedBoardSpace.ConnectedSpaces[i].isEmpty)
			{
				result = jumpedBoardSpace.ConnectedSpaces[i];
				_capture++;
				jumpedBoardSpace.CheckerPiece.Capture((float)_capture * 0.25f);
				break;
			}
		}
		return result;
	}

	private Sequence PlayNextMove()
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		Dictionary<bool, List<List<CH3CheckerBoardSpace>>> potentialMoves = GetPotentialMoves();
		if (potentialMoves.Count <= 0)
		{
			Debug.Log((object)"Zero potential moves, something wrong!");
		}
		List<CH3CheckerBoardSpace> moves = new List<CH3CheckerBoardSpace>();
		bool key = potentialMoves.ContainsKey(key: true);
		int index = Random.Range(0, potentialMoves[key].Count - 1);
		moves = potentialMoves[key][index];
		float num = 0f;
		if (moves.Count <= 0)
		{
			Debug.Log((object)"No Available Moves...");
			return val;
		}
		CH3CheckerPiece piece = moves[0].CheckerPiece;
		for (int i = 1; i < moves.Count; i++)
		{
			Vector3 position = moves[i].Position;
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveX(piece.transform, position.x, 0.25f, false), (Ease)7));
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(piece.transform, position.z, 0.25f, false), (Ease)7));
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(piece.transform, position.y + 0.2f, 0.125f, false), (Ease)5));
			TweenSettingsExtensions.Insert(val, num + 0.125f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(piece.transform, position.y, 0.125f, false), (Ease)6));
			num += 0.25f;
		}
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			for (int j = 0; j < moves.Count; j++)
			{
				moves[j].isJumping = false;
				if (j < moves.Count - 1)
				{
					moves[j].CheckerPiece = null;
				}
				else
				{
					foreach (CH3CheckerPiece item in (!m_IsWhiteTurn) ? m_BlackPieces : m_WhitePieces)
					{
						if (((object)item).Equals((object)piece))
						{
							if (moves[j].Row == 0 || moves[j].Row == 7)
							{
								item.Promote();
							}
							item.CheckerBoardSpace = moves[j];
							moves[j].CheckerPiece = item;
							break;
						}
					}
				}
			}
		});
		return val;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
