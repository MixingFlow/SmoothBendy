using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH3BridgeBlocker : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject m_Blocker;

	[SerializeField]
	private LightController m_LightController;

	[SerializeField]
	private Transform[] m_Gates;

	[SerializeField]
	private Transform[] m_Wheels;

	private Vector3 m_GateOriginPosition;

	private Vector3 m_GateDownPosition;

	public event EventHandler OnOpen;

	public event EventHandler OnClose;

	public override void InitOnComplete()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_Blocker.SetActive(false);
		m_LightController.TurnOff();
		m_GateOriginPosition = m_Gates[0].localPosition;
		m_GateDownPosition = m_GateOriginPosition;
		ref Vector3 gateDownPosition = ref m_GateDownPosition;
		gateDownPosition.z -= 7.75f;
	}

	public void Close()
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Sequence val = DOTween.Sequence();
		m_Blocker.SetActive(true);
		S13AudioManager.Instance.PlayAudio("sfx_bridge_blocker_door_close");
		for (int i = 0; i < m_Gates.Length; i++)
		{
			Transform val2 = m_Gates[i];
			TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(val2, m_GateDownPosition.z + (float)i, 0.5f, false), (Ease)6));
		}
		bool flag = false;
		Vector3 val4 = default(Vector3);
		for (int j = 0; j < m_Wheels.Length; j++)
		{
			Transform val3 = m_Wheels[j];
			((Vector3)(ref val4))._002Ector(0f, (!flag) ? 360f : (-360f), 0f);
			TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(val3, val4, 0.5f, (RotateMode)3), (Ease)6));
			flag = true;
		}
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(CloseOnComplete));
	}

	private void CloseOnComplete()
	{
		m_LightController.TurnOn();
		this.OnClose.Send(this);
	}

	public void ForceOpen()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Gates.Length; i++)
		{
			Transform val = m_Gates[i];
			Vector3 localPosition = val.localPosition;
			localPosition.z = m_GateOriginPosition.z;
			val.localPosition = localPosition;
		}
		m_Blocker.SetActive(false);
		m_LightController.TurnOff();
	}

	public void Open()
	{
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		Sequence val = DOTween.Sequence();
		S13AudioManager.Instance.PlayAudio("sfx_bridge_blocker_door_open");
		for (int i = 0; i < m_Gates.Length; i++)
		{
			Transform val2 = m_Gates[i];
			TweenSettingsExtensions.Insert(val, 0.5f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(val2, m_GateOriginPosition.z, 0.5f, false), (Ease)6));
		}
		bool flag = false;
		Vector3 val4 = default(Vector3);
		for (int j = 0; j < m_Wheels.Length; j++)
		{
			Transform val3 = m_Wheels[j];
			((Vector3)(ref val4))._002Ector(0f, (!flag) ? (-360f) : 360f, 0f);
			TweenSettingsExtensions.Insert(val, 0.5f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(val3, val4, 0.5f, (RotateMode)3), (Ease)6));
			flag = true;
		}
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(OpenOnComplete));
	}

	private void OpenOnComplete()
	{
		m_Blocker.SetActive(false);
		m_LightController.TurnOff();
		this.OnOpen.Send(this);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
