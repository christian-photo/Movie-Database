#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.Core;
using MovieDatabase.MovieSpace.Database;
using MovieDatabase.MVVM.ViewModel;
using MovieDatabase.Util;
using Serilog;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace MovieDatabase
{
    public partial class MainWindow : UiWindow
    {
        public static ProgramInformations informations;
        public static DatabaseMediator databaseMediator;

        public MainWindow()
        {
            Directory.CreateDirectory(ApplicationConstants.DataPath);
            Directory.CreateDirectory(ApplicationConstants.LogFolder);
            Directory.CreateDirectory(ApplicationConstants.CoverFolder);
            Directory.CreateDirectory(ApplicationConstants.DatabaseFolder);

            informations = ProgramInformations.Create();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(informations.LogLevel)
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(ApplicationConstants.LogFolder, DateTime.Now.ToString("yyy-M-HH-mm-ss") + ".log"), outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            new Loc().Load();

            if (Loc.Instance.Languages.Contains(informations.Language))
            {
                Loc.Instance.SelectedLang = informations.Language;
            } else
            {
                Loc.Instance.SelectedLang = Loc.Instance.Languages[0];
            }
            if (Loc.Instance.IMDBLanguages.ContainsKey(informations.IMDBLanguage))
            {
                Loc.Instance.SelectedIMDBLang = informations.IMDBLanguage;
            } else
            {
                Loc.Instance.SelectedIMDBLang = "english";
            }

            databaseMediator = DatabaseMediator.Load();

            new MainViewModel(databaseMediator);

            InitializeComponent();

            if (string.IsNullOrWhiteSpace(informations.GetApiKey()))
            {
                APIKeyPrompt p = new()
                {
                    Topmost = true
                };
                p.ShowDialog();
            }

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Directory.CreateDirectory(ApplicationConstants.DataPath);

            databaseMediator.Save();

            informations.Save();
        }

        private void KeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                if (MainViewModel.Instance.HomeVM.SelectedIndex > 0 && MainViewModel.Instance.CurrentView == MainViewModel.Instance.HomeVM)
                {
                    MainViewModel.Instance.HomeVM.SelectedIndex -= 1;
                }
                else if (MainViewModel.Instance.SearchVM.SelectedIndex > 0 && MainViewModel.Instance.CurrentView == MainViewModel.Instance.SearchVM)
                {
                    MainViewModel.Instance.SearchVM.SelectedIndex -= 1;
                }
            }
            else if (e.Key == Key.Right)
            {
                if (MainViewModel.Instance.HomeVM.Collection.Count - 1 > MainViewModel.Instance.HomeVM.SelectedIndex && MainViewModel.Instance.CurrentView == MainViewModel.Instance.HomeVM)
                {
                    MainViewModel.Instance.HomeVM.SelectedIndex += 1;
                }
                else if (MainViewModel.Instance.SearchVM.Collection.Count - 1 > MainViewModel.Instance.SearchVM.SelectedIndex && MainViewModel.Instance.CurrentView == MainViewModel.Instance.SearchVM)
                {
                    MainViewModel.Instance.SearchVM.SelectedIndex += 1;
                }
            }
        }
    }
}
