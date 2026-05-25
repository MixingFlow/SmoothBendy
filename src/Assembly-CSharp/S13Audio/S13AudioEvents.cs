using UnityEngine;

namespace S13Audio;

public class S13AudioEvents : MonoBehaviour
{
	public S13AudioManager am;

	public S13ObjectContainer oc;

	private void Start()
	{
		if ((Object)(object)am == (Object)null)
		{
			am = Object.FindObjectOfType<S13AudioManager>();
			if ((Object)(object)am == (Object)null)
			{
				Debug.LogError((object)(((Object)this).name + ": S13AudioManager not found in scene."), (Object)(object)((Component)this).gameObject);
			}
		}
	}

	private void evt_game_is_awake()
	{
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Master", 0f);
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Dialogue", 0f);
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Music", 0f);
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Effects", 0f);
	}

	private void evt_game_at_main_menu()
	{
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 0f);
		am.ToSnapshot("TMGAudioMixer", "mxs_base", 0f);
	}

	private void evt_game_at_loading_chapter1()
	{
		am.LerpMixerProperty("S13BaseMixer", "S13MasterVolume", -80f, 0f, ignoreTimeScale: true);
		am.LerpMixerProperty("S13BaseMixer", "S13MasterVolume", 3f, 6f, ignoreTimeScale: true);
		am.UnloadAllSoundBanks();
		am.LoadSoundBank("CH1 Soundbank");
	}

	private void evt_game_at_loading_chapter2()
	{
		am.LerpMixerProperty("S13BaseMixer", "S13MasterVolume", -80f, 0f, ignoreTimeScale: true);
		am.LerpMixerProperty("S13BaseMixer", "S13MasterVolume", 3f, 6f, ignoreTimeScale: true);
		am.UnloadAllSoundBanks();
		am.LoadSoundBank("CH2 Soundbank");
	}

	private void evt_game_at_loading_chapter3()
	{
		am.LerpMixerProperty("S13BaseMixer", "S13MasterVolume", -80f, 0f, ignoreTimeScale: true);
		am.LerpMixerProperty("S13BaseMixer", "S13MasterVolume", 3f, 6f, ignoreTimeScale: true);
		am.UnloadAllSoundBanks();
		am.LoadSoundBank("CH3 Soundbank");
	}

	private void evt_game_at_loading_chapter4()
	{
		am.LerpMixerProperty("S13BaseMixer", "S13MasterVolume", -80f, 0f, ignoreTimeScale: true);
		am.LerpMixerProperty("S13BaseMixer", "S13MasterVolume", 3f, 6f, ignoreTimeScale: true);
		am.UnloadAllSoundBanks();
		am.LoadSoundBank("CH4 Soundbank");
	}

	private void evt_game_at_loading_chapter5()
	{
		am.LerpMixerProperty("S13BaseMixer", "S13MasterVolume", -80f, 0f, ignoreTimeScale: true);
		am.LerpMixerProperty("S13BaseMixer", "S13MasterVolume", 3f, 6f, ignoreTimeScale: true);
		am.UnloadAllSoundBanks();
		am.LoadSoundBank("CH5 Soundbank");
	}

	private void evt_game_at_chapter_titles()
	{
	}

	private void evt_game_at_gameplay()
	{
	}

	private void evt_game_at_new_objective()
	{
	}

	private void evt_game_at_paused()
	{
		am.SetMixerProperty("S13BaseMixer", "S13MasterVolume", -80f);
	}

	private void evt_game_at_resume()
	{
		am.SetMixerProperty("S13BaseMixer", "S13MasterVolume", 3f);
	}

	private void evt_game_at_quit_prompt()
	{
	}

	private void evt_save_punchin()
	{
		am.PlayAudio("vo_henry_save");
		am.PlayAudio("sfx_save_bell");
	}

	private void evt_deathtunnel_start()
	{
		am.ToSnapshot("TMGAudioMixer", "mxs_death_tunnel", 0.5f);
		am.PlayAudio("sfx_death_tunnel_start_ink");
		am.PlayAudio("sfx_death_tunnel_start_lfe");
		am.PlayAudio("sfx_death_tunnel_loop");
	}

	private void evt_deathtunnel_stop()
	{
		am.ToSnapshot("TMGAudioMixer", "mxs_base", 2f);
		am.StopAudio("sfx_death_tunnel_loop");
		am.PlayAudio("sfx_death_tunnel_stop");
	}

	private void evt_horror_vision_start()
	{
		am.PlayAudio("sfx_horror_vision_start");
		am.PlayAudio("sfx_horror_vision_loop");
		am.PlayAudio("sfx_horror_voices_loop");
	}

	private void evt_horror_vision_stop()
	{
		am.StopAudio("sfx_horror_vision_start");
		am.StopAudio("sfx_horror_vision_loop");
		am.StopAudio("sfx_horror_voices_loop");
		am.PlayAudio("sfx_horror_vision_stop");
	}

	private void evt_miracle_station_enter()
	{
		am.ToSnapshot("S13BaseMixer", "mxs_CH0_inside_LMS", 2f);
		am.PlayAudio("sfx_lms_enter");
	}

	private void evt_miracle_station_exit()
	{
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 1f);
		am.PlayAudio("sfx_lms_exit");
	}

	private void evt_valve_drains_ink()
	{
		am.PlayAudio("sfx_ink_valve_drain");
		am.PlayAudio("sfx_ink_vavle_blubs");
	}

	private void evt_player_dead()
	{
	}

	private void evt_player_respawned()
	{
	}

	private void evt_screen_shake_start()
	{
	}

	private void evt_screen_shake_stop()
	{
	}

	private void evt_ink_machine_passby_start()
	{
		am.PlayAudio("sfx_ink_machine_passby_loop", 3f);
	}

	private void evt_seeing_tool_on()
	{
		am.PlayAudio("sfx_seeing_tool_on");
		am.PlayAudio("sfx_seeing_tool_loop");
	}

	private void evt_seeing_tool_off()
	{
		am.StopAudio("sfx_seeing_tool_on");
		am.StopAudio("sfx_seeing_tool_loop");
		am.PlayAudio("sfx_seeing_tool_off");
	}

	private void evt_new_test()
	{
	}

	private void evt_battery_pack_on()
	{
		am.PlayAudio("sfx_battery_start");
		am.PlayAudio("sfx_battery_loop");
	}

	private void evt_ink_machine_reveal_start()
	{
		am.PlayAudio("sfx_ink_machine_unlock");
		am.PlayAudio("sfx_ink_machine_moving_chains", 13f);
		am.PlayAudioDelayed("sfx_ink_machine_moving_loop", 6f);
		am.StopAudioDelayed("sfx_ink_machine_moving_loop", 19f);
		am.PlayAudioDelayed("sfx_ink_machine_stop", 19f);
		am.PlayAudio("amb_3gears_metal_barn");
		am.PlayAudio("amb_3gears_loop_barn");
	}

	private void evt_ink_machine_reveal_stop()
	{
		am.PlayAudio("amb_cage_idle_moves_left");
		am.PlayAudio("amb_cage_idle_moves_room");
	}

	private void evt_ink_pressure_restored()
	{
		oc.Play("InkPipes");
		am.PlayAudio("sfx_ink_heavy_flow_loop");
		am.PlayAudio("amb_tile_wall_mech_theatre");
		am.PlayAudio("sfx_ink_pipe_burst");
		am.PlayAudio("sfx_ink_pipe_flow");
		am.PlayAudio("sfx_ink_floor");
		am.PlayAudio("amb_tile_wall_mech_power1");
		am.PlayAudio("amb_tile_wall_mech_power2");
		oc.Stop("Theatre");
		am.StopAudio("amb_closeup_loop");
		am.ToSnapshot("PropsStatic", "ink_pumping", 2f);
	}

	private void evt_main_power_switch_activated()
	{
		am.PlayAudio("sfx_main_power_switched_on");
		am.PlayAudioDelayed("sfx_ink_machine_working_loop", 5f);
		am.ToSnapshot("S13BaseMixer", "mxs_CH1_main_power", 0f);
		am.InvokeEvent("evt_main_power_mix_reset", 6f);
	}

	private void evt_main_power_mix_reset()
	{
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 6f);
	}

	private void evt_bendy_appears()
	{
		am.PlayAudio("sfx_ink_heavy_everywhere");
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 0f);
		am.ToSnapshot("PropsStatic", "default", 1f);
		am.ToSnapshot("TMGAudioMixer", "mxs_bendy_overload", 4f);
		am.ToSnapshot("CH1Defaults", "mxs_running_away", 0f);
		oc.Enable("Stairwell");
		oc.Enable("Basement");
	}

	private void evt_floor_caves_in()
	{
		am.PlayAudio("sfx_floor_cavein_mix");
		am.PlayAudio("sfx_ink_machine_working_finale");
		am.StopAudio("sfx_ink_machine_working_loop");
		am.StopAudio("sfx_ink_heavy_everywhere");
		am.ToSnapshot("TMGAudioMixer", "mxs_base", 0f);
		am.ToSnapshot("CH1Defaults", "mxs_base", 0.5f);
		oc.Destroy("MainRoom");
		oc.Destroy("DreamsHallway");
		oc.Destroy("LargeHallway");
		oc.Destroy("BorisRoom");
		oc.Destroy("PowerRoom");
		oc.Destroy("Theatre");
		oc.Destroy("InkBarn");
		oc.Destroy("ArtRoom");
		oc.Destroy("ArtBath");
		oc.Destroy("BreakRoom");
		oc.Destroy("InkPipes");
	}

	private void evt_stairwell_valve1()
	{
		am.StopAudio("sfx_valve1_ink_flood");
		am.InvokeEvent("evt_valve_drains_ink", 1f);
	}

	private void evt_stairwell_valve2()
	{
		am.StopAudio("sfx_valve2_ink_flood");
		am.InvokeEvent("evt_valve_drains_ink", 1f);
	}

	private void evt_stairwell_valve3()
	{
		am.StopAudio("sfx_valve3_ink_flood");
		am.InvokeEvent("evt_valve_drains_ink", 1f);
		am.StopAudio("sfx_ink_machine_working_finale");
	}

	private void evt_bendy_finale_scare()
	{
		am.StopAllAudio();
		am.ToSnapshot("TMGAudioMixer", "mxs_bendy_overload", 0f);
	}

	private void evt_CH1_save_point_01()
	{
		am.PlayAudio("amb_3gears_metal_barn");
		am.PlayAudio("amb_3gears_loop_barn");
		am.PlayAudio("amb_cage_idle_moves_left");
		am.PlayAudio("amb_cage_idle_moves_room");
		am.PlayAudio("sfx_battery_loop");
	}

	private void evt_CH1_save_point_03()
	{
		evt_CH1_save_point_01();
		oc.Stop("Theatre");
		am.StopAudio("amb_closeup_loop");
		am.PlayAudio("sfx_ink_heavy_flow_loop");
		am.PlayAudio("amb_tile_wall_mech_theatre");
		am.PlayAudio("sfx_ink_floor");
		am.PlayAudio("amb_tile_wall_mech_power1");
		am.PlayAudio("amb_tile_wall_mech_power2");
		am.ToSnapshot("PropsStatic", "ink_pumping", 2f);
		oc.Play("InkPipes");
	}

	private void evt_CH1_save_point_04()
	{
		evt_CH1_save_point_03();
		am.PlayAudio("sfx_ink_machine_working_loop");
	}

	private void evt_CH1_save_point_05()
	{
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 0f);
		am.ToSnapshot("PropsStatic", "default", 0f);
		oc.Destroy("MainRoom");
		oc.Destroy("DreamsHallway");
		oc.Destroy("LargeHallway");
		oc.Destroy("BorisRoom");
		oc.Destroy("PowerRoom");
		oc.Destroy("Theatre");
		oc.Destroy("InkBarn");
		oc.Destroy("ArtRoom");
		oc.Destroy("ArtBath");
		oc.Destroy("BreakRoom");
		am.PlayAudio("sfx_ink_machine_working_finale");
		am.PlayAudio("amb_original_bendy_loop");
		am.StopAudio("sfx_ink_machine_working_loop");
		am.StopAudio("amb_tile_wall_mech_power1");
		am.StopAudio("amb_tile_wall_mech_power2");
		oc.Enable("Stairwell");
		oc.Enable("Basement");
		oc.Play("Stairwell");
	}

	private void evt_CH1_save_point_06()
	{
		evt_CH1_save_point_05();
		oc.Play("Stairwell");
		am.StopAudio("sfx_valve1_ink_flood");
		am.StopAudio("sfx_valve2_ink_flood");
		am.StopAudio("sfx_valve3_ink_flood");
		am.StopAudio("sfx_ink_machine_working_finale");
	}

	private void evt_main_room_enter()
	{
		oc.Play("MainRoom");
		am.PlayAudio("amb_original_horror_loop");
	}

	private void evt_main_room_exit()
	{
		oc.Stop("MainRoom");
		am.StopAudio("amb_original_horror_loop");
	}

	private void evt_dreams_hallway_enter()
	{
		oc.Play("DreamsHallway");
	}

	private void evt_dreams_hallway_exit()
	{
		oc.Stop("DreamsHallway");
	}

	private void evt_large_hallway_enter()
	{
		oc.Play("LargeHallway");
	}

	private void evt_large_hallway_exit()
	{
		oc.Stop("LargeHallway");
	}

	private void evt_boris_room_enter()
	{
		oc.Play("BorisRoom");
	}

	private void evt_boris_room_exit()
	{
		oc.Stop("BorisRoom");
	}

	private void evt_boris_rumbletrap_enter()
	{
		am.ToSnapshot("CH1 Ambience by Section", "mxs_boris_rumble", 1f);
	}

	private void evt_boris_rumbletrap_exit()
	{
		am.ToSnapshot("CH1 Ambience by Section", "mxs_base", 1f);
	}

	private void evt_power_room_enter()
	{
		oc.Play("PowerRoom");
		am.PlayAudio("amb_industrial_loop");
		am.StopAudio("amb_original_horror_loop");
	}

	private void evt_power_room_exit()
	{
		am.PlayAudio("amb_original_horror_loop");
		oc.Stop("PowerRoom");
		am.StopAudio("amb_industrial_loop");
	}

	private void evt_theatre_enter()
	{
		oc.Play("Theatre");
		am.PlayAudio("amb_closeup_loop");
		am.StopAudio("amb_original_horror_loop");
	}

	private void evt_theatre_exit()
	{
		am.PlayAudio("amb_original_horror_loop");
		am.StopAudio("amb_closeup_loop");
	}

	private void evt_theatre_mixtrap_ink_enter()
	{
		am.ToSnapshot("CH1ScriptedEvents", "Theatre", 0.5f);
	}

	private void evt_theatre_mixtrap_ink_exit()
	{
		am.ToSnapshot("CH1ScriptedEvents", "Base", 4f);
	}

	private void evt_ink_barn_enter()
	{
		am.PlayAudio("amb_airy_outdoor");
		am.StopAudio("amb_original_horror_loop");
	}

	private void evt_ink_barn_exit()
	{
		am.PlayAudio("amb_original_horror_loop");
		am.StopAudio("amb_airy_outdoor");
	}

	private void evt_art_room_enter()
	{
		oc.Play("ArtRoom");
		am.PlayAudio("amb_closeup_loop");
		am.StopAudio("amb_original_horror_loop");
	}

	private void evt_art_room_exit()
	{
		am.PlayAudio("amb_original_horror_loop");
		oc.Stop("ArtRoom");
		am.StopAudio("amb_closeup_loop");
	}

	private void evt_art_bath_enter()
	{
		oc.Play("ArtBath");
	}

	private void evt_art_bath_exit()
	{
		oc.Stop("ArtBath");
	}

	private void evt_break_room_enter()
	{
		oc.Play("BreakRoom");
		am.PlayAudio("amb_closeup_loop");
		am.StopAudio("amb_original_horror_loop");
	}

	private void evt_break_room_exit()
	{
		am.PlayAudio("amb_original_horror_loop");
		oc.Stop("BreakRoom");
		am.StopAudio("amb_closeup_loop");
	}

	private void evt_stairwell_enter()
	{
		oc.Play("Stairwell");
		am.PlayAudio("amb_original_bendy_loop");
	}

	private void evt_basement_final_enter()
	{
		oc.Play("Basement");
		am.PlayAudio("amb_closeup_loop");
		am.StopAudio("amb_original_bendy_loop");
	}

	private void evt_basement_final_exit()
	{
		am.PlayAudio("amb_original_bendy_loop");
		am.StopAudio("amb_closeup_loop");
	}

	private void evt_enter_music_department()
	{
		am.PlayAudio("sfx_gate_pipe", 4f);
		am.PlayAudio("sfx_gate_metal_rolling_open");
		am.PlayAudio("sfx_gate_rumble");
	}

	private void evt_ch2_dept_ink_blob_fall()
	{
		am.PlayAudio("sfx_ink_blob_fall");
	}

	private void evt_ch2_dept_ink_blob_land()
	{
		am.PlayAudio("sfx_ink_blob_land");
	}

	private void evt_office_stairs_ink_drained()
	{
		oc.Destroy("OfficeFlood");
	}

	private void evt_office_door_ink_drained()
	{
		oc.Destroy("OfficeInk");
	}

	private void evt_office_lever_thrown()
	{
		am.InvokeEvent("evt_valve_drains_ink", 1f);
		am.PlayAudio("sfx_pump_control_bangs", 6f);
		am.PlayAudio("sfx_pump_control_more", 6f);
		am.PlayAudio("sfx_pump_control_switched_on");
	}

	private void evt_sammy_knocks_out_player()
	{
		am.PlayAudio("m_sammy_revealed");
	}

	private void evt_sewer_puzzle_winch_up_start()
	{
		am.PlayAudio("sfx_sewer_puzzle_winch_up_start");
		am.PlayAudio("sfx_sewer_puzzle_winch_up_start2");
		am.PlayAudioDelayed("sfx_sewer_puzzle_winch_up_stop", 7.5f);
		am.StopAudioDelayed("sfx_sewer_puzzle_winch_up_start", 8f);
	}

	private void evt_sewer_puzzle_winch_down()
	{
		am.PlayAudio("sfx_sewer_puzzle_winch_down");
	}

	private void evt_CH2_save_point_01()
	{
	}

	private void evt_CH2_save_point_02()
	{
	}

	private void evt_CH2_save_point_03()
	{
	}

	private void evt_CH2_save_point_04()
	{
	}

	private void evt_CH2_save_point_05()
	{
		evt_musicdepartment_exit();
		evt_sammyoffice_enter_from_dept();
	}

	private void evt_CH2_save_point_06()
	{
		evt_sammyoffice_exit_to_dept();
	}

	private void evt_CH2_save_point_07()
	{
	}

	private void evt_CH2_save_point_08()
	{
		evt_sammyoffice_enter_from_dept();
	}

	private void evt_CH2_save_point_09()
	{
	}

	private void evt_CH2_save_point_10()
	{
		evt_office_door_ink_drained();
		evt_sewers_exit();
		evt_sammyoffice_enter_from_dept();
	}

	private void evt_CH2_save_point_11()
	{
	}

	private void evt_CH2_save_point_12()
	{
	}

	private void evt_CH2_save_point_13()
	{
	}

	private void evt_ch2_opening_enter()
	{
		oc.Play("Opening");
	}

	private void evt_ch2_opening_exit()
	{
		oc.Stop("Opening");
	}

	private void evt_ch2_opening_loops_enter()
	{
		am.ToSnapshot("CH2Defaults", "Opening", 0.5f);
		am.PlayAudio("amb_original_bendy_loop");
		am.PlayAudio("amb_closeup_loop");
	}

	private void evt_ch2_opening_loops_exit()
	{
		am.StopAudio("amb_original_bendy_loop");
	}

	private void evt_sammyhallway_enter()
	{
		oc.Play("SammyHallway");
		am.PlayAudio("amb_closeup_loop");
		am.ToSnapshot("CH2Defaults", "Opening", 0.5f);
	}

	private void evt_sammyhallway_exit()
	{
		oc.Stop("SammyHallway");
		am.StopAudio("amb_original_bendy_loop");
	}

	private void evt_musicdepartment_enter()
	{
		oc.Play("MusicDepartment");
		am.PlayAudio("amb_industrial_loop_studio");
		am.PlayAudio("amb_closeup_loop");
		am.ToSnapshot("CH2Defaults", "MusicDepartment", 2f);
	}

	private void evt_musicdepartment_exit()
	{
		oc.Stop("MusicDepartment");
		am.StopAudio("amb_industrial_loop_studio");
	}

	private void evt_music_ink_enter()
	{
		am.PlayAudio("amb_water_flooded_loop_stairs");
		am.ToSnapshot("CH2 Ambience by Section", "mxs_sammy_ink", 1f);
	}

	private void evt_music_ink_exit()
	{
		am.StopAudio("amb_water_flooded_loop_stairs");
		am.ToSnapshot("CH2 Ambience by Section", "mxs_default", 1f);
	}

	private void evt_recordingstudio_enter()
	{
		oc.Play("RecordingStudio");
		am.ToSnapshot("CH2Defaults", "RecordingStudio", 2f);
		am.ToSnapshot("PropsStatic", "default", 1f);
	}

	private void evt_recordingstudio_exit()
	{
		oc.Stop("RecordingStudio");
		am.ToSnapshot("CH2Defaults", "MusicDepartment", 2f);
	}

	private void evt_recordingstudio_balcony_enter()
	{
		am.PlayAudio("amb_industrial_loop_balcony");
	}

	private void evt_recordingstudio_balcony_exit()
	{
		am.StopAudio("amb_industrial_loop_balcony");
	}

	private void evt_secretroom_enter()
	{
		oc.Play("SecretRoom");
		am.PlayAudio("amb_original_bendy_loop");
		am.ToSnapshot("PropsStatic", "ink_pumping", 1f);
		am.ToSnapshot("CH2Defaults", "SecretSideRoom", 0.1f);
	}

	private void evt_secretroom_exit()
	{
		oc.Stop("SecretRoom");
		am.StopAudio("amb_original_bendy_loop");
		am.ToSnapshot("PropsStatic", "default", 1f);
		am.ToSnapshot("CH2Defaults", "RecordingStudio", 0.1f);
	}

	private void evt_office_ink_enter()
	{
		oc.Play("OfficeFlood");
		am.ToSnapshot("CH2 Ambience by Section", "mxs_sammy_ink", 1f);
	}

	private void evt_office_ink_exit()
	{
		oc.Stop("OfficeFlood");
		am.ToSnapshot("CH2 Ambience by Section", "mxs_default", 3f);
	}

	private void evt_sammyoffice_enter_from_dept()
	{
		oc.Play("SammyOffice");
		oc.Play("OfficeInk");
		am.PlayAudio("amb_original_horror_loop");
		am.PlayAudio("amb_heavy_loop");
		am.ToSnapshot("CH2Defaults", "Office", 2f);
	}

	private void evt_sammyoffice_exit_to_dept()
	{
		oc.Stop("SammyOffice");
		oc.Stop("OfficeInk");
		am.StopAudio("amb_original_horror_loop");
		am.StopAudio("amb_heavy_loop");
		am.ToSnapshot("CH2Defaults", "MusicDepartment", 2f);
	}

	private void evt_sammyoffice_enter_from_infirmary()
	{
		oc.Play("SammyOffice");
		oc.Play("OfficeInk");
		am.PlayAudio("amb_closeup_loop");
		am.ToSnapshot("CH2Defaults", "Office", 2f);
	}

	private void evt_sammyoffice_exit_to_infirmary()
	{
		oc.Stop("SammyOffice");
		oc.Stop("OfficeInk");
		am.StopAudio("amb_closeup_loop");
	}

	private void evt_infirmary_enter()
	{
		oc.Play("Infirmary");
		am.PlayAudio("amb_original_horror_loop");
		am.PlayAudio("amb_heavy_loop");
		am.PlayAudio("amb_industrial_loop");
		am.StopAudio("amb_closeup_loop");
		am.ToSnapshot("CH2Defaults", "Infirmary", 3f);
	}

	private void evt_infirmary_exit()
	{
		oc.Stop("Infirmary");
		am.StopAudio("amb_industrial_loop");
	}

	private void evt_sewers_enter()
	{
		oc.Play("Sewers");
		am.PlayAudio("amb_original_horror_loop");
		am.ToSnapshot("CH2Defaults", "Horror", 3f);
	}

	private void evt_sewers_exit()
	{
		oc.Stop("Sewers");
	}

	private void evt_machineroom_enter()
	{
		oc.Play("MachineRoom");
		am.PlayAudio("amb_industrial_loop");
		am.PlayAudio("amb_heavy_loop");
		am.ToSnapshot("CH2Defaults", "MachineRoom", 1f);
		am.ToSnapshot("Sewers", "MachineRoom", 1f);
	}

	private void evt_machineroom_exit()
	{
		oc.Stop("MachineRoom");
		am.StopAudio("amb_industrial_loop");
		am.ToSnapshot("CH2Defaults", "Horror", 3f);
		am.ToSnapshot("Sewers", "Base", 1f);
	}

	private void evt_sacrifice_enter()
	{
		oc.Play("Sacrifice");
		am.PlayAudio("amb_original_horror_loop");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_heavy_loop");
		am.ToSnapshot("CH2Defaults", "Office", 1f);
	}

	private void evt_sacrifice_exit()
	{
		oc.Stop("Sacrifice");
		am.StopAudio("amb_original_horror_loop");
		am.StopAudio("amb_closeup_loop");
	}

	private void evt_bendychase_enter()
	{
		oc.Play("BendyChase");
		am.PlayAudio("amb_heavy_loop");
		am.ToSnapshot("CH2Defaults", "Heavy", 2f);
	}

	private void evt_bendychase_exit()
	{
		oc.Stop("BendyChase");
	}

	private void evt_borisreveal_enter()
	{
		oc.Play("BorisReveal");
		am.PlayAudio("amb_original_horror_loop");
		am.ToSnapshot("CH2Defaults", "Horror", 2f);
	}

	private void evt_borisreveal_exit()
	{
		oc.Stop("BorisReveal");
		am.StopAudio("amb_original_horror_loop");
	}

	private void evt_safehouse_closed()
	{
		oc.Destroy("SafeHouse");
	}

	private void evt_darkhallway_closed()
	{
	}

	private void evt_activate_workshop_mech_right()
	{
		oc.Enable("Workshop_Mech_Right");
		oc.Play("Workshop_Mech_Right");
	}

	private void evt_activate_workshop_mech_left()
	{
		oc.Enable("Workshop_Mech_Left");
		oc.Play("Workshop_Mech_Left");
	}

	private void evt_toys_found1()
	{
		am.ToSnapshot("CH3Workshop", "1Active", 2f);
	}

	private void evt_toys_found2()
	{
		am.ToSnapshot("CH3Workshop", "2Active", 2f);
	}

	private void evt_toys_found3()
	{
		am.ToSnapshot("CH3Workshop", "3Active", 2f);
	}

	private void evt_toys_found4()
	{
		am.ToSnapshot("CH3Workshop", "AllActive", 2f);
	}

	private void evt_alice_reveal_start()
	{
		oc.Stop("AliceReveal");
		am.StopAudio("amb_original_bendy_loop");
		am.ToSnapshot("TMGAudioMixer", "mxs_alice_monologues", 1f);
	}

	private void evt_alice_reveal_complete()
	{
		oc.Play("AliceReveal");
		am.PlayAudio("amb_original_bendy_loop");
		am.ToSnapshot("TMGAudioMixer", "mxs_base", 1f);
	}

	private void evt_elevator_start()
	{
	}

	private void evt_elevator_stop()
	{
	}

	private void evt_ch3_secret_flood_empty()
	{
		oc.Destroy("Level_P_flood");
	}

	private void evt_safehouse_enter()
	{
		oc.Play("SafeHouse");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_heavy_loop");
		am.ToSnapshot("CH3Defaults", "mxs_safehouse", 1f);
		am.ToSnapshot("S13BaseMixer", "mxs_reverb_tiny", 0.5f);
	}

	private void evt_safehouse_exit()
	{
		oc.Stop("SafeHouse");
	}

	private void evt_darkhallway_enter()
	{
		oc.Play("DarkHallway");
		am.ToSnapshot("CH3Defaults", "mxs_darkhallway", 1f);
		oc.Enable("HeavenlyToys");
		oc.Enable("Workshop");
	}

	private void evt_darkhallway_exit()
	{
		oc.Stop("DarkHallway");
		am.StopAudio("amb_closeup_loop");
		am.StopAudio("amb_heavy_loop");
	}

	private void evt_heavenlytoys_enter()
	{
		oc.Play("HeavenlyToys");
		am.PlayAudio("amb_airy_outdoor");
		am.ToSnapshot("CH3Defaults", "mxs_heavenlytoys", 3f);
		oc.Destroy("SafeHouse");
	}

	private void evt_heavenlytoys_exit()
	{
		oc.Stop("HeavenlyToys");
		am.StopAudio("amb_airy_outdoor");
	}

	private void evt_workshop_enter()
	{
		oc.Play("Workshop");
		am.ToSnapshot("CH3Defaults", "mxs_workshop", 2f);
		oc.Enable("AliceReveal");
	}

	private void evt_workshop_exit()
	{
		oc.Stop("Workshop");
		am.ToSnapshot("CH3Defaults", "mxs_heavenlytoys", 1f);
	}

	private void evt_alicereveal_enter()
	{
		am.PlayAudio("amb_original_bendy_loop");
		am.ToSnapshot("CH3Defaults", "mxs_alicereveal", 2f);
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 0.5f);
		oc.Enable("ChoicesHallways");
		oc.Enable("ChoicesDevil");
		oc.Enable("ChoicesAngel");
	}

	private void evt_alicereveal_exit()
	{
		am.StopAudio("amb_original_bendy_loop");
		am.ToSnapshot("S13BaseMixer", "mxs_reverb_corridor", 0.5f);
	}

	private void evt_choices_enter()
	{
		oc.Play("ChoicesHallways");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_original_horror_loop");
		am.ToSnapshot("CH3Defaults", "mxs_choices", 2.5f);
		oc.Enable("LiftHallways");
	}

	private void evt_choices_exit()
	{
		oc.Stop("ChoicesHallways");
		am.StopAudio("amb_closeup_loop");
		am.StopAudio("amb_original_horror_loop");
	}

	private void evt_choices_devil_enter()
	{
		oc.Play("ChoicesDevil");
		oc.Destroy("ChoicesAngel");
	}

	private void evt_choices_devil_exit()
	{
		oc.Stop("ChoicesDevil");
	}

	private void evt_choices_angel_enter()
	{
		oc.Play("ChoicesAngel");
		oc.Destroy("ChoicesDevil");
	}

	private void evt_choices_angel_exit()
	{
		oc.Stop("ChoicesAngel");
	}

	private void evt_from_choices_to_lift()
	{
		oc.Stop("ChoicesHallways");
		am.StopAudio("amb_closeup_loop");
		evt_lift_hallways_enter();
		oc.Enable("TrailerRoom");
		oc.Enable("LiftMain1");
	}

	private void evt_from_lift_to_choices()
	{
		oc.Stop("LiftHallways");
		evt_choices_enter();
	}

	private void evt_lift_hallways_enter()
	{
		oc.Play("LiftHallways");
		am.PlayAudio("amb_original_horror_loop");
		am.ToSnapshot("CH3Defaults", "mxs_lift_halls", 2.5f);
	}

	private void evt_lift_hallways_exit()
	{
		oc.Stop("LiftHallways");
		am.StopAudio("amb_original_horror_loop");
	}

	private void evt_trailerroom_enter()
	{
		oc.Play("TrailerRoom");
		am.PlayAudio("amb_original_bendy_loop");
		am.ToSnapshot("CH3Defaults", "mxs_trailer", 0.5f);
	}

	private void evt_trailerroom_exit()
	{
		oc.Stop("TrailerRoom");
		am.StopAudio("amb_original_bendy_loop");
	}

	private void evt_lift_main1_enter()
	{
		oc.Play("LiftMain1");
		am.PlayAudio("amb_original_bendy_loop");
		am.ToSnapshot("CH3Defaults", "mxs_lift_main1", 1.5f);
		am.ToSnapshot("S13BaseMixer", "mxs_reverb_large", 0.5f);
		oc.Enable("LiftShaft");
		oc.Enable("Stairwell");
	}

	private void evt_lift_main1_exit()
	{
		oc.Stop("LiftMain1");
		am.StopAudio("amb_original_bendy_loop");
	}

	private void evt_floor2_enter()
	{
		oc.Play("Level_11");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_original_bendy_loop");
		am.ToSnapshot("CH3Defaults", "mxs_level_11", 0.5f);
	}

	private void evt_floor2_exit()
	{
		oc.Stop("Level_11");
		am.StopAudio("amb_closeup_loop");
		am.StopAudio("amb_original_bendy_loop");
	}

	private void evt_floor3_enter()
	{
		oc.Play("Level_P");
		am.PlayAudio("amb_industrial_loop");
		am.PlayAudio("amb_original_bendy_loop");
		am.ToSnapshot("CH3Defaults", "mxs_level_p", 0.5f);
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 0.5f);
		oc.Enable("Level_P_Lab");
	}

	private void evt_floor3_exit()
	{
		oc.Stop("Level_P");
		am.StopAudio("amb_industrial_loop");
		am.StopAudio("amb_original_bendy_loop");
		oc.Disable("Level_P_Lab");
	}

	private void evt_floor3_lab1_enter()
	{
		oc.Play("Level_P_Lab");
		am.ToSnapshot("CH3Defaults", "mxs_level_p_lab", 2f);
		am.ToSnapshot("S13BaseMixer", "mxs_reverb_corridor", 0.5f);
	}

	private void evt_floor3_lab1_exit()
	{
		oc.Stop("Level_P_Lab");
		am.ToSnapshot("CH3Defaults", "mxs_level_p", 0.5f);
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 0.5f);
	}

	private void evt_floor3_lab2_enter()
	{
		am.ToSnapshot("CH3Defaults", "mxs_level_p_lab", 2f);
	}

	private void evt_floor3_lab2_exit()
	{
		am.ToSnapshot("CH3Defaults", "mxs_level_p", 0.5f);
	}

	private void evt_floor4_enter()
	{
		oc.Play("Main");
		am.PlayAudio("amb_original_horror_loop");
		am.PlayAudio("amb_heavy_loop");
		am.PlayAudio("amb_closeup_loop");
		am.ToSnapshot("CH3Defaults", "mxs_level_9", 0.5f);
		oc.Enable("Hallway");
	}

	private void evt_floor4_exit()
	{
		oc.Stop("Main");
		am.StopAudio("amb_original_horror_loop");
		am.StopAudio("amb_heavy_loop");
		am.StopAudio("amb_closeup_loop");
	}

	private void evt_floor4_hall_enter()
	{
		oc.Play("Hallway");
		am.PlayAudio("amb_original_horror_loop");
		am.PlayAudio("amb_heavy_loop");
		am.PlayAudio("amb_closeup_loop");
		am.ToSnapshot("CH3Defaults", "mxs_level_9_hall", 0.5f);
		oc.Enable("AlicesLair");
	}

	private void evt_floor4_hall_exit()
	{
		oc.Stop("Hallway");
	}

	private void evt_aliceslair_enter()
	{
		oc.Play("AlicesLair");
		am.PlayAudio("amb_ink_flood_loop");
		am.PlayAudio("amb_industrial_loop");
		am.StopAudio("amb_heavy_loop");
		am.StopAudio("amb_closeup_loop");
		am.ToSnapshot("CH3Defaults", "mxs_level_9_alice", 0.5f);
		oc.Enable("TortureRoom");
	}

	private void evt_aliceslair_exit()
	{
		oc.Stop("AlicesLair");
		am.StopAudio("amb_ink_flood_loop");
		am.StopAudio("amb_industrial_loop");
	}

	private void evt_tortureroom_enter()
	{
		oc.Play("TortureRoom");
		am.PlayAudio("amb_heavy_loop");
		am.PlayAudio("amb_closeup_loop");
		am.ToSnapshot("CH3Defaults", "mxs_level_9_torture", 0.5f);
	}

	private void evt_tortureroom_exit()
	{
		oc.Stop("TortureRoom");
	}

	private void evt_floor5_common_enter()
	{
		oc.Play("Level_14_Common");
		oc.Enable("Level_14_Main");
		oc.Enable("InkFlood");
		oc.Enable("Labyrinth");
	}

	private void evt_floor5_common_exit()
	{
		oc.Stop("Level_14_Common");
	}

	private void evt_floor5_enter()
	{
		oc.Play("Level_14_Main");
		am.ToSnapshot("CH3Defaults", "mxs_level_14_main", 0.5f);
	}

	private void evt_floor5_exit()
	{
		oc.Stop("Level_14_Main");
	}

	private void evt_inkflood_enter()
	{
		oc.Play("InkFlood");
		am.PlayAudio("amb_ink_flood_loop");
		am.ToSnapshot("CH3Defaults", "mxs_level_14_inkflood", 0.5f);
	}

	private void evt_inkflood_exit()
	{
		oc.Stop("InkFlood");
		am.StopAudio("amb_ink_flood_loop");
	}

	private void evt_labyrinth_enter()
	{
		oc.Play("Labyrinth");
		am.ToSnapshot("CH3Defaults", "mxs_level_14_labyrinth", 0.5f);
	}

	private void evt_labyrinth_exit()
	{
		oc.Stop("Labyrinth");
	}

	private void evt_liftshaft_enter()
	{
		oc.Play("LiftShaft");
	}

	private void evt_liftshaft_exit()
	{
		oc.Stop("LiftShaft");
	}

	private void evt_ch3stairwells_enter()
	{
		oc.Play("Stairwell");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_original_horror_loop");
		am.ToSnapshot("CH3Defaults", "mxs_stairwell", 1f);
		am.ToSnapshot("S13BaseMixer", "mxs_reverb_corridor", 0.5f);
	}

	private void evt_ch3stairwells_exit()
	{
		oc.Stop("Stairwell");
		am.StopAudio("amb_closeup_loop");
		am.StopAudio("amb_original_horror_loop");
	}

	private void evt_ch3stairwells_exit_to_level9()
	{
		oc.Stop("Stairwell");
		evt_floor4_enter();
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 0.5f);
	}

	private void evt_ch3stairwells_enter_from_level9()
	{
		oc.Play("Stairwell");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_original_horror_loop");
		am.StopAudio("amb_heavy_loop");
		am.ToSnapshot("CH3Defaults", "mxs_stairwell", 1f);
		am.ToSnapshot("S13BaseMixer", "mxs_reverb_corridor", 0.5f);
	}

	private void evt_finale_enter()
	{
		oc.Play("Finale");
		am.ToSnapshot("CH3Defaults", "mxs_finale", 0.5f);
		am.ToSnapshot("S13BaseMixer", "mxs_reverb_cavern", 0.5f);
	}

	private void evt_finale_exit()
	{
		oc.Stop("Finale");
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 0.5f);
	}

	private void evt_ch3_arrive_at_floor_1()
	{
		oc.Enable("Level_K");
		oc.Disable("Level_11");
	}

	private void evt_ch3_arrive_at_floor_2()
	{
		oc.Enable("Level_11");
		oc.Disable("Level_K");
		oc.Disable("Level_P");
	}

	private void evt_ch3_arrive_at_floor_3()
	{
		oc.Enable("Level_P");
		oc.Disable("Level_11");
		oc.Disable("Level_9");
	}

	private void evt_ch3_arrive_at_floor_4()
	{
		oc.Enable("Level_9");
		oc.Disable("Level_P");
		oc.Disable("Level_14");
	}

	private void evt_ch3_arrive_at_floor_5()
	{
		oc.Enable("Level_14");
		oc.Disable("Level_9");
	}

	private void evt_CH3_save_point_01()
	{
		evt_ch3_arrive_at_floor_1();
		oc.Disable("Safehouse");
	}

	private void evt_CH3_save_point_02()
	{
		evt_darkhallway_exit();
		evt_heavenlytoys_enter();
	}

	private void evt_CH3_save_point_03()
	{
		am.InvokeEvent("evt_workshop_enter", 3f);
	}

	private void evt_CH3_save_point_04()
	{
		evt_alice_reveal_complete();
	}

	private void evt_CH3_save_point_05()
	{
		oc.Destroy("ChoicesAngel");
		evt_choices_enter();
	}

	private void evt_CH3_save_point_06()
	{
		evt_choices_enter();
	}

	private void evt_CH3_save_point_07()
	{
		oc.Enable("TrailerRoom");
		oc.Enable("LiftMain1");
		evt_lift_hallways_enter();
	}

	private void evt_CH3_save_point_08()
	{
		evt_lift_main1_enter();
	}

	private void evt_CH3_save_point_09()
	{
		oc.Disable("Level_K");
		evt_floor4_enter();
		evt_lift_main1_exit();
	}

	private void evt_CH3_save_point_10()
	{
		evt_floor4_exit();
	}

	private void evt_CH3_save_point_11()
	{
		evt_floor4_enter();
	}

	private void evt_CH3_save_point_12()
	{
	}

	private void evt_CH3_save_point_13()
	{
	}

	private void evt_player_hit_by_boris()
	{
		am.PlayAudio("sfx_player_hit_by_boris");
		am.PlayAudio("vo_henry_injured");
	}

	private void evt_boris_death_melt()
	{
		am.PlayAudio("sfx_boris_death_melt");
		am.ToSnapshot("CharacterAnimations", "mxs_boris_death", 14f);
		am.StopAudioDelayed("sfx_boris_death_melt", 14f);
	}

	private void evt_bert_boss_startup()
	{
		am.ToSnapshot("CharacterAnimations", "mxs_bert_startup", 0f);
		am.PlayAudio("sfx_bert_boss_startup");
		am.PlayAudioDelayed("sfx_bert_hub_spin_start", 2f);
		am.StopAudioDelayed("sfx_bert_hub_spin_start", 14f);
		am.ToSnapshot("CharacterAnimations", "mxs_base", 17f);
		am.ToSnapshot("RideStorage", "mxs_boss_fight", 6f);
	}

	private void evt_bert_boss_head_reveal()
	{
		am.ToSnapshot("TMGAudioMixer", "mxs_bert_boss", 30f);
	}

	private void evt_bert_hub_turn_start()
	{
		am.StopAudio("sfx_bert_hub_idle");
		am.PlayAudio("sfx_bert_hub_spinning");
		am.PlayAudio("sfx_bert_hub_chunk");
	}

	private void evt_bert_hub_turn_stop()
	{
		am.PlayAudio("sfx_bert_hub_idle");
		am.StopAudio("sfx_bert_hub_spinning");
		am.PlayAudio("sfx_bert_hub_chunk");
	}

	private void evt_bert_arms_tired()
	{
		am.ToSnapshot("CharacterAnimations", "mxs_bert_tired", 3f);
	}

	private void evt_bert_arms_restored()
	{
		am.ToSnapshot("CharacterAnimations", "mxs_base", 3f);
	}

	private void evt_bert_arm1_dead()
	{
		am.PlayAudio("sfx_bert_arm_debris");
	}

	private void evt_bert_arm2_dead()
	{
		am.PlayAudio("sfx_bert_arm_debris");
	}

	private void evt_bert_arm3_dead()
	{
		am.PlayAudio("sfx_bert_arm_debris");
	}

	private void evt_bert_arm4_dead()
	{
		am.PlayAudio("sfx_bert_arm_debris");
	}

	private void evt_bert_boss_final_freakout()
	{
		am.PlayAudioDelayed("sfx_bert_boss_finale", 1.5f);
		am.ToSnapshot("TMGAudioMixer", "mxs_base", 10f);
	}

	private void evt_bert_boss_defeated()
	{
		am.StopAudio("sfx_bert_hub_idle");
		am.ToSnapshot("RideStorage", "mxs_base", 2f);
	}

	private void evt_startinglift_sideroom_opened()
	{
		am.PlayAudio("amb_side_room_whispers", 8f);
		am.ToSnapshot("StartingLift", "DoorOpened", 0.5f);
		am.ToSnapshot("StartingLift", "DoorOpenedDestination", 6f);
		am.StopAudioDelayed("amb_side_room_rumble", 8f);
	}

	private void evt_accounting_exit_door_open()
	{
		am.PlayAudio("sfx_bridge_blend_loop");
	}

	private void evt_stage_entry_door_open()
	{
		am.PlayAudio("amb_stage_blend_loop");
	}

	private void evt_swolen_searcher_appears()
	{
		am.PlayAudio("sfx_searcher_ink_burble");
		am.PlayAudio("sfx_searcher_ink_bubbles");
		am.PlayAudio("sfx_searcher_ink_splash");
	}

	private void evt_ink_collected()
	{
		am.PlayAudio("sfx_ink_in_hand_loop");
	}

	private void evt_ink_deposited()
	{
		am.StopAudio("sfx_ink_in_hand_loop");
	}

	private void evt_ink_pipe_opens()
	{
		am.PlayAudio("sfx_ink_bath_pipe_opens");
		am.ToSnapshot("Bridge", "BaseMix", 6f);
	}

	private void evt_headbanger_mixtrap_enter()
	{
		am.ToSnapshot("CharacterAnimations", "mxs_headbanger_mix", 2f);
	}

	private void evt_headbanger_mixtrap_exit()
	{
		am.ToSnapshot("CharacterAnimations", "mxs_base", 2f);
	}

	private void evt_warehouse_door_open()
	{
		am.PlayAudio("sfx_warehouse_turns_on");
	}

	private void evt_haunted_house_start()
	{
		am.PlayAudio("sfx_haunted_house_powerup", 2f);
		am.PlayAudio("sfx_creepy_laugh");
		am.PlayAudio("sfx_haunted_house_running_ink");
		am.PlayAudio("sfx_haunted_house_running_tracks");
		am.ToSnapshot("TMGAudioMixer", "mxs_alice_monologues", 2f);
	}

	private void evt_haunted_house_cart_start()
	{
		am.StopAudio("sfx_creepy_laugh");
		am.PlayAudio("sfx_haunted_house_cart_loop");
		am.PlayAudio("sfx_haunted_house_cart_start");
	}

	private void evt_haunted_house_cart_stop()
	{
		am.StopAudio("sfx_haunted_house_cart_loop");
	}

	private void evt_haunted_house_cart_smashed()
	{
		am.PlayAudio("sfx_haunted_house_cart_smash");
		am.StopAudio("sfx_haunted_house_running_ink");
		am.StopAudio("sfx_haunted_house_running_tracks");
		am.ToSnapshot("TMGAudioMixer", "mxs_alice_monologues_finalCH4", 1f);
	}

	private void evt_default_env_enter()
	{
		am.PlayAudio("amb_original_ambience_loop");
	}

	private void evt_default_env_exit()
	{
		am.StopAudio("amb_original_ambience_loop");
	}

	private void evt_starting_lift_enter()
	{
		am.PlayAudio("amb_side_room_rumble");
		oc.Play("StartingLift");
	}

	private void evt_starting_lift_exit()
	{
		oc.Stop("StartingLift");
	}

	private void evt_archives_room_enter()
	{
		oc.Play("Archives");
		am.StopAudio("amb_stage_blend_loop");
		oc.Enable("Bridge");
	}

	private void evt_archives_room_exit()
	{
		oc.Stop("Archives");
	}

	private void evt_bridge_enter()
	{
		oc.Play("Bridge");
		oc.Enable("SpiralStairs");
		oc.Enable("Holding");
		oc.Destroy("StartingLift");
	}

	private void evt_bridge_exit()
	{
		oc.Stop("Bridge");
	}

	private void evt_spiral_stairs_enter()
	{
		oc.Play("SpiralStairs");
		am.ToSnapshot("TMGAudioMixer", "mxs_alice_monologues", 1f);
	}

	private void evt_spiral_stairs_exit()
	{
		oc.Stop("SpiralStairs");
		am.ToSnapshot("TMGAudioMixer", "mxs_base", 1f);
	}

	private void evt_holding_room_enter()
	{
		oc.Play("Holding");
		oc.Enable("Vent");
		oc.Destroy("Archives");
		oc.Destroy("Bridge");
	}

	private void evt_holding_room_exit()
	{
		oc.Stop("Holding");
	}

	private void evt_holding_room2_enter()
	{
		am.ToSnapshot("Holding", "room2", 1f);
	}

	private void evt_holding_room2_exit()
	{
		am.ToSnapshot("Holding", "room1", 1f);
	}

	private void evt_vent_enter()
	{
		am.PlayAudio("sfx_vent_enter");
		oc.Play("Vent");
		oc.Enable("MapRoom");
	}

	private void evt_vent_exit()
	{
		am.PlayAudio("sfx_vent_exit");
		oc.Play("MapRoom");
		oc.Stop("Vent");
		oc.Enable("Warehouse");
		oc.Destroy("SpiralStairs");
		oc.Destroy("Holding");
	}

	private void evt_map_room_enter()
	{
		oc.Play("MapRoom");
		am.ToSnapshot("PropsStatic", "maproom", 2f);
	}

	private void evt_map_room_exit()
	{
		oc.Stop("MapRoom");
		am.ToSnapshot("PropsStatic", "default", 2f);
	}

	private void evt_warehouse_enter()
	{
		oc.Play("Warehouse");
		oc.Destroy("Vent");
		oc.Enable("ResearchAndDesign");
		oc.Enable("ResearchAndDesign_Lower");
		oc.Enable("RideStorage");
		oc.Enable("Maintenance");
		oc.Enable("HauntedHouse");
	}

	private void evt_warehouse_exit()
	{
		oc.Stop("Warehouse");
	}

	private void evt_research_and_design_upper_enter()
	{
		oc.Play("ResearchAndDesign");
	}

	private void evt_research_and_design_upper_exit()
	{
		oc.Stop("ResearchAndDesign");
	}

	private void evt_research_and_design_mix_enter()
	{
		am.ToSnapshot("ResearchAndDesign", "mxs_upper_occlusion", 3f);
	}

	private void evt_research_and_design_mix_exit()
	{
		am.ToSnapshot("ResearchAndDesign", "mxs_base", 3f);
	}

	private void evt_research_and_design_lower_enter()
	{
		oc.Play("ResearchAndDesign_Lower");
	}

	private void evt_research_and_design_lower_exit()
	{
		oc.Stop("ResearchAndDesign_Lower");
	}

	private void evt_ride_storage_enter()
	{
		oc.Play("RideStorage");
	}

	private void evt_ride_storage_exit()
	{
		oc.Stop("RideStorage");
	}

	private void evt_maintenance_enter()
	{
		am.PlayAudio("amb_original_horror_loop");
		oc.Play("Maintenance");
	}

	private void evt_maintenance_exit()
	{
		am.StopAudio("amb_original_horror_loop");
		oc.Stop("Maintenance");
	}

	private void evt_maintenance_upper_enter()
	{
		am.ToSnapshot("Maintenance", "upper", 3f);
	}

	private void evt_maintenance_upper_exit()
	{
		am.ToSnapshot("Maintenance", "lower", 2f);
	}

	private void evt_haunted_house_enter()
	{
		am.PlayAudio("amb_original_horror_loop");
		oc.Play("HauntedHouse");
		oc.Enable("Ballroom");
	}

	private void evt_haunted_house_exit()
	{
		am.StopAudio("amb_original_horror_loop");
		oc.Stop("HauntedHouse");
	}

	private void evt_ballroom_enter()
	{
		am.StopAudio("amb_original_ambience_loop");
		oc.Play("Ballroom");
		oc.Destroy("MapRoom");
		oc.Destroy("ResearchAndDesign");
		oc.Destroy("ResearchAndDesign_Lower");
		oc.Destroy("Maintenance");
		oc.Destroy("RideStorage");
	}

	private void evt_ballroom_exit()
	{
		oc.Stop("Ballroom");
		oc.Destroy("HauntedHouse");
	}

	private void evt_CH4_save_point_01()
	{
		oc.Stop("StartingLift");
	}

	private void evt_CH4_save_point_02()
	{
		evt_CH4_save_point_01();
		oc.Destroy("StartingLift");
		oc.Destroy("Archives");
	}

	private void evt_CH4_save_point_03()
	{
		evt_CH4_save_point_02();
	}

	private void evt_CH4_save_point_04()
	{
		evt_CH4_save_point_03();
		oc.Play("Vent");
		oc.Enable("MapRoom");
		oc.Destroy("SpiralStairs");
	}

	private void evt_CH4_save_point_05()
	{
		evt_CH4_save_point_04();
		oc.Stop("Vent");
		oc.Play("MapRoom");
	}

	private void evt_CH4_save_point_06()
	{
		oc.Destroy("Vent");
	}

	private void evt_CH4_save_point_07()
	{
		evt_CH4_save_point_06();
		evt_warehouse_enter();
		oc.Stop("MapRoom");
	}

	private void evt_CH4_save_point_08()
	{
		oc.Stop("Warehouse");
		oc.Play("Warehouse");
		oc.Enable("ResearchAndDesign");
		oc.Enable("ResearchAndDesign_Lower");
	}

	private void evt_CH4_save_point_09()
	{
		oc.Stop("Warehouse");
		oc.Play("Warehouse");
		oc.Enable("RideStorage");
	}

	private void evt_CH4_save_point_10()
	{
		oc.Stop("Warehouse");
		oc.Play("Warehouse");
		oc.Enable("Maintenance");
	}

	private void evt_CH4_save_point_11()
	{
		oc.Stop("Warehouse");
		oc.Play("Warehouse");
	}

	private void evt_CH4_save_point_12()
	{
		evt_CH4_save_point_11();
	}

	private void evt_CH4_save_point_13()
	{
		evt_CH4_save_point_12();
	}

	private void evt_CH4_save_point_14()
	{
		evt_CH4_save_point_13();
	}

	private void evt_CH4_save_point_15()
	{
		evt_CH4_save_point_14();
	}

	private void evt_ch5_scene06_complete()
	{
		am.PlayAudio("sfx_scene07_rocks_crumble");
		am.PlayAudio("sfx_scene07_stones");
		am.PlayAudio("sfx_scene07_bendy_heartbeat");
	}

	private void evt_ch5_scene07_complete()
	{
		am.PlayAudio("sfx_scene07_shaking_loop");
	}

	private void evt_ch5_seeing_tool_active()
	{
		am.StopAudio("sfx_scene07_shaking_loop");
	}

	private void evt_ch5_boat_in_distance()
	{
		am.PlayAudio("sfx_boat_in_distance");
		am.StopAudio("sfx_scene07_rocks_crumble");
		am.StopAudio("sfx_scene07_stones");
		am.PlayAudio("sfx_boat_in_distance");
		am.PlayAudio("sfx_boat_launch");
	}

	private void evt_chute_brake_active()
	{
		am.PlayAudio("sfx_chutebrake_release_1");
		am.PlayAudio("sfx_chutebrake_release_2");
		am.PlayAudioDelayed("sfx_chutebrake_engage_1", 5f);
		am.PlayAudioDelayed("sfx_chutebrake_engage_2", 5f);
	}

	private void evt_boat_chute_slide1()
	{
		am.PlayAudio("sfx_chute_slide1");
	}

	private void evt_boat_chute_slide2()
	{
		am.PlayAudio("sfx_chute_slide2");
	}

	private void evt_ch5_abyss_fall()
	{
		am.PlayAudio("sfx_abyss_fall");
	}

	private void evt_ch5_joey_office_hiding()
	{
		am.PlayAudio("test_tone_beep");
	}

	private void evt_ch5_puzzle_piece_added()
	{
		am.PlayAudio("sfx_pipe_piece_added");
	}

	private void evt_film_vault_door_cleared()
	{
		am.PlayAudio("sfx_ink_drained_puzzle");
		oc.Destroy("VaultPuzzle_Flood", ignoreFades: false);
	}

	private void evt_ch5_joey_speaks()
	{
		am.LerpMixerProperty("CH5 Ambience by Section", "WhistleVolume", -80f, 0.2f);
		am.StopAudioDelayed("amb_joey_dishes", 6f);
	}

	private void evt_ch5_joey_kitchen_exit()
	{
		am.StopAudioDelayed("amb_joey_kitchen", 6f);
		am.StopAudio("amb_joey_whistle");
		am.PlayAudioDelayed("amb_chapter1_bake", 5.5f);
	}

	private void evt_ch5_joey_ketchen_resume()
	{
		am.PlayAudio("amb_joey_dishes");
		am.PlayAudio("amb_joey_kitchen");
	}

	private void evt_ch5_safehouse_enter()
	{
		oc.Play("Safehouse");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_bendy_loop");
		am.StopAudio("amb_wind_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_safehouse", 2.5f);
	}

	private void evt_ch5_safehouse_exit()
	{
		oc.Stop("Safehouse");
	}

	private void evt_ch5_caves_enter()
	{
		oc.Play("Caves");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_wind_loop");
		am.StopAudio("amb_bendy_loop");
		am.StopAudio("amb_tunnel_loop");
		am.StopAudio("amb_airy_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_caves", 4f);
	}

	private void evt_ch5_caves_exit()
	{
		oc.Stop("Caves");
	}

	private void evt_ch5_dock_enter()
	{
		oc.Play("Dock");
		am.StopAudio("amb_wind_loop");
		am.PlayAudio("amb_airy_loop");
		am.StopAudio("amb_closeup_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_dock", 2f);
	}

	private void evt_ch5_dock_exit()
	{
		oc.Stop("Dock");
	}

	private void evt_ch5_tunnels_enter()
	{
		oc.Play("Tunnels");
		am.PlayAudio("amb_tunnel_loop");
		am.PlayAudio("amb_flood_loop");
		am.StopAudio("amb_airy_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_tunnels", 2f);
	}

	private void evt_ch5_tunnels_exit()
	{
		oc.Stop("Tunnels");
		am.StopAudio("amb_flood_loop");
	}

	private void evt_ch5_lost_harbour_enter()
	{
		oc.Play("LostHarbour");
		am.PlayAudio("amb_airy_loop");
		am.PlayAudio("amb_deep_loop");
		am.StopAudio("amb_tunnel_loop");
		am.StopAudio("amb_noisy_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_lost_harbour", 2f);
	}

	private void evt_ch5_lost_harbour_exit()
	{
		oc.Stop("LostHarbour");
		am.StopAudio("amb_airy_loop");
	}

	private void evt_ch5_abyss_enter()
	{
		oc.Play("Abyss");
		am.PlayAudio("amb_deep_loop");
		am.PlayAudio("amb_noisy_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_abyss", 2f);
	}

	private void evt_ch5_abyss_exit()
	{
		oc.Stop("Abyss");
	}

	private void evt_ch5_administration_enter()
	{
		oc.Play("Administration");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_noisy_loop");
		am.StopAudio("amb_deep_loop");
		am.StopAudio("amb_industrial_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_administration", 4f);
	}

	private void evt_ch5_administration_exit()
	{
		oc.Stop("Administration");
	}

	private void evt_ch5_joeys_office_enter()
	{
		oc.Play("JoeysOffice");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_tunnel_loop");
		am.StopAudio("amb_noisy_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_joeys_office", 2f);
	}

	private void evt_ch5_joeys_office_exit()
	{
		oc.Stop("JoeysOffice");
		am.StopAudio("amb_tunnel_loop");
	}

	private void evt_ch5_vault_puzzle_enter()
	{
		oc.Play("VaultPuzzle");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_noisy_loop");
		am.PlayAudio("amb_industrial_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_vault_puzzle", 2f);
	}

	private void evt_ch5_vault_puzzle_exit()
	{
		oc.Stop("VaultPuzzle");
	}

	private void evt_ch5_vault_enter()
	{
		oc.Play("Vault");
		am.PlayAudio("amb_closeup_loop");
		am.PlayAudio("amb_noisy_loop");
		am.PlayAudio("amb_industrial_loop");
		am.StopAudio("amb_horror_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_vault", 2f);
	}

	private void evt_ch5_vault_exit()
	{
		oc.Stop("Vault");
	}

	private void evt_ch5_back_hall_enter()
	{
		oc.Play("BackHall");
		am.PlayAudio("amb_horror_loop");
		am.PlayAudio("amb_industrial_loop");
		am.StopAudio("amb_closeup_loop");
		am.StopAudio("amb_noisy_loop");
		am.StopAudio("amb_airy_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_back_hall", 2f);
	}

	private void evt_ch5_back_hall_exit()
	{
		oc.Stop("BackHall");
	}

	private void evt_ch5_giant_ink_machine_enter()
	{
		oc.Play("GiantInkMachine");
		am.PlayAudio("amb_bendy_loop");
		am.PlayAudio("amb_airy_loop");
		am.StopAudio("amb_industrial_loop");
		am.StopAudio("amb_horror_loop");
		am.LerpMixerProperty("CH5 Ambience by Section", "SteamVolume", 0f, 2f, ignoreTimeScale: true);
		am.ToSnapshot("CH5 Defaults", "mxs_giant_ink_machine", 2f);
	}

	private void evt_ch5_giant_ink_machine_exit()
	{
		oc.Stop("GiantInkMachine");
		am.PlayAudio("amb_industrial_loop");
		am.StopAudio("amb_bendy_loop");
		am.StopAudio("amb_airy_loop");
		am.LerpMixerProperty("CH5 Ambience by Section", "SteamVolume", -24f, 2f, ignoreTimeScale: true);
		am.ToSnapshot("CH5 Defaults", "mxs_throne_room", 2f);
	}

	private void evt_ch5_machine_interior_enter()
	{
		oc.Play("MachineInterior");
	}

	private void evt_ch5_machine_interior_exit()
	{
		oc.Stop("MachineInterior");
	}

	private void evt_ch5_throne_room_enter()
	{
		oc.Play("ThroneRoom");
		am.PlayAudio("amb_industrial_loop");
		am.PlayAudio("amb_wind_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_throne_room", 2f);
	}

	private void evt_ch5_throne_room_exit()
	{
		oc.Stop("ThroneRoom");
		am.StopAudio("amb_industrial_loop");
		am.StopAudio("amb_wind_loop");
	}

	private void evt_ch5_bendy_arena_enter()
	{
		oc.Play("BendyArena");
		am.PlayAudio("amb_industrial_loop");
		am.PlayAudio("amb_deep_loop");
		am.ToSnapshot("CH5 Defaults", "mxs_bendy_arena", 2f);
	}

	private void evt_ch5_bendy_arena_exit()
	{
		oc.Stop("BendyArena");
		am.StopAudio("amb_industrial_loop");
		am.StopAudio("amb_deep_loop");
	}

	private void ResetMixers(int chapter = 0)
	{
		am.ToSnapshot("TMGAudioMixer", "mxs_base", 0f);
		am.ToSnapshot("S13BaseMixer", "mxs_reset", 0f);
		am.ToSnapshot("CharacterAnimations", "mxs_base", 0f);
		am.ToSnapshot("PropStatic", "default", 0f);
		switch (chapter)
		{
		case 1:
			am.ToSnapshot("CH1 Ambience by Section", "mxs_base", 0f);
			am.ToSnapshot("CH1Defaults", "mxs_base", 0f);
			am.ToSnapshot("CH1ScriptedEvents", "Base", 0f);
			break;
		case 2:
			am.ToSnapshot("CH2 Ambience by Section", "mxs_default", 0f);
			am.ToSnapshot("CH2Defaults", "Opening", 0f);
			am.ToSnapshot("Sewers", "Base", 0f);
			break;
		case 3:
			am.ToSnapshot("CH3 Ambience by Section", "mxs_ambience_base", 0f);
			am.ToSnapshot("CH3Defaults", "mxs_base", 0f);
			am.ToSnapshot("CH3Workshop", "1Active", 0f);
			break;
		case 4:
			am.ToSnapshot("CH4 Ambience by Section", "mxs_ambience_base", 0f);
			am.ToSnapshot("Bridge", "BathMuted", 0f);
			am.ToSnapshot("Holding", "room1", 0f);
			am.ToSnapshot("Maintenance", "upper", 0f);
			am.ToSnapshot("ResearchAndDesign", "mxs_base", 0f);
			am.ToSnapshot("RideStorage", "mxs_base", 0f);
			am.ToSnapshot("StartingLift", "BaseMix", 0f);
			break;
		case 5:
			am.ToSnapshot("CH5 Ambience by Section", "mxs_base", 0f);
			am.ToSnapshot("CH5Defaults", "mxs_base", 0f);
			am.ToSnapshot("CH5ScriptedEvents", "mxs_base", 0f);
			break;
		default:
			Debug.Log((object)"No chapter selected, default mixers reset.", (Object)(object)((Component)this).gameObject);
			break;
		}
	}
}
