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
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieDatabase.MovieSpace
{
    public class ImdbProvider : IMovieInfoProvider
    {
        private string apikey;
        public ImdbProvider(string apiKey)
        {
            apikey = apiKey;
        }

        public async Task<MovieInfo> MakeInfo(string title, Movie movie, bool newGuid = true, string Guid = null)
        {
            try
            {
                string lang = "en";
                string id;

                HttpClient client = new HttpClient(Utility.FastClientHandler);

                string _json = await client.GetStringAsync("http://imdb-api.com/" + lang + "/API/SearchMovie/" + apikey + "/" + title);

                if (JObject.Parse(_json).Value<string>("errorMessage").StartsWith("Maximum usage"))
                {
                    Log.Logger.Warning("Limit of API Requests reached");
                    return null;
                }
                
                JToken token = JObject.Parse(_json).SelectToken("$.results[0].id");

                if (token is null)
                {
                    Log.Logger.Warning("token is null. Timeout?");
                    return null;
                }

                id = token.ToObject<string>();

                string json = await client.GetStringAsync("http://imdb-api.com/" + lang + "/API/Title/" + apikey + "/" + id);

                MovieInfo Info = CreateByJson(json, movie.FilePath, newGuid, Guid);
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

        private static MovieInfo CreateByJson(string json, string moviePath, bool NewGuid = true, string uuid = null)
        {
            try
            {
                JObject obj = JObject.Parse(json);
                MovieInfo movie = new MovieInfo
                {
                    Title = obj.Value<string>("title"),
                    Year = obj.Value<string>("year"),
                    Image = obj.Value<string>("image"),
                    ReleaseDate = obj.Value<string>("releaseDate"),
                    RuntimeStr = obj.Value<string>("runtimeStr"),
                    Plot = Loc.Instance.SelectedIMDBLang == "english" ? obj.Value<string>("plot") : obj.Value<string>("plotlocal"),
                    Awards = obj.Value<string>("awards"),
                    Actors = obj.Value<object>("stars").ToString(),
                    Genres = obj.Value<object>("genres").ToString(),
                    Languages = obj.Value<object>("languages").ToString(),
                    ImDbRating = obj.Value<string>("imDbRating"),
                    MetacriticRating = obj.Value<string>("metacriticRating"),
                    FilePath = moviePath
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
    }
}
