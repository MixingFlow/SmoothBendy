using System;
using System.Diagnostics;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class CH3BridgeBlocker : TMGMonoBehaviour
{
	// Token: 0x1400000E RID: 14
	// (add) Token: 0x060008DF RID: 2271 RVA: 0x0004AD24 File Offset: 0x00048F24
	// (remove) Token: 0x060008E0 RID: 2272 RVA: 0x0004AD5C File Offset: 0x00048F5C
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event EventHandler OnOpen;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x060008E1 RID: 2273 RVA: 0x0004AD94 File Offset: 0x00048F94
	// (remove) Token: 0x060008E2 RID: 2274 RVA: 0x0004ADCC File Offset: 0x00048FCC
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event EventHandler OnClose;

	// Token: 0x060008E3 RID: 2275 RVA: 0x0004AE04 File Offset: 0x00049004
	public override void InitOnComplete()
	{
		base.InitOnComplete();
		this.m_Blocker.SetActive(false);
		this.m_LightController.TurnOff();
		this.EnsureInitialized();
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0004AE64 File Offset: 0x00049064
	public void Close()
	{
		this.EnsureInitialized();
		Sequence sequence = DOTween.Sequence();
		this.m_Blocker.SetActive(true);
		S13AudioManager.Instance.PlayAudio("sfx_bridge_blocker_door_close");
		for (int i = 0; i < this.m_Gates.Length; i++)
		{
			Transform transform = this.m_Gates[i];
			TweenSettingsExtensions.Insert(sequence, 0f, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(transform, this.m_GateDownPosition.z + (float)i, 0.5f, false), 6));
		}
		bool flag = false;
		for (int j = 0; j < this.m_Wheels.Length; j++)
		{
			Transform transform2 = this.m_Wheels[j];
			Vector3 vector;
			vector..ctor(0f, (!flag) ? 360f : -360f, 0f);
			TweenSettingsExtensions.Insert(sequence, 0f, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(transform2, vector, 0.5f, 3), 6));
			flag = true;
		}
		TweenSettingsExtensions.OnComplete<Sequence>(sequence, new TweenCallback(this.CloseOnComplete));
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00009225 File Offset: 0x00007425
	private void CloseOnComplete()
	{
		this.m_LightController.TurnOn();
		this.OnClose.Send(this);
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0004AF68 File Offset: 0x00049168
	public void ForceOpen()
	{
		this.EnsureInitialized();
		for (int i = 0; i < this.m_Gates.Length; i++)
		{
			Transform transform = this.m_Gates[i];
			Vector3 localPosition = transform.localPosition;
			localPosition.z = this.m_GateOriginPosition.z;
			transform.localPosition = localPosition;
		}
		this.m_Blocker.SetActive(false);
		this.m_LightController.TurnOff();
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x0004AFD0 File Offset: 0x000491D0
	public void Open()
	{
		this.EnsureInitialized();
		Sequence sequence = DOTween.Sequence();
		S13AudioManager.Instance.PlayAudio("sfx_bridge_blocker_door_open");
		for (int i = 0; i < this.m_Gates.Length; i++)
		{
			Transform transform = this.m_Gates[i];
			TweenSettingsExtensions.Insert(sequence, 0.5f, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(transform, this.m_GateOriginPosition.z, 0.5f, false), 6));
		}
		bool flag = false;
		for (int j = 0; j < this.m_Wheels.Length; j++)
		{
			Transform transform2 = this.m_Wheels[j];
			Vector3 vector;
			vector..ctor(0f, (!flag) ? -360f : 360f, 0f);
			TweenSettingsExtensions.Insert(sequence, 0.5f, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(transform2, vector, 0.5f, 3), 6));
			flag = true;
		}
		TweenSettingsExtensions.OnComplete<Sequence>(sequence, new TweenCallback(this.OpenOnComplete));
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x0000923E File Offset: 0x0000743E
	private void OpenOnComplete()
	{
		this.m_Blocker.SetActive(false);
		this.m_LightController.TurnOff();
		this.OnOpen.Send(this);
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x000061D6 File Offset: 0x000043D6
	protected override void OnDisposed()
	{
		base.OnDisposed();
	}

	// Token: 0x04000741 RID: 1857
	[SerializeField]
	private GameObject m_Blocker;

	// Token: 0x04000742 RID: 1858
	[SerializeField]
	private LightController m_LightController;

	// Token: 0x04000743 RID: 1859
	[SerializeField]
	private Transform[] m_Gates;

	// Token: 0x04000744 RID: 1860
	[SerializeField]
	private Transform[] m_Wheels;

	// Token: 0x04000745 RID: 1861
	private Vector3 m_GateOriginPosition;

	// Token: 0x04000746 RID: 1862
	private Vector3 m_GateDownPosition;

	private bool m_IsPositionInitialized;

	private void EnsureInitialized()
	{
		if (this.m_IsPositionInitialized)
		{
			return;
		}
		this.m_IsPositionInitialized = true;
		this.m_GateOriginPosition = this.m_Gates[0].localPosition;
		this.m_GateDownPosition = this.m_GateOriginPosition;
		this.m_GateDownPosition.z = this.m_GateDownPosition.z - 7.75f;
	}
}