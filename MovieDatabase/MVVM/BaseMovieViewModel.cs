#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.Core;
using MovieDatabase.MovieSpace;
using MovieDatabase.MovieSpace.Database;
using MovieDatabase.MVVM.Model;
using MovieDatabase.MVVM.View;
using MovieDatabase.MVVM.ViewModel;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MovieDatabase.MVVM
{
    public class BaseMovieViewModel : ObservableObject
    {
        #region "Movie Variables"

        public RelayCommand SelectedChanged { get; set; }
        public RelayCommand PlayMovieCommand { get; set; }

        private ImageSource _image;
        public ImageSource Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        private Movie _selectedMovie;
        public Movie SelectedMovie
        {
            get { return _selectedMovie; }
            set
            {
                _selectedMovie = value;
                OnPropertyChanged(nameof(SelectedMovie));
            }
        }

        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _year;
        public string Year
        {
            get
            {
                if (string.IsNullOrEmpty(_year))
                {
                    return "";
                }
                return $"{Loc.Instance[TranslationString.YEAR]}: " + _year;
            }
            set
            {
                _year = value;
                OnPropertyChanged(nameof(Year));
            }
        }

        private string _runtime;
        public string Runtime
        {
            get
            {
                if (string.IsNullOrEmpty(_runtime))
                {
                    return "";
                }
                return $"{Loc.Instance[TranslationString.RUNTIME]}: " + _runtime;
            }
            set
            {
                _runtime = value;
                OnPropertyChanged(nameof(Runtime));
            }
        }

        private string _actors;
        public string Actors
        {
            get
            {
                if (string.IsNullOrEmpty(_actors))
                {
                    return "";
                }
                return $"{Loc.Instance[TranslationString.ACTORS]}: " + _actors;
            }
            set
            {
                _actors = value;
                OnPropertyChanged(nameof(Actors));
            }
        }

        private string _awards;
        public string Awards
        {
            get
            {
                if (string.IsNullOrEmpty(_awards))
                {
                    return "";
                }
                return $"{Loc.Instance[TranslationString.AWARDS]}: " + _awards;
            }
            set
            {
                _awards = value;
                OnPropertyChanged(nameof(Awards));
            }
        }

        private string _genres;
        public string Genres
        {
            get
            {
                if (string.IsNullOrEmpty(_genres))
                {
                    return "";
                }
                return $"{Loc.Instance[TranslationString.GENRES]}: " + _genres;
            }
            set
            {
                _genres = value;
                OnPropertyChanged(nameof(Genres));
            }
        }

        private string _imdbRating;
        public string IMDBRating
        {
            get
            {
                if (string.IsNullOrEmpty(_imdbRating))
                {
                    return "";
                }
                return $"{Loc.Instance[TranslationString.IMDB]}: " + _imdbRating;
            }
            set
            {
                _imdbRating = value;
                OnPropertyChanged(nameof(IMDBRating));
            }
        }

        private string _metaScore;
        public string MetaScore
        {
            get
            {
                if (string.IsNullOrEmpty(_metaScore))
                {
                    return "";
                }
                return $"{Loc.Instance[TranslationString.METASCORE]}: " + _metaScore;
            }
            set
            {
                _metaScore = value;
                OnPropertyChanged(nameof(MetaScore));
            }
        }

        private string _plot = Loc.Instance[TranslationString.CLICK_TO_SHOW_PLOT];
        public string Plot
        {
            get
            {
                return _plot;
            }
            set
            {
                _plot = value;
                OnPropertyChanged(nameof(Plot));
            }
        }

        private string _playText = Loc.Instance[TranslationString.PLAY_TEXT];
        public string PlayText
        {
            get { return _playText; }
            set
            {
                _playText = value;
                OnPropertyChanged(nameof(PlayText));
            }
        }

        private Visibility _editVisibility;
        public Visibility EditVisibility
        {
            get { return _editVisibility; }
            set
            {
                _editVisibility = value;
                OnPropertyChanged(nameof(EditVisibility));
            }
        }

        #endregion

        #region "Class Variables"

        private List<MovieControl> _collection { get; set; }

        public List<MovieControl> Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                OnPropertyChanged(nameof(Collection));
            }
        }

        private MovieControl lastBorder;

        protected readonly DatabaseMediator databaseMediator;

        #endregion

        public BaseMovieViewModel(DatabaseMediator mediator)
        {
            databaseMediator = mediator;

            Collection = new List<MovieControl>();
            SelectedChanged = new RelayCommand(o =>
            {
                if (SelectedIndex != -1)
                {
                    if (lastBorder != null)
                    {
                        lastBorder.IsSelected = false;
                    }

                    MovieControl b = Collection[SelectedIndex];

                    b.IsSelected = true;

                    lastBorder = b;

                    SetSelectedMovie(databaseMediator.GetCurrentDatabase().GetMovieByName(b.Title));
                } 
            });

            PlayMovieCommand = new RelayCommand(o => PlayMovie());
        }

        public void AddMovieToView(string Title, Movie movie)
        {
            MovieControl control = new MovieControl(Title, movie.GetCover()) 
            { 
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#0E3C4F"),
                Margin = new Thickness(25, 15, 25, 20),
                Foreground = Brushes.White
            };
            
            List<MovieControl> list = new List<MovieControl>();
            list.AddRange(Collection);
            list.Add(control);
            Collection = list;
            if (SelectedMovie != null) 
                SetSelectedMovie(SelectedMovie);

            Log.Logger.Verbose($"Added movie {Title} to view");
        }

        public void ClearView()
        {
            Log.Logger.Debug("Clearing movie view");
            Collection = new List<MovieControl>();

            SelectedMovie = null;

            Image = null;

            // Show Title
            Title = "";
            // Show Year
            Year = "";
            // Show Runtime
            Runtime = "";
            // Show Actors
            Actors = "";
            // Show awards
            Awards = "";
            // Show genres
            Genres = "";
            // Show IMDBRating
            IMDBRating = "";
            // Show MetaScore
            MetaScore = "";

            EditVisibility = Visibility.Hidden;
        }

        private void PlayMovie()
        {
            Log.Logger.Information($"Starting movie {SelectedMovie.Info.Title}");
            try
            {
                Process.Start(new ProcessStartInfo(SelectedMovie.FilePath) { UseShellExecute = true });
            } catch (Exception ex)
            {
                Log.Logger.Error($"Something went wrong while starting the movie, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
            }

        }

        public void SetSelectedMovie(Movie movie)
        {
            SelectedMovie = movie;

            Image = movie?.GetCover();

            // Show Title
            Title = movie.Info.Title;
            // Show Year
            Year = movie.Info.Year;
            // Show Runtime
            Runtime = movie.Info.RuntimeStr;
            // Show Actors
            Actors = movie.Info.Actors;
            // Show awards
            Awards = movie.Info.Awards;
            // Show genres
            Genres = movie.Info.Genres;
            // Show IMDBRating
            IMDBRating = movie.Info.ImDbRating;
            // Show MetaScore
            MetaScore = movie.Info.MetacriticRating;

            EditVisibility = Visibility.Visible;
        }
    }
}