using System;
using UnityEngine;

namespace InControl;

[Serializable]
public class TouchSprite
{
	[SerializeField]
	private Sprite idleSprite;

	[SerializeField]
	private Sprite busySprite;

	[SerializeField]
	private Color idleColor = new Color(1f, 1f, 1f, 0.5f);

	[SerializeField]
	private Color busyColor = new Color(1f, 1f, 1f, 1f);

	[SerializeField]
	private TouchSpriteShape shape;

	[SerializeField]
	private TouchUnitType sizeUnitType;

	[SerializeField]
	private Vector2 size = new Vector2(10f, 10f);

	[SerializeField]
	private bool lockAspectRatio = true;

	[SerializeField]
	[HideInInspector]
	private Vector2 worldSize;

	private Transform spriteParentTransform;

	private GameObject spriteGameObject;

	private SpriteRenderer spriteRenderer;

	private bool state;

	public bool Dirty { get; set; }

	public bool Ready { get; set; }

	public bool State
	{
		get
		{
			return state;
		}
		set
		{
			if (state != value)
			{
				state = value;
				Dirty = true;
			}
		}
	}

	public Sprite BusySprite
	{
		get
		{
			return busySprite;
		}
		set
		{
			if ((Object)(object)busySprite != (Object)(object)value)
			{
				busySprite = value;
				Dirty = true;
			}
		}
	}

	public Sprite IdleSprite
	{
		get
		{
			return idleSprite;
		}
		set
		{
			if ((Object)(object)idleSprite != (Object)(object)value)
			{
				idleSprite = value;
				Dirty = true;
			}
		}
	}

	public Sprite Sprite
	{
		set
		{
			if ((Object)(object)idleSprite != (Object)(object)value)
			{
				idleSprite = value;
				Dirty = true;
			}
			if ((Object)(object)busySprite != (Object)(object)value)
			{
				busySprite = value;
				Dirty = true;
			}
		}
	}

