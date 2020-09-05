using System;
using ATL;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// TODO: Need to be refactored

[RequireComponent(typeof(AudioSource))]
public class MusicPlaybackController : MonoBehaviour
{
    [SerializeField] private AudioClip[] playlist;

    [Header("Playback params")]
    [SerializeField] private Button playTrackButton;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;
    
    [Header("Track info")]
    [SerializeField] private InputField trackTitleInputField;
    [SerializeField] private InputField trackAuthorInputField;
    [SerializeField] private Text trackDurationText;
    
    [Header("Audio configuration")]
    [SerializeField] private Slider audioTimeSlider;
    [SerializeField] private Slider audioVolumeSlider;
    [SerializeField] private Slider audioPitchSlider;
    [SerializeField] private Button audioVolumeButton;
    [SerializeField] private Button audioPitchButton;

    private Text _audioVolumeText;
    private Text _audioPitchText;

    private float _defaultAudioVolume;
    private float _defaultAudioPitch;
    
    private AudioSource _audioSource;
    private int _currentTrackIndex;
    private Track _currentTrack;
    private readonly int _exposureTimeToResetTrackInSeconds = 5;

    private bool _isTrackTimeSliderDragged;
    
    private void Start()
    {
        _audioVolumeText = audioVolumeButton.GetComponentInChildren<Text>();
        _audioPitchText = audioPitchButton.GetComponentInChildren<Text>();
        
        _defaultAudioVolume = audioVolumeSlider.value;
        _defaultAudioPitch = audioPitchSlider.value;
        
        _audioSource = GetComponent<AudioSource>();
        SetTrackInPlaylist(0);
    }
    
    private void Update()
    {
        if (_audioSource.isPlaying)
        {
            var currentDuration = TimeSpan.FromSeconds(_audioSource.time);
            var totalDuration = TimeSpan.FromSeconds(_audioSource.clip.length);

            trackDurationText.text = PrimitiveTypeExtensions.BuildStringInRelevantFormatWithSpans(currentDuration, totalDuration);
            audioTimeSlider.value = (float) (currentDuration.TotalSeconds / totalDuration.TotalSeconds);
        }
    }

    public void SetTrackTime(float value)
    {
        if (_isTrackTimeSliderDragged == false) return;
        _audioSource.time = value * _audioSource.clip.length;
    }

    public void OnBeginDrag(BaseEventData data)
    {
        _isTrackTimeSliderDragged = true;
    }

    public void OnEndDrag(BaseEventData data)
    {
        _isTrackTimeSliderDragged = false;
    }

    public void SetAudioVolume(float value)
    {
        _audioSource.volume = value;
        _audioVolumeText.fontStyle = Mathf.Approximately(_defaultAudioVolume, value) 
            ? FontStyle.Normal 
            : FontStyle.Bold;
    }

    public void SetAudioPitch(float value)
    {
        _audioSource.pitch = value;
        _audioPitchText.fontStyle = Mathf.Approximately(_defaultAudioPitch, value) 
            ? FontStyle.Normal 
            : FontStyle.Bold;
    }
    
    public void ResetAudioVolume()
    {
        audioVolumeSlider.value = _defaultAudioVolume;
    }

    public void ResetAudioPitch()
    {
        audioPitchSlider.value = _defaultAudioPitch;
    }
    
    public void SetPrevTrackInPlaylist()
    {
        var nextTrackIndex = _audioSource.time < _exposureTimeToResetTrackInSeconds
            ? _currentTrackIndex - 1
            : _currentTrackIndex;
        
        SetTrackInPlaylist(nextTrackIndex);
    }

    public void TogglePlayTrackInPlaylist()
    {
        if (_audioSource.isPlaying) PauseCurrentTrack();
        else PlayCurrentTrack();
    }

    public void SetNextTrackInPlaylist()
    {
        if (_currentTrackIndex == playlist.Length - 1)
        {
            SetTrackInPlaylist(0, false);
            return;
        }
        
        SetTrackInPlaylist(_currentTrackIndex + 1);
    }

    public void ChangeTrackTitle(string value)
    {
        _currentTrack.Title = value;
    }

    public void ChangeTrackAuthor(string value)
    {
        _currentTrack.Artist = value;
    }

    private void SetTrackInPlaylist(int trackIndex, bool play = true)
    {
        _currentTrackIndex = Mathf.Clamp(trackIndex, 0, playlist.Length - 1);;
        _audioSource.clip = playlist[_currentTrackIndex];
        StopCurrentTrack();
        if (play) PlayCurrentTrack();

        _currentTrack?.Save();

        _currentTrack = new Track(AssetDatabase.GetAssetPath(playlist[_currentTrackIndex]));
        trackTitleInputField.text = _currentTrack.Title;
        trackAuthorInputField.text = _currentTrack.Artist;
    }

    private void PlayCurrentTrack()
    {
        _audioSource.Play();
        playTrackButton.image.sprite = pauseSprite;
    }

    private void PauseCurrentTrack()
    {
        _audioSource.Pause();
        playTrackButton.image.sprite = playSprite;
    }

    private void StopCurrentTrack()
    {
        _audioSource.Stop();
        _audioSource.time = 0;
        playTrackButton.image.sprite = playSprite;
    }
}
