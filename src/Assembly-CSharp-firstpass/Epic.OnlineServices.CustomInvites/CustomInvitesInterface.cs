using System;

namespace Epic.OnlineServices.CustomInvites;

public sealed class CustomInvitesInterface : Handle
{
	public const int AddnotifycustominviteacceptedApiLatest = 1;

	public const int AddnotifycustominvitereceivedApiLatest = 1;

	public const int FinalizeinviteApiLatest = 1;

	public const int MaxPayloadLength = 500;

	public const int SendcustominviteApiLatest = 1;

	public const int SetcustominviteApiLatest = 1;

	public CustomInvitesInterface()
	{
	}

	public CustomInvitesInterface(IntPtr innerHandle)
		: base(innerHandle)
	{
	}

	public ulong AddNotifyCustomInviteAccepted(AddNotifyCustomInviteAcceptedOptions options, object clientData, OnCustomInviteAcceptedCallback notificationFn)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddNotifyCustomInviteAcceptedOptionsInternal, AddNotifyCustomInviteAcceptedOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnCustomInviteAcceptedCallbackInternal onCustomInviteAcceptedCallbackInternal = OnCustomInviteAcceptedCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onCustomInviteAcceptedCallbackInternal);
		ulong num = Bindings.EOS_CustomInvites_AddNotifyCustomInviteAccepted(base.InnerHandle, target, clientDataAddress, onCustomInviteAcceptedCallbackInternal);
		Helper.TryMarshalDispose(ref target);
		Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
		return num;
	}

	public ulong AddNotifyCustomInviteReceived(AddNotifyCustomInviteReceivedOptions options, object clientData, OnCustomInviteReceivedCallback notificationFn)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddNotifyCustomInviteReceivedOptionsInternal, AddNotifyCustomInviteReceivedOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnCustomInviteReceivedCallbackInternal onCustomInviteReceivedCallbackInternal = OnCustomInviteReceivedCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onCustomInviteReceivedCallbackInternal);
		ulong num = Bindings.EOS_CustomInvites_AddNotifyCustomInviteReceived(base.InnerHandle, target, clientDataAddress, onCustomInviteReceivedCallbackInternal);
		Helper.TryMarshalDispose(ref target);
		Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
		return num;
	}

	public Result FinalizeInvite(FinalizeInviteOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<FinalizeInviteOptionsInternal, FinalizeInviteOptions>(ref target, options);
		Result result = Bindings.EOS_CustomInvites_FinalizeInvite(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public void RemoveNotifyCustomInviteAccepted(ulong inId)
	{
		Helper.TryRemoveCallbackByNotificationId(inId);
		Bindings.EOS_CustomInvites_RemoveNotifyCustomInviteAccepted(base.InnerHandle, inId);
	}

	public void RemoveNotifyCustomInviteReceived(ulong inId)
	{
		Helper.TryRemoveCallbackByNotificationId(inId);
		Bindings.EOS_CustomInvites_RemoveNotifyCustomInviteReceived(base.InnerHandle, inId);
	}

	public void SendCustomInvite(SendCustomInviteOptions options, object clientData, OnSendCustomInviteCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<SendCustomInviteOptionsInternal, SendCustomInviteOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnSendCustomInviteCallbackInternal onSendCustomInviteCallbackInternal = OnSendCustomInviteCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onSendCustomInviteCallbackInternal);
		Bindings.EOS_CustomInvites_SendCustomInvite(base.InnerHandle, target, clientDataAddress, onSendCustomInviteCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	public Result SetCustomInvite(SetCustomInviteOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<SetCustomInviteOptionsInternal, SetCustomInviteOptions>(ref target, options);
		Result result = Bindings.EOS_CustomInvites_SetCustomInvite(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	[MonoPInvokeCallback(typeof(OnCustomInviteAcceptedCallbackInternal))]
	internal static void OnCustomInviteAcceptedCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnCustomInviteAcceptedCallback, OnCustomInviteAcceptedCallbackInfoInternal, OnCustomInviteAcceptedCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnCustomInviteReceivedCallbackInternal))]
	internal static void OnCustomInviteReceivedCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnCustomInviteReceivedCallback, OnCustomInviteReceivedCallbackInfoInternal, OnCustomInviteReceivedCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnSendCustomInviteCallbackInternal))]
	internal static void OnSendCustomInviteCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnSendCustomInviteCallback, SendCustomInviteCallbackInfoInternal, SendCustomInviteCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}
}
