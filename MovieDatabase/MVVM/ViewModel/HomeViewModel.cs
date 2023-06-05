#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.MovieSpace;
using MovieDatabase.MovieSpace.Database;
using MovieDatabase.MVVM.Model;
using System.Collections.Generic;
using System.Windows;

namespace MovieDatabase.MVVM.ViewModel
{
    public class HomeViewModel : BaseMovieViewModel
    {
        public HomeViewModel(DatabaseMediator mediator) : base(mediator)
        {
            EditVisibility = Visibility.Hidden;
            SelectedIndex = 0;

            if (Collection.Count == 0)
            {
                CreateMovieList();
            }
        }

        public void RaiseAllPropertiesChanged()
        {
            CreateMovieList();
            AllPropertiesChanged();
        }

        public void CreateMovieList()
        {
            Collection = new List<MovieControl>();

            foreach (Movie mov in databaseMediator.GetCurrentDatabase().Movies)
            {
                AddMovieToView(mov.Info.Title, mov);
            }

            if (databaseMediator.GetCurrentDatabase().Movies.Count != 0)
            {
                SelectedMovie = databaseMediator.GetCurrentDatabase().Movies[0];
                SetSelectedMovie(databaseMediator.GetCurrentDatabase().Movies[0]);
                SelectedChanged.Execute(null);
            }

            else
            {
                Image = null;

                // Clear Title
                Title = string.Empty;
                // Clear Year
                Year = string.Empty;
                // Clear Runtime
                Runtime = string.Empty;
                // Clear Actors
                Actors = string.Empty;
                // Clear awards
                Awards = string.Empty;
                // Clear genres
                Genres = string.Empty;
                // Clear IMDBRating
                IMDBRating = string.Empty;
                // Clear MetaScore
                MetaScore = string.Empty;

                EditVisibility = Visibility.Hidden;
            }
        }
    }
}