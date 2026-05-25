using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMG.Core;
using TMG.Data;
using UnityEngine;

public class GameDataManager : TMGAbstractDisposable
{
	private const string OLD_COMPANY = "TheMeatly Games";

	private const string NEW_COMPANY = "Joey Drew Studios";

	private const string SAVE_FOLDER_OLD = "Saves/";

	private const string SAVE_FOLDER = "/Saves/";

	private const string FILE_NAME = "batim.game";

	private const string SAVE_FILE = "/Saves/batim.game";

	private string m_FullPathOld;

	private string m_FullPath;

	private GameData m_GameData;

	private AsyncLoaderController m_AsyncLoaderController;

	public event EventHandler OnLoad;

	public event EventHandler OnLoadComplete;

	public event EventHandler OnSave;

	public event EventHandler OnSaveComplete;

	public GameDataManager()
	{
		string text = Application.persistentDataPath.Replace("Joey Drew Studios", "TheMeatly Games");
		m_FullPathOld = text + "Saves//Saves/batim.game";
		m_FullPathOld = m_FullPathOld.Replace("/Saves/", string.Empty);
		m_FullPath = Application.persistentDataPath + "/Saves/batim.game";
		if (!Directory.Exists(Application.persistentDataPath + "/Saves/"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Saves/");
		}
		if (!File.Exists(m_FullPath) && Directory.Exists(text) && File.Exists(m_FullPathOld))
		{
			File.Copy(m_FullPathOld, m_FullPath, overwrite: true);
		}
		GameManager.Instance.GameData = new GameData();
	}

	public void Save(bool isObjectiveDataOnly = false, bool shouldShowSaveIndicator = true)
	{
		if (GameManager.Instance.GameData.CurrentSaveFile != null && (GameManager.Instance.GameData.NoSaveFile == null || GameManager.Instance.GameData.CurrentSaveFile.ID != GameManager.Instance.GameData.NoSaveFile.ID))
		{
			Save(GameManager.Instance.GameData.CurrentSaveFile, isObjectiveDataOnly, shouldShowSaveIndicator);
		}
	}

	public void Save(SaveFileData saveFileData, bool isObjectiveDataOnly = false, bool shouldShowSaveIndicator = true)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		HandleOnSave();
		if (!isObjectiveDataOnly && Object.op_Implicit((Object)(object)GameManager.Instance.Player))
		{
			if (GameManager.Instance.CurrentChapter.Chapter == Chapters.ONE)
			{
				saveFileData.CH1Data.PlayerPosition = new Vector3DataVO(GameManager.Instance.Player.transform.position);
				saveFileData.CH1Data.PlayerRotation = new Vector3DataVO(GameManager.Instance.Player.transform.eulerAngles);
			}
			else if (GameManager.Instance.CurrentChapter.Chapter == Chapters.TWO)
			{
				saveFileData.CH2Data.PlayerPosition = new Vector3DataVO(GameManager.Instance.Player.transform.position);
				saveFileData.CH2Data.PlayerRotation = new Vector3DataVO(GameManager.Instance.Player.transform.eulerAngles);
			}
			else if (GameManager.Instance.CurrentChapter.Chapter == Chapters.THREE)
			{
				saveFileData.CH3Data.PlayerPosition = new Vector3DataVO(GameManager.Instance.Player.transform.position);
				saveFileData.CH3Data.PlayerRotation = new Vector3DataVO(GameManager.Instance.Player.transform.eulerAngles);
			}
			else if (GameManager.Instance.CurrentChapter.Chapter == Chapters.FOUR)
			{
				saveFileData.CH4Data.PlayerPosition = new Vector3DataVO(GameManager.Instance.Player.transform.position);
				saveFileData.CH4Data.PlayerRotation = new Vector3DataVO(GameManager.Instance.Player.transform.eulerAngles);
			}
			else if (GameManager.Instance.CurrentChapter.Chapter == Chapters.FIVE)
			{
				saveFileData.CH5Data.PlayerPosition = new Vector3DataVO(GameManager.Instance.Player.transform.position);
				saveFileData.CH5Data.PlayerRotation = new Vector3DataVO(GameManager.Instance.Player.transform.eulerAngles);
			}
		}
		for (int i = 0; i < GameManager.Instance.GameData.SaveFiles.Length; i++)
		{
			if (GameManager.Instance.GameData.SaveFiles[i] != null && GameManager.Instance.GameData.SaveFiles[i].ID == saveFileData.ID)
			{
				GameManager.Instance.GameData.SaveFiles[i] = GameManager.Instance.GameData.CurrentSaveFile;
				break;
			}
		}
		SaveAsync(GameManager.Instance.GameData, shouldShowSaveIndicator);
	}

