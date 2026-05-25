using UnityEngine;

public class PoolItem
{
	private ProjectionPool pool;

	private GameObject gameObject;

	private Projection projection;

	private FadeMethod fadeMethod;

	private float delay;

	private float inDuration;

	private float outDuration;

	private CullMethod cullMethod;

	private float cullDuration;

	private float timeElapsed;

	private float timeSinceSeen;

	public ProjectionPool Pool => pool;

	public GameObject GameObject => gameObject;

	public Projection Projection => projection;

	public PoolItem(ProjectionPool Pool)
	{
		pool = Pool;
	}

	public void Reset(ProjectionType Type)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if ((Object)(object)gameObject == (Object)null)
		{
			gameObject = new GameObject("Projection");
		}
		gameObject.transform.SetParent(pool.Parent);
		gameObject.SetActive(true);
		timeElapsed = 0f;
		fadeMethod = FadeMethod.None;
		inDuration = 0f;
		delay = 0f;
		outDuration = 0f;
		cullMethod = CullMethod.None;
		timeSinceSeen = 0f;
		cullDuration = 0f;
		Eraser eraser = gameObject.GetComponent<Eraser>();
		OmniDecal omniDecal = gameObject.GetComponent<OmniDecal>();
		Decal decal = gameObject.GetComponent<Decal>();
		if ((Object)(object)eraser != (Object)null)
		{
			((Behaviour)eraser).enabled = false;
		}
		if ((Object)(object)omniDecal != (Object)null)
		{
			((Behaviour)omniDecal).enabled = false;
		}
		if ((Object)(object)decal != (Object)null)
		{
			((Behaviour)decal).enabled = false;
		}
		switch (Type)
		{
		case ProjectionType.Decal:
			if ((Object)(object)decal == (Object)null)
			{
				decal = gameObject.AddComponent<Decal>();
			}
			projection = decal;
			break;
		case ProjectionType.Eraser:
			if ((Object)(object)eraser == (Object)null)
			{
				eraser = gameObject.AddComponent<Eraser>();
			}
			projection = eraser;
			break;
		case ProjectionType.OmniDecal:
			if ((Object)(object)omniDecal == (Object)null)
			{
				omniDecal = gameObject.AddComponent<OmniDecal>();
			}
			projection = omniDecal;
			break;
		}
		projection.PoolItem = this;
		((Behaviour)projection).enabled = true;
		projection.AlphaModifier = 1f;
		projection.ScaleModifier = 1f;
	}

	public void Update(float deltaTime)
	{
		if (fadeMethod != FadeMethod.None)
		{
			float num = 1f;
			timeElapsed += deltaTime;
			if (timeElapsed < inDuration)
			{
				num = 1f - (inDuration - timeElapsed) / inDuration;
			}
			if (timeElapsed > inDuration + delay)
			{
				num = (inDuration + delay + outDuration - timeElapsed) / outDuration;
			}
			if (fadeMethod == FadeMethod.Alpha || fadeMethod == FadeMethod.Both)
			{
				projection.AlphaModifier = num;
			}
			if (fadeMethod == FadeMethod.Scale || fadeMethod == FadeMethod.Both)
			{
				projection.ScaleModifier = num;
			}
			if (timeElapsed >= inDuration + delay + outDuration)
			{
				pool.Return(this);
			}
		}
		else
		{
			projection.AlphaModifier = 1f;
			projection.ScaleModifier = 1f;
		}
		if (cullMethod != CullMethod.None)
		{
			if (projection.Visible)
			{
				timeSinceSeen = 0f;
			}
			else
			{
				timeSinceSeen += deltaTime;
			}
			if (timeSinceSeen > cullDuration)
			{
				pool.Return(this);
			}
		}
	}

	public void Fade(FadeMethod Method, float InDuration, float Delay, float OutDuration)
	{
		fadeMethod = Method;
		delay = Delay;
		inDuration = InDuration;
		outDuration = OutDuration;
	}

	public void Culled(CullMethod Method, float Duration)
	{
		cullMethod = Method;
		cullDuration = Duration;
	}

	public void Return()
	{
		pool.Return(this);
	}
}
