using UnityEngine;

namespace I2.Loc;

public class SimpleButton : MonoBehaviour
{
	public void OnMouseUp()
	{
		((Component)this).gameObject.SendMessage("OnClick", (SendMessageOptions)1);
	}
}
