using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH5InkMakerController : TMGMonoBehaviour
{
	private enum INK_TYPE
	{
		PIPE_FOUR_WAY,
		PIPE_BASIC,
		PIPE_CORNER,
		PIPE_THREE_WAY,
		COUNT
	}

	[SerializeField]
	private Transform m_MakerTransform;

	[SerializeField]
	private Transform m_Dial;

	[SerializeField]
	private Interactable m_DialFrame;

	[SerializeField]
	private Interactable m_Crank;

	[SerializeField]
	private Interactable m_InkTray;

	[SerializeField]
	private GameObject m_Ink;

	[SerializeField]
	private Transform m_Eject;

	[Header("Ink Maker Objects")]
	[SerializeField]
	private Rigidbody m_InkMakerPipeBasic;

	[SerializeField]
	private Rigidbody m_InkMakerPipeCorner;

	[SerializeField]
	private Rigidbody m_InkMakerPipeThreeWay;

	[SerializeField]
	private Rigidbody m_InkMakerGear;

	[SerializeField]
	private GameObject m_InkMakerBlob;

	private int m_CurrentDialIndex;

	private S13Switch m_AudioSwitch;

	public bool IsPipeBasicCreated { get; private set; }

	public bool IsPipeCornerCreated { get; private set; }

	public bool IsPipeThreeWayCreated { get; private set; }

	public event EventHandler OnTrayInteracted;

	public event EventHandler OnPipeBasicInteracted;

	public event EventHandler OnPipeCornerInteracted;

	public event EventHandler OnPipeThreeWayInteracted;

	public event EventHandler OnComplete;

	public override void Init()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_CurrentDialIndex = 2;
		m_Dial.localEulerAngles = new Vector3(0f, 0f, -180f);
		m_Ink.SetActive(false);
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		AddDialListeners();
		m_AudioSwitch = ((Component)this).GetComponentInChildren<S13Switch>();
	}

	public void Activate()
	{
		Deactivate();
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleBasic.IsComplete)
		{
			IsPipeBasicCreated = true;
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleCorner.IsComplete)
		{
			IsPipeCornerCreated = true;
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleThreeWay.IsComplete)
		{
			IsPipeThreeWayCreated = true;
		}
		AddTrayListeners();
	}

	public void Deactivate()
	{
		RemoveTrayListeners();
		RemoveCrankListeners();
	}

	private void HandleInkTrayOnInteracted(object sender, EventArgs e)
	{
		RemoveTrayListeners();
		this.OnTrayInteracted.Send(this);
		m_AudioSwitch.Play("add_ink");
		m_Ink.SetActive(true);
		AddCrankListeners();
	}

	private void AddTrayListeners()
	{
		m_InkTray.OnInteracted += HandleInkTrayOnInteracted;
		m_InkTray.SetActive(active: true);
	}

	private void RemoveTrayListeners()
	{
		m_InkTray.OnInteracted -= HandleInkTrayOnInteracted;
		m_InkTray.SetActive(active: false);
	}

	private void HandleCrackOnInteracted(object sender, EventArgs e)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		RemoveCrankListeners();
		RemoveDialListeners();
		m_AudioSwitch.Play("making");
		float num = 0f;
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Crank.transform, new Vector3(0f, 0f, 360f), 1.25f, (RotateMode)3), (Ease)28));
		num += 0.25f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_InkTray.transform, new Vector3(0f, 0f, 98f), 0.6f, (RotateMode)3), (Ease)7));
		num += 0.6f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_Ink.SetActive(false);
		});
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(m_MakerTransform, 1.01f, 0.1f), (Ease)7), 30, (LoopType)1));
		num += 0.2f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_InkTray.transform, new Vector3(0f, 0f, -98f), 0.6f, (RotateMode)3), (Ease)30));
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(MakerOnComplete));
	}

	private void AddCrankListeners()
	{
		m_Crank.OnInteracted += HandleCrackOnInteracted;
		m_Crank.SetActive(active: true);
	}

	private void RemoveCrankListeners()
	{
		m_Crank.OnInteracted -= HandleCrackOnInteracted;
		m_Crank.SetActive(active: false);
	}

	private void HandleDialFrameOnInteracted(object sender, EventArgs e)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		RemoveDialListeners();
		m_AudioSwitch.Play("select");
		m_CurrentDialIndex++;
		if (m_CurrentDialIndex >= 4)
		{
			m_CurrentDialIndex = 0;
		}
		ShortcutExtensions.DOKill((Component)(object)m_Dial, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Dial, new Vector3(0f, 0f, -90f), 1f, (RotateMode)3), (Ease)28), new TweenCallback(AddDialListeners));
	}

	private void AddDialListeners()
	{
		m_DialFrame.OnInteracted += HandleDialFrameOnInteracted;
		m_DialFrame.SetActive(active: true);
	}

	private void RemoveDialListeners()
	{
		m_DialFrame.OnInteracted -= HandleDialFrameOnInteracted;
		m_DialFrame.SetActive(active: false);
	}

	private void MakerOnComplete()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Expected O, but got Unknown
		m_MakerTransform.localScale = Vector3.one;
		Rigidbody inkObject = GetInkObjectType();
		((Component)inkObject).transform.position = m_Eject.position;
		((Component)inkObject).transform.localEulerAngles = new Vector3((float)Random.Range(-35, 35), (float)Random.Range(-35, 35), (float)Random.Range(-35, 35));
		inkObject.AddForce(m_Eject.forward * 300f + Vector3.down * 1.5f + m_Eject.right * Random.Range(-10f, 10f));
		MeshRenderer inkRenderer = ((Component)inkObject).GetComponentInChildren<MeshRenderer>();
		((Component)inkRenderer).transform.localScale = Vector3.one * 0.01f;
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(((Component)inkRenderer).transform, 1f, 1f), (Ease)7);
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val, 0.25f, (Tween)(object)ShortcutExtensions.DOFloat(((Renderer)inkRenderer).materials[1], 1f, "_Dissolve", 2.5f));
		if (m_CurrentDialIndex == 3 && !IsPipeBasicCreated)
		{
			IsPipeBasicCreated = true;
		}
		else if (m_CurrentDialIndex == 0 && !IsPipeCornerCreated)
		{
			IsPipeCornerCreated = true;
		}
		else if (m_CurrentDialIndex == 1 && !IsPipeThreeWayCreated)
		{
			IsPipeThreeWayCreated = true;
		}
		else
		{
			TweenSettingsExtensions.InsertCallback(val, 35f, (TweenCallback)delegate
			{
				if (Object.op_Implicit((Object)(object)inkObject))
				{
					ShortcutExtensions.DOFloat(((Renderer)inkRenderer).materials[1], 0f, "_Dissolve", 2f);
				}
			});
			TweenSettingsExtensions.InsertCallback(val, 37f, (TweenCallback)delegate
			{
				if (Object.op_Implicit((Object)(object)inkObject))
				{
					((Component)inkObject).GetComponentInChildren<InkExplosionEffect>().ExplodeOnly();
					((Renderer)((Component)inkRenderer).GetComponent<MeshRenderer>()).enabled = false;
					Object.Destroy((Object)(object)((Component)inkObject).gameObject, 2f);
				}
			});
		}
		Transform inkBlob = Object.Instantiate<GameObject>(m_InkMakerBlob).transform;
		inkBlob.SetParent(((Component)inkObject).transform);
		inkBlob.localPosition = Vector3.zero;
		inkBlob.localEulerAngles = Vector3.zero;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(inkBlob, 0f, 1f), (Ease)7), (TweenCallback)delegate
		{
			((Component)inkBlob).GetComponent<DisposableObject>().Dispose();
		});
		AddDialListeners();
		this.OnComplete.Send(this);
	}

	private Rigidbody GetInkObjectType()
	{
		Rigidbody val = null;
		switch (m_CurrentDialIndex)
		{
		case 0:
			val = Object.Instantiate<Rigidbody>(m_InkMakerPipeCorner);
			if (!IsPipeCornerCreated)
			{
				Interactable component3 = ((Component)val).GetComponent<Interactable>();
				if (Object.op_Implicit((Object)(object)component3))
				{
					component3.OnInteracted += HandlePipeCornerOnInteracted;
					component3.SetActive(active: true);
				}
			}
			break;
		case 1:
			val = Object.Instantiate<Rigidbody>(m_InkMakerPipeThreeWay);
			if (!IsPipeThreeWayCreated)
			{
				Interactable component2 = ((Component)val).GetComponent<Interactable>();
				if (Object.op_Implicit((Object)(object)component2))
				{
					component2.OnInteracted += HandlePipeThreeWayOnInteracted;
					component2.SetActive(active: true);
				}
			}
			break;
		case 2:
			val = Object.Instantiate<Rigidbody>(m_InkMakerGear);
			break;
		case 3:
			val = Object.Instantiate<Rigidbody>(m_InkMakerPipeBasic);
			if (!IsPipeBasicCreated)
			{
				Interactable component = ((Component)val).GetComponent<Interactable>();
				if (Object.op_Implicit((Object)(object)component))
				{
					component.OnInteracted += HandlePipeBasicOnInteracted;
					component.SetActive(active: true);
				}
			}
			break;
		}
		return val;
	}

	private void HandlePipeBasicOnInteracted(object sender, EventArgs e)
	{
		Interactable interactable = sender as Interactable;
		interactable.OnInteracted -= HandlePipeBasicOnInteracted;
		this.OnPipeBasicInteracted.Send(this);
		Object.Destroy((Object)(object)interactable.gameObject);
	}

	private void HandlePipeCornerOnInteracted(object sender, EventArgs e)
	{
		Interactable interactable = sender as Interactable;
		interactable.OnInteracted -= HandlePipeCornerOnInteracted;
		this.OnPipeCornerInteracted.Send(this);
		Object.Destroy((Object)(object)interactable.gameObject);
	}

	private void HandlePipeThreeWayOnInteracted(object sender, EventArgs e)
	{
		Interactable interactable = sender as Interactable;
		interactable.OnInteracted -= HandlePipeThreeWayOnInteracted;
		this.OnPipeThreeWayInteracted.Send(this);
		Object.Destroy((Object)(object)interactable.gameObject);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
