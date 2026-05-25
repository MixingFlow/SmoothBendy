using TMG.Core;
using UnityEngine;

public class HittableObject : TMGMonoBehaviour, IHittable
{
	public override void Init()
	{
		base.Init();
	}

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (weaponInfo == null)
		{
			return;
		}
		string text = string.Empty;
		float num = 0f;
		if (weaponInfo.ImpactType == ImpactType.AXE)
		{
			text = "GamePlay/Decals/Axe_Hit_Effect";
		}
		else if (weaponInfo.ImpactType == ImpactType.BLUNT)
		{
			text = "GamePlay/Decals/Blunt_Hit_Effect";
			num = Random.Range(0f, 360f);
		}
		if (!string.IsNullOrEmpty(text))
		{
			GameObject fromPool = GameManager.Instance.PoolingManager.GetFromPool(text);
			fromPool.transform.localScale = Vector3.one;
			fromPool.transform.position = ((RaycastHit)(ref hit)).point;
			fromPool.transform.rotation = Quaternion.FromToRotation(Vector3.forward, ((RaycastHit)(ref hit)).normal);
			if (num > 0f)
			{
				foreach (Transform item in fromPool.transform)
				{
					Transform val = item;
					Vector3 localEulerAngles = ((Component)val).transform.localEulerAngles;
					localEulerAngles.z = num;
					((Component)val).transform.localEulerAngles = localEulerAngles;
				}
			}
			fromPool.transform.SetParent(((RaycastHit)(ref hit)).transform);
		}
		if (weaponInfo.Audio != null && weaponInfo.Audio.Count > 0)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(weaponInfo.Audio[Random.Range(0, weaponInfo.Audio.Count)], ((RaycastHit)(ref hit)).point);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
