using System.Collections;
using I2.Loc;
using TMPro;
using UnityEngine;

public class VerticalTextLayout : MonoBehaviour
{
	public TextMeshPro textMeshPro;

	private void Start()
	{
		if (!Object.op_Implicit((Object)(object)textMeshPro))
		{
			textMeshPro = ((Component)this).GetComponent<TextMeshPro>();
		}
	}

	public void DoLayout(Localize loc)
	{
		((MonoBehaviour)this).StartCoroutine(LayoutVertically());
	}

	private IEnumerator LayoutVertically()
	{
		yield return null;
		string output = string.Empty;
		for (int i = 0; i < textMeshPro.text.Length; i++)
		{
			output = output + textMeshPro.text[i] + "\n";
		}
		textMeshPro.text = output;
	}
}
