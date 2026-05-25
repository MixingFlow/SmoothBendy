using TMG.Core;
using UnityEngine;

public class SeasonalController : TMGMonoBehaviour
{
	public GameObject Chapter1;

	public GameObject Chapter2;

	public GameObject Chapter3;

	public GameObject Chapter4;

	public GameObject Chapter5;

	private static SeasonalController m_Instance;

	public static SeasonalController Instance => m_Instance;

	public void Initialize()
	{
		m_Instance = this;
		Object.DontDestroyOnLoad((Object)(object)base.gameObject);
	}

	public void Activate(Chapters chapter)
	{
		if (chapter == Chapters.ONE && (Object)(object)Chapter1 != (Object)null)
		{
			InstantiateChapter(ref Chapter1);
		}
		else if (chapter == Chapters.TWO && (Object)(object)Chapter2 != (Object)null)
		{
			InstantiateChapter(ref Chapter2);
		}
		else if (chapter == Chapters.THREE && (Object)(object)Chapter3 != (Object)null)
		{
			InstantiateChapter(ref Chapter3);
		}
		else if (chapter == Chapters.FOUR && (Object)(object)Chapter4 != (Object)null)
		{
			InstantiateChapter(ref Chapter4);
		}
		else if (chapter == Chapters.FIVE && (Object)(object)Chapter5 != (Object)null)
		{
			InstantiateChapter(ref Chapter5);
		}
	}

	private void InstantiateChapter(ref GameObject chapterREF)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Object.Instantiate<GameObject>(chapterREF);
		val.transform.SetParent(GameManager.Instance.CurrentChapter.transform);
		val.transform.position = Vector3.zero;
		val.transform.rotation = Quaternion.identity;
		val.transform.localScale = Vector3.one;
	}
}
