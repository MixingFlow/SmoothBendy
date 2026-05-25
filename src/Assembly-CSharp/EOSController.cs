using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Epic.OnlineServices;
using Epic.OnlineServices.Achievements;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Platform;
using Epic.OnlineServices.PlayerDataStorage;
using Epic.OnlineServices.UserInfo;
using UnityEngine;

public class EOSController : MonoBehaviour
{
	private class DataStoreWrapper
	{
		private string m_filename;

		private byte[] m_data;

		private WriteFileOptions m_options;

		private int m_progress;

		public DataStoreWrapper(byte[] content, WriteFileOptions ops)
		{
			m_filename = ops.Filename;
			m_data = content;
			m_options = ops;
			m_options.WriteFileDataCallback = UploadPlayerStorageChunk;
		}

		private WriteResult UploadPlayerStorageChunk(WriteFileDataCallbackInfo info, out byte[] chunk)
		{
			int val = m_data.Length - m_progress;
			int num = Math.Min(val, 65536);
			chunk = new byte[num];
			Array.Copy(m_data, m_progress, chunk, 0, num);
			m_progress += num;
			if (m_progress >= m_data.Length)
			{
				return WriteResult.CompleteRequest;
			}
			return WriteResult.ContinueWriting;
		}

		public void WriteFile(PlayerDataStorageInterface storageInterface, Action<Result, string> callback)
		{
			storageInterface.WriteFile(m_options, null, delegate(WriteFileCallbackInfo info)
			{
				if (callback != null)
				{
					callback(info.ResultCode, m_filename);
				}
			});
		}
	}

	private const AuthScopeFlags BATIM_EOS_SCOPE = AuthScopeFlags.BasicProfile | AuthScopeFlags.FriendsList | AuthScopeFlags.Presence;

	private const float TICK_TIME = 3f / 32f;

	private const string COMMANDLINE_PREFIX_AUTHPASWORD = "-AUTH_PASSWORD";

	private const string COMMANDLINE_PREFIX_AUTHTYPE = "-AUTH_TYPE";

	private const string COMMANDLINE_PREFIX_AUTHLOGIN = "-AUTH_LOGIN";

	private const string COMMANDLINE_PREFIX_SANDBOXID = "-epicsandboxid";

	private const string COMMANDLINE_PREFIX_DEPLOYMENTID = "-epicdeploymentid";

	private const string COMMANDLINE_PREFIX_USERNAME = "-epicusername";

	private const string COMMANDLINE_PREFIX_LOCALE = "-epiclocale";

	public static Action<EOSController> onUserSignin;

	public static Action<EOSController> onInitialized;

	[SerializeField]
	private string m_productName;

	[SerializeField]
	private string m_productVersion;

	[SerializeField]
	private string m_productId;

	[SerializeField]
	private string m_sandboxId;

	[SerializeField]
	private string m_deploymentId;

	[SerializeField]
	private string m_clientId;

	[SerializeField]
	private string m_clientSecret;

	[SerializeField]
	private string m_artifactId;

	[SerializeField]
	private string m_encryptionKey;

	[SerializeField]
	private string m_cacheDirectory;

	private string m_loginId = string.Empty;

	private string m_loginToken = string.Empty;

	private bool m_isExchangeCode;

	private LoginCredentialType m_loginCredentialType;

	private static string s_debughistory;

	private PlatformInterface m_platformInterface;

	private AchievementsInterface m_achievementInterface;

	private PlayerDataStorageInterface m_dataStorageInterface;

	private EpicAccountId m_localId;

	private ProductUserId m_productUserId;

	private Coroutine m_tickCoroutine;

	private HashSet<string> m_pendingAchievements = new HashSet<string>();

	public static EOSController instance;

	public bool IsSaveReady { get; private set; }

	public bool IsInitialized { get; private set; }

	public string DisplayName { get; private set; }

	public string PreferredLanguage { get; private set; }

	private void OnDestroy()
	{
		if (m_tickCoroutine != null)
		{
			((MonoBehaviour)this).StopCoroutine(m_tickCoroutine);
		}
		PlatformInterface.Shutdown();
	}

	[Conditional("CHEATER")]
	private static void DebugLog(string m)
	{
		Debug.Log((object)m);
		s_debughistory = s_debughistory + m + "\n";
	}

