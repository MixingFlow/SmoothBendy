using System;

namespace Epic.OnlineServices.RTCAdmin;

public sealed class RTCAdminInterface : Handle
{
	public const int CopyusertokenbyindexApiLatest = 2;

	public const int CopyusertokenbyuseridApiLatest = 2;

	public const int KickApiLatest = 1;

	public const int QueryjoinroomtokenApiLatest = 2;

	public const int SetparticipanthardmuteApiLatest = 1;

	public const int UsertokenApiLatest = 1;

	public RTCAdminInterface()
	{
	}

	public RTCAdminInterface(IntPtr innerHandle)
		: base(innerHandle)
	{
	}

	public Result CopyUserTokenByIndex(CopyUserTokenByIndexOptions options, out UserToken outUserToken)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<CopyUserTokenByIndexOptionsInternal, CopyUserTokenByIndexOptions>(ref target, options);
		IntPtr outUserToken2 = IntPtr.Zero;
		Result result = Bindings.EOS_RTCAdmin_CopyUserTokenByIndex(base.InnerHandle, target, ref outUserToken2);
		Helper.TryMarshalDispose(ref target);
		if (Helper.TryMarshalGet<UserTokenInternal, UserToken>(outUserToken2, out outUserToken))
		{
			Bindings.EOS_RTCAdmin_UserToken_Release(outUserToken2);
		}
		return result;
	}

	public Result CopyUserTokenByUserId(CopyUserTokenByUserIdOptions options, out UserToken outUserToken)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<CopyUserTokenByUserIdOptionsInternal, CopyUserTokenByUserIdOptions>(ref target, options);
		IntPtr outUserToken2 = IntPtr.Zero;
		Result result = Bindings.EOS_RTCAdmin_CopyUserTokenByUserId(base.InnerHandle, target, ref outUserToken2);
		Helper.TryMarshalDispose(ref target);
		if (Helper.TryMarshalGet<UserTokenInternal, UserToken>(outUserToken2, out outUserToken))
		{
			Bindings.EOS_RTCAdmin_UserToken_Release(outUserToken2);
		}
		return result;
	}

	public void Kick(KickOptions options, object clientData, OnKickCompleteCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<KickOptionsInternal, KickOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnKickCompleteCallbackInternal onKickCompleteCallbackInternal = OnKickCompleteCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onKickCompleteCallbackInternal);
		Bindings.EOS_RTCAdmin_Kick(base.InnerHandle, target, clientDataAddress, onKickCompleteCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	public void QueryJoinRoomToken(QueryJoinRoomTokenOptions options, object clientData, OnQueryJoinRoomTokenCompleteCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<QueryJoinRoomTokenOptionsInternal, QueryJoinRoomTokenOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnQueryJoinRoomTokenCompleteCallbackInternal onQueryJoinRoomTokenCompleteCallbackInternal = OnQueryJoinRoomTokenCompleteCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryJoinRoomTokenCompleteCallbackInternal);
		Bindings.EOS_RTCAdmin_QueryJoinRoomToken(base.InnerHandle, target, clientDataAddress, onQueryJoinRoomTokenCompleteCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	public void SetParticipantHardMute(SetParticipantHardMuteOptions options, object clientData, OnSetParticipantHardMuteCompleteCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<SetParticipantHardMuteOptionsInternal, SetParticipantHardMuteOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnSetParticipantHardMuteCompleteCallbackInternal onSetParticipantHardMuteCompleteCallbackInternal = OnSetParticipantHardMuteCompleteCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onSetParticipantHardMuteCompleteCallbackInternal);
		Bindings.EOS_RTCAdmin_SetParticipantHardMute(base.InnerHandle, target, clientDataAddress, onSetParticipantHardMuteCompleteCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	[MonoPInvokeCallback(typeof(OnKickCompleteCallbackInternal))]
	internal static void OnKickCompleteCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnKickCompleteCallback, KickCompleteCallbackInfoInternal, KickCompleteCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnQueryJoinRoomTokenCompleteCallbackInternal))]
	internal static void OnQueryJoinRoomTokenCompleteCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnQueryJoinRoomTokenCompleteCallback, QueryJoinRoomTokenCompleteCallbackInfoInternal, QueryJoinRoomTokenCompleteCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnSetParticipantHardMuteCompleteCallbackInternal))]
	internal static void OnSetParticipantHardMuteCompleteCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnSetParticipantHardMuteCompleteCallback, SetParticipantHardMuteCompleteCallbackInfoInternal, SetParticipantHardMuteCompleteCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}
}
