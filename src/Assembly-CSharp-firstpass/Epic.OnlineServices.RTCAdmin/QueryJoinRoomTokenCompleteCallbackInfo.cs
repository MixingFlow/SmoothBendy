namespace Epic.OnlineServices.RTCAdmin;

public class QueryJoinRoomTokenCompleteCallbackInfo : ICallbackInfo, ISettable
{
	public Result ResultCode { get; private set; }

	public object ClientData { get; private set; }

	public string RoomName { get; private set; }

	public string ClientBaseUrl { get; private set; }

	public uint QueryId { get; private set; }

	public uint TokenCount { get; private set; }

	public Result? GetResultCode()
	{
		return ResultCode;
	}

	internal void Set(QueryJoinRoomTokenCompleteCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ResultCode = other.Value.ResultCode;
			ClientData = other.Value.ClientData;
			RoomName = other.Value.RoomName;
			ClientBaseUrl = other.Value.ClientBaseUrl;
			QueryId = other.Value.QueryId;
			TokenCount = other.Value.TokenCount;
		}
	}

	public void Set(object other)
	{
		Set(other as QueryJoinRoomTokenCompleteCallbackInfoInternal?);
	}
}
