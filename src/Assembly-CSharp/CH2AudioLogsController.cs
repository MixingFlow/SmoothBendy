using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public class CH2AudioLogsController : BaseController
{
	[Header("<Controllers>")]
	[SerializeField]
	private CH2RecordingStudioController m_PuzzleController;

	[SerializeField]
	private CH2LostKeysController m_KeyController;

	[Header("<Audio Logs>")]
	[SerializeField]
	private AudioLog m_AudioLogLostKeys;

	[SerializeField]
	private AudioLog m_AudioLogThePrayer;

	[SerializeField]
	private AudioLog m_AudioLogDistractions;

	[SerializeField]
	private AudioLog m_AudioLogTheProjectionist;

	[SerializeField]
	private AudioLog m_AudioLogTheNewVoiceAcress;

	[SerializeField]
	private AudioLog m_AudioLogFavoriteSong;

	[SerializeField]
	private AudioLog m_AudioLogJackFain;

	private AudioObject m_AudioObjectThePrayerFinale;

	private AudioClip m_ActiveAudioClip;

	private AudioClip m_LostKeysClip;

	private AudioClip m_ThePrayerClip;

	private AudioClip m_ThePrayerFinaleClip;

	private AudioClip m_DistractionsClip;

	private AudioClip m_ProjectionistClip;

	private AudioClip m_TheNewVoiceActressClip;

	private AudioClip m_PuzzleClip;

	private AudioClip m_JackFainClip;

	private AudioLogModalController m_AudioLogController;

	private bool m_IsSammyScareComplete;

	private string m_PuzzleLog;

	private bool m_HasKeyObjective;

	private bool m_HasFavoriteSongObjective;

	public event EventHandler OnLostKeyObjective;

	public event EventHandler OnFavoriteSongObjective;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_LostKeysClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/AudioLogs/DIA_WAL_Diary_Lost_Keys_01_temp");
		m_ThePrayerClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/AudioLogs/DIA_SammyC2_Audio_Diarry_01");
		m_ThePrayerFinaleClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Sammy/DIA_SammyC2_01");
		m_DistractionsClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/AudioLogs/DIA_SAM_Diary_Distractions_01_temp");
		m_ProjectionistClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/AudioLogs/DIA_NOR_Diary_Projectionist_01_temp");
		m_TheNewVoiceActressClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/AudioLogs/DIA_SUS_Diary_New_Voice_Actress_01_temp");
		m_JackFainClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/AudioLogs/ch2_audiolog_jackfain");
		m_AudioLogThePrayer.OnInteracted += HandleAudioLogThePrayerOnInteracted;
		m_AudioLogThePrayer.SetActive(active: true);
		m_AudioLogDistractions.OnInteracted += HandleAudioLogDistractionsOnInteracted;
		m_AudioLogDistractions.SetActive(active: true);
		m_AudioLogTheProjectionist.OnInteracted += HandleAudioLogTheProjectionistOnInteracted;
		m_AudioLogTheProjectionist.SetActive(active: true);
		m_AudioLogTheNewVoiceAcress.OnInteracted += HandleAudioLogTheNewVoiceAcressOnInteracted;
		m_AudioLogTheNewVoiceAcress.SetActive(active: true);
		m_AudioLogJackFain.OnInteracted += HandleAudioLogJackFainOnInteracted;
		m_AudioLogJackFain.SetActive(active: true);
		m_AudioLogLostKeys.SetActive(active: false);
		m_AudioLogFavoriteSong.SetActive(active: false);
		m_PuzzleController.GeneratePuzzle();
		GenerateDialogue();
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.LostKeysObjective.IsStarted)
		{
			m_HasKeyObjective = true;
			this.OnLostKeyObjective.Send(this);
		}
		m_AudioLogLostKeys.OnInteracted += HandleAudioLogLostKeyOnInteracted;
		m_AudioLogLostKeys.SetActive(active: true);
	}

	public void ActivateFavoriteSong()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.MusicPuzzleObjective.IsStarted)
		{
			m_HasFavoriteSongObjective = true;
			this.OnFavoriteSongObjective.Send(this);
		}
		m_AudioLogFavoriteSong.OnInteracted += HandleAudioLogFavoriteSongOnInteracted;
		m_AudioLogFavoriteSong.SetActive(active: true);
	}

	private void GenerateDialogue()
	{
		string Translation = string.Empty;
		string audioLog = "AudioLog/SAMMY_MY_FAVORITE_SONG_HEADER";
		if (LocalizationManager.TryGetTranslation("AudioLog/SAMMY_MY_FAVORITE_SONG_HEADER", out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			audioLog = Translation;
		}
		List<AudioClip> audioClips = new List<AudioClip>();
		audioClips.Add(GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Sammy/Puzzle/DIA_Sammy_Puzzle_Diary_Intro_01"));
		int index = 0;
		int index2 = 0;
		int index3 = 0;
		int index4 = 0;
		int index5 = 0;
		for (int i = 0; i < m_PuzzleController.InstrumentOrder.Count; i++)
		{
			switch (m_PuzzleController.InstrumentOrder[i])
			{
			case 0:
				UpdatePuzzleAudioLog(AudioLogConstants.SAMMY_BANJO, GameManager.Instance.GetAudioClips("Audio/DIA/CH2/Sammy/Puzzle/Banjo/"), ref audioLog, ref index, ref audioClips);
				break;
			case 1:
				UpdatePuzzleAudioLog(AudioLogConstants.SAMMY_DRUM, GameManager.Instance.GetAudioClips("Audio/DIA/CH2/Sammy/Puzzle/Drum/"), ref audioLog, ref index2, ref audioClips);
				break;
			case 2:
				UpdatePuzzleAudioLog(AudioLogConstants.SAMMY_BASS, GameManager.Instance.GetAudioClips("Audio/DIA/CH2/Sammy/Puzzle/BassFiddle/"), ref audioLog, ref index3, ref audioClips);
				break;
			case 3:
				UpdatePuzzleAudioLog(AudioLogConstants.SAMMY_VIOLIN, GameManager.Instance.GetAudioClips("Audio/DIA/CH2/Sammy/Puzzle/Violin/"), ref audioLog, ref index4, ref audioClips);
				break;
			case 4:
				UpdatePuzzleAudioLog(AudioLogConstants.SAMMY_PIANO, GameManager.Instance.GetAudioClips("Audio/DIA/CH2/Sammy/Puzzle/Piano/"), ref audioLog, ref index5, ref audioClips);
				break;
			}
		}
		audioClips.Add(GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Sammy/Puzzle/DIA_Sammy_Puzzle_Diary_Outro_01"));
		string text = "AudioLog/SAMMY_MY_FAVORITE_SONG_FOOTER";
		if (LocalizationManager.TryGetTranslation(text, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			text = Translation;
		}
		audioLog += text;
		m_PuzzleLog = audioLog;
		m_PuzzleClip = GameManager.Instance.AudioManager.Combine(audioClips);
	}

	private void UpdatePuzzleAudioLog(string[] instrumentLog, AudioClip[] dialogueClips, ref string audioLog, ref int index, ref List<AudioClip> audioClips)
	{
		string Translation = string.Empty;
		string text = instrumentLog[index];
		if (LocalizationManager.TryGetTranslation(text, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			text = Translation;
		}
		audioLog += text;
		audioClips.Add(dialogueClips[index]);
		index++;
	}

	private void HandleAudioLogLostKeyOnInteracted(object sender, EventArgs e)
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogLostKeys.SetActive(active: false);
		m_AudioLogLostKeys.OnInteracted -= HandleAudioLogLostKeyOnInteracted;
		TryAudioLogAchievement(m_AudioLogLostKeys.GetID());
		if (!m_HasKeyObjective)
		{
			m_HasKeyObjective = true;
			ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_UNLOCK_CLOSET", "OBJECTIVES/CH2_OBJECTIVE_UNLOCK_CLOSET_TIP", 4f);
			objectiveDataVO.AddItemCounter(m_KeyController.KeySprite, 0);
			GameManager.Instance.ShowObjective(objectiveDataVO);
			this.OnLostKeyObjective.Send(this);
		}
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_WALLY",
			Log = "AudioLog/WALLY_LOST_KEYS",
			LogWorldPosition = m_AudioLogLostKeys.transform.position
		}, m_AudioLogLostKeys, m_LostKeysClip, HandleAudioLostKeyOnComplete);
	}

	private void HandleAudioLostKeyOnComplete()
	{
		if ((Object)(object)m_ActiveAudioClip == (Object)(object)m_LostKeysClip)
		{
			m_AudioLogController.PlayOut();
		}
		m_AudioLogLostKeys.SetActive(active: true);
		m_AudioLogLostKeys.OnInteracted += HandleAudioLogLostKeyOnInteracted;
	}

	private void HandleAudioLogThePrayerOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogThePrayer.SetActive(active: false);
		m_AudioLogThePrayer.OnInteracted -= HandleAudioLogThePrayerOnInteracted;
		TryAudioLogAchievement(m_AudioLogThePrayer.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_SAMMY",
			Log = "AudioLog/SAMMY_THE_PRAYER",
			LogWorldPosition = m_AudioLogThePrayer.transform.position
		}, m_AudioLogThePrayer, m_ThePrayerClip, HandleAudioThePrayerOnComplete);
	}

	private void HandleAudioThePrayerOnComplete()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsSammyScareComplete && Vector3.Distance(GameManager.Instance.Player.transform.position, m_AudioLogThePrayer.transform.position) < 15f)
		{
			m_IsSammyScareComplete = true;
			Vector3 position = GameManager.Instance.Player.transform.position;
			position -= GameManager.Instance.Player.transform.forward * 4f;
			m_AudioObjectThePrayerFinale = GameManager.Instance.AudioManager.PlayAtPosition(m_ThePrayerFinaleClip, position, AudioObjectType.DIALOGUE, 0, isQueued: false, GameManager.Instance.Player.transform);
		}
		if ((Object)(object)m_ActiveAudioClip == (Object)(object)m_ThePrayerClip)
		{
			m_AudioLogController.PlayOut();
		}
		m_AudioLogThePrayer.SetActive(active: true);
		m_AudioLogThePrayer.OnInteracted += HandleAudioLogThePrayerOnInteracted;
	}

	private void HandleAudioLogDistractionsOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogDistractions.SetActive(active: false);
		m_AudioLogDistractions.OnInteracted -= HandleAudioLogDistractionsOnInteracted;
		TryAudioLogAchievement(m_AudioLogDistractions.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_SAMMY",
			Log = "AudioLog/SAMMY_DISTRACTIONS",
			LogWorldPosition = m_AudioLogDistractions.transform.position
		}, m_AudioLogDistractions, m_DistractionsClip, HandleAudioDistractionsOnComplete);
	}

	private void HandleAudioDistractionsOnComplete()
	{
		if ((Object)(object)m_ActiveAudioClip == (Object)(object)m_DistractionsClip)
		{
			m_AudioLogController.PlayOut();
		}
		m_AudioLogDistractions.SetActive(active: true);
		m_AudioLogDistractions.OnInteracted += HandleAudioLogDistractionsOnInteracted;
	}

	private void HandleAudioLogTheProjectionistOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogTheProjectionist.SetActive(active: false);
		m_AudioLogTheProjectionist.OnInteracted -= HandleAudioLogTheProjectionistOnInteracted;
		TryAudioLogAchievement(m_AudioLogTheProjectionist.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_NORMAN",
			Log = "AudioLog/NORMAN_THE_PROJECTIONIST",
			LogWorldPosition = m_AudioLogTheProjectionist.transform.position
		}, m_AudioLogTheProjectionist, m_ProjectionistClip, HandleAudioTheProjectionistOnComplete);
	}

	private void HandleAudioTheProjectionistOnComplete()
	{
		if ((Object)(object)m_ActiveAudioClip == (Object)(object)m_ProjectionistClip)
		{
			m_AudioLogController.PlayOut();
		}
		m_AudioLogTheProjectionist.SetActive(active: true);
		m_AudioLogTheProjectionist.OnInteracted += HandleAudioLogTheProjectionistOnInteracted;
	}

	private void HandleAudioLogTheNewVoiceAcressOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogTheNewVoiceAcress.SetActive(active: false);
		m_AudioLogTheNewVoiceAcress.OnInteracted -= HandleAudioLogTheNewVoiceAcressOnInteracted;
		TryAudioLogAchievement(m_AudioLogTheNewVoiceAcress.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_SUSIE",
			Log = "AudioLog/SUSIE_THE_NEW_VOICE_ACTRESS",
			LogWorldPosition = m_AudioLogTheNewVoiceAcress.transform.position
		}, m_AudioLogTheNewVoiceAcress, m_TheNewVoiceActressClip, HandleAudioTheNewVoiceAcressOnComplete);
	}

	private void HandleAudioTheNewVoiceAcressOnComplete()
	{
		if ((Object)(object)m_ActiveAudioClip == (Object)(object)m_TheNewVoiceActressClip)
		{
			m_AudioLogController.PlayOut();
		}
		m_AudioLogTheNewVoiceAcress.SetActive(active: true);
		m_AudioLogTheNewVoiceAcress.OnInteracted += HandleAudioLogTheNewVoiceAcressOnInteracted;
	}

	private void HandleAudioLogJackFainOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogJackFain.SetActive(active: false);
		m_AudioLogJackFain.OnInteracted -= HandleAudioLogJackFainOnInteracted;
		TryAudioLogAchievement(m_AudioLogJackFain.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_JACK",
			Log = "AudioLog/JACK_NOSE_CLOSED",
			LogWorldPosition = m_AudioLogJackFain.transform.position
		}, m_AudioLogJackFain, m_JackFainClip, HandleAudioLogJackFainOnComplete);
	}

	private void HandleAudioLogJackFainOnComplete()
	{
		if ((Object)(object)m_ActiveAudioClip == (Object)(object)m_JackFainClip)
		{
			m_AudioLogController.PlayOut();
		}
		m_AudioLogJackFain.SetActive(active: true);
		m_AudioLogJackFain.OnInteracted += HandleAudioLogJackFainOnInteracted;
	}

	private void HandleAudioLogFavoriteSongOnInteracted(object sender, EventArgs e)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogFavoriteSong.SetActive(active: false);
		m_AudioLogFavoriteSong.OnInteracted -= HandleAudioLogFavoriteSongOnInteracted;
		TryAudioLogAchievement(m_AudioLogFavoriteSong.GetID());
		if (!m_HasFavoriteSongObjective)
		{
			m_HasFavoriteSongObjective = true;
			this.OnFavoriteSongObjective.Send(this);
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_SANCTUARY", "OBJECTIVES/CH2_OBJECTIVE_SANCTUARY_TIP", 4f));
			m_PuzzleController.EnableTip();
		}
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_SAMMY",
			LogString = m_PuzzleLog,
			LogWorldPosition = m_AudioLogFavoriteSong.transform.position
		}, m_AudioLogFavoriteSong, m_PuzzleClip, HandleAudioFavoriteSongOnComplete);
	}

	private void HandleAudioFavoriteSongOnComplete()
	{
		if ((Object)(object)m_ActiveAudioClip == (Object)(object)m_PuzzleClip)
		{
			m_AudioLogController.PlayOut();
		}
		m_AudioLogFavoriteSong.SetActive(active: true);
		m_AudioLogFavoriteSong.OnInteracted += HandleAudioLogFavoriteSongOnInteracted;
	}

	private void TryAudioLogAchievement(int id)
	{
		if (!GameManager.Instance.GameData.CH2AchievementData.AudioLogs.Contains(id))
		{
			GameManager.Instance.GameData.CH2AchievementData.AudioLogs.Add(id);
		}
		if (GameManager.Instance.GameData.CH2AchievementData.AudioLogs.Count >= 7)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.OLD_PROBLEMS);
		}
		AudioLogAllAchievements.Check();
	}

	private void PlayAudioLog(AudioLogDataVO vo, AudioLog audioLog, AudioClip audioClip, Action onComplete)
	{
		AudioControllerReset();
		m_ActiveAudioClip = audioClip;
		m_AudioLogController = GameManager.Instance.UIManager.Show<AudioLogModalController>("UI/Modals/AudioLogModalController", "MODAL", vo);
		audioLog.Play(m_ActiveAudioClip, onComplete);
	}

	private void AudioControllerReset()
	{
		if ((Object)(object)m_AudioLogController != (Object)null)
		{
			m_AudioLogController.Dispose();
			m_AudioLogController = null;
		}
	}

	protected override void OnDisposed()
	{
		AudioControllerReset();
		if (Object.op_Implicit((Object)(object)m_AudioLogThePrayer))
		{
			m_AudioLogThePrayer.OnInteracted -= HandleAudioLogThePrayerOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_AudioLogDistractions))
		{
			m_AudioLogDistractions.OnInteracted -= HandleAudioLogDistractionsOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_AudioLogTheProjectionist))
		{
			m_AudioLogTheProjectionist.OnInteracted -= HandleAudioLogTheProjectionistOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_AudioLogTheNewVoiceAcress))
		{
			m_AudioLogTheNewVoiceAcress.OnInteracted -= HandleAudioLogTheNewVoiceAcressOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_AudioLogFavoriteSong))
		{
			m_AudioLogFavoriteSong.OnInteracted -= HandleAudioLogFavoriteSongOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_AudioLogLostKeys))
		{
			m_AudioLogLostKeys.OnInteracted -= HandleAudioLogLostKeyOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_AudioLogJackFain))
		{
			m_AudioLogJackFain.OnInteracted -= HandleAudioLogJackFainOnInteracted;
		}
		if ((Object)(object)m_AudioObjectThePrayerFinale != (Object)null)
		{
			m_AudioObjectThePrayerFinale.Clear();
			m_AudioObjectThePrayerFinale = null;
		}
		m_LostKeysClip = null;
		m_ThePrayerClip = null;
		m_ThePrayerFinaleClip = null;
		m_DistractionsClip = null;
		m_ProjectionistClip = null;
		m_TheNewVoiceActressClip = null;
		m_PuzzleClip = null;
		m_JackFainClip = null;
		base.OnDisposed();
	}
}