	public Color BusyColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return busyColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (busyColor != value)
			{
				busyColor = value;
				Dirty = true;
			}
		}
	}

	public Color IdleColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return idleColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (idleColor != value)
			{
				idleColor = value;
				Dirty = true;
			}
		}
	}

	public TouchSpriteShape Shape
	{
		get
		{
			return shape;
		}
		set
		{
			if (shape != value)
			{
				shape = value;
				Dirty = true;
			}
		}
	}

	public TouchUnitType SizeUnitType
	{
		get
		{
			return sizeUnitType;
		}
		set
		{
			if (sizeUnitType != value)
			{
				sizeUnitType = value;
				Dirty = true;
			}
		}
	}

	public Vector2 Size
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return size;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (size != value)
			{
				size = value;
				Dirty = true;
			}
		}
	}

	public Vector2 WorldSize => worldSize;

	public Vector3 Position
	{
		get
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return (!Object.op_Implicit((Object)(object)spriteGameObject)) ? Vector3.zero : spriteGameObject.transform.position;
		}
		set
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)spriteGameObject))
			{
				spriteGameObject.transform.position = value;
			}
		}
	}

	public TouchSprite()
	{
	}//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	//IL_001a: Unknown result type (might be due to invalid IL or missing references)
	//IL_0034: Unknown result type (might be due to invalid IL or missing references)
	//IL_0039: Unknown result type (might be due to invalid IL or missing references)
	//IL_0049: Unknown result type (might be due to invalid IL or missing references)
	//IL_004e: Unknown result type (might be due to invalid IL or missing references)


	public TouchSprite(float size)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		this.size = Vector2.one * size;
	}

	public void Create(string gameObjectName, Transform parentTransform, int sortingOrder)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		spriteGameObject = CreateSpriteGameObject(gameObjectName, parentTransform);
		spriteRenderer = CreateSpriteRenderer(spriteGameObject, idleSprite, sortingOrder);
		spriteRenderer.color = idleColor;
		Ready = true;
	}

	public void Delete()
	{
		Ready = false;
		Object.Destroy((Object)(object)spriteGameObject);
	}

	public void Update()
	{
		Update(forceUpdate: false);
	}

	public void Update(bool forceUpdate)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		if (Dirty || forceUpdate)
		{
			if ((Object)(object)spriteRenderer != (Object)null)
			{
				spriteRenderer.sprite = ((!State) ? idleSprite : busySprite);
			}
			if (sizeUnitType == TouchUnitType.Pixels)
			{
				Vector2 val = TouchUtility.RoundVector(size);
				ScaleSpriteInPixels(spriteGameObject, spriteRenderer, val);
				worldSize = val * TouchManager.PixelToWorld;
			}
			else
			{
				ScaleSpriteInPercent(spriteGameObject, spriteRenderer, size);
				if (lockAspectRatio)
				{
					worldSize = size * TouchManager.PercentToWorld;
				}
				else
				{
					worldSize = Vector2.Scale(size, Vector2.op_Implicit(TouchManager.ViewSize));
				}
			}
			Dirty = false;
		}
		if ((Object)(object)spriteRenderer != (Object)null)
		{
			Color val2 = ((!State) ? idleColor : busyColor);
			if (spriteRenderer.color != val2)
			{
				spriteRenderer.color = Utility.MoveColorTowards(spriteRenderer.color, val2, 5f * Time.deltaTime);
			}
		}
	}

	private GameObject CreateSpriteGameObject(string name, Transform parentTransform)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		val.transform.parent = parentTransform;
		val.transform.localPosition = Vector3.zero;
		val.transform.localScale = Vector3.one;
		val.layer = ((Component)parentTransform).gameObject.layer;
		return val;
	}

	private SpriteRenderer CreateSpriteRenderer(GameObject spriteGameObject, Sprite sprite, int sortingOrder)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		SpriteRenderer val = spriteGameObject.AddComponent<SpriteRenderer>();
		val.sprite = sprite;
		((Renderer)val).sortingOrder = sortingOrder;
		((Renderer)val).sharedMaterial = new Material(Shader.Find("Sprites/Default"));
		((Renderer)val).sharedMaterial.SetFloat("PixelSnap", 1f);
		return val;
	}

	private void ScaleSpriteInPixels(GameObject spriteGameObject, SpriteRenderer spriteRenderer, Vector2 size)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)spriteGameObject == (Object)null) && !((Object)(object)spriteRenderer == (Object)null) && !((Object)(object)spriteRenderer.sprite == (Object)null))
		{
			Rect rect = spriteRenderer.sprite.rect;
			float width = ((Rect)(ref rect)).width;
			Bounds bounds = spriteRenderer.sprite.bounds;
			float num = width / ((Bounds)(ref bounds)).size.x;
			float num2 = TouchManager.PixelToWorld * num;
			float num3 = num2 * size.x;
			Rect rect2 = spriteRenderer.sprite.rect;
			float num4 = num3 / ((Rect)(ref rect2)).width;
			float num5 = num2 * size.y;
			Rect rect3 = spriteRenderer.sprite.rect;
			float num6 = num5 / ((Rect)(ref rect3)).height;
			spriteGameObject.transform.localScale = new Vector3(num4, num6);
		}
	}

	private void ScaleSpriteInPercent(GameObject spriteGameObject, SpriteRenderer spriteRenderer, Vector2 size)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)spriteGameObject == (Object)null) && !((Object)(object)spriteRenderer == (Object)null) && !((Object)(object)spriteRenderer.sprite == (Object)null))
		{
			if (lockAspectRatio)
			{
				float num = Mathf.Min(TouchManager.ViewSize.x, TouchManager.ViewSize.y);
				float num2 = num * size.x;
				Bounds bounds = spriteRenderer.sprite.bounds;
				float num3 = num2 / ((Bounds)(ref bounds)).size.x;
				float num4 = num * size.y;
				Bounds bounds2 = spriteRenderer.sprite.bounds;
				float num5 = num4 / ((Bounds)(ref bounds2)).size.y;
				spriteGameObject.transform.localScale = new Vector3(num3, num5);
			}
			else
			{
				float num6 = TouchManager.ViewSize.x * size.x;
				Bounds bounds3 = spriteRenderer.sprite.bounds;
				float num7 = num6 / ((Bounds)(ref bounds3)).size.x;
				float num8 = TouchManager.ViewSize.y * size.y;
				Bounds bounds4 = spriteRenderer.sprite.bounds;
				float num9 = num8 / ((Bounds)(ref bounds4)).size.y;
				spriteGameObject.transform.localScale = new Vector3(num7, num9);
			}
		}
	}

	public bool Contains(Vector2 testWorldPoint)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (shape == TouchSpriteShape.Oval)
		{
			float num = (testWorldPoint.x - Position.x) / worldSize.x;
			float num2 = (testWorldPoint.y - Position.y) / worldSize.y;
			return num * num + num2 * num2 < 0.25f;
		}
		float num3 = Utility.Abs(testWorldPoint.x - Position.x) * 2f;
		float num4 = Utility.Abs(testWorldPoint.y - Position.y) * 2f;
		return num3 <= worldSize.x && num4 <= worldSize.y;
	}

	public bool Contains(Touch touch)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Contains(Vector2.op_Implicit(TouchManager.ScreenToWorldPoint(touch.position)));
	}

	public void DrawGizmos(Vector3 position, Color color)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (shape == TouchSpriteShape.Oval)
		{
			Utility.DrawOvalGizmo(Vector2.op_Implicit(position), WorldSize, color);
		}
		else
		{
			Utility.DrawRectGizmo(Vector2.op_Implicit(position), WorldSize, color);
		}
	}
}
