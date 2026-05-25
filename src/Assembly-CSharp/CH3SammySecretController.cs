using System;
using System.Collections.Generic;
using UnityEngine;

public class CH3SammySecretController : BaseController
{
	[SerializeField]
	private Transform m_SammyPosition;

	[SerializeField]
	private List<InteractableMusicalInstrument> m_Violin;

	[SerializeField]
	private InteractableMusicalInstrument m_Drum;

	[SerializeField]
	private InteractableMusicalInstrument m_Piano;

	[SerializeField]
	private List<InteractableMusicalInstrument> m_OtherInstruments;

	private List<InteractableMusicalInstrument> m_CancelInstruments = new List<InteractableMusicalInstrument>();

	private AudioClip m_SammySecretClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_SammySecretClip = GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/DIA/CH3/Sammy/DIA_CH3_SAMMY_SECRET_01");
		Activate();
	}

	public override void Activate()
	{
		for (int i = 0; i < m_OtherInstruments.Count; i++)
		{
			m_OtherInstruments[i].OnNotePlayed += HandleOtherInstrumentOnNotePlayed;
		}
	}

	private void HandleOtherInstrumentOnNotePlayed(object sender, EventArgs e)
	{
		ClearInstruments();
		m_Drum.OnNotePlayed += HandleDrumOnNotePlayed;
	}

	private void HandleDrumOnNotePlayed(object sender, EventArgs e)
	{
		m_Drum.OnNotePlayed -= HandleDrumOnNotePlayed;
		ClearOtherInstruments();
		m_CancelInstruments.Add(m_Drum);
		m_CancelInstruments.Add(m_Piano);
		for (int i = 0; i < m_CancelInstruments.Count; i++)
		{
			m_CancelInstruments[i].OnNotePlayed += HandleOtherInstrumentOnNotePlayed;
		}
		for (int j = 0; j < m_Violin.Count; j++)
		{
			m_Violin[j].OnNotePlayed += HandleViolinOnNotePlayed;
		}
	}

	private void HandleViolinOnNotePlayed(object sender, EventArgs e)
	{
		ClearOtherInstruments();
		m_CancelInstruments.Add(m_Drum);
		for (int i = 0; i < m_Violin.Count; i++)
		{
			m_Violin[i].OnNotePlayed -= HandleViolinOnNotePlayed;
			m_CancelInstruments.Add(m_Violin[i]);
		}
		for (int j = 0; j < m_CancelInstruments.Count; j++)
		{
			m_CancelInstruments[j].OnNotePlayed += HandleOtherInstrumentOnNotePlayed;
		}
		m_Piano.OnNotePlayed += HandlePianoOnNotePlayed;
	}

	private void HandlePianoOnNotePlayed(object sender, EventArgs e)
	{
		m_Piano.OnNotePlayed -= HandlePianoOnNotePlayed;
		ClearOtherInstruments();
		m_CancelInstruments.Add(m_Piano);
		for (int i = 0; i < m_Violin.Count; i++)
		{
			m_CancelInstruments.Add(m_Violin[i]);
		}
		for (int j = 0; j < m_CancelInstruments.Count; j++)
		{
			m_CancelInstruments[j].OnNotePlayed += HandleOtherInstrumentOnNotePlayed;
		}
		m_Drum.OnNotePlayed += HandleDrumTwoOnNotePlayed;
	}

	private void HandleDrumTwoOnNotePlayed(object sender, EventArgs e)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		m_Drum.OnNotePlayed -= HandleDrumTwoOnNotePlayed;
		GameManager.Instance.AudioManager.PlayAtPosition(m_SammySecretClip, m_SammyPosition.position);
		Dispose();
	}

	private void ClearOtherInstruments()
	{
		for (int i = 0; i < m_CancelInstruments.Count; i++)
		{
			m_CancelInstruments[i].OnNotePlayed -= HandleOtherInstrumentOnNotePlayed;
		}
		m_CancelInstruments.Clear();
	}

	private void ClearInstruments()
	{
		for (int i = 0; i < m_Violin.Count; i++)
		{
			m_Violin[i].OnNotePlayed -= HandleViolinOnNotePlayed;
		}
		for (int j = 0; j < m_CancelInstruments.Count; j++)
		{
			m_CancelInstruments[j].OnNotePlayed -= HandleOtherInstrumentOnNotePlayed;
		}
		m_Drum.OnNotePlayed -= HandleDrumOnNotePlayed;
		m_Drum.OnNotePlayed -= HandleDrumTwoOnNotePlayed;
		m_Piano.OnNotePlayed -= HandlePianoOnNotePlayed;
	}

	protected override void OnDisposed()
	{
		m_SammySecretClip = null;
		if (m_CancelInstruments != null)
		{
			m_CancelInstruments.Clear();
			m_CancelInstruments = null;
		}
		base.OnDisposed();
	}
}
