namespace Epic.OnlineServices.P2P;

public class OnPeerConnectionEstablishedInfo : ICallbackInfo, ISettable
{
	public object ClientData { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public ProductUserId RemoteUserId { get; private set; }

	public SocketId SocketId { get; private set; }

	public ConnectionEstablishedType ConnectionType { get; private set; }

	public Result? GetResultCode()
	{
		return null;
	}

	internal void Set(OnPeerConnectionEstablishedInfoInternal? other)
	{
		if (other.HasValue)
		{
			ClientData = other.Value.ClientData;
			LocalUserId = other.Value.LocalUserId;
			RemoteUserId = other.Value.RemoteUserId;
			SocketId = other.Value.SocketId;
			ConnectionType = other.Value.ConnectionType;
		}
	}

	public void Set(object other)
	{
		Set(other as OnPeerConnectionEstablishedInfoInternal?);
	}
}
