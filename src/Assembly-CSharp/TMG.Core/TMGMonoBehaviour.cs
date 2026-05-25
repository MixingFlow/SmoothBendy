using System;
using UnityEngine;

namespace TMG.Core;

public class TMGMonoBehaviour : MonoBehaviour, IDisposable
{
	protected bool m_IsAwake;

	protected bool m_IsStart;

	protected bool m_IsComponent;

	private Transform m_Transform;

	private GameObject m_GameObject;

	public bool IsDisposed { get; private set; }

	public bool IsDestroyed { get; private set; }

	public Transform transform
	{
		get
		{
			if (!Object.op_Implicit((Object)(object)m_Transform))
			{
				m_Transform = ((Component)this).transform;
			}
			return m_Transform;
		}
	}

	public GameObject gameObject
	{
		get
		{
			if (!Object.op_Implicit((Object)(object)m_GameObject))
			{
				m_GameObject = ((Component)this).gameObject;
			}
			return m_GameObject;
		}
	}

	public void Awake()
	{
		if (!m_IsAwake)
		{
			Init();
			m_IsAwake = true;
		}
	}

	public void Start()
	{
		if (!m_IsStart)
		{
			InitOnComplete();
			m_IsStart = true;
		}
	}

	public virtual void Init()
	{
	}

	public virtual void InitOnComplete()
	{
	}

	public virtual void OnEnable()
	{
	}

	public virtual void OnDisable()
	{
	}

	protected virtual void OnDisposed()
	{
	}

	public void SetParentAndAlign(Transform parent)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		transform.SetParent(parent);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
	}

	public void SetParentAndAlignWithScale(Transform parent)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SetParentAndAlign(parent);
		transform.localScale = Vector3.one;
	}

	protected void DebugLog(string message)
	{
	}

	public void Dispose()
	{
		if (IsDisposed)
		{
			return;
		}
		OnDisposed();
		IsDisposed = true;
		GC.SuppressFinalize(this);
		if (!IsDestroyed)
		{
			IsDestroyed = true;
			if (m_IsComponent)
			{
				Object.Destroy((Object)(object)this);
			}
			else
			{
				Object.Destroy((Object)(object)gameObject);
			}
		}
	}

	public void OnDestroy()
	{
		if (!IsDestroyed)
		{
			IsDestroyed = true;
			Dispose();
		}
	}
}
