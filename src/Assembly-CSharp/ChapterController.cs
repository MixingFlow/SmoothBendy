using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterController : AbstractChapterController
{
	[Header("< General >")]
	[SerializeField]
	private Chapters m_Chapter;

	[SerializeField]
	private Transform m_ChapterStartPoint;

	[Header("< Controllers >")]
	[SerializeField]
	private BaseController[] m_Controllers;

	private int m_CurrentController;

	private bool m_IsActive;

	public Chapters Chapter => m_Chapter;

	public DeathController DeathController { get; private set; }

	public override void Init()
	{
		base.Init();
		AudioListener.volume = 0f;
		GameManager.Instance.InitChapter(m_Chapter, m_ChapterStartPoint, this);
		if (m_Chapter != Chapters.ONE)
		{
			DeathController = GameManager.Instance.AssetManager.CreateAsset<DeathController>("GamePlay/Scenes/DeathController");
		}
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		GameManager.Instance.ChapterInitOnComplete();
		if (GameManager.Instance.GameData.CurrentSaveFile.IsNewGamePlus)
		{
			GameManager.Instance.Player.AllowSeeingTool(active: true);
			GameManager.Instance.Player.EnableSeeingTool(active: true);
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.Internecions[0] == 1)
		{
			AmplifyPostProcess component = ((Component)GameManager.Instance.GameCamera.WeaponCamera).GetComponent<AmplifyPostProcess>();
			((Behaviour)component).enabled = true;
		}
		StartChapter();
	}

	public override void InitializeChapter()
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create(string.Empty, string.Empty, string.Empty));
		if (Object.op_Implicit((Object)(object)DeathController))
		{
			DeathController.OnDeath += HandleDeathControllerOnDeath;
			DeathController.Activate();
		}
		BaseController baseController = m_Controllers[m_CurrentController];
		if (Object.op_Implicit((Object)(object)baseController))
		{
			baseController.OnComplete += HandleControllerOnComplete;
			baseController.Activate();
		}
		TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(DOTweenUtil.DOAudioListenerVolume(1f, 1f), 1f), (Ease)5);
		m_IsActive = true;
	}

	private void Update()
	{
		if (m_IsActive && GameManager.Instance.GameData.CurrentSaveFile != null)
		{
			GameManager.Instance.GameData.CurrentSaveFile.PlayTime += Time.deltaTime;
		}
	}

	private void HandleControllerOnComplete(object sender, EventArgs e)
	{
		m_Controllers[m_CurrentController].Dispose();
		m_CurrentController++;
		if (m_CurrentController >= m_Controllers.Length)
		{
			CompleteChapter();
			return;
		}
		BaseController baseController = m_Controllers[m_CurrentController];
		if (Object.op_Implicit((Object)(object)baseController))
		{
			baseController.OnComplete += HandleControllerOnComplete;
			baseController.Activate();
		}
	}

	private void CompleteChapter()
	{
		m_IsActive = false;
		switch (m_Chapter)
		{
		case Chapters.ONE:
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.IsChapterComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH2Data = new CH2DataVO();
			GameManager.Instance.GameData.CurrentSaveFile.CurrentChapter = 2;
			LoadChapter("CH2");
			break;
		case Chapters.TWO:
			GameManager.Instance.GameData.CurrentSaveFile.CH2Data.IsChapterComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data = new CH3DataVO();
			GameManager.Instance.GameData.CurrentSaveFile.CurrentChapter = 3;
			LoadChapter("CH3");
			break;
		case Chapters.THREE:
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsChapterComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH4Data = new CH4DataVO();
			GameManager.Instance.GameData.CurrentSaveFile.CurrentChapter = 4;
			LoadChapter("CH4");
			break;
		case Chapters.FOUR:
			GameManager.Instance.GameData.CurrentSaveFile.CH4Data.IsChapterComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH5Data = new CH5DataVO();
			GameManager.Instance.GameData.CurrentSaveFile.CurrentChapter = 5;
			LoadChapter("CH5");
			break;
		case Chapters.FIVE:
			SceneManager.LoadScene("Apartment");
			Dispose();
			break;
		}
	}

	private void LoadChapter(string chapter)
	{
		GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
		GameManager.Instance.ClearObjective();
		GameManager.Instance.LoadScene(new GenericLoaderDataVO(chapter));
	}

	private void HandleDeathControllerOnDeath(object sender, EventArgs e)
	{
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.GameData.CurrentSaveFile.HasDied = true;
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data != null)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InternecionValue = -1;
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data != null)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.InternecionValue = -1;
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data != null)
		{
			for (int i = 0; i < GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools.Length; i++)
			{
				GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[i] = -1;
			}
		}
		switch (m_Chapter)
		{
		case Chapters.ONE:
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.HasDied = true;
			RenderSettings.skybox.color = AmbienceColors.CH1_Base;
			break;
		case Chapters.TWO:
			GameManager.Instance.GameData.CurrentSaveFile.CH2Data.HasDied = true;
			RenderSettings.skybox.color = AmbienceColors.CH2_Base;
			break;
		case Chapters.THREE:
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HasDied = true;
			RenderSettings.skybox.color = AmbienceColors.CH3_Base;
			break;
		case Chapters.FOUR:
			GameManager.Instance.GameData.CurrentSaveFile.CH4Data.HasDied = true;
			RenderSettings.skybox.color = AmbienceColors.CH4_Base;
			break;
		case Chapters.FIVE:
			GameManager.Instance.GameData.CurrentSaveFile.CH5Data.HasDied = true;
			RenderSettings.skybox.color = AmbienceColors.CH5_Base;
			break;
		}
		DynamicGI.UpdateEnvironment();
		GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true);
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)DeathController))
		{
			DeathController.Dispose();
		}
		for (int i = 0; i < m_Controllers.Length; i++)
		{
			BaseController baseController = m_Controllers[i];
			if (Object.op_Implicit((Object)(object)baseController))
			{
				baseController.Dispose();
			}
		}
		base.OnDisposed();
	}
}
