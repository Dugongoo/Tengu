﻿using Avalonia.Collections;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tengu.Business.API;
using Tengu.Business.Commons;

namespace Tengu.ViewModels
{
    public class LatestEpisodesControlViewModel : ViewModelBase
    {
        private AvaloniaList<EpisodeModel> latestEpisodesList = new();
        private int latestEpisodesOffset = 0;
        private Hosts selectedHost = Hosts.AnimeSaturn;
        private int currentPage;

        private bool loading = false;
        private bool canPrev = false;


        #region Properties
        public ICommand CmdNextPage { get; private set; }
        public ICommand CmdPrevPage { get; private set; }
        
        public AvaloniaList<EpisodeModel> LatestEpisodesList
        {
            get => latestEpisodesList;
            set => this.RaiseAndSetIfChanged(ref latestEpisodesList, value);
        }
        public Hosts SelectedHost
        {
            get => selectedHost;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedHost, value);
                Initialize();
            }
        }
        public int LatestEpisodesOffset
        {
            get => latestEpisodesOffset;
            set
            {
                this.RaiseAndSetIfChanged(ref latestEpisodesOffset, value);
                CanPrev = value >= 10;

                CurrentPage = LatestEpisodesOffset / 10;
            }
        }
        public int CurrentPage
        {
            get => currentPage;
            set => this.RaiseAndSetIfChanged(ref currentPage, value);
        }
        public bool Loading
        {
            get => loading;
            set => this.RaiseAndSetIfChanged(ref loading, value);
        }
        public bool CanPrev
        {
            get => canPrev;
            set => this.RaiseAndSetIfChanged(ref canPrev, value);
        }
        #endregion

        public LatestEpisodesControlViewModel()
        {
            CmdNextPage = ReactiveCommand.Create(LatestNextPage);
            CmdPrevPage = ReactiveCommand.Create(LatestPrevPage);

            // TODO - SAVE IT
            //SelectedHost = Hosts.AnimeSaturn;
        }

        public void Initialize()
        {
            TenguApi.CurrentHosts = new Hosts[] { SelectedHost };

            LatestEpisodesOffset = 0;

            RefreshLatestEpisodes();
        }

        private async void RefreshLatestEpisodes()
        {
            Loading = true;

            try
            {
                LatestEpisodesList.Clear();

                foreach (EpisodeModel episode in await TenguApi.GetLatestEpisodeAsync(LatestEpisodesOffset, LatestEpisodesOffset + 10))
                {
                    LatestEpisodesList.Add(episode);
                }
            }
            finally
            {
                Loading = false;
            }
        }

        public void LatestNextPage()
        {
            LatestEpisodesOffset += 10;
            // log.Trace($"Next page >> {CurrentPage}");

            RefreshLatestEpisodes();
        }

        public void LatestPrevPage()
        {
            if (LatestEpisodesOffset >= 10)
            {
                LatestEpisodesOffset -= 10;
                // log.Trace($"Prev page >> {CurrentPage}");

                RefreshLatestEpisodes();
            }
        }
    }
}
