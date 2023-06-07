#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.Core;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MovieDatabase.MovieSpace.Database
{
    public class MovieDB
    {
        [JsonProperty]
        private Guid ID;
        public string Name { get; set; }
        public List<Movie> Movies { get; set; } = new List<Movie>();

        private MovieDB(string name)
        {
            Name = name;
            ID = Guid.NewGuid();
        }

        [JsonConstructor]
        private MovieDB()
        {

        }

        /// <summary>
        /// Creates a new database with the specified name
        /// </summary>
        /// <param name="name">The name for the database</param>
        /// <returns></returns>
        public static MovieDB Create(string name)
        {
            MovieDB db = new MovieDB(name);
            return db;
        }

        /// <summary>
        /// Saves the database to its associated file
        /// </summary>
        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            Directory.CreateDirectory(ApplicationConstants.DatabaseFolder);
            File.WriteAllText(Path.Combine(ApplicationConstants.DatabaseFolder, ID.ToString() + $".{ApplicationConstants.DatabaseExtension}"), json);
            Log.Logger.Information($"Successfully saved database {this}");
        }

        /// <summary>
        /// Deletes all files that are used by this database except for the database itself
        /// </summary>
        public void Cleanup()
        {
            try
            {
                foreach (Movie movie in Movies)
                {
                    movie.Cleanup();
                }

                Log.Logger.Information($"Successfully cleaned up files");
            } catch (Exception ex)
            {
                Log.Logger.Error($"Something went wrong while cleaning up database {this}, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
            }
        }

        public Guid GetID() => ID;

        public Movie GetMovieByName(string name)
        {
            return Movies.Where(x => x.Info.Title == name).First();
        }

        public override string ToString()
        {
            return $"Name: {Name}, ID: {ID}";
        }
    }
}
