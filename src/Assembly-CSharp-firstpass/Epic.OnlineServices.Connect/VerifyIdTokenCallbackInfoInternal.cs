using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct VerifyIdTokenCallbackInfoInternal : ICallbackInfoInternal
{
	private Result m_ResultCode;

	private IntPtr m_ClientData;

	private IntPtr m_ProductUserId;

	private int m_IsAccountInfoPresent;

	private ExternalAccountType m_AccountIdType;

	private IntPtr m_AccountId;

	private IntPtr m_Platform;

	private IntPtr m_DeviceType;

	private IntPtr m_ClientId;

	private IntPtr m_ProductId;

	private IntPtr m_SandboxId;

	private IntPtr m_DeploymentId;

	public Result ResultCode => m_ResultCode;

	public object ClientData
	{
		get
		{
			Helper.TryMarshalGet(m_ClientData, out object target);
			return target;
		}
	}

	public IntPtr ClientDataAddress => m_ClientData;

	public ProductUserId ProductUserId
	{
		get
		{
			Helper.TryMarshalGet(m_ProductUserId, out ProductUserId target);
			return target;
		}
	}

	public bool IsAccountInfoPresent
	{
		get
		{
			Helper.TryMarshalGet(m_IsAccountInfoPresent, out var target);
			return target;
		}
	}

	public ExternalAccountType AccountIdType => m_AccountIdType;

	public string AccountId
	{
		get
		{
			Helper.TryMarshalGet(m_AccountId, out string target);
			return target;
		}
	}

	public string Platform
	{
		get
		{
			Helper.TryMarshalGet(m_Platform, out string target);
			return target;
		}
	}

	public string DeviceType
	{
		get
		{
			Helper.TryMarshalGet(m_DeviceType, out string target);
			return target;
		}
	}

	public string ClientId
	{
		get
		{
			Helper.TryMarshalGet(m_ClientId, out string target);
			return target;
		}
	}

	public string ProductId
	{
		get
		{
			Helper.TryMarshalGet(m_ProductId, out string target);
			return target;
		}
	}

	public string SandboxId
	{
		get
		{
			Helper.TryMarshalGet(m_SandboxId, out string target);
			return target;
		}
	}

	public string DeploymentId
	{
		get
		{
			Helper.TryMarshalGet(m_DeploymentId, out string target);
			return target;
		}
	}
}
