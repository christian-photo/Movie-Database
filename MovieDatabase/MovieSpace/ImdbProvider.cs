#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using IMDbApiLib;
using IMDbApiLib.Models;
using MovieDatabase.Core;
using MovieDatabase.Util;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieDatabase.MovieSpace
{
    public class ImdbProvider : IMovieInfoProvider
    {
        private ApiLib client;
        private Language lang;

        public ImdbProvider(string apiKey, Language language)
        {
            client = new ApiLib(apiKey);
            lang = language;
        }

        public async Task<MovieInfo> MakeInfo(string title, bool newGuid = true, string Guid = null)
        {
            try
            {
                SearchData result = await client.SearchMovieAsync(title);

                if (result.ErrorMessage.StartsWith("Maximum usage"))
                {
                    Log.Logger.Warning("Limit of API Requests reached");
                    return null;
                }

                TitleData data = await client.TitleAsync(result.Results[0].Id, lang, Ratings: true);

                if (data.ErrorMessage.StartsWith("Maximum usage"))
                {
                    Log.Logger.Warning("Limit of API Requests reached");
                    return null;
                }

                MovieInfo Info = CreateByData(data, newGuid, Guid);
                if (Info != null)
                {
                    return Info;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Something went wrong while loading information for the movie, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
                return null;
            }
        }

        private static MovieInfo CreateByData(TitleData data, bool NewGuid = true, string uuid = null)
        {
            try
            {
                MovieInfo movie = new MovieInfo
                {
                    Title = data.Title,
                    Year = data.Year,
                    Image = data.Image,
                    ReleaseDate = data.ReleaseDate,
                    RuntimeStr = data.RuntimeStr,
                    Plot = Loc.Instance.SelectedIMDBLang == "english" ? data.Plot : data.PlotLocal,
                    Awards = data.Awards,
                    Director = data.Directors,
                    Genres = data.Genres,
                    Actors = data.Stars,
                    Languages = data.Languages,
                    ImDbRating = data.IMDbRating,
                    MetacriticRating = data.MetacriticRating,
                    RottenTomatoesRating = data.Ratings.RottenTomatoes + "%"
                };

                if (NewGuid)
                {
                    movie.Uuid = Guid.NewGuid().ToString();
                }
                else
                {
                    movie.Uuid = uuid;
                }

                return movie;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Something went wrong while creating the MovieInfo, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<bool> Validate()
        {
            try
            {
                SearchData result = await client.SearchMovieAsync("Inception");

                if (result.ErrorMessage.StartsWith("Maximum usage"))
                {
                    Log.Logger.Warning("Limit of API Requests reached");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Error while validating IMDB, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
                return false;
            }
        }

        public string GetName() => "Imdb Provider";
    }
}