	public void Init()
	{
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		instance = this;
		Dictionary<string, string> commandLineKeyValues = GetCommandLineKeyValues();
		foreach (string key in commandLineKeyValues.Keys)
		{
		}
		m_loginCredentialType = LoginCredentialType.AccountPortal;
		string text = Path.Combine(Application.persistentDataPath, m_cacheDirectory);
		Directory.CreateDirectory(text);
		m_isExchangeCode = GetValue(commandLineKeyValues, "-AUTH_TYPE", string.Empty) == "exchangecode";
		m_sandboxId = GetValue(commandLineKeyValues, "-epicsandboxid", m_sandboxId);
		m_deploymentId = GetValue(commandLineKeyValues, "-epicdeploymentid", m_deploymentId);
		if (m_isExchangeCode)
		{
			m_loginId = GetValue(commandLineKeyValues, "-AUTH_LOGIN", m_loginId);
			m_loginToken = GetValue(commandLineKeyValues, "-AUTH_PASSWORD", m_loginToken);
			DisplayName = GetValue(commandLineKeyValues, "-epicusername", null);
			PreferredLanguage = GetValue(commandLineKeyValues, "-epiclocale", null);
			m_loginCredentialType = LoginCredentialType.ExchangeCode;
		}
		InitializeOptions initializeOptions = new InitializeOptions();
		initializeOptions.ProductName = m_productName;
		initializeOptions.ProductVersion = m_productVersion;
		InitializeOptions options = initializeOptions;
		Result result = PlatformInterface.Initialize(options);
		AssertTermination(result == Result.Success, "Epic Store Initialization Failure");
		Options options2 = new Options();
		options2.ProductId = m_productId;
		options2.SandboxId = m_sandboxId;
		options2.DeploymentId = m_deploymentId;
		options2.TickBudgetInMilliseconds = 0u;
		options2.ClientCredentials = new ClientCredentials
		{
			ClientId = m_clientId,
			ClientSecret = m_clientSecret
		};
		options2.CacheDirectory = text;
		options2.IsServer = false;
		Options options3 = options2;
		if (!string.IsNullOrEmpty(m_encryptionKey))
		{
			options3.EncryptionKey = m_encryptionKey;
		}
		m_platformInterface = PlatformInterface.Create(options3);
		m_tickCoroutine = ((MonoBehaviour)this).StartCoroutine(EosTickCoroutine());
		Authenticate();
	}

	private void Authenticate()
	{
		if (m_isExchangeCode && string.IsNullOrEmpty(m_loginToken))
		{
			IsInitialized = true;
			if (onInitialized != null)
			{
				onInitialized(this);
			}
			return;
		}
		AuthScopeFlags scopeFlags = AuthScopeFlags.BasicProfile | AuthScopeFlags.FriendsList | AuthScopeFlags.Presence;
		AuthInterface authInterface = m_platformInterface.GetAuthInterface();
		AssertTermination(authInterface != null, "Epic Store Authentication Failure");
		string id = ((!string.IsNullOrEmpty(m_loginId)) ? m_loginId : null);
		string token = ((!string.IsNullOrEmpty(m_loginToken)) ? m_loginToken : null);
		Epic.OnlineServices.Auth.LoginOptions loginOptions = new Epic.OnlineServices.Auth.LoginOptions();
		loginOptions.ScopeFlags = scopeFlags;
		loginOptions.Credentials = new Epic.OnlineServices.Auth.Credentials
		{
			Type = m_loginCredentialType,
			Id = id,
			Token = token
		};
		Epic.OnlineServices.Auth.LoginOptions options = loginOptions;
		authInterface.Login(options, null, delegate(Epic.OnlineServices.Auth.LoginCallbackInfo info)
		{
			if (info.ResultCode == Result.Success)
			{
				m_localId = info.LocalUserId;
				InitUser();
			}
			else if (info.ResultCode == Result.NoConnection || info.ResultCode == Result.TimedOut)
			{
				IsInitialized = true;
				if (onInitialized != null)
				{
					onInitialized(this);
				}
			}
			else
			{
				AssertTermination(assertion: false, "Epic Store Authentication Failure " + info.ResultCode);
			}
		});
	}

