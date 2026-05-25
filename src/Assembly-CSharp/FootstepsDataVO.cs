using TMG.Core;
using UnityEngine;

public class FootstepsDataVO : TMGAbstractDisposable
{
	public FootstepTypes Type;

	public AudioClip[] Clips;

	public string ID;

	public static FootstepsDataVO Create(FootstepTypes type, AudioClip[] clips)
	{
		FootstepsDataVO footstepsDataVO = new FootstepsDataVO();
		footstepsDataVO.Type = type;
		footstepsDataVO.Clips = clips;
		return footstepsDataVO;
	}

	public static FootstepsDataVO Create(FootstepTypes type, string id)
	{
		FootstepsDataVO footstepsDataVO = new FootstepsDataVO();
		footstepsDataVO.Type = type;
		footstepsDataVO.ID = id;
		return footstepsDataVO;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
