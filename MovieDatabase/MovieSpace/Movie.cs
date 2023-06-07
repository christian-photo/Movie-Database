#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.Core;
using MovieDatabase.Util;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

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

        public ImageSource GetCover(int width = 175)
        {
            if (!string.IsNullOrWhiteSpace(Info.CoverPath)) // Check if cover is downloaded
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                if (width > 0)
                {
                    bitmapImage.DecodePixelHeight = (int)(GetHeight(width) * 1.75);
                    bitmapImage.DecodePixelWidth = (int)(width * 1.75);
                }
                bitmapImage.UriSource = new Uri(Info.CoverPath);
                bitmapImage.EndInit();
                if (bitmapImage.CanFreeze)
                    bitmapImage.Freeze();

                return bitmapImage;
            }
            else if (Utility.IsConnectedToInternet()) // Check if internet connection is available to download the cover
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                if (width > 0)
                {
                    bitmapImage.DecodePixelHeight = (int)(GetHeight(width) * 1.75);
                    bitmapImage.DecodePixelWidth = (int)(width * 1.75);
                }
                bitmapImage.UriSource = new Uri(Info.Image);
                bitmapImage.EndInit();
                if (bitmapImage.CanFreeze)
                    bitmapImage.Freeze();

                return bitmapImage;
            }
            else // Use a fall back icon
            {
                return (ImageSource)Application.Current.MainWindow.FindResource("CoverIcon");
            }
        }

        private double GetHeight(int width)
        {
            Stream s = File.OpenRead(Path.Combine(Info.CoverPath));
            Image img = Bitmap.FromStream(s, false, false); // Read only the metadata
            double factor = (double)img.Width / (double)width;
            double targetHeight = (double)img.Height / factor;

            return targetHeight;
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
