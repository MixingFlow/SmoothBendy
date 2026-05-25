using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH5OpeningScenes : TMGMonoBehaviour
{
	[Header("Allison")]
	[SerializeField]
	private GameObject m_Allison;

	[SerializeField]
	private Animator m_Allison_Animator;

	[SerializeField]
	private AnimationClip m_Allison_Scene_01;

	[SerializeField]
	private AnimationClip m_Allison_Scene_02;

	[SerializeField]
	private AnimationClip m_Allison_Scene_04;

	[SerializeField]
	private AnimationClip m_Allison_Scene_05;

	[SerializeField]
	private AnimationClip m_Allison_Scene_06;

	[SerializeField]
	private AnimationClip m_Allison_Scene_07;

	[Header("Tom")]
	[SerializeField]
	private GameObject m_Tom;

	[SerializeField]
	private Animator m_Tom_Animator;

	[SerializeField]
	private AnimationClip m_Tom_Scene_02;

	[SerializeField]
	private AnimationClip m_Tom_Scene_03;

	[SerializeField]
	private AnimationClip m_Tom_Scene_04;

	[SerializeField]
	private AnimationClip m_Tom_Scene_06;

	[SerializeField]
	private AnimationClip m_Tom_Scene_07;

	[Header("Objects")]
	[SerializeField]
	private GameObject[] m_ObjectsScene01;

	[SerializeField]
	private GameObject[] m_ObjectsScene02;

	[SerializeField]
	private GameObject[] m_ObjectsScene03;

	[SerializeField]
	private GameObject[] m_ObjectsScene04;

	[SerializeField]
	private GameObject[] m_ObjectsScene05;

	[SerializeField]
	private GameObject[] m_ObjectsScene06;

	[SerializeField]
	private GameObject[] m_ObjectsScene07;

	[Header("Other")]
	[SerializeField]
	private CH5SafehouseDoor m_SafehouseDoor;

	[SerializeField]
	private CH5SoupBowl m_SoupBowl;

	[SerializeField]
	private GameObject m_PaintBrush;

	[SerializeField]
	private Transform m_SeeingTool;

	private AudioClip[] m_Scene1Clips;

	private AudioClip[] m_Scene2Clips;

	private AudioClip[] m_Scene4Clips;

	private AudioClip[] m_Scene5Clips;

	private AudioClip[] m_Scene6Clips;

	private AudioClip[] m_Scene7Clips;

	private Sequence m_CallbackSequence;

	private Sequence m_CallbackSequenceSecondary;

	public event EventHandler Scene01OnComplete;

	public event EventHandler Scene02OnComplete;

	public event EventHandler Scene03OnComplete;

	public event EventHandler Scene04OnComplete;

	public event EventHandler Scene05OnComplete;

	public event EventHandler Scene06OnComplete;

	public event EventHandler Scene07OnComplete;

	public event EventHandler Scene01Event01;

	public event EventHandler Scene02Event01;

	public event EventHandler Scene03Event01;

	public event EventHandler Scene05Event01;

	public event EventHandler Scene05Event02;

	public event EventHandler Scene05Event03;

	public event EventHandler Scene05Event04;

	public event EventHandler Scene05Event05;

	public event EventHandler Scene06Event01;

	public override void InitOnComplete()
	{
		AnimationEventUtil.AddAnimationEvent(ref m_Tom_Animator, ((Object)m_Tom_Scene_02).name, "CloseDoor", 497);
		AnimationEventUtil.AddAnimationEvent(ref m_Tom_Animator, ((Object)m_Tom_Scene_04).name, "TossSoupBowl", 552);
		AnimationEventUtil.AddAnimationEvent(ref m_Tom_Animator, ((Object)m_Tom_Scene_04).name, "CloseDoor", 873);
		AnimationEventUtil.AddAnimationEvent(ref m_Tom_Animator, ((Object)m_Tom_Scene_07).name, "OpenDoor", 705);
		AnimationEventUtil.AddAnimationEvent(ref m_Allison_Animator, ((Object)m_Allison_Scene_05).name, "DisableBrush", 1246);
		AnimationEventUtil.AddAnimationEvent(ref m_Allison_Animator, ((Object)m_Allison_Scene_05).name, "EnableSeeingTool", 1395);
		AnimationEventUtil.AddAnimationEvent(ref m_Allison_Animator, ((Object)m_Allison_Scene_05).name, "DropSeeingTool", 2033);
		AnimationEventUtil.AddAnimationEvent(ref m_Allison_Animator, ((Object)m_Allison_Scene_05).name, "EnableBrush", 3462);
		m_Scene1Clips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/Opening/Scene1");
		m_Scene2Clips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/Opening/Scene2");
		m_Scene4Clips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/Opening/Scene4");
		m_Scene5Clips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/Opening/Scene5");
		m_Scene6Clips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/Opening/Scene6");
		m_Scene7Clips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/Opening/Scene7");
		m_PaintBrush.SetActive(false);
		((Component)m_SeeingTool).gameObject.SetActive(false);
		DisableAll();
		base.InitOnComplete();
	}

	public void ForceComplete()
	{
		m_SafehouseDoor.ForceOpen();
		m_Tom.SetActive(false);
		m_Allison.SetActive(false);
		for (int i = 0; i < m_ObjectsScene07.Length; i++)
		{
			m_ObjectsScene07[i].SetActive(true);
		}
	}

	public void TossSoupBowl()
	{
		m_SoupBowl.AddForce();
	}

	public void OpenDoor()
	{
		m_SafehouseDoor.OpenDoor();
	}

	public void CloseDoor()
	{
		m_SafehouseDoor.CloseDoor();
	}

	public void DisableBrush()
	{
		m_PaintBrush.SetActive(false);
	}

	public void EnableBrush()
	{
		m_PaintBrush.SetActive(true);
	}

	public void EnableSeeingTool()
	{
		((Component)m_SeeingTool).gameObject.SetActive(true);
	}

	public void DropSeeingTool()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		m_SeeingTool.SetParent((Transform)null);
		Vector3 val = m_SeeingTool.position + Vector3.right * 5f;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_SeeingTool, val, 0.5f, false), (Ease)5), 1f), (TweenCallback)delegate
		{
			((Component)m_SeeingTool).gameObject.SetActive(false);
		});
	}

	public void ActivateScene01()
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		Scene01SetActive(active: true);
		m_SafehouseDoor.ForceClose();
		m_Tom.SetActive(false);
		m_Allison.SetActive(true);
		m_Allison_Animator.Play(((Object)m_Allison_Scene_01).name);
		for (int i = 0; i < m_Scene1Clips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Scene1Clips[i], SubtitleConstants.DIA_CH5_OPENING_SCENE_01[i]));
			if (i == m_Scene1Clips.Length - 2)
			{
				audioObject.OnComplete += delegate
				{
					this.Scene01Event01.Send(this);
				};
			}
		}
		ClearCallbackSequence();
		m_CallbackSequence = TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), m_Allison_Scene_01.length, new TweenCallback(SendScene01OnComplete));
	}

	private void SendScene01OnComplete()
	{
		this.Scene01OnComplete.Send(this);
	}

	public void ActivateScene02()
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		DisableAll();
		Scene02SetActive(active: true);
		m_SafehouseDoor.ForceOpen();
		m_Tom.SetActive(true);
		m_Allison.SetActive(true);
		m_Allison_Animator.Play(((Object)m_Allison_Scene_02).name);
		m_Tom_Animator.Play(((Object)m_Tom_Scene_02).name);
		for (int i = 0; i < m_Scene2Clips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Scene2Clips[i], SubtitleConstants.DIA_CH5_OPENING_SCENE_02[i]));
			if (i == m_Scene2Clips.Length - 2)
			{
				audioObject.OnComplete += delegate
				{
					this.Scene02Event01.Send(this);
				};
			}
		}
		float length = m_Allison_Scene_02.length;
		if (m_Tom_Scene_02.length > length)
		{
			length = m_Tom_Scene_02.length;
		}
		ClearCallbackSequence();
		m_CallbackSequence = TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), length, new TweenCallback(SendScene02OnComplete));
	}

	private void SendScene02OnComplete()
	{
		this.Scene02OnComplete.Send(this);
	}

	public void ActivateScene03()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		DisableAll();
		Scene03SetActive(active: true);
		m_SafehouseDoor.ForceClose();
		m_Tom.SetActive(true);
		m_Allison.SetActive(false);
		m_Tom_Animator.Play(((Object)m_Tom_Scene_03).name);
		ClearCallbackSequence();
		m_CallbackSequence = TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), m_Tom_Scene_03.length * 3f, (TweenCallback)delegate
		{
			this.Scene03Event01.Send(this);
		});
		ClearCallbackSequenceSecondary();
		m_CallbackSequenceSecondary = TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), m_Tom_Scene_03.length * 4f, new TweenCallback(SendScene03OnComplete));
	}

	private void SendScene03OnComplete()
	{
		this.Scene03OnComplete.Send(this);
	}

	public void ActivateScene04()
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		DisableAll();
		Scene04SetActive(active: true);
		m_SafehouseDoor.ForceOpen();
		m_Tom.SetActive(true);
		m_Allison.SetActive(true);
		m_Allison_Animator.Play(((Object)m_Allison_Scene_04).name);
		m_Tom_Animator.Play(((Object)m_Tom_Scene_04).name);
		for (int i = 0; i < m_Scene4Clips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Scene4Clips[i], SubtitleConstants.DIA_CH5_OPENING_SCENE_04[i]));
		}
		float length = m_Allison_Scene_04.length;
		if (m_Tom_Scene_04.length > length)
		{
			length = m_Tom_Scene_04.length;
		}
		ClearCallbackSequence();
		m_CallbackSequence = TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), length, new TweenCallback(SendScene04OnComplete));
	}

	private void SendScene04OnComplete()
	{
		this.Scene04OnComplete.Send(this);
	}

	public void ActivateScene05()
	{
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		DisableAll();
		Scene05SetActive(active: true);
		m_SafehouseDoor.ForceClose();
		m_Tom.SetActive(false);
		m_Allison.SetActive(true);
		m_Allison_Animator.Play(((Object)m_Allison_Scene_05).name);
		for (int i = 0; i < m_Scene5Clips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Scene5Clips[i], SubtitleConstants.DIA_CH5_OPENING_SCENE_05[i]));
			switch (i)
			{
			case 15:
				audioObject.OnComplete += delegate
				{
					this.Scene05Event01.Send(this);
				};
				continue;
			case 22:
				audioObject.OnComplete += delegate
				{
					this.Scene05Event02.Send(this);
				};
				continue;
			case 25:
				audioObject.OnComplete += delegate
				{
					this.Scene05Event03.Send(this);
				};
				continue;
			case 26:
				audioObject.OnComplete += delegate
				{
					this.Scene05Event04.Send(this);
				};
				continue;
			}
			if (i == m_Scene5Clips.Length - 1)
			{
				audioObject.OnComplete += delegate
				{
					this.Scene05Event05.Send(this);
				};
			}
		}
		ClearCallbackSequence();
		m_CallbackSequence = TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), m_Allison_Scene_05.length, new TweenCallback(SendScene05OnComplete));
	}

	private void SendScene05OnComplete()
	{
		this.Scene05OnComplete.Send(this);
	}

	public void ActivateScene06()
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		DisableAll();
		Scene06SetActive(active: true);
		m_SafehouseDoor.ForceClose();
		m_Tom.SetActive(true);
		m_Allison.SetActive(true);
		m_Allison_Animator.Play(((Object)m_Allison_Scene_06).name);
		m_Tom_Animator.Play(((Object)m_Tom_Scene_06).name);
		for (int i = 0; i < m_Scene6Clips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Scene6Clips[i], SubtitleConstants.DIA_CH5_OPENING_SCENE_06[i]));
			if (i == m_Scene6Clips.Length - 2)
			{
				audioObject.OnComplete += delegate
				{
					this.Scene06Event01.Send(this);
				};
			}
		}
		float length = m_Allison_Scene_06.length;
		if (m_Tom_Scene_06.length > length)
		{
			length = m_Tom_Scene_06.length;
		}
		ClearCallbackSequence();
		m_CallbackSequence = TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), length, new TweenCallback(SendScene06OnComplete));
	}

	private void SendScene06OnComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_ch5_scene06_complete");
		this.Scene06OnComplete.Send(this);
	}

	public void ActivateScene07()
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		DisableAll();
		Scene07SetActive(active: true);
		m_SafehouseDoor.ForceClose();
		m_Tom.SetActive(true);
		m_Allison.SetActive(true);
		m_Allison_Animator.Play(((Object)m_Allison_Scene_07).name);
		m_Tom_Animator.Play(((Object)m_Tom_Scene_07).name);
		for (int i = 0; i < m_Scene7Clips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Scene7Clips[i], SubtitleConstants.DIA_CH5_OPENING_SCENE_07[i]));
		}
		float length = m_Allison_Scene_07.length;
		if (m_Tom_Scene_07.length > length)
		{
			length = m_Tom_Scene_07.length;
		}
		ClearCallbackSequence();
		m_CallbackSequence = TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), length, new TweenCallback(SendScene07OnComplete));
	}

	private void SendScene07OnComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_ch5_scene07_complete");
		this.Scene07OnComplete.Send(this);
	}

	private void Scene01SetActive(bool active)
	{
		for (int i = 0; i < m_ObjectsScene01.Length; i++)
		{
			m_ObjectsScene01[i].SetActive(active);
		}
	}

	private void Scene02SetActive(bool active)
	{
		for (int i = 0; i < m_ObjectsScene02.Length; i++)
		{
			m_ObjectsScene02[i].SetActive(active);
		}
	}

	private void Scene03SetActive(bool active)
	{
		for (int i = 0; i < m_ObjectsScene03.Length; i++)
		{
			m_ObjectsScene03[i].SetActive(active);
		}
	}

	private void Scene04SetActive(bool active)
	{
		for (int i = 0; i < m_ObjectsScene04.Length; i++)
		{
			m_ObjectsScene04[i].SetActive(active);
		}
	}

	private void Scene05SetActive(bool active)
	{
		for (int i = 0; i < m_ObjectsScene05.Length; i++)
		{
			m_ObjectsScene05[i].SetActive(active);
		}
	}

	private void Scene06SetActive(bool active)
	{
		for (int i = 0; i < m_ObjectsScene06.Length; i++)
		{
			m_ObjectsScene06[i].SetActive(active);
		}
	}

	private void Scene07SetActive(bool active)
	{
		for (int i = 0; i < m_ObjectsScene07.Length; i++)
		{
			m_ObjectsScene07[i].SetActive(active);
		}
	}

	private void ClearCallbackSequence()
	{
		if (m_CallbackSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_CallbackSequence, false);
			m_CallbackSequence = null;
		}
	}

	private void ClearCallbackSequenceSecondary()
	{
		if (m_CallbackSequenceSecondary != null)
		{
			TweenExtensions.Kill((Tween)(object)m_CallbackSequenceSecondary, false);
			m_CallbackSequenceSecondary = null;
		}
	}

	private void DisableAll()
	{
		Scene01SetActive(active: false);
		Scene02SetActive(active: false);
		Scene03SetActive(active: false);
		Scene04SetActive(active: false);
		Scene05SetActive(active: false);
		Scene06SetActive(active: false);
		Scene07SetActive(active: false);
	}

	public void SkipFinal()
	{
		DisableAll();
		Scene07SetActive(active: true);
		ClearCallbackSequence();
		ClearCallbackSequenceSecondary();
		this.Scene01OnComplete = null;
		this.Scene02OnComplete = null;
		this.Scene03OnComplete = null;
		this.Scene04OnComplete = null;
		this.Scene05OnComplete = null;
		this.Scene06OnComplete = null;
		this.Scene07OnComplete = null;
		this.Scene01Event01 = null;
		this.Scene02Event01 = null;
		this.Scene03Event01 = null;
		this.Scene05Event01 = null;
		this.Scene05Event02 = null;
		this.Scene05Event03 = null;
		this.Scene05Event04 = null;
		this.Scene05Event05 = null;
		this.Scene06Event01 = null;
		m_SafehouseDoor.ForceOpen();
		Object.Destroy((Object)(object)m_Allison);
		Object.Destroy((Object)(object)m_Tom);
	}

	protected override void OnDisposed()
	{
		ClearCallbackSequence();
		ClearCallbackSequenceSecondary();
		this.Scene01OnComplete = null;
		this.Scene02OnComplete = null;
		this.Scene03OnComplete = null;
		this.Scene04OnComplete = null;
		this.Scene05OnComplete = null;
		this.Scene06OnComplete = null;
		this.Scene07OnComplete = null;
		this.Scene01Event01 = null;
		this.Scene02Event01 = null;
		this.Scene03Event01 = null;
		this.Scene05Event01 = null;
		this.Scene05Event02 = null;
		this.Scene05Event03 = null;
		this.Scene05Event04 = null;
		this.Scene05Event05 = null;
		this.Scene06Event01 = null;
		Object.Destroy((Object)(object)m_Allison);
		Object.Destroy((Object)(object)m_Tom);
		m_Allison = null;
		m_Allison_Animator = null;
		m_Allison_Scene_01 = null;
		m_Allison_Scene_02 = null;
		m_Allison_Scene_04 = null;
		m_Allison_Scene_05 = null;
		m_Allison_Scene_06 = null;
		m_Allison_Scene_07 = null;
		m_Tom = null;
		m_Tom_Animator = null;
		m_Tom_Scene_02 = null;
		m_Tom_Scene_03 = null;
		m_Tom_Scene_04 = null;
		m_Tom_Scene_06 = null;
		m_Tom_Scene_07 = null;
		m_ObjectsScene01 = null;
		m_ObjectsScene02 = null;
		m_ObjectsScene03 = null;
		m_ObjectsScene04 = null;
		m_ObjectsScene05 = null;
		m_ObjectsScene06 = null;
		m_ObjectsScene07 = null;
		m_SafehouseDoor = null;
		m_SoupBowl = null;
		m_PaintBrush = null;
		m_SeeingTool = null;
		base.OnDisposed();
	}
}
