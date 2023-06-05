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
    public class DatabaseMediator
    {
        private List<MovieDB> databases;

        private MovieDB currentDatabase;

        private DatabaseMediator()
        {
            databases = new List<MovieDB>();
        }

        public static DatabaseMediator CreateNew()
        {
            return new DatabaseMediator();
        }

        public static DatabaseMediator Load()
        {
            string currentID = string.Empty;
            DatabaseMediator mediator = CreateNew();

            if (Path.Exists(ApplicationConstants.DatabaseFolder))
            {
                foreach (string file in Directory.GetFiles(ApplicationConstants.DatabaseFolder).Where(x => x.EndsWith(ApplicationConstants.DatabaseExtension)))
                {
                    mediator.databases.Add(JsonConvert.DeserializeObject<MovieDB>(File.ReadAllText(Path.Combine(ApplicationConstants.DatabaseFolder, file))));
                }
            }

            Log.Logger.Information($"Loaded {mediator.databases.Count} databases");

            if (mediator.databases.Count == 0)
            {
                Log.Logger.Debug("Creating default database");
                mediator.databases.Add(MovieDB.Create("default"));
                mediator.currentDatabase = mediator.databases[0];
                return mediator;
            }
            if (currentID.Equals(string.Empty))
            {
                mediator.currentDatabase = mediator.databases[0];
                return mediator;
            }
            mediator.currentDatabase = mediator.databases.Where(x => x.GetID() == Guid.Parse(currentID)).FirstOrDefault(mediator.databases[0]);

            return mediator;
        }

        public void Save()
        {
            Log.Logger.Information("Saving all databases");
            foreach (MovieDB movieDB in databases)
            {
                movieDB.Save();
            }
        }

        public void AddDatabase(MovieDB database)
        {
            if (database.GetID() == Guid.Empty)
            {
                Log.Logger.Warning($"Not adding database {database.Name} because of invalid GUID");
                return;
            }
            databases.Add(database);
            Log.Logger.Information($"Successfully added Database {database}");
        }

        public void RemoveDatabase(MovieDB database)
        {
            database.Cleanup();
            if (databases.Remove(database))
            {
                File.Delete(Path.Combine(ApplicationConstants.DatabaseFolder, database.GetID() + "." + ApplicationConstants.DatabaseExtension));
            }
            Log.Information($"Successfully removed and cleaned up database {database}");
        }

        public bool SetCurrentDatabase(Guid ID)
        {
            MovieDB newDB = databases.Where(x => x.GetID() == ID).FirstOrDefault(currentDatabase);
            if (newDB.GetID() == ID)
            {
                currentDatabase = newDB;
                Log.Logger.Information($"Switched loaded database to {newDB}");
                return true;
            }
            return false;
        }

        public MovieDB GetCurrentDatabase() => currentDatabase;

        public List<MovieDB> GetDatabases() => databases;
    }
}
