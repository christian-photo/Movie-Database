#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.Core;
using MovieDatabase.MVVM.ViewModel;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System.IO;

namespace MovieDatabase.Util
{
    public class ProgramInformations
    {
        public string Language = string.Empty;
        public string IMDBLanguage = string.Empty;
        public LogEventLevel LogLevel = LogEventLevel.Information; 

        [JsonProperty]
        private string _apiKey = string.Empty;

        public static ProgramInformations Create()
        {
            if (File.Exists(Path.Combine(ApplicationConstants.DataPath, "Settings.json")))
            {
                string text = File.ReadAllText(Path.Combine(ApplicationConstants.DataPath, "Settings.json"));
                ProgramInformations inf = JsonConvert.DeserializeObject<ProgramInformations>(text);
                Log.Logger.Debug($"Loaded {nameof(ProgramInformations)} from file");
                return inf;
            }
            Log.Logger.Debug($"Created new {nameof(ProgramInformations)}");
            return new ProgramInformations();
        }

        public void Save()
        {
            Language = MainViewModel.Instance.ImportVM.SelectedLang;
            IMDBLanguage = MainViewModel.Instance.ImportVM.SelectedIMDBLang;
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(Path.Combine(ApplicationConstants.DataPath, "Settings.json"), json);
            Log.Logger.Information($"Successfully saved {nameof(ProgramInformations)}");
        }

        internal string GetApiKey() => _apiKey;

        internal void SetApiKey(string key)
        {
            _apiKey = key;
        }
    }
}
