using DG.Tweening;
using TMG.Controls;
using UnityEngine;

public class Minigame_BallWheel : WinnableMiniGameBaseController
{
	[Header("Ball Toss Props")]
	[SerializeField]
	private GameObject m_BallObject;

	[SerializeField]
	private Transform Wheel1;

	[SerializeField]
	private Transform Wheel2;

	[SerializeField]
	private Transform Wheel3;

	[SerializeField]
	private float m_CurrentScore;

	[SerializeField]
	private float m_WinScore;

	private float m_TimeSinceStart;

	private bool m_Reloading;

	public override void Init()
	{
		base.Init();
	}

	public override void HandleGameStartupSequence()
	{
		base.HandleGameStartupSequence();
		HandleOnPrepHeldObject();
	}

	public override void BeginGameLoops()
	{
		base.BeginGameLoops();
	}

	public override void Update()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		Wheel1.Rotate(0f, 1f, 0f);
		Wheel2.Rotate(0f, -0.4f, 0f);
		Wheel3.Rotate(0f, 0.8f, 0f);
		if (base.CurrentState == MiniGameState.ACTIVE && !m_Reloading)
		{
			m_TimeSinceStart += Time.deltaTime;
			if (PlayerInput.Attack())
			{
				GameObject val = Object.Instantiate<GameObject>(m_BallObject, GameManager.Instance.GameCamera.transform.position, GameManager.Instance.GameCamera.transform.rotation);
				val.SetActive(true);
				m_Reloading = true;
				ThrowSequence();
			}
		}
	}

	public override void AddScore(int Ammount)
	{
		base.AddScore(Ammount);
		m_CurrentScore += Ammount;
		TestWinGame();
	}

	private void TestWinGame()
	{
		if (!(m_CurrentScore >= m_WinScore))
		{
		}
	}

	public void ThrowSequence()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		if (base.CurrentState == MiniGameState.ACTIVE)
		{
			Sequence val = DOTween.Sequence();
			float num = 0f;
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(base.HeldObject, new Vector3(0f, -5f, 5f), 0.2f, false), (Ease)6));
			num += 0.2f;
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(base.HeldObject, new Vector3(0f, 0f, 0f), 0.2f, false), (Ease)6));
			num += 0.2f;
			TweenSettingsExtensions.InsertCallback(val, num + 0.1f, (TweenCallback)delegate
			{
				m_Reloading = false;
			});
		}
	}
}
