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
using System.Collections.Generic;
using System.Windows;
using MovieDatabase.Util;
using MovieDatabase.MVVM.View;
using System.Windows.Media;
using MovieDatabase.MovieSpace.Database;
using MovieDatabase.MVVM.Model;
using Serilog;

namespace MovieDatabase.MVVM.ViewModel
{
    public class SearchViewModel : BaseMovieViewModel
    {
        #region "Search Variables"
        private List<string> _searchList;
        public List<string> SearchList
        {
            get { return _searchList; }
            set
            {
                _searchList = value;
                OnPropertyChanged(nameof(SearchList));
            }
        }

        private int _selectedSearchIndex = 0;
        public int SelectedSearchIndex
        {
            get { return _selectedSearchIndex; }
            set
            {
                _selectedSearchIndex = value;
                OnPropertyChanged(nameof(SelectedSearchIndex));
            }
        }

        private string _searchContent;
        public string SearchContent
        {
            get { return _searchContent; }
            set
            {
                _searchContent = value;
                OnPropertyChanged(nameof(SearchContent));
            }
        }

        private Brush _borderColor = Brushes.Transparent;
        public Brush BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                OnPropertyChanged(nameof(BorderColor));
            }
        }
        #endregion

        public SearchViewModel(DatabaseMediator mediator) : base(mediator)
        {
            EditVisibility = Visibility.Hidden;
            SearchList = new List<string>
            {
                Loc.Instance[TranslationString.TITLE],
                Loc.Instance[TranslationString.ACTORS],
                Loc.Instance[TranslationString.YEAR],
                Loc.Instance[TranslationString.GENRES],
                Loc.Instance[TranslationString.IMDB],
                Loc.Instance[TranslationString.METASCORE]
            };

            Collection = new List<MovieControl>();
        }

        public void RaiseAllPropertiesChanged()
        {
            AllPropertiesChanged();
        }

        public void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchContent))
            {
                return;
            }
            Validate();
            List<Movie> movies = new List<Movie>();
            Log.Logger.Debug($"User searching for {SearchContent} in Category {SearchList[SelectedSearchIndex]}");
            if (SearchList[SelectedSearchIndex] == SearchList[0])
            {
                foreach (Movie movie in databaseMediator.GetCurrentDatabase().Movies)
                {
                    if (movie.Info.Title.ToLower().Contains(SearchContent.ToLower()))
                    {
                        movies.Add(movie);
                    }
                }

            }
            else if (SearchList[SelectedSearchIndex] == SearchList[1])
            {
                foreach (Movie movie in databaseMediator.GetCurrentDatabase().Movies)
                {
                    if (movie.Info.Actors.ToLower().Contains(SearchContent.ToLower()))
                    {
                        movies.Add(movie);
                    }
                }

            } 
            else if (SearchList[SelectedSearchIndex] == SearchList[2])
            {
                foreach (Movie movie in databaseMediator.GetCurrentDatabase().Movies)
                {
                    if (movie.Info.Year == SearchContent)
                    {
                        movies.Add(movie);
                    }
                }

            } 
            else if (SearchList[SelectedSearchIndex] == SearchList[3])
            {
                foreach (Movie movie in databaseMediator.GetCurrentDatabase().Movies)
                {
                    if (movie.Info.Genres.ToLower().Contains(SearchContent.ToLower()))
                    {
                        movies.Add(movie);
                    }
                }
            }
            else if (SearchList[SelectedSearchIndex] == SearchList[4] && float.TryParse(SearchContent, out float _))
            {
                foreach (Movie movie in databaseMediator.GetCurrentDatabase().Movies)
                {
                    if (float.Parse(movie.Info.ImDbRating) >= float.Parse(SearchContent) * 10)
                    {
                        movies.Add(movie);
                    }
                }
            }
            else if (SearchList[SelectedSearchIndex] == SearchList[5] && int.TryParse(SearchContent, out int _))
            {
                foreach (Movie movie in databaseMediator.GetCurrentDatabase().Movies)
                {
                    if (int.Parse(movie.Info.MetacriticRating) >= int.Parse(SearchContent))
                    {
                        movies.Add(movie);
                    }
                }
            }

            if (movies.Count == 0)
            {
                return;
            }
            Collection.Clear();
            foreach (Movie mov in movies)
            {
                AddMovieToView(mov.Info.Title, mov);
            }
            SearchView.Reload();
            SetSelectedMovie(movies[0]);
            SelectedChanged.Execute(null);
        }

        private void Validate()
        {
            if (SearchList[SelectedSearchIndex] == "ImDB")
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(SearchList[SelectedSearchIndex], @"^(?:(?:\d)|(?:\d\.[0-9]{0,1}))$"))
                {
                    BorderColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#AE3100");
                }
            }
            else if (SearchList[SelectedSearchIndex] == "MetaScore")
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(SearchList[SelectedSearchIndex], @"^\d{1,2}$"))
                {
                    BorderColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#AE3100");
                }
            }
            else if (SearchList[SelectedSearchIndex] == Loc.Instance[TranslationString.YEAR])
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(SearchList[SelectedSearchIndex], @"^\d{1,4}$"))
                {
                    BorderColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#AE3100");
                }
            }
        }
    }
}