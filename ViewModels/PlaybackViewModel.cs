using System;
using System.Collections.Generic;
using Microsoft.UI.Dispatching;

namespace MPA.ViewModels
{
    public sealed class PlaybackViewModel : ObservableObject
    {
        private readonly DispatcherQueueTimer _timer;
        private readonly List<PlaybackTrack> _queue;
        private readonly Random _random = new();
        private int _currentIndex;

        private string _trackTitle = "";
        private string _trackSubtitle = "";
        private string _albumArtUri = "";
        private double _durationSeconds;
        private double _positionSeconds;
        private bool _isPlaying = true;
        private bool _isShuffleEnabled;
        private bool _isLiked;
        private RepeatMode _repeatMode = RepeatMode.All;
        private double _volume = 65;

        public PlaybackViewModel(DispatcherQueue dispatcherQueue)
        {
            _queue = new List<PlaybackTrack>
            {
                new("Time", "Hans Zimmer · Inception", "https://contents.quanghuy.dev/79D2427E-4988-4C6D-9A5A-09F4F8D2E12B_sk1.jpeg", TimeSpan.FromMinutes(4) + TimeSpan.FromSeconds(35)),
                new("Firewatch (Original Soundtrack)", "Chris Remo", "https://contents.quanghuy.dev/79EEE411-BF3C-4F63-BD5E-39C673FFA737_sk1.jpeg", TimeSpan.FromMinutes(51) + TimeSpan.FromSeconds(12)),
                new("Lust for Life", "Lana Del Rey", "https://contents.quanghuy.dev/73494CD3-B6D7-4931-8978-CD3E3C6EC7EF_sk1.jpeg", TimeSpan.FromMinutes(4) + TimeSpan.FromSeconds(24)),
                new("N? Th?n M?t Tr?ng (Mônangel)", "Bùi Lan H??ng", "https://contents.quanghuy.dev/118CD291-17C4-4E0E-B51C-D8504A57E4D5_sk1.jpeg", TimeSpan.FromMinutes(3) + TimeSpan.FromSeconds(11)),
            };

            TogglePlayPauseCommand = new RelayCommand(TogglePlayPause);
            NextCommand = new RelayCommand(PlayNext);
            PreviousCommand = new RelayCommand(PlayPrevious);
            CycleRepeatCommand = new RelayCommand(CycleRepeatMode);

            _timer = dispatcherQueue.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (_, _) => AdvancePlayback();

            SetActiveTrack(0);
            IsPlaying = true;
        }

        public RelayCommand TogglePlayPauseCommand { get; }

        public RelayCommand NextCommand { get; }

        public RelayCommand PreviousCommand { get; }

        public RelayCommand CycleRepeatCommand { get; }

        public string TrackTitle
        {
            get => _trackTitle;
            private set => SetProperty(ref _trackTitle, value);
        }

        public string TrackSubtitle
        {
            get => _trackSubtitle;
            private set => SetProperty(ref _trackSubtitle, value);
        }

        public string AlbumArtUri
        {
            get => _albumArtUri;
            private set => SetProperty(ref _albumArtUri, value);
        }

        public double DurationSeconds
        {
            get => _durationSeconds;
            private set
            {
                if (SetProperty(ref _durationSeconds, value))
                {
                    if (_positionSeconds > value)
                    {
                        SetProperty(ref _positionSeconds, value);
                        OnPropertyChanged(nameof(PositionLabel));
                    }

                    OnPropertyChanged(nameof(DurationLabel));
                }
            }
        }

        public double PositionSeconds
        {
            get => _positionSeconds;
            set
            {
                var clamped = value;
                if (DurationSeconds > 0)
                {
                    clamped = Math.Max(0, Math.Min(DurationSeconds, value));
                }

                if (SetProperty(ref _positionSeconds, clamped))
                {
                    OnPropertyChanged(nameof(PositionLabel));
                }
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (SetProperty(ref _isPlaying, value))
                {
                    OnPropertyChanged(nameof(PlayPauseGlyph));
                    if (value)
                    {
                        _timer.Start();
                    }
                    else
                    {
                        _timer.Stop();
                    }
                }
            }
        }

