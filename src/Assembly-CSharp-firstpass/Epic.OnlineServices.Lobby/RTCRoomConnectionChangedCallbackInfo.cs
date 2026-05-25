namespace Epic.OnlineServices.Lobby;

public class RTCRoomConnectionChangedCallbackInfo : ICallbackInfo, ISettable
{
	public object ClientData { get; private set; }

	public string LobbyId { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public bool IsConnected { get; private set; }

	public Result DisconnectReason { get; private set; }

	public Result? GetResultCode()
	{
		return null;
	}

	internal void Set(RTCRoomConnectionChangedCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ClientData = other.Value.ClientData;
			LobbyId = other.Value.LobbyId;
			LocalUserId = other.Value.LocalUserId;
			IsConnected = other.Value.IsConnected;
			DisconnectReason = other.Value.DisconnectReason;
		}
	}

	public void Set(object other)
	{
		Set(other as RTCRoomConnectionChangedCallbackInfoInternal?);
	}
}
