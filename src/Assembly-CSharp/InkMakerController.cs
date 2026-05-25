using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class InkMakerController : TMGMonoBehaviour
{
	private enum INK_TYPE
	{
		GEAR,
		RADIO,
		BONE,
		CUP,
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
	private Rigidbody m_InkMakerBone;

	[SerializeField]
	private Rigidbody m_InkMakerCup;

	[SerializeField]
	private Rigidbody m_InkMakerRadio;

	[SerializeField]
	private Rigidbody m_InkMakerGear;

	[SerializeField]
	private GameObject m_InkMakerBlob;

	[Header("Weapon Maker")]
	[SerializeField]
	private GameObject m_GentPipeDial;

	[SerializeField]
	private GameObject m_PlungerDial;

	[SerializeField]
	private Rigidbody m_InkMakerPlunger;

	[Header("Options")]
	[SerializeField]
	private bool m_IsAbyssInkMaker;

	[SerializeField]
	private bool m_DevTestEnabled;

	private int m_CurrentDialIndex;

	private S13Switch m_AudioSwitch;

	public bool isGearCreated { get; private set; }

	public bool HasPlunger => m_PlungerDial.activeSelf;

	public event EventHandler OnTrayInteracted;

	public event EventHandler OnGearInteracted;

	public event EventHandler OnComplete;

	public override void Init()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_CurrentDialIndex = 3;
		m_Dial.localEulerAngles = new Vector3(0f, 0f, -270f);
		m_Ink.SetActive(false);
		m_PlungerDial.SetActive(false);
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
		AddTrayListeners();
	}

	public void GivePlunger()
	{
		m_GentPipeDial.SetActive(false);
		m_PlungerDial.SetActive(true);
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
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
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
		if (m_IsAbyssInkMaker && m_CurrentDialIndex == 0 && !isGearCreated)
		{
			isGearCreated = true;
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
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody val = null;
		switch (m_CurrentDialIndex)
		{
		case 0:
			val = Object.Instantiate<Rigidbody>(m_InkMakerGear);
			if (!isGearCreated && m_IsAbyssInkMaker)
			{
				Interactable component = ((Component)val).GetComponent<Interactable>();
				if ((Object)(object)component != (Object)null)
				{
					component.OnInteracted += HandleGearOnInteracted;
					component.SetActive(active: true);
				}
			}
			break;
		case 1:
		{
			if (HasPlunger)
			{
				val = Object.Instantiate<Rigidbody>(m_InkMakerPlunger);
				break;
			}
			val = Object.Instantiate<Rigidbody>(m_InkMakerRadio);
			Rigidbody obj2 = val;
			obj2.centerOfMass += Vector3.down * 0.5f;
			break;
		}
		case 2:
			val = Object.Instantiate<Rigidbody>(m_InkMakerBone);
			break;
		case 3:
		{
			val = Object.Instantiate<Rigidbody>(m_InkMakerCup);
			Rigidbody obj = val;
			obj.centerOfMass += Vector3.down;
			break;
		}
		}
		return val;
	}

	private void HandleGearOnInteracted(object sender, EventArgs e)
	{
		Interactable interactable = sender as Interactable;
		interactable.OnInteracted -= HandleGearOnInteracted;
		this.OnGearInteracted.Send(this);
		Object.Destroy((Object)(object)interactable.gameObject);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
