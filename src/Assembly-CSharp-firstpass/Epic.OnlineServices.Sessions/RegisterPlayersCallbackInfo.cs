namespace Epic.OnlineServices.Sessions;

public class RegisterPlayersCallbackInfo : ICallbackInfo, ISettable
{
	public Result ResultCode { get; private set; }

	public object ClientData { get; private set; }

	public ProductUserId[] RegisteredPlayers { get; private set; }

	public ProductUserId[] SanctionedPlayers { get; private set; }

	public Result? GetResultCode()
	{
		return ResultCode;
	}

	internal void Set(RegisterPlayersCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ResultCode = other.Value.ResultCode;
			ClientData = other.Value.ClientData;
			RegisteredPlayers = other.Value.RegisteredPlayers;
			SanctionedPlayers = other.Value.SanctionedPlayers;
		}
	}

	public void Set(object other)
	{
		Set(other as RegisterPlayersCallbackInfoInternal?);
	}
}
