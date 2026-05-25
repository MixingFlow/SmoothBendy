using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct VerifyIdTokenCallbackInfoInternal : ICallbackInfoInternal
{
	private Result m_ResultCode;

	private IntPtr m_ClientData;

	private IntPtr m_ApplicationId;

	private IntPtr m_ClientId;

	private IntPtr m_ProductId;

	private IntPtr m_SandboxId;

	private IntPtr m_DeploymentId;

	private IntPtr m_DisplayName;

	private int m_IsExternalAccountInfoPresent;

	private ExternalAccountType m_ExternalAccountIdType;

	private IntPtr m_ExternalAccountId;

	private IntPtr m_ExternalAccountDisplayName;

	private IntPtr m_Platform;

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

	public string ApplicationId
	{
		get
		{
			Helper.TryMarshalGet(m_ApplicationId, out string target);
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

	public string DisplayName
	{
		get
		{
			Helper.TryMarshalGet(m_DisplayName, out string target);
			return target;
		}
	}

	public bool IsExternalAccountInfoPresent
	{
		get
		{
			Helper.TryMarshalGet(m_IsExternalAccountInfoPresent, out var target);
			return target;
		}
	}

	public ExternalAccountType ExternalAccountIdType => m_ExternalAccountIdType;

	public string ExternalAccountId
	{
		get
		{
			Helper.TryMarshalGet(m_ExternalAccountId, out string target);
			return target;
		}
	}

	public string ExternalAccountDisplayName
	{
		get
		{
			Helper.TryMarshalGet(m_ExternalAccountDisplayName, out string target);
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
}
