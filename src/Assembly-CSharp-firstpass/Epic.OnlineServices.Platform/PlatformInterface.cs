using System;
using Epic.OnlineServices.Achievements;
using Epic.OnlineServices.AntiCheatClient;
using Epic.OnlineServices.AntiCheatServer;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.CustomInvites;
using Epic.OnlineServices.Ecom;
using Epic.OnlineServices.Friends;
using Epic.OnlineServices.KWS;
using Epic.OnlineServices.Leaderboards;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.Metrics;
using Epic.OnlineServices.Mods;
using Epic.OnlineServices.P2P;
using Epic.OnlineServices.PlayerDataStorage;
using Epic.OnlineServices.Presence;
using Epic.OnlineServices.ProgressionSnapshot;
using Epic.OnlineServices.RTC;
using Epic.OnlineServices.RTCAdmin;
using Epic.OnlineServices.Reports;
using Epic.OnlineServices.Sanctions;
using Epic.OnlineServices.Sessions;
using Epic.OnlineServices.Stats;
using Epic.OnlineServices.TitleStorage;
using Epic.OnlineServices.UI;
using Epic.OnlineServices.UserInfo;

namespace Epic.OnlineServices.Platform;

public sealed class PlatformInterface : Handle
{
	public const int AndroidinitializeoptionssysteminitializeoptionsApiLatest = 2;

	public const int CountrycodeMaxBufferLen = 5;

	public const int CountrycodeMaxLength = 4;

	public const int InitializeApiLatest = 4;

	public const int InitializeThreadaffinityApiLatest = 1;

	public const int LocalecodeMaxBufferLen = 10;

	public const int LocalecodeMaxLength = 9;

	public const int OptionsApiLatest = 11;

	public const int RtcoptionsApiLatest = 1;

	public const int PlatformWindowsrtcoptionsplatformspecificoptionsApiLatest = 1;

	public PlatformInterface()
	{
	}

	public PlatformInterface(IntPtr innerHandle)
		: base(innerHandle)
	{
	}

	public static Result Initialize(AndroidInitializeOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AndroidInitializeOptionsInternal, AndroidInitializeOptions>(ref target, options);
		Result result = Bindings.EOS_Initialize(target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public Result CheckForLauncherAndRestart()
	{
		return Bindings.EOS_Platform_CheckForLauncherAndRestart(base.InnerHandle);
	}

	public static PlatformInterface Create(Options options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<OptionsInternal, Options>(ref target, options);
		IntPtr source = Bindings.EOS_Platform_Create(target);
		Helper.TryMarshalDispose(ref target);
		Helper.TryMarshalGet(source, out PlatformInterface target2);
		return target2;
	}

	public AchievementsInterface GetAchievementsInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetAchievementsInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out AchievementsInterface target);
		return target;
	}

