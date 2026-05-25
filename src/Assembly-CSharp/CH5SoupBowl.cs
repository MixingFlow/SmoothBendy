using System.Collections;
using TMG.Core;
using UnityEngine;

public class CH5SoupBowl : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject m_Full;

	[SerializeField]
	private GameObject m_Empty;

	private Rigidbody m_Rigidbody;

	public override void Init()
	{
		base.Init();
		m_Full.SetActive(true);
		m_Empty.SetActive(false);
		m_Rigidbody = ((Component)this).GetComponent<Rigidbody>();
	}

	public void AddForce()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = (-base.transform.forward + -base.transform.right) * 10f;
		m_Rigidbody.AddForce(val, (ForceMode)1);
	}

	private IEnumerator EmptyBowl()
	{
		yield return (object)new WaitForSeconds(1f);
		while (GameManager.Instance.isPaused)
		{
			yield return null;
		}
		if (!base.IsDisposed)
		{
			m_Full.SetActive(false);
			m_Empty.SetActive(true);
		}
	}

	protected override void OnDisposed()
	{
		m_Rigidbody = null;
		base.OnDisposed();
	}
}