        public bool IsShuffleEnabled
        {
            get => _isShuffleEnabled;
            set => SetProperty(ref _isShuffleEnabled, value);
        }

        public bool IsLiked
        {
            get => _isLiked;
            set => SetProperty(ref _isLiked, value);
        }

        public RepeatMode RepeatMode
        {
            get => _repeatMode;
            set
            {
                if (SetProperty(ref _repeatMode, value))
                {
                    OnPropertyChanged(nameof(RepeatButtonOpacity));
                    OnPropertyChanged(nameof(RepeatGlyph));
                }
            }
        }

        public double Volume
        {
            get => _volume;
            set => SetProperty(ref _volume, Math.Max(0, Math.Min(100, value)));
        }

        public string PlayPauseGlyph => IsPlaying ? "\uE103" : "\uE102";

        public string RepeatGlyph => RepeatMode == RepeatMode.One ? "\uE1CC" : "\uE1CD";

        public double RepeatButtonOpacity => RepeatMode == RepeatMode.Off ? 0.45 : 1.0;

        public string PositionLabel => FormatTime(PositionSeconds);

        public string DurationLabel => FormatTime(DurationSeconds);

        private void TogglePlayPause() => IsPlaying = !IsPlaying;

        private void PlayNext()
        {
            if (_queue.Count == 0)
            {
                return;
            }

            if (IsShuffleEnabled)
            {
                var nextIndex = _random.Next(_queue.Count);
                _currentIndex = nextIndex;
            }
            else
            {
                _currentIndex = (_currentIndex + 1) % _queue.Count;
            }

            SetActiveTrack(_currentIndex);
        }

        private void PlayPrevious()
        {
            if (_queue.Count == 0)
            {
                return;
            }

            if (PositionSeconds > 5)
            {
                PositionSeconds = 0;
                return;
            }

            if (IsShuffleEnabled)
            {
                _currentIndex = _random.Next(_queue.Count);
            }
            else
            {
                _currentIndex = (_currentIndex - 1 + _queue.Count) % _queue.Count;
            }

            SetActiveTrack(_currentIndex);
        }

        private void CycleRepeatMode()
        {
            RepeatMode = RepeatMode switch
            {
                RepeatMode.Off => RepeatMode.All,
                RepeatMode.All => RepeatMode.One,
                _ => RepeatMode.Off,
            };
        }

        private void AdvancePlayback()
        {
            if (!IsPlaying || DurationSeconds <= 0)
            {
                return;
            }

            PositionSeconds += 1;
            if (PositionSeconds + 0.1 >= DurationSeconds)
            {
                if (RepeatMode == RepeatMode.One)
                {
                    PositionSeconds = 0;
                }
                else
                {
                    if (RepeatMode == RepeatMode.Off && _currentIndex == _queue.Count - 1)
                    {
                        IsPlaying = false;
                        PositionSeconds = DurationSeconds;
                    }
                    else
                    {
                        PlayNext();
                    }
                }
            }
        }

        private void SetActiveTrack(int index)
        {
            var track = _queue[index];
            _currentIndex = index;
            TrackTitle = track.Title;
            TrackSubtitle = track.Subtitle;
            AlbumArtUri = track.ImageUri;
            DurationSeconds = track.Duration.TotalSeconds;
            PositionSeconds = 0;
        }

        private static string FormatTime(double seconds)
        {
            if (double.IsNaN(seconds) || double.IsInfinity(seconds) || seconds < 0)
            {
                seconds = 0;
            }

            var ts = TimeSpan.FromSeconds(seconds);
            return ts.Hours > 0 ? ts.ToString("h\\:mm\\:ss") : ts.ToString("m\\:ss");
        }
    }

    public enum RepeatMode
    {
        Off,
        All,
        One
    }

    internal sealed record PlaybackTrack(string Title, string Subtitle, string ImageUri, TimeSpan Duration);
}
