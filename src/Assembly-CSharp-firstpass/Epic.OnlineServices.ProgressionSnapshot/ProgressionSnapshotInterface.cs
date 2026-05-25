using System;

namespace Epic.OnlineServices.ProgressionSnapshot;

public sealed class ProgressionSnapshotInterface : Handle
{
	public const int AddprogressionApiLatest = 1;

	public const int BeginsnapshotApiLatest = 1;

	public const int DeletesnapshotApiLatest = 1;

	public const int EndsnapshotApiLatest = 1;

	public const int InvalidProgressionsnapshotid = 0;

	public const int SubmitsnapshotApiLatest = 1;

	public ProgressionSnapshotInterface()
	{
	}

	public ProgressionSnapshotInterface(IntPtr innerHandle)
		: base(innerHandle)
	{
	}

	public Result AddProgression(AddProgressionOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<AddProgressionOptionsInternal, AddProgressionOptions>(ref target, options);
		Result result = Bindings.EOS_ProgressionSnapshot_AddProgression(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public Result BeginSnapshot(BeginSnapshotOptions options, out uint outSnapshotId)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<BeginSnapshotOptionsInternal, BeginSnapshotOptions>(ref target, options);
		outSnapshotId = Helper.GetDefault<uint>();
		Result result = Bindings.EOS_ProgressionSnapshot_BeginSnapshot(base.InnerHandle, target, ref outSnapshotId);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public void DeleteSnapshot(DeleteSnapshotOptions options, object clientData, OnDeleteSnapshotCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<DeleteSnapshotOptionsInternal, DeleteSnapshotOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnDeleteSnapshotCallbackInternal onDeleteSnapshotCallbackInternal = OnDeleteSnapshotCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onDeleteSnapshotCallbackInternal);
		Bindings.EOS_ProgressionSnapshot_DeleteSnapshot(base.InnerHandle, target, clientDataAddress, onDeleteSnapshotCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	public Result EndSnapshot(EndSnapshotOptions options)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<EndSnapshotOptionsInternal, EndSnapshotOptions>(ref target, options);
		Result result = Bindings.EOS_ProgressionSnapshot_EndSnapshot(base.InnerHandle, target);
		Helper.TryMarshalDispose(ref target);
		return result;
	}

	public void SubmitSnapshot(SubmitSnapshotOptions options, object clientData, OnSubmitSnapshotCallback completionDelegate)
	{
		IntPtr target = IntPtr.Zero;
		Helper.TryMarshalSet<SubmitSnapshotOptionsInternal, SubmitSnapshotOptions>(ref target, options);
		IntPtr clientDataAddress = IntPtr.Zero;
		OnSubmitSnapshotCallbackInternal onSubmitSnapshotCallbackInternal = OnSubmitSnapshotCallbackInternalImplementation;
		Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onSubmitSnapshotCallbackInternal);
		Bindings.EOS_ProgressionSnapshot_SubmitSnapshot(base.InnerHandle, target, clientDataAddress, onSubmitSnapshotCallbackInternal);
		Helper.TryMarshalDispose(ref target);
	}

	[MonoPInvokeCallback(typeof(OnDeleteSnapshotCallbackInternal))]
	internal static void OnDeleteSnapshotCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnDeleteSnapshotCallback, DeleteSnapshotCallbackInfoInternal, DeleteSnapshotCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}

	[MonoPInvokeCallback(typeof(OnSubmitSnapshotCallbackInternal))]
	internal static void OnSubmitSnapshotCallbackInternalImplementation(IntPtr data)
	{
		if (Helper.TryGetAndRemoveCallback<OnSubmitSnapshotCallback, SubmitSnapshotCallbackInfoInternal, SubmitSnapshotCallbackInfo>(data, out var callback, out var callbackInfo))
		{
			callback(callbackInfo);
		}
	}
}
