using UnityEngine;

public class InteractableGeneric : Interactable
{
	[SerializeField]
	private GameObject[] m_Objects;

	public bool isOn { get; private set; }

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if (m_Objects.Length > 0)
		{
			isOn = m_Objects[0].activeSelf;
		}
	}

	public override void OnInteract()
	{
		for (int i = 0; i < m_Objects.Length; i++)
		{
			GameObject val = m_Objects[i];
			isOn = !val.activeSelf;
			val.SetActive(isOn);
		}
	}
}
