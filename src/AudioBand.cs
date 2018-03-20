﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSDeskBand;
using CSDeskBand.Win;
using Svg;
using Size = System.Drawing.Size;

namespace AudioBand
{
    [Guid("957D8782-5B07-4126-9B24-1E917BAAAD64")]
    [ComVisible(true)]
    [CSDeskBandRegistration(Name = "Audio Band")]
    public partial class AudioBand : CSDeskBandWin
    {
        private const int FixedWidth = 250;
        private readonly int _maxHeight = CSDeskBandOptions.TaskbarHorizontalHeightLarge;
        private readonly int _minHeight = CSDeskBandOptions.TaskbarHorizontalHeightSmall;
        private readonly EnhancedProgressBar audioProgress = new EnhancedProgressBar();
        private readonly SvgDocument _playButtonSvg = SvgDocument.Open<SvgDocument>(new MemoryStream(Properties.Resources.play));
        private readonly SvgDocument _pauseButtonSvg = SvgDocument.Open<SvgDocument>(new MemoryStream(Properties.Resources.pause));
        private readonly SvgDocument _nextButtonSvg = SvgDocument.Open<SvgDocument>(new MemoryStream(Properties.Resources.next));
        private readonly SvgDocument _previousButtonSvg = SvgDocument.Open<SvgDocument>(new MemoryStream(Properties.Resources.previous));
        private readonly AudioBandViewModel _audioBandViewModel = new AudioBandViewModel();

        public AudioBand()
        {
            InitializeComponent();

            mainTable.Controls.Add(audioProgress, 0, 2);
            mainTable.SetColumnSpan(audioProgress, 2);
            audioProgress.Dock = DockStyle.Fill;
            audioProgress.ForeColor = Color.DodgerBlue;
            audioProgress.Location = new Point(0, 28);
            audioProgress.Margin = new Padding(0);
            audioProgress.Name = "audioProgress";
            audioProgress.Size = new Size(300, 2);
            audioProgress.TabIndex = 3;
            audioProgress.Value = 100;

            Options.Fixed = true;
            Options.Increment = 0;
            Options.Horizontal = Size = new Size(FixedWidth, _maxHeight);
            Options.MinHorizontal = MinimumSize = new Size(FixedWidth, _minHeight);
            Options.MaxHorizontal = MaximumSize = Size;

            SizeChanged += OnSizeChanged;
            playPauseButton.Click += PlayPauseButtonOnClick;
            previousButton.Click += PreviousButtonOnClick;
            nextButton.Click += NextButtonOnClick;
            _audioBandViewModel.PropertyChanged += AudioBandViewModelOnPropertyChanged;

            nowPlayingText.DataBindings.Add("Text", _audioBandViewModel, nameof(AudioBandViewModel.IsPlaying));
            albumArt.DataBindings.Add("Image", _audioBandViewModel, nameof(AudioBandViewModel.AlbumArt));
            audioProgress.DataBindings.Add("Value", _audioBandViewModel, nameof(AudioBandViewModel.AudioProgress));
        }

        private void AudioBandViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(AudioBandViewModel.IsPlaying):
                    DrawControlSvgs();
                    break;
                default: break;
            }
        }

        private void PlayPauseButtonOnClick(object sender, EventArgs eventArgs)
        {
            _audioBandViewModel.IsPlaying = !_audioBandViewModel.IsPlaying;
        }

        private void PreviousButtonOnClick(object sender, EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        private void NextButtonOnClick(object sender, EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            UpdateAlbumArtSize();
            DrawControlSvgs();
        }

        private void UpdateAlbumArtSize()
        {
            var height = mainTable.GetRowHeights().Take(2).Sum();
            mainTable.ColumnStyles[0].SizeType = SizeType.Absolute;
            mainTable.ColumnStyles[0].Width = height;
            _audioBandViewModel.AlbumArtSize = new Size(height, height);
        }

        private void DrawControlSvgs()
        {
            // Issues with svg
            const int padding = 3;
            var height = buttonsTable.GetRowHeights()[0] - padding;

            SvgDocument playPauseSvg = _audioBandViewModel.IsPlaying ? _pauseButtonSvg : _playButtonSvg;
            playPauseSvg.Width = playPauseButton.Width;
            playPauseSvg.Height = height;
            playPauseButton.Image = DrawSvg(playPauseSvg);

            _nextButtonSvg.Width = nextButton.Width;
            _nextButtonSvg.Height = height;
            nextButton.Image = DrawSvg(_nextButtonSvg);

            _previousButtonSvg.Width = previousButton.Width;
            _previousButtonSvg.Height = height;
            previousButton.Image = DrawSvg(_previousButtonSvg);
        }

        private Bitmap DrawSvg(SvgDocument svg)
        {
            var bmp = new Bitmap((int)svg.Width.Value, (int)svg.Height.Value);
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.High;
                svg.Draw(graphics);
                return bmp;
            }
        }
    }
}
