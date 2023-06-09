#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.Util;
using Newtonsoft.Json.Linq;
using OMDbApiNet;
using OMDbApiNet.Model;
using Serilog;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieDatabase.MovieSpace
{
    public class OmdbInfoProvider : IMovieInfoProvider
    {
        private bool fullPlot;

        private AsyncOmdbClient client;

        public OmdbInfoProvider(string apiKey, bool fullplot)
        {
            this.fullPlot = fullplot;

            client = new AsyncOmdbClient(apiKey, true);
        }

        public async Task<MovieInfo> MakeInfo(string title, bool newGuid = true, string Guid = null)
        {
            try
            {
                Item movie = await client.GetItemByTitleAsync(title, OmdbType.Movie, fullPlot);

                if (!bool.Parse(movie.Response))
                {
                    Log.Logger.Warning(movie.Error);
                    return null;
                }

                MovieInfo Info = CreateByMovie(movie, newGuid, Guid);
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

        private static MovieInfo CreateByMovie(Item movie, bool NewGuid = true, string uuid = null)
        {
            try
            {
                MovieInfo info = new MovieInfo
                {
                    Title = movie.Title,
                    Year = movie.Year,
                    Image = movie.Poster,
                    ReleaseDate = movie.Released,
                    RuntimeStr = movie.Runtime,
                    Plot = movie.Plot,
                    Awards = movie.Awards,
                    Actors = movie.Actors,
                    Director = movie.Director,
                    Genres = movie.Genre,
                    Languages = movie.Language,
                    ImDbRating = movie.ImdbRating,
                    RottenTomatoesRating = movie.Ratings[1].Value,
                    MetacriticRating = movie.Metascore
                };

                if (NewGuid)
                {
                    info.Uuid = Guid.NewGuid().ToString();
                }
                else
                {
                    info.Uuid = uuid;
                }

                return info;
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
                return bool.Parse((await client.GetItemByTitleAsync("Inception", OmdbType.Movie, fullPlot)).Response);
            } 
            catch(Exception ex)
            {
                Log.Logger.Error($"Error while validating OMDB, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
                return false;
            }
        }

        public string GetName() => "Omdb Provider";
    }
}