	private void SaveAsync(GameData data, bool shouldShowSaveIndicator = true)
	{
		if (!Object.op_Implicit((Object)(object)m_AsyncLoaderController))
		{
			m_AsyncLoaderController = GameManager.Instance.UIManager.Show<AsyncLoaderController>("UI/Loaders/AsyncLoader", "ASYNC LOADER");
		}
		if (shouldShowSaveIndicator)
		{
			m_AsyncLoaderController.Show();
		}
		AsyncFileAPI.OnFileWrittenEvent += OnFileWrittenEventCallback;
		AsyncFileAPI.SaveData(data, m_FullPath);
	}

	private void OnFileWrittenEventCallback()
	{
		AsyncFileAPI.OnFileWrittenEvent -= OnFileWrittenEventCallback;
		HandleOnSaveComplete();
	}

	private void HandleOnSave()
	{
		this.OnSave.Send(this);
		Log("SAVING");
	}

	private void HandleOnSaveComplete()
	{
		m_AsyncLoaderController.Hide();
		this.OnSaveComplete.Send(this);
		Log("SAVE COMPLETE");
	}

	public void Load()
	{
		HandleOnLoad();
		if (File.Exists(m_FullPath))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(m_FullPath, FileMode.Open);
			m_GameData = (GameData)binaryFormatter.Deserialize(fileStream);
			for (int i = 0; i < m_GameData.SaveFiles.Length; i++)
			{
				SaveFileData saveFileData = m_GameData.SaveFiles[i];
				if (saveFileData != null && (saveFileData.Internecions == null || saveFileData.Internecions.Length <= 0))
				{
					saveFileData.Internecions = new int[5];
				}
			}
			if (m_GameData.CurrentSaveFile != null && (m_GameData.CurrentSaveFile.Internecions == null || m_GameData.CurrentSaveFile.Internecions.Length <= 0))
			{
				m_GameData.CurrentSaveFile.Internecions = new int[5];
			}
			if (m_GameData.CH1AchievementData == null)
			{
				m_GameData.CH1AchievementData = new AchievementSaveData();
			}
			if (m_GameData.CH2AchievementData == null)
			{
				m_GameData.CH2AchievementData = new AchievementSaveData();
			}
			if (m_GameData.CH3AchievementData == null)
			{
				m_GameData.CH3AchievementData = new AchievementSaveData();
			}
			if (m_GameData.CH4AchievementData == null)
			{
				m_GameData.CH4AchievementData = new AchievementSaveData();
			}
			if (m_GameData.CH5AchievementData == null)
			{
				m_GameData.CH5AchievementData = new AchievementSaveData();
			}
			GameManager.Instance.GameData = m_GameData;
			fileStream.Close();
		}
		else
		{
			GameManager.Instance.GameData = new GameData();
		}
		HandleOnLoadComplete();
	}

	private void HandleOnLoad()
	{
		this.OnLoad.Send(this);
		Log("LOADING");
	}

	private void HandleOnLoadComplete()
	{
		this.OnLoadComplete.Send(this);
		Log("LOAD COMPLETE");
	}

	private void Log(string message)
	{
	}

	protected override void OnDisposed()
	{
		this.OnLoad = null;
		this.OnLoadComplete = null;
		this.OnSave = null;
		this.OnSaveComplete = null;
		m_AsyncLoaderController = null;
		m_GameData = null;
		base.OnDisposed();
	}
}
