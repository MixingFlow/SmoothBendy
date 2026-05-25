using System;

namespace Epic.OnlineServices.RTCAudio;

public sealed class RTCAudioInterface : Handle
{
	public const int AddnotifyaudiobeforerenderApiLatest = 1;

	public const int AddnotifyaudiobeforesendApiLatest = 1;

	public const int AddnotifyaudiodeviceschangedApiLatest = 1;

	public const int AddnotifyaudioinputstateApiLatest = 1;

	public const int AddnotifyaudiooutputstateApiLatest = 1;

	public const int AddnotifyparticipantupdatedApiLatest = 1;

	public const int AudiobufferApiLatest = 1;

	public const int AudioinputdeviceinfoApiLatest = 1;

	public const int AudiooutputdeviceinfoApiLatest = 1;

	public const int GetaudioinputdevicebyindexApiLatest = 1;

	public const int GetaudioinputdevicescountApiLatest = 1;

	public const int GetaudiooutputdevicebyindexApiLatest = 1;

	public const int GetaudiooutputdevicescountApiLatest = 1;

	public const int RegisterplatformaudiouserApiLatest = 1;

	public const int SendaudioApiLatest = 1;

	public const int SetaudioinputsettingsApiLatest = 1;

	public const int SetaudiooutputsettingsApiLatest = 1;

	public const int UnregisterplatformaudiouserApiLatest = 1;

	public const int UpdatereceivingApiLatest = 1;

	public const int UpdatesendingApiLatest = 1;

	public RTCAudioInterface()
	{
	}

	public RTCAudioInterface(IntPtr innerHandle)
		: base(innerHandle)
	{
	}

