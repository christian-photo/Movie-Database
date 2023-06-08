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
using Serilog;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieDatabase.MovieSpace
{
    public class OmdbInfoProvider : IMovieInfoProvider
    {
        private string apiKey;
        private bool fullPlot;

        public OmdbInfoProvider(string apiKey, bool fullplot)
        {
            this.apiKey = apiKey;
            this.fullPlot = fullplot;
        }

        public async Task<MovieInfo> MakeInfo(string title, bool newGuid = true, string Guid = null)
        {
            try
            {
                string plot = fullPlot ? "full" : "short";
                HttpClient client = new HttpClient(Utility.FastClientHandler);

                string json = await client.GetStringAsync($"http://omdbapi.com/?apikey={apiKey}&type=movie&plot={plot}&t={title}");

                if (!JObject.Parse(json).Value<bool>("Response"))
                {
                    Log.Logger.Warning(JObject.Parse(json).Value<string>("Error"));
                    return null;
                }

                MovieInfo Info = CreateByJson(json, newGuid, Guid);
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

        private static MovieInfo CreateByJson(string json, bool NewGuid = true, string uuid = null)
        {
            try
            {
                JObject obj = JObject.Parse(json);
                MovieInfo movie = new MovieInfo
                {
                    Title = obj.Value<string>("Title"),
                    Year = obj.Value<string>("Year"),
                    Image = obj.Value<string>("Poster"),
                    ReleaseDate = obj.Value<string>("Released"),
                    RuntimeStr = obj.Value<string>("Runtime"),
                    Plot = obj.Value<string>("Plot"),
                    Awards = obj.Value<string>("Awards"),
                    Actors = obj.Value<string>("Actors"),
                    Director = obj.Value<string>("Director"),
                    Genres = obj.Value<string>("Genre"),
                    Languages = obj.Value<string>("Language"),
                    ImDbRating = obj.Value<string>("imdbRating"),
                    RottenTomatoesRating = obj["Ratings"].FirstOrDefault(r => r["Source"].Value<string>().Equals("Rotten Tomatoes"))?["Value"].Value<string>(),
                    MetacriticRating = obj.Value<string>("Metascore")
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
                HttpClient client = new HttpClient(Utility.FastClientHandler);

                Log.Logger.Verbose("OMDB Validation call: " + $"http://www.omdbapi.com/?apikey={apiKey}&t=Inception");
                string json = await client.GetStringAsync($"http://www.omdbapi.com/?apikey={apiKey}&t=Inception");

                return JObject.Parse(json).Value<bool>("Response");
            } 
            catch(Exception ex)
            {
                Log.Logger.Error($"Error while validating OMDB, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
                return false;
            }
        }
    }
}
