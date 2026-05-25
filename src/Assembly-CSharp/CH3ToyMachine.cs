using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH3ToyMachine : TMGMonoBehaviour
{
	[Serializable]
	public class Spinners
	{
		public Transform LargeWheel;

		public Transform SmallWheel;

		public List<Interactable> Toys;

		[HideInInspector]
		public bool isSpinning;
	}

	[Header("Conveyer Content")]
	[SerializeField]
	private Transform m_ConveyerContent;

	[SerializeField]
	private Interactable m_ConveyerSwitch;

	[SerializeField]
	private Transform m_Lever;

	[SerializeField]
	private Transform m_DollConveyerContent;

	[Header("Machines")]
	[SerializeField]
	private List<Transform> m_Machines;

	[SerializeField]
	private MeshRenderer m_Cables;

	[Header("Spinners")]
	[SerializeField]
	private List<Spinners> m_SpinnersLeft;

	[SerializeField]
	private List<Spinners> m_SpinnersRight;

	[Header("Bottom Wheels")]
	[SerializeField]
	private Transform m_WheelLeft;

	[SerializeField]
	private Transform m_WheelRight;

	[Header("Light")]
	[SerializeField]
	private GameObject m_Light;

	private List<Transform> m_ConveyerContents = new List<Transform>();

	private List<Vector3> m_ConveyerPositions = new List<Vector3>();

	private List<Transform> m_DollConveyerContents = new List<Transform>();

	private List<Vector3> m_DollConveyerPositions = new List<Vector3>();

	private bool m_IsLeftSolved;

	private bool m_IsRightSolved;

	private int m_ToysFound;

	private Transform m_InitialBlockage;

	private AudioClip m_ToyRackClip;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ToyRackClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_toyrack");
		((Renderer)m_Cables).material.SetFloat("_Shimmer", 0f);
		m_ConveyerSwitch.SetActive(active: false);
		m_Light.SetActive(false);
		RandomizeToys(ref m_SpinnersLeft);
		RandomizeToys(ref m_SpinnersRight);
		RandomizeConveyer();
	}

	private void RandomizeConveyer()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item7 in m_DollConveyerContent)
		{
			Transform item = item7;
			m_DollConveyerContents.Add(item);
		}
		for (int i = 0; i < m_DollConveyerContents.Count; i++)
		{
			m_DollConveyerPositions.Add(m_DollConveyerContents[i].localPosition);
		}
		m_DollConveyerContents.Shuffle();
		for (int j = 0; j < m_DollConveyerContents.Count; j++)
		{
			m_DollConveyerContents[j].localPosition = m_DollConveyerPositions[j];
		}
		foreach (Transform item8 in m_ConveyerContent)
		{
			Transform item2 = item8;
			m_ConveyerContents.Add(item2);
		}
		for (int k = 0; k < m_ConveyerContents.Count; k++)
		{
			m_ConveyerPositions.Add(m_ConveyerContents[k].localPosition);
		}
		Transform item3 = m_ConveyerContents[0];
		m_ConveyerContents.RemoveAt(0);
		Transform item4 = m_ConveyerContents[0];
		m_ConveyerContents.RemoveAt(0);
		Transform item5 = m_ConveyerContents[0];
		m_ConveyerContents.RemoveAt(0);
		Transform item6 = m_ConveyerContents[0];
		m_ConveyerContents.RemoveAt(0);
		m_ConveyerContents.Shuffle();
		Transform val = m_ConveyerContents[4];
		m_ConveyerContents.RemoveAt(4);
		Vector3 val2 = m_ConveyerPositions[6];
		m_ConveyerPositions.RemoveAt(6);
		m_ConveyerContents.Add(item3);
		m_ConveyerContents.Add(item4);
		m_ConveyerContents.Add(item5);
		m_ConveyerContents.Add(item6);
		m_ConveyerContents.Shuffle();
		for (int l = 0; l < m_ConveyerContents.Count; l++)
		{
			m_ConveyerContents[l].localPosition = m_ConveyerPositions[l];
		}
		val.localPosition = val2;
		m_ConveyerContents.Add(val);
		m_ConveyerPositions.Insert(6, val2);
		List<Transform> list = new List<Transform>();
		for (int m = 0; m < m_ConveyerPositions.Count; m++)
		{
			foreach (Transform conveyerContent in m_ConveyerContents)
			{
				if (conveyerContent.localPosition == m_ConveyerPositions[m])
				{
					list.Add(conveyerContent);
					if (m == 6)
					{
						m_InitialBlockage = conveyerContent;
					}
				}
			}
		}
		m_ConveyerContents.Clear();
		m_ConveyerContents = new List<Transform>(list);
	}

	private void RandomizeToys(ref List<Spinners> spinners)
	{
		for (int i = 0; i < spinners.Count; i++)
		{
			Spinners spinners2 = spinners[i];
			if (Random.value > 0.2f)
			{
				int index = Random.Range(0, spinners2.Toys.Count);
				Interactable interactable = spinners2.Toys[index];
				spinners2.Toys.RemoveAt(index);
				interactable.Dispose();
			}
		}
	}

	public void Activate(bool isComplete = false)
	{
		((Renderer)m_Cables).material.SetFloat("_Shimmer", 1f);
		for (int i = 0; i < m_Machines.Count; i++)
		{
			Transform val = m_Machines[i];
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(val, 1.004f, 0.25f), (Ease)7), -1, (LoopType)1);
		}
		ActivateSpinners(ref m_SpinnersLeft, ref m_WheelLeft);
		ActivateSpinners(ref m_SpinnersRight, ref m_WheelRight);
		m_Light.SetActive(true);
		if (isComplete)
		{
			ForceComplete();
		}
	}

	private void ActivateSpinners(ref List<Spinners> spinners, ref Transform wheel)
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < spinners.Count; i++)
		{
			Spinners spinners2 = spinners[i];
			foreach (Interactable toy in spinners2.Toys)
			{
				toy.SetActive(active: true);
				toy.OnInteracted += HandleToyOnInteracted;
			}
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(spinners2.LargeWheel, new Vector3(0f, 0f, -3f), 0.25f, (RotateMode)3), (Ease)7), -1, (LoopType)1);
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(spinners2.SmallWheel, new Vector3(0f, 0f, -3f), 0.25f, (RotateMode)3), (Ease)7), -1, (LoopType)1);
		}
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(wheel, new Vector3(0f, 0f, 6f), 0.5f, (RotateMode)3), (Ease)7), -1, (LoopType)1);
	}

	private void HandleToyOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		Interactable interactable = (Interactable)sender;
		interactable.OnInteracted -= HandleToyOnInteracted;
		GameManager.Instance.Player.PlayPickUpSound();
		if (!m_IsLeftSolved && CheckSpinner(interactable, ref m_SpinnersLeft))
		{
			ShortcutExtensions.DOKill((Component)(object)m_WheelLeft, false);
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_WheelLeft, new Vector3(0f, 0f, 360f), 5f, (RotateMode)3), (Ease)1), -1, (LoopType)0);
			m_IsLeftSolved = true;
		}
		if (!m_IsRightSolved && CheckSpinner(interactable, ref m_SpinnersRight))
		{
			ShortcutExtensions.DOKill((Component)(object)m_WheelRight, false);
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_WheelRight, new Vector3(0f, 0f, 360f), 5f, (RotateMode)3), (Ease)1), -1, (LoopType)0);
			m_IsRightSolved = true;
		}
		if (m_IsLeftSolved && m_IsRightSolved)
		{
			this.OnComplete.Send(this);
			EnableConveyer();
			EnableDollConveyer();
		}
	}

	private void EnableConveyer()
	{
		m_ConveyerSwitch.OnInteracted += HndleConveyerSwitchOnInteracted;
		m_ConveyerSwitch.SetActive(active: true);
	}

	private void HndleConveyerSwitchOnInteracted(object sender, EventArgs e)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		m_ConveyerSwitch.OnInteracted -= HndleConveyerSwitchOnInteracted;
		m_ConveyerSwitch.SetActive(active: false);
		((Renderer)m_Cables).material.SetFloat("_Shimmer", 0f);
		TweenSettingsExtensions.OnComplete<Sequence>(DOMoveConveyer(), new TweenCallback(EnableConveyer));
	}

	private void EnableDollConveyer()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		Transform val = m_DollConveyerContents[m_DollConveyerContents.Count - 1];
		val.localPosition = m_DollConveyerPositions[0] + new Vector3(0f, 0f, 1f);
		m_DollConveyerContents.Insert(0, val);
		m_DollConveyerContents.RemoveAt(m_DollConveyerContents.Count - 1);
		Sequence val2 = DOTween.Sequence();
		for (int i = 0; i < m_DollConveyerContents.Count; i++)
		{
			Transform val3 = m_DollConveyerContents[i];
			TweenSettingsExtensions.Insert(val2, 0f, (Tween)(object)TweenSettingsExtensions.SetRelative<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(val3, -1f, 2f, false), (Ease)1), true));
		}
		TweenSettingsExtensions.OnComplete<Sequence>(val2, new TweenCallback(EnableDollConveyer));
	}

	private Sequence DOMoveConveyer()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		Vector3 localPosition = m_Lever.localPosition;
		GameManager.Instance.AudioManager.Play(m_ToyRackClip);
		TweenSettingsExtensions.Insert(val, 0.5f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveX(m_Lever, localPosition.x - 0.75f, 0.5f, false), (Ease)6));
		TweenSettingsExtensions.Insert(val, 1.25f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveX(m_Lever, localPosition.x, 0.5f, false), (Ease)7));
		for (int i = 0; i < m_ConveyerContents.Count; i++)
		{
			Transform val2 = m_ConveyerContents[i];
			float num = val2.localPosition.z - 6f;
			TweenSettingsExtensions.Insert(val, 0.5f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(val2, num, 4f, false), (Ease)28));
		}
		TweenSettingsExtensions.InsertCallback(val, 5.85f, (TweenCallback)delegate
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			Transform val3 = m_ConveyerContents[m_ConveyerContents.Count - 1];
			val3.localPosition = m_ConveyerPositions[0];
			m_ConveyerContents.Remove(val3);
			m_ConveyerContents.Insert(0, val3);
		});
		return val;
	}

	private bool CheckSpinner(Interactable toy, ref List<Spinners> spinners)
	{
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		bool result = true;
		for (int i = 0; i < spinners.Count; i++)
		{
			Spinners spinners2 = spinners[i];
			if (spinners2.Toys.Contains(toy))
			{
				spinners2.Toys.Remove(toy);
				toy.Dispose();
				if (spinners2.Toys.Count <= 0)
				{
					if (spinners.Equals(m_SpinnersLeft))
					{
						S13AudioManager.Instance.InvokeEvent("evt_activate_workshop_mech_left");
					}
					else
					{
						S13AudioManager.Instance.InvokeEvent("evt_activate_workshop_mech_right");
					}
					S13AudioManager.Instance.InvokeEvent("evt_toys_found" + ++m_ToysFound, 0.05f);
					spinners2.isSpinning = true;
					ShortcutExtensions.DOKill((Component)(object)spinners2.LargeWheel, false);
					TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(spinners2.LargeWheel, new Vector3(0f, 0f, -360f), 2f, (RotateMode)3), (Ease)1), -1, (LoopType)0);
					ShortcutExtensions.DOKill((Component)(object)spinners2.SmallWheel, false);
					TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(spinners2.SmallWheel, new Vector3(0f, 0f, -360f), 1f, (RotateMode)3), (Ease)1), -1, (LoopType)0);
				}
			}
			if (!spinners2.isSpinning)
			{
				result = false;
			}
		}
		return result;
	}

	private void ForceComplete()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		((Component)m_InitialBlockage).gameObject.SetActive(false);
		m_IsLeftSolved = true;
		m_IsRightSolved = true;
		for (int i = 0; i < m_SpinnersLeft.Count; i++)
		{
			Spinners spinners = m_SpinnersLeft[i];
			foreach (Interactable toy in spinners.Toys)
			{
				toy.Dispose();
			}
			spinners.isSpinning = true;
			ShortcutExtensions.DOKill((Component)(object)spinners.LargeWheel, false);
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(spinners.LargeWheel, new Vector3(0f, 0f, -360f), 2f, (RotateMode)3), (Ease)1), -1, (LoopType)0);
			ShortcutExtensions.DOKill((Component)(object)spinners.SmallWheel, false);
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(spinners.SmallWheel, new Vector3(0f, 0f, -360f), 1f, (RotateMode)3), (Ease)1), -1, (LoopType)0);
		}
		for (int j = 0; j < m_SpinnersRight.Count; j++)
		{
			Spinners spinners2 = m_SpinnersRight[j];
			foreach (Interactable toy2 in spinners2.Toys)
			{
				toy2.Dispose();
			}
			spinners2.isSpinning = true;
			ShortcutExtensions.DOKill((Component)(object)spinners2.LargeWheel, false);
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(spinners2.LargeWheel, new Vector3(0f, 0f, -360f), 2f, (RotateMode)3), (Ease)1), -1, (LoopType)0);
			ShortcutExtensions.DOKill((Component)(object)spinners2.SmallWheel, false);
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(spinners2.SmallWheel, new Vector3(0f, 0f, -360f), 1f, (RotateMode)3), (Ease)1), -1, (LoopType)0);
		}
		S13AudioManager.Instance.InvokeEvent("evt_activate_workshop_mech_left");
		S13AudioManager.Instance.InvokeEvent("evt_activate_workshop_mech_right");
		S13AudioManager.Instance.InvokeEvent("evt_toys_found4");
		((Renderer)m_Cables).material.SetFloat("_Shimmer", 0f);
		this.OnComplete.Send(this);
		EnableConveyer();
		EnableDollConveyer();
	}

	protected override void OnDisposed()
	{
		m_ToyRackClip = null;
		ShortcutExtensions.DOKill((Component)(object)m_WheelLeft, false);
		ShortcutExtensions.DOKill((Component)(object)m_WheelRight, false);
		for (int i = 0; i < m_SpinnersLeft.Count; i++)
		{
			Spinners spinners = m_SpinnersLeft[i];
			ShortcutExtensions.DOKill((Component)(object)spinners.LargeWheel, false);
			ShortcutExtensions.DOKill((Component)(object)spinners.SmallWheel, false);
			foreach (Interactable toy in spinners.Toys)
			{
				toy.OnInteracted -= HandleToyOnInteracted;
			}
		}
		for (int j = 0; j < m_SpinnersRight.Count; j++)
		{
			Spinners spinners2 = m_SpinnersRight[j];
			ShortcutExtensions.DOKill((Component)(object)spinners2.LargeWheel, false);
			ShortcutExtensions.DOKill((Component)(object)spinners2.SmallWheel, false);
			foreach (Interactable toy2 in spinners2.Toys)
			{
				toy2.OnInteracted -= HandleToyOnInteracted;
			}
		}
		base.OnDisposed();
	}
}
