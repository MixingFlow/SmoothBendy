using System;
using Epic.OnlineServices.RTCAudio;

namespace Epic.OnlineServices.RTC;

public sealed class RTCInterface : Handle
{
	public const int AddnotifydisconnectedApiLatest = 1;

	public const int AddnotifyparticipantstatuschangedApiLatest = 1;

	public const int BlockparticipantApiLatest = 1;

	public const int JoinroomApiLatest = 1;

	public const int LeaveroomApiLatest = 1;

	public const int ParticipantmetadataApiLatest = 1;

	public const int ParticipantmetadataKeyMaxcharcount = 256;

	public const int ParticipantmetadataValueMaxcharcount = 256;

	public const int SetroomsettingApiLatest = 1;

	public const int SetsettingApiLatest = 1;

	public RTCInterface()
	{
	}

	public RTCInterface(IntPtr innerHandle)
		: base(innerHandle)
	{
	}

	public ulong AddNotifyDisconnected(AddNotifyDisconnectedOptions options, object clientData, OnDisconnectedCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddNotifyDisconnectedOptionsInternal, AddNotifyDisconnectedOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnDisconnectedCallbackInternal onDisconnectedCallbackInternal = OnDisconnectedCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onDisconnectedCallbackInternal);
		ulong num = Bindings.EOS_RTC_AddNotifyDisconnected(base.InnerHandle, target, clientDataAddress, onDisconnectedCallbackInternal);
		Helper.TryMarshalDispose(ref target);
		Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
		return num;
	}

	public ulong AddNotifyParticipantStatusChanged(AddNotifyParticipantStatusChangedOptions options, object clientData, OnParticipantStatusChangedCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddNotifyParticipantStatusChangedOptionsInternal, AddNotifyParticipantStatusChangedOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnParticipantStatusChangedCallbackInternal onParticipantStatusChangedCallbackInternal = OnParticipantStatusChangedCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onParticipantStatusChangedCallbackInternal);
		ulong num = Bindings.EOS_RTC_AddNotifyParticipantStatusChanged(base.InnerHandle, target, clientDataAddress, onParticipantStatusChangedCallbackInternal);
		Helper.TryMarshalDispose(ref target);
		Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
		return num;
	}

	public void BlockParticipant(BlockParticipantOptions options, object clientData, OnBlockParticipantCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<BlockParticipantOptionsInternal, BlockParticipantOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnBlockParticipantCallbackInternal onBlockParticipantCallbackInternal = OnBlockParticipantCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onBlockParticipantCallbackInternal);
		Bindings.EOS_RTC_BlockParticipant(base.InnerHandle, target, clientDataAddress, onBlockParticipantCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	public RTCAudioInterface GetAudioInterface()
	{
		IntPtr source = Bindings.EOS_RTC_GetAudioInterface(base.InnerHandle);
		Helper.TryMarshalGet(source, out RTCAudioInterface target);
		return target;
	}

	public void JoinRoom(JoinRoomOptions options, object clientData, OnJoinRoomCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<JoinRoomOptionsInternal, JoinRoomOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnJoinRoomCallbackInternal onJoinRoomCallbackInternal = OnJoinRoomCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onJoinRoomCallbackInternal);
		Bindings.EOS_RTC_JoinRoom(base.InnerHandle, target, clientDataAddress, onJoinRoomCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	public void LeaveRoom(LeaveRoomOptions options, object clientData, OnLeaveRoomCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<LeaveRoomOptionsInternal, LeaveRoomOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnLeaveRoomCallbackInternal onLeaveRoomCallbackInternal = OnLeaveRoomCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onLeaveRoomCallbackInternal);
		Bindings.EOS_RTC_LeaveRoom(base.InnerHandle, target, clientDataAddress, onLeaveRoomCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	public void RemoveNotifyDisconnected(ulong notificationId)
	{
		Helper.TryRemoveCallbackByNotificationId(notificationId);
		Bindings.EOS_RTC_RemoveNotifyDisconnected(base.InnerHandle, notificationId);
	}

	public void RemoveNotifyParticipantStatusChanged(ulong notificationId)
	{
		Helper.TryRemoveCallbackByNotificationId(notificationId);
		Bindings.EOS_RTC_RemoveNotifyParticipantStatusChanged(base.InnerHandle, notificationId);
	}

	public Result SetRoomSetting(SetRoomSettingOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<SetRoomSettingOptionsInternal, SetRoomSettingOptions>(ref target, options);
		Result result = Bindings.EOS_RTC_SetRoomSetting(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public Result SetSetting(SetSettingOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<SetSettingOptionsInternal, SetSettingOptions>(ref target, options);
		Result result = Bindings.EOS_RTC_SetSetting(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	[MonoPInvokeCallback(typeof(OnBlockParticipantCallbackInternal))]
	internal static void OnBlockParticipantCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnBlockParticipantCallback, BlockParticipantCallbackInfoInternal, BlockParticipantCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnDisconnectedCallbackInternal))]
	internal static void OnDisconnectedCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnDisconnectedCallback, DisconnectedCallbackInfoInternal, DisconnectedCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnJoinRoomCallbackInternal))]
	internal static void OnJoinRoomCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnJoinRoomCallback, JoinRoomCallbackInfoInternal, JoinRoomCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnLeaveRoomCallbackInternal))]
	internal static void OnLeaveRoomCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnLeaveRoomCallback, LeaveRoomCallbackInfoInternal, LeaveRoomCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnParticipantStatusChangedCallbackInternal))]
	internal static void OnParticipantStatusChangedCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnParticipantStatusChangedCallback, ParticipantStatusChangedCallbackInfoInternal, ParticipantStatusChangedCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}
}
