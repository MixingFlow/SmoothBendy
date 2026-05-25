using TMG.Core;
using UnityEngine;

public class EditorScenePlay : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject UIVisuals;

	[SerializeField]
	private GameObject GameInitializer;

	[SerializeField]
	private GameObject s13AudioManager;

	private float m_Timer;

	public override void Init()
	{
		base.Init();
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
	}
}
