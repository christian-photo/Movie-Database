#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.MovieSpace;
using MovieDatabase.MVVM;
using MovieDatabase.MVVM.Model;
using MovieDatabase.MVVM.ViewModel;
using MovieDatabase.Util;
using Serilog;
using System;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace MovieDatabase.Core.Utility.EditPopup
{
    /// <summary>
    /// Interaktionslogik für EditPopup.xaml
    /// </summary>
    public partial class EditPopup : UiWindow
    {
        readonly Movie mov;
        readonly IMovieInfoProvider movieInfoProvider;
        public EditPopup(Movie mov, IMovieInfoProvider infoProvider)
        {
            InitializeComponent();
            movieInfoProvider = infoProvider;
            this.mov = mov;
        }

        private async void CloseWindow(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(IMDB_Name.Text))
            {
                try
                {
                    Movie m = new Movie(mov.FilePath);
                    BaseMovieViewModel VM = (BaseMovieViewModel) MainViewModel.Instance.CurrentView;

                    if (!await m.GetInfo(movieInfoProvider, IMDB_Name.Text, false, VM.SelectedMovie.Info.Uuid))
                    {
                        return;
                    }

                    m.Info.Uuid = VM.SelectedMovie.Info.Uuid;
                    string oldTitle = VM.SelectedMovie.Info.Title;

                    VM.SelectedMovie.Cleanup();

                    VM.SelectedMovie.OverrideInfo(m.Info);

                    MovieControl b = VM.Collection[VM.SelectedIndex];
                    b.Title = m.Info.Title;
                    b.ImageSource = m.GetCover();

                    VM.SetSelectedMovie(m);

                    Log.Logger.Information($"Successfully edited movie from {oldTitle} to {m.Info.Title}");
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Something went wrong while editing the movie {mov.Info.Title}, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
                }
                Close();
            } else if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
