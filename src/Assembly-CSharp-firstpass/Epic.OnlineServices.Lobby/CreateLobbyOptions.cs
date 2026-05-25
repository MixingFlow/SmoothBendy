namespace Epic.OnlineServices.Lobby;

public class CreateLobbyOptions
{
	public ProductUserId LocalUserId { get; set; }

	public uint MaxLobbyMembers { get; set; }

	public LobbyPermissionLevel PermissionLevel { get; set; }

	public bool PresenceEnabled { get; set; }

	public bool AllowInvites { get; set; }

	public string BucketId { get; set; }

	public bool DisableHostMigration { get; set; }

	public bool EnableRTCRoom { get; set; }

	public LocalRTCOptions LocalRTCOptions { get; set; }

	public string LobbyId { get; set; }
}
