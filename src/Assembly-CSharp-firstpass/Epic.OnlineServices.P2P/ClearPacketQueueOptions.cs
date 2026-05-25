namespace Epic.OnlineServices.P2P;

public class ClearPacketQueueOptions
{
	public ProductUserId LocalUserId { get; set; }

	public ProductUserId RemoteUserId { get; set; }

	public SocketId SocketId { get; set; }
}
