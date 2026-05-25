using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class DeathController : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private EventTrigger m_ExitTrigger;

	private PlayerSpawnNode[] m_SpawnPoints;

	private Vector3 m_DeathPosition;

	public event EventHandler OnDeath;

	public event EventHandler OnSpawned;

	public override void InitOnComplete()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		base.transform.position = new Vector3(0f, 1500f, 0f);
		base.transform.eulerAngles = Vector3.zero;
		m_SpawnPoints = Resources.FindObjectsOfTypeAll<PlayerSpawnNode>();
	}

	public void Activate()
	{
		GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		this.OnDeath.Send(this);
		m_DeathPosition = GameManager.Instance.Player.transform.position;
		PlayerController player = GameManager.Instance.Player;
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			if (player.isSeeingToolActive)
			{
				player.ForceHideSeeingTool();
			}
			player.AllowSeeingTool(active: false);
			player.EnableSeeingTool(active: false);
			if (Object.op_Implicit((Object)(object)player.WeaponGameObject))
			{
				player.WeaponGameObject.SetActive(false);
			}
			if (Object.op_Implicit((Object)(object)player.InactiveWeapon))
			{
				player.InactiveWeapon.SetActive(false);
			}
			player.SetCombatStatus(CombatStatus.Hiding);
			player.GoToAndLookAt(m_StartLocation);
			player.SetJump(active: false);
			player.SetLock(active: true);
		});
		num += 2f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			player.SetLock(active: false);
			player.GoToAndLookAt(m_StartLocation);
			player.SetSlowed(active: true);
			player.SetBackMovement(active: false);
			player.LockRotation(10f, 10f);
			GameManager.Instance.GameCamera.VisionEffect.BeginEffect(isDeath: true);
			GameManager.Instance.HideCrosshair();
			GameManager.Instance.HideScreenBlocker();
		});
		m_ExitTrigger.ResetTrigger();
		m_ExitTrigger.OnEnter += HandleExitTriggerOnEnter;
		m_ExitTrigger.SetActive(active: true);
	}

	private void HandleExitTriggerOnEnter(object sender, EventArgs e)
	{
		m_ExitTrigger.OnEnter -= HandleExitTriggerOnEnter;
		GameManager.Instance.GameCamera.VisionEffect.OnStop += HandleVisionEffectOnStop;
		GameManager.Instance.GameCamera.VisionEffect.EndEffect(isDeath: true);
	}

	private void HandleVisionEffectOnStop(object sender, EventArgs e)
	{
		GameManager.Instance.GameCamera.VisionEffect.OnStop -= HandleVisionEffectOnStop;
		PlayerController player = GameManager.Instance.Player;
		if (Object.op_Implicit((Object)(object)player.WeaponGameObject))
		{
			player.WeaponGameObject.SetActive(true);
		}
		PlayerSpawnNode closestSpawnPoint = GetClosestSpawnPoint();
		if (Object.op_Implicit((Object)(object)closestSpawnPoint))
		{
			player.GoToAndLookAt(closestSpawnPoint.transform);
			player.PlayRespawnEffects();
			player.SetBackMovement(active: true);
			player.SetSlowed(active: false);
			player.UnlockRotation();
			player.SetCombatStatus(CombatStatus.Idle);
			player.SetJump(active: true);
			GameManager.Instance.ShowCrosshair();
			if (GameManager.Instance.GameData.CurrentSaveFile.IsNewGamePlus || GameManager.Instance.CurrentChapter.Chapter == Chapters.FIVE)
			{
				player.AllowSeeingTool(active: true);
				player.EnableSeeingTool(active: true);
			}
			this.OnSpawned.Send(this);
		}
		else
		{
			Debug.Log((object)"[DeathController] - There's no spawn points enabled!");
		}
	}

	private PlayerSpawnNode GetClosestSpawnPoint()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		PlayerSpawnNode result = null;
		float num = float.PositiveInfinity;
		if (m_SpawnPoints != null)
		{
			PlayerSpawnNode[] spawnPoints = m_SpawnPoints;
			foreach (PlayerSpawnNode playerSpawnNode in spawnPoints)
			{
				if (playerSpawnNode.gameObject.activeSelf)
				{
					Vector3 position = playerSpawnNode.transform.position;
					float num2 = Vector3.Distance(m_DeathPosition, position) + Mathf.Abs(m_DeathPosition.y - position.y);
					if (num2 < num)
					{
						num = num2;
						result = playerSpawnNode;
					}
				}
			}
		}
		return result;
	}

	protected override void OnDisposed()
	{
		this.OnSpawned = null;
		m_SpawnPoints = null;
		base.OnDisposed();
	}
}
