using UnityEngine;

namespace S13Audio;

public class GUISwitchLoopControl : MonoBehaviour
{
	public GameObject loopSwitchSetting;

	public Rect controlRect = new Rect(300f, 40f, 600f, 40f);

	private S13AudioManager _audioManager;

	private S13ObjectLoopSwitch _loopSwitch;

	private int _currentSegment;

	private int _numSegments;

	private bool _isPlaying;

	private void Start()
	{
		_audioManager = Object.FindObjectOfType<S13AudioManager>();
		if ((Object)(object)_audioManager == (Object)null)
		{
			Debug.LogError((object)(((Object)this).name + ": AudioManager not found."));
		}
		_loopSwitch = loopSwitchSetting.GetComponent<S13ObjectLoopSwitch>();
		_currentSegment = 0;
		_numSegments = _loopSwitch.audioSegments.Length;
		_isPlaying = _loopSwitch.playOnAwake;
	}

	private void OnGUI()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		GUI.BeginGroup(controlRect);
		GUI.Box(new Rect(0f, 0f, ((Rect)(ref controlRect)).width, ((Rect)(ref controlRect)).height), string.Empty);
		GUI.enabled = !_isPlaying;
		if (GUI.Button(new Rect(10f, 5f, 160f, 30f), "Play Music"))
		{
			_loopSwitch.Play();
			_isPlaying = true;
		}
		GUI.enabled = _isPlaying;
		if (GUI.Button(new Rect(220f, 5f, 160f, 30f), "Switch to next Track"))
		{
			_currentSegment = ((++_currentSegment < _numSegments) ? _currentSegment : 0);
			_loopSwitch.TransitionToSegment(_currentSegment);
		}
		if (GUI.Button(new Rect(430f, 5f, 160f, 30f), "Stop Music"))
		{
			_loopSwitch.Stop(ignoreFade: false);
			_isPlaying = false;
		}
		GUI.EndGroup();
	}
}