	private void InitUser()
	{
		QueryUserInfoOptions queryUserInfoOptions = new QueryUserInfoOptions();
		queryUserInfoOptions.LocalUserId = m_localId;
		queryUserInfoOptions.TargetUserId = m_localId;
		QueryUserInfoOptions options = queryUserInfoOptions;
		UserInfoInterface userInterface = m_platformInterface.GetUserInfoInterface();
		userInterface.QueryUserInfo(options, null, delegate(QueryUserInfoCallbackInfo user)
		{
			if (user.ResultCode == Result.Success)
			{
				CopyUserInfoOptions options2 = new CopyUserInfoOptions
				{
					LocalUserId = m_localId,
					TargetUserId = m_localId
				};
				if (userInterface.CopyUserInfo(options2, out var outUserInfo) == Result.Success)
				{
					DisplayName = outUserInfo.DisplayName;
					if (onUserSignin != null)
					{
						onUserSignin(this);
					}
					CopyUserAuthTokenOptions options3 = new CopyUserAuthTokenOptions();
					m_platformInterface.GetAuthInterface().CopyUserAuthToken(options3, m_localId, out var outUserAuthToken);
					ConnectInterface connect = m_platformInterface.GetConnectInterface();
					Epic.OnlineServices.Connect.Credentials credentials = new Epic.OnlineServices.Connect.Credentials
					{
						Type = ExternalCredentialType.Epic,
						Token = outUserAuthToken.AccessToken
					};
					Epic.OnlineServices.Connect.LoginOptions options4 = new Epic.OnlineServices.Connect.LoginOptions
					{
						Credentials = credentials
					};
					connect.Login(options4, null, delegate(Epic.OnlineServices.Connect.LoginCallbackInfo loginInfo)
					{
						if (loginInfo.ResultCode == Result.InvalidUser)
						{
							CreateUserOptions options5 = new CreateUserOptions
							{
								ContinuanceToken = loginInfo.ContinuanceToken
							};
							connect.CreateUser(options5, null, delegate(CreateUserCallbackInfo createInfo)
							{
								AssertTermination(createInfo.ResultCode == Result.Success, "Epic Store User Failure");
								m_productUserId = createInfo.LocalUserId;
								InitAchievements();
							});
						}
						else if (loginInfo.ResultCode == Result.Success)
						{
							m_productUserId = loginInfo.LocalUserId;
							InitAchievements();
						}
						else
						{
							AssertTermination(assertion: false, "Epic Store User Failure");
						}
					});
				}
			}
		});
	}

	private void InitDataStorage()
	{
		m_dataStorageInterface = m_platformInterface.GetPlayerDataStorageInterface();
		AssertTermination(m_dataStorageInterface != null, "Data Storage Not Initialized");
		IsInitialized = true;
		IsSaveReady = true;
		if (onInitialized != null)
		{
			onInitialized(this);
		}
	}

	public void RequestDataStorageDirectory(Action<Result, List<FileMetadata>> callback)
	{
		if (m_dataStorageInterface == null)
		{
			if (callback != null)
			{
				callback(Result.NoConnection, new List<FileMetadata>());
			}
			return;
		}
		QueryFileListOptions queryFileListOptions = new QueryFileListOptions();
		queryFileListOptions.LocalUserId = m_productUserId;
		QueryFileListOptions queryFileListOptions2 = queryFileListOptions;
		List<FileMetadata> directoryMetaData = new List<FileMetadata>();
		m_dataStorageInterface.QueryFileList(queryFileListOptions2, null, delegate(QueryFileListCallbackInfo info)
		{
			if (info.ResultCode == Result.Success)
			{
				uint fileCount = info.FileCount;
				for (uint num = 0u; num < fileCount; num++)
				{
					CopyFileMetadataAtIndexOptions copyFileMetadataOptions = new CopyFileMetadataAtIndexOptions
					{
						LocalUserId = m_productUserId,
						Index = num
					};
					if (m_dataStorageInterface.CopyFileMetadataAtIndex(copyFileMetadataOptions, out var outMetadata) == Result.Success)
					{
						directoryMetaData.Add(outMetadata);
					}
				}
			}
			if (callback != null)
			{
				callback(info.ResultCode, directoryMetaData);
			}
		});
	}

	public void DownloadPlayerStorageFile(string filename, Action<Result, byte[]> callback)
	{
		if (m_dataStorageInterface == null)
		{
			if (callback != null)
			{
				callback(Result.NoConnection, new byte[0]);
			}
			return;
		}
		List<byte> data = new List<byte>();
		ReadFileOptions readFileOptions = new ReadFileOptions();
		readFileOptions.LocalUserId = m_productUserId;
		readFileOptions.Filename = filename;
		readFileOptions.ReadChunkLengthBytes = 4096u;
		readFileOptions.ReadFileDataCallback = delegate(ReadFileDataCallbackInfo chunk)
		{
			data.AddRange(chunk.DataChunk);
			return ReadResult.ContinueReading;
		};
		readFileOptions.FileTransferProgressCallback = null;
		ReadFileOptions readOptions = readFileOptions;
		m_dataStorageInterface.ReadFile(readOptions, null, delegate(ReadFileCallbackInfo res)
		{
			if (callback != null)
			{
				callback(res.ResultCode, data.ToArray());
			}
		});
	}

	public void UploadPlayerStorageFile(string filename, string filedata, Action<Result, string> callback)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(filedata);
		UploadPlayerStorageFile(filename, bytes, callback);
	}

	public void UploadPlayerStorageFile(string filename, byte[] data, Action<Result, string> callback)
	{
		if (m_dataStorageInterface == null)
		{
			callback?.Invoke(Result.NoConnection, string.Empty);
			return;
		}
		WriteFileOptions writeFileOptions = new WriteFileOptions();
		writeFileOptions.LocalUserId = m_productUserId;
		writeFileOptions.Filename = filename;
		writeFileOptions.ChunkLengthBytes = 65536u;
		WriteFileOptions ops = writeFileOptions;
		DataStoreWrapper dataStoreWrapper = new DataStoreWrapper(data, ops);
		dataStoreWrapper.WriteFile(m_dataStorageInterface, callback);
	}

	public void DeletePlayerStorageFile(string filename, Action<Result, string> callback)
	{
		if (m_dataStorageInterface == null)
		{
			if (callback != null)
			{
				callback(Result.NoConnection, string.Empty);
			}
			return;
		}
		DeleteFileOptions deleteFileOptions = new DeleteFileOptions();
		deleteFileOptions.LocalUserId = m_productUserId;
		deleteFileOptions.Filename = filename;
		DeleteFileOptions deleteOptions = deleteFileOptions;
		m_dataStorageInterface.DeleteFile(deleteOptions, null, delegate(DeleteFileCallbackInfo info)
		{
			if (callback != null)
			{
				callback(info.ResultCode, filename);
			}
		});
	}

	private void InitAchievements()
	{
		QueryDefinitionsOptions queryDefinitionsOptions = new QueryDefinitionsOptions();
		queryDefinitionsOptions.LocalUserId = m_productUserId;
		QueryDefinitionsOptions options = queryDefinitionsOptions;
		m_achievementInterface = m_platformInterface.GetAchievementsInterface();
		m_achievementInterface.QueryDefinitions(options, null, delegate(OnQueryDefinitionsCompleteCallbackInfo info)
		{
			if (info.ResultCode == Result.Success)
			{
				InitDataStorage();
			}
		});
	}

	public double QueryAchievement(string achievementName, bool includePending = true)
	{
		if (m_achievementInterface == null)
		{
			return 0.0;
		}
		if (includePending && m_pendingAchievements.Contains(achievementName))
		{
			return 1.0;
		}
		CopyPlayerAchievementByAchievementIdOptions copyPlayerAchievementByAchievementIdOptions = new CopyPlayerAchievementByAchievementIdOptions();
		copyPlayerAchievementByAchievementIdOptions.AchievementId = achievementName;
		copyPlayerAchievementByAchievementIdOptions.LocalUserId = m_productUserId;
		copyPlayerAchievementByAchievementIdOptions.TargetUserId = m_productUserId;
		CopyPlayerAchievementByAchievementIdOptions options = copyPlayerAchievementByAchievementIdOptions;
		if (m_achievementInterface.CopyPlayerAchievementByAchievementId(options, out var outAchievement) == Result.Success)
		{
			return outAchievement.Progress;
		}
		return 0.0;
	}

	public void UnlockAchievementAsync(string achievement)
	{
		if (!(m_achievementInterface == null))
		{
			m_pendingAchievements.Add(achievement);
			UnlockAchievementsOptions unlockAchievementsOptions = new UnlockAchievementsOptions();
			unlockAchievementsOptions.UserId = m_productUserId;
			unlockAchievementsOptions.AchievementIds = new string[1] { achievement };
			UnlockAchievementsOptions options = unlockAchievementsOptions;
			m_achievementInterface.UnlockAchievements(options, null, delegate
			{
				m_pendingAchievements.Remove(achievement);
			});
		}
	}

	private static string GetValue(Dictionary<string, string> kvargs, string key, string defaultValue)
	{
		if (kvargs.TryGetValue(key, out var value))
		{
			return value;
		}
		return defaultValue;
	}

	public static Dictionary<string, string> GetCommandLineKeyValues()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = commandLineArgs;
		foreach (string text in array)
		{
			string[] array2 = text.Split('=');
			if (array2.Length == 2)
			{
				string key = array2[0];
				string value = array2[1];
				dictionary[key] = value;
			}
			else
			{
				dictionary[text] = string.Empty;
			}
		}
		return dictionary;
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

	public static void AssertTermination(bool assertion, string message)
	{
		if (!assertion)
		{
			Debug.LogError((object)("Assertion failure: " + message));
			MessageBox(IntPtr.Zero, message, "Fatal Error", 0u);
			Application.Quit();
		}
	}

	private IEnumerator EosTickCoroutine()
	{
		while (m_platformInterface != null)
		{
			yield return (object)new WaitForSecondsRealtime(3f / 32f);
			m_platformInterface.Tick();
		}
	}
}
