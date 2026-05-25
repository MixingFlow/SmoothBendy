using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct CreateLobbyOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private uint m_MaxLobbyMembers;

	private LobbyPermissionLevel m_PermissionLevel;

	private int m_PresenceEnabled;

	private int m_AllowInvites;

	private IntPtr m_BucketId;

	private int m_DisableHostMigration;

	private int m_EnableRTCRoom;

	private IntPtr m_LocalRTCOptions;

	private IntPtr m_LobbyId;

	public ProductUserId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public uint MaxLobbyMembers
	{
		set
		{
			m_MaxLobbyMembers = value;
		}
	}

	public LobbyPermissionLevel PermissionLevel
	{
		set
		{
			m_PermissionLevel = value;
		}
	}

	public bool PresenceEnabled
	{
		set
		{
			Helper.TryMarshalSet(ref m_PresenceEnabled, value);
		}
	}

	public bool AllowInvites
	{
		set
		{
			Helper.TryMarshalSet(ref m_AllowInvites, value);
		}
	}

	public string BucketId
	{
		set
		{
			Helper.TryMarshalSet(ref m_BucketId, value);
		}
	}

	public bool DisableHostMigration
	{
		set
		{
			Helper.TryMarshalSet(ref m_DisableHostMigration, value);
		}
	}

	public bool EnableRTCRoom
	{
		set
		{
			Helper.TryMarshalSet(ref m_EnableRTCRoom, value);
		}
	}

	public LocalRTCOptions LocalRTCOptions
	{
		set
		{
			Helper.TryMarshalSet<LocalRTCOptionsInternal, LocalRTCOptions>(ref m_LocalRTCOptions, value);
		}
	}

	public string LobbyId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LobbyId, value);
		}
	}

	public void Set(CreateLobbyOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 7;
			LocalUserId = other.LocalUserId;
			MaxLobbyMembers = other.MaxLobbyMembers;
			PermissionLevel = other.PermissionLevel;
			PresenceEnabled = other.PresenceEnabled;
			AllowInvites = other.AllowInvites;
			BucketId = other.BucketId;
			DisableHostMigration = other.DisableHostMigration;
			EnableRTCRoom = other.EnableRTCRoom;
			LocalRTCOptions = other.LocalRTCOptions;
			LobbyId = other.LobbyId;
		}
	}

	public void Set(object other)
	{
		Set(other as CreateLobbyOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_BucketId);
		Helper.TryMarshalDispose(ref m_LocalRTCOptions);
		Helper.TryMarshalDispose(ref m_LobbyId);
	}
}
