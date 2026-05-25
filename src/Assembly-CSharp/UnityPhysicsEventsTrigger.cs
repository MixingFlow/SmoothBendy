using UnityEngine;
using UnityEngine.Events;

public class UnityPhysicsEventsTrigger : MonoBehaviour
{
	public UnityEventCollider OnTriggerEnterEvent;

	public UnityEventCollider OnTriggerExitEvent;

	private void OnTriggerEnter(Collider collider)
	{
		((UnityEvent<Collider>)OnTriggerEnterEvent).Invoke(collider);
	}

	private void OnTriggerExit(Collider collider)
	{
		((UnityEvent<Collider>)OnTriggerExitEvent).Invoke(collider);
	}
}