	public ulong AddNotifyAudioBeforeRender(AddNotifyAudioBeforeRenderOptions options, object clientData, OnAudioBeforeRenderCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddNotifyAudioBeforeRenderOptionsInternal, AddNotifyAudioBeforeRenderOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnAudioBeforeRenderCallbackInternal onAudioBeforeRenderCallbackInternal = OnAudioBeforeRenderCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onAudioBeforeRenderCallbackInternal);
		ulong num = Bindings.EOS_RTCAudio_AddNotifyAudioBeforeRender(base.InnerHandle, target, clientDataAddress, onAudioBeforeRenderCallbackInternal);
		Helper.TryMarshalDispose(ref target);
		Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
		return num;
	}

	public ulong AddNotifyAudioBeforeSend(AddNotifyAudioBeforeSendOptions options, object clientData, OnAudioBeforeSendCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddNotifyAudioBeforeSendOptionsInternal, AddNotifyAudioBeforeSendOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnAudioBeforeSendCallbackInternal onAudioBeforeSendCallbackInternal = OnAudioBeforeSendCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onAudioBeforeSendCallbackInternal);
		ulong num = Bindings.EOS_RTCAudio_AddNotifyAudioBeforeSend(base.InnerHandle, target, clientDataAddress, onAudioBeforeSendCallbackInternal);
		Helper.TryMarshalDispose(ref target);
		Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
		return num;
	}

	public ulong AddNotifyAudioDevicesChanged(AddNotifyAudioDevicesChangedOptions options, object clientData, OnAudioDevicesChangedCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddNotifyAudioDevicesChangedOptionsInternal, AddNotifyAudioDevicesChangedOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnAudioDevicesChangedCallbackInternal onAudioDevicesChangedCallbackInternal = OnAudioDevicesChangedCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onAudioDevicesChangedCallbackInternal);
		ulong num = Bindings.EOS_RTCAudio_AddNotifyAudioDevicesChanged(base.InnerHandle, target, clientDataAddress, onAudioDevicesChangedCallbackInternal);
		Helper.TryMarshalDispose(ref target);
		Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
		return num;
	}

	public ulong AddNotifyAudioInputState(AddNotifyAudioInputStateOptions options, object clientData, OnAudioInputStateCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddNotifyAudioInputStateOptionsInternal, AddNotifyAudioInputStateOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnAudioInputStateCallbackInternal onAudioInputStateCallbackInternal = OnAudioInputStateCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onAudioInputStateCallbackInternal);
		ulong num = Bindings.EOS_RTCAudio_AddNotifyAudioInputState(base.InnerHandle, target, clientDataAddress, onAudioInputStateCallbackInternal);
		Helper.TryMarshalDispose(ref target);
		Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
		return num;
	}

	public ulong AddNotifyAudioOutputState(AddNotifyAudioOutputStateOptions options, object clientData, OnAudioOutputStateCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddNotifyAudioOutputStateOptionsInternal, AddNotifyAudioOutputStateOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnAudioOutputStateCallbackInternal onAudioOutputStateCallbackInternal = OnAudioOutputStateCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onAudioOutputStateCallbackInternal);
		ulong num = Bindings.EOS_RTCAudio_AddNotifyAudioOutputState(base.InnerHandle, target, clientDataAddress, onAudioOutputStateCallbackInternal);
		Helper.TryMarshalDispose(ref target);
		Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
		return num;
	}

	public ulong AddNotifyParticipantUpdated(AddNotifyParticipantUpdatedOptions options, object clientData, OnParticipantUpdatedCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddNotifyParticipantUpdatedOptionsInternal, AddNotifyParticipantUpdatedOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnParticipantUpdatedCallbackInternal onParticipantUpdatedCallbackInternal = OnParticipantUpdatedCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onParticipantUpdatedCallbackInternal);
		ulong num = Bindings.EOS_RTCAudio_AddNotifyParticipantUpdated(base.InnerHandle, target, clientDataAddress, onParticipantUpdatedCallbackInternal);
		Helper.TryMarshalDispose(ref target);
		Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
		return num;
	}

	public AudioInputDeviceInfo GetAudioInputDeviceByIndex(GetAudioInputDeviceByIndexOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<GetAudioInputDeviceByIndexOptionsInternal, GetAudioInputDeviceByIndexOptions>(ref target, options);
		IntPtr source = Bindings.EOS_RTCAudio_GetAudioInputDeviceByIndex(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		Helper.TryMarshalGet<AudioInputDeviceInfoInternal, AudioInputDeviceInfo>(source, out var target2);
		return target2;
	}

	public uint GetAudioInputDevicesCount(GetAudioInputDevicesCountOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<GetAudioInputDevicesCountOptionsInternal, GetAudioInputDevicesCountOptions>(ref target, options);
		uint result = Bindings.EOS_RTCAudio_GetAudioInputDevicesCount(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public AudioOutputDeviceInfo GetAudioOutputDeviceByIndex(GetAudioOutputDeviceByIndexOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<GetAudioOutputDeviceByIndexOptionsInternal, GetAudioOutputDeviceByIndexOptions>(ref target, options);
		IntPtr source = Bindings.EOS_RTCAudio_GetAudioOutputDeviceByIndex(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		Helper.TryMarshalGet<AudioOutputDeviceInfoInternal, AudioOutputDeviceInfo>(source, out var target2);
		return target2;
	}

	public uint GetAudioOutputDevicesCount(GetAudioOutputDevicesCountOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<GetAudioOutputDevicesCountOptionsInternal, GetAudioOutputDevicesCountOptions>(ref target, options);
		uint result = Bindings.EOS_RTCAudio_GetAudioOutputDevicesCount(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public Result RegisterPlatformAudioUser(RegisterPlatformAudioUserOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<RegisterPlatformAudioUserOptionsInternal, RegisterPlatformAudioUserOptions>(ref target, options);
		Result result = Bindings.EOS_RTCAudio_RegisterPlatformAudioUser(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public void RemoveNotifyAudioBeforeRender(ulong notificationId)
	{
		Helper.TryRemoveCallbackByNotificationId(notificationId);
		Bindings.EOS_RTCAudio_RemoveNotifyAudioBeforeRender(base.InnerHandle, notificationId);
	}

	public void RemoveNotifyAudioBeforeSend(ulong notificationId)
	{
		Helper.TryRemoveCallbackByNotificationId(notificationId);
		Bindings.EOS_RTCAudio_RemoveNotifyAudioBeforeSend(base.InnerHandle, notificationId);
	}

	public void RemoveNotifyAudioDevicesChanged(ulong notificationId)
	{
		Helper.TryRemoveCallbackByNotificationId(notificationId);
		Bindings.EOS_RTCAudio_RemoveNotifyAudioDevicesChanged(base.InnerHandle, notificationId);
	}

	public void RemoveNotifyAudioInputState(ulong notificationId)
	{
		Helper.TryRemoveCallbackByNotificationId(notificationId);
		Bindings.EOS_RTCAudio_RemoveNotifyAudioInputState(base.InnerHandle, notificationId);
	}

	public void RemoveNotifyAudioOutputState(ulong notificationId)
	{
		Helper.TryRemoveCallbackByNotificationId(notificationId);
		Bindings.EOS_RTCAudio_RemoveNotifyAudioOutputState(base.InnerHandle, notificationId);
	}

	public void RemoveNotifyParticipantUpdated(ulong notificationId)
	{
		Helper.TryRemoveCallbackByNotificationId(notificationId);
		Bindings.EOS_RTCAudio_RemoveNotifyParticipantUpdated(base.InnerHandle, notificationId);
	}

	public Result SendAudio(SendAudioOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<SendAudioOptionsInternal, SendAudioOptions>(ref target, options);
		Result result = Bindings.EOS_RTCAudio_SendAudio(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public Result SetAudioInputSettings(SetAudioInputSettingsOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<SetAudioInputSettingsOptionsInternal, SetAudioInputSettingsOptions>(ref target, options);
		Result result = Bindings.EOS_RTCAudio_SetAudioInputSettings(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public Result SetAudioOutputSettings(SetAudioOutputSettingsOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<SetAudioOutputSettingsOptionsInternal, SetAudioOutputSettingsOptions>(ref target, options);
		Result result = Bindings.EOS_RTCAudio_SetAudioOutputSettings(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public Result UnregisterPlatformAudioUser(UnregisterPlatformAudioUserOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<UnregisterPlatformAudioUserOptionsInternal, UnregisterPlatformAudioUserOptions>(ref target, options);
		Result result = Bindings.EOS_RTCAudio_UnregisterPlatformAudioUser(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public void UpdateReceiving(UpdateReceivingOptions options, object clientData, OnUpdateReceivingCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<UpdateReceivingOptionsInternal, UpdateReceivingOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnUpdateReceivingCallbackInternal onUpdateReceivingCallbackInternal = OnUpdateReceivingCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onUpdateReceivingCallbackInternal);
		Bindings.EOS_RTCAudio_UpdateReceiving(base.InnerHandle, target, clientDataAddress, onUpdateReceivingCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	public void UpdateSending(UpdateSendingOptions options, object clientData, OnUpdateSendingCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<UpdateSendingOptionsInternal, UpdateSendingOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnUpdateSendingCallbackInternal onUpdateSendingCallbackInternal = OnUpdateSendingCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onUpdateSendingCallbackInternal);
		Bindings.EOS_RTCAudio_UpdateSending(base.InnerHandle, target, clientDataAddress, onUpdateSendingCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	[MonoPInvokeCallback(typeof(OnAudioBeforeRenderCallbackInternal))]
	internal static void OnAudioBeforeRenderCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnAudioBeforeRenderCallback, AudioBeforeRenderCallbackInfoInternal, AudioBeforeRenderCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnAudioBeforeSendCallbackInternal))]
	internal static void OnAudioBeforeSendCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnAudioBeforeSendCallback, AudioBeforeSendCallbackInfoInternal, AudioBeforeSendCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnAudioDevicesChangedCallbackInternal))]
	internal static void OnAudioDevicesChangedCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnAudioDevicesChangedCallback, AudioDevicesChangedCallbackInfoInternal, AudioDevicesChangedCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnAudioInputStateCallbackInternal))]
	internal static void OnAudioInputStateCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnAudioInputStateCallback, AudioInputStateCallbackInfoInternal, AudioInputStateCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnAudioOutputStateCallbackInternal))]
	internal static void OnAudioOutputStateCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnAudioOutputStateCallback, AudioOutputStateCallbackInfoInternal, AudioOutputStateCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnParticipantUpdatedCallbackInternal))]
	internal static void OnParticipantUpdatedCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnParticipantUpdatedCallback, ParticipantUpdatedCallbackInfoInternal, ParticipantUpdatedCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnUpdateReceivingCallbackInternal))]
	internal static void OnUpdateReceivingCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnUpdateReceivingCallback, UpdateReceivingCallbackInfoInternal, UpdateReceivingCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnUpdateSendingCallbackInternal))]
	internal static void OnUpdateSendingCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnUpdateSendingCallback, UpdateSendingCallbackInfoInternal, UpdateSendingCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}
}
