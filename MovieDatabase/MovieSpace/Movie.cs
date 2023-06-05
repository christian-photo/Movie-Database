#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.Util;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MovieDatabase.MovieSpace
{
    public class Movie
    {
        public MovieInfo Info;
        public string FilePath;

        public Movie(string path)
        {
            FilePath = path;
        }

        public ImageSource GetCover()
        {
            if (Utility.IsConnectedToInternet() && string.IsNullOrWhiteSpace(Info.CoverPath))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(Info.Image);
                bitmapImage.EndInit();
                if (bitmapImage.CanFreeze)
                    bitmapImage.Freeze();

                return bitmapImage;
            }
            else if (!string.IsNullOrWhiteSpace(Info.CoverPath))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(Info.CoverPath);
                bitmapImage.EndInit();
                if (bitmapImage.CanFreeze)
                    bitmapImage.Freeze();

                return bitmapImage;
            }
            else
            {
                return (ImageSource)Application.Current.MainWindow.FindResource("CoverIcon");
            }
        }

        public void OverrideInfo(MovieInfo info)
        {
            Info.Awards = info.Awards;
            Info.CoverPath = info.CoverPath;
            Info.Genres = info.Genres;
            Info.Image = info.Image;
            Info.ImDbRating = info.ImDbRating;
            Info.Languages = info.Languages;
            Info.Title = info.Title;
            Info.MetacriticRating = info.MetacriticRating;
            Info.Plot = info.Plot;
            Info.ReleaseDate = info.ReleaseDate;
            Info.RuntimeStr = info.RuntimeStr;
            Info.Actors = info.Actors;
            Info.Year = info.Year;
        }

        public async Task<bool> GetInfo(IMovieInfoProvider infoProvider, string title, bool newGuid = true, string Guid = null)
        {
            MovieInfo info = await infoProvider.MakeInfo(title, this, newGuid, Guid);

            if (info != null)
            {
                Info = info;
                return true;
            }
            return false;
        }
    }
}