	public Result GetActiveCountryCode(EpicAccountId localUserId, out string outBuffer)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet(ref target, localUserId);
		IntPtr target2 = IntPtr.Zero;
		int inOutBufferLength = 5;
		Helper.TryMarshalAllocate(ref target2, inOutBufferLength);
		Result result = Bindings.EOS_Platform_GetActiveCountryCode(base.InnerHandle, target, target2, ref inOutBufferLength);
		Helper.TryMarshalGet(target2, out outBuffer);
		Helper.TryMarshalDispose(ref target2);
		return result;
	}

	public Result GetActiveLocaleCode(EpicAccountId localUserId, out string outBuffer)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet(ref target, localUserId);
		IntPtr target2 = IntPtr.Zero;
		int inOutBufferLength = 10;
		Helper.TryMarshalAllocate(ref target2, inOutBufferLength);
		Result result = Bindings.EOS_Platform_GetActiveLocaleCode(base.InnerHandle, target, target2, ref inOutBufferLength);
		Helper.TryMarshalGet(target2, out outBuffer);
		Helper.TryMarshalDispose(ref target2);
		return result;
	}

	public AntiCheatClientInterface GetAntiCheatClientInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetAntiCheatClientInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out AntiCheatClientInterface target);
		return target;
	}

	public AntiCheatServerInterface GetAntiCheatServerInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetAntiCheatServerInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out AntiCheatServerInterface target);
		return target;
	}

	public AuthInterface GetAuthInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetAuthInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out AuthInterface target);
		return target;
	}

	public ConnectInterface GetConnectInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetConnectInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out ConnectInterface target);
		return target;
	}

	public CustomInvitesInterface GetCustomInvitesInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetCustomInvitesInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out CustomInvitesInterface target);
		return target;
	}

	public EcomInterface GetEcomInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetEcomInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out EcomInterface target);
		return target;
	}

	public FriendsInterface GetFriendsInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetFriendsInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out FriendsInterface target);
		return target;
	}

	public KWSInterface GetKWSInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetKWSInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out KWSInterface target);
		return target;
	}

	public LeaderboardsInterface GetLeaderboardsInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetLeaderboardsInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out LeaderboardsInterface target);
		return target;
	}

	public LobbyInterface GetLobbyInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetLobbyInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out LobbyInterface target);
		return target;
	}

	public MetricsInterface GetMetricsInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetMetricsInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out MetricsInterface target);
		return target;
	}

	public ModsInterface GetModsInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetModsInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out ModsInterface target);
		return target;
	}

	public Result GetOverrideCountryCode(out string outBuffer)
	{
		IntPtr target = IntPtr.Zero;
		int inOutBufferLength = 5;
		Helper.TryMarshalAllocate(ref target, inOutBufferLength);
		Result result = Bindings.EOS_Platform_GetOverrideCountryCode(base.InnerHandle, target, ref inOutBufferLength);
		Helper.TryMarshalGet(target, out outBuffer);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public Result GetOverrideLocaleCode(out string outBuffer)
	{
		IntPtr target = IntPtr.Zero;
		int inOutBufferLength = 10;
		Helper.TryMarshalAllocate(ref target, inOutBufferLength);
		Result result = Bindings.EOS_Platform_GetOverrideLocaleCode(base.InnerHandle, target, ref inOutBufferLength);
		Helper.TryMarshalGet(target, out outBuffer);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public P2PInterface GetP2PInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetP2PInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out P2PInterface target);
		return target;
	}

	public PlayerDataStorageInterface GetPlayerDataStorageInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetPlayerDataStorageInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out PlayerDataStorageInterface target);
		return target;
	}

	public PresenceInterface GetPresenceInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetPresenceInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out PresenceInterface target);
		return target;
	}

	public ProgressionSnapshotInterface GetProgressionSnapshotInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetProgressionSnapshotInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out ProgressionSnapshotInterface target);
		return target;
	}

	public RTCAdminInterface GetRTCAdminInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetRTCAdminInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out RTCAdminInterface target);
		return target;
	}

	public RTCInterface GetRTCInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetRTCInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out RTCInterface target);
		return target;
	}

	public ReportsInterface GetReportsInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetReportsInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out ReportsInterface target);
		return target;
	}

	public SanctionsInterface GetSanctionsInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetSanctionsInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out SanctionsInterface target);
		return target;
	}

	public SessionsInterface GetSessionsInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetSessionsInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out SessionsInterface target);
		return target;
	}

	public StatsInterface GetStatsInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetStatsInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out StatsInterface target);
		return target;
	}

	public TitleStorageInterface GetTitleStorageInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetTitleStorageInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out TitleStorageInterface target);
		return target;
	}

	public UIInterface GetUIInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetUIInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out UIInterface target);
		return target;
	}

	public UserInfoInterface GetUserInfoInterface()
	{
		IntPtr source = Bindings.EOS_Platform_GetUserInfoInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out UserInfoInterface target);
		return target;
	}

	public static Result Initialize(InitializeOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<InitializeOptionsInternal, InitializeOptions>(ref target, options);
		Result result = Bindings.EOS_Initialize(target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public void Release()
	{
		Bindings.EOS_Platform_Release(base.InnerHandle);
	}

	public Result SetOverrideCountryCode(string newCountryCode)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet(ref target, newCountryCode);
		Result result = Bindings.EOS_Platform_SetOverrideCountryCode(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public Result SetOverrideLocaleCode(string newLocaleCode)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet(ref target, newLocaleCode);
		Result result = Bindings.EOS_Platform_SetOverrideLocaleCode(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public static Result Shutdown()
	{
		return Bindings.EOS_Shutdown();
	}

	public void Tick()
	{
		Bindings.EOS_Platform_Tick(base.InnerHandle);
	}

	public static PlatformInterface Create(WindowsOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<WindowsOptionsInternal, WindowsOptions>(ref target, options);
		IntPtr source = Bindings.EOS_Platform_Create(target);
		Helper.TryMarshalDispose(ref target);
		Helper.TryMarshalGet(source, out PlatformInterface target2);
		return target2;
	}
}
