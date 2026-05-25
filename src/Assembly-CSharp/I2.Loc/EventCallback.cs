using System;
using UnityEngine;

namespace I2.Loc;

[Serializable]
public class EventCallback
{
	public MonoBehaviour Target;

	public string MethodName = string.Empty;

	public void Execute(Object Sender = null)
	{
		if (HasCallback() && Application.isPlaying)
		{
			((Component)Target).gameObject.SendMessage(MethodName, (object)Sender, (SendMessageOptions)1);
		}
	}

	public bool HasCallback()
	{
		return (Object)(object)Target != (Object)null && !string.IsNullOrEmpty(MethodName);
	}
}
