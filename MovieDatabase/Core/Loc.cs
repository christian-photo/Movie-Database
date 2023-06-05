#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Serilog;

namespace MovieDatabase.Core
{
    public class TranslationString
    {
        private TranslationString(string value) { Value = value; }

        public string Value { get; private set; }

        public static TranslationString TITLE = new TranslationString("title");
        public static TranslationString ACTORS = new TranslationString("actors");
        public static TranslationString YEAR = new TranslationString("year");
        public static TranslationString GENRES = new TranslationString("genres");
        public static TranslationString RUNTIME = new TranslationString("runtime");
        public static TranslationString AWARDS = new TranslationString("awards");
        public static TranslationString IMDB = new TranslationString("imdb");
        public static TranslationString METASCORE = new TranslationString("metascore");
        public static TranslationString NO_WORK = new TranslationString("no_work");
        public static TranslationString ERR_NO_CONN_FOLDER = new TranslationString("err_no_conn_folder");
        public static TranslationString ERR_NO_CONN_FILE = new TranslationString("err_no_conn_file");
        public static TranslationString IMPORT_FOLDERS = new TranslationString("import_folders");
        public static TranslationString IMPORT_FILES = new TranslationString("import_files");
        public static TranslationString DOWNLOAD_COVERS = new TranslationString("download_covers");
        public static TranslationString TITLE_BY_FILENAME = new TranslationString("title_by_filename");
        public static TranslationString API_PROMPT = new TranslationString("api_prompt");
        public static TranslationString SUBMIT = new TranslationString("submit");
        public static TranslationString ERR_DOWNLOAD_COVER = new TranslationString("err_cover");
        public static TranslationString COLLECTING_INFORMATION = new TranslationString("coll_info");
        public static TranslationString CLICK_TO_SHOW_PLOT = new TranslationString("click_to_show_plot");
        public static TranslationString LOADING_CHACHED_INFORMATION = new TranslationString("l_cached_info");
        public static TranslationString SEARCH = new TranslationString("search");
        public static TranslationString IMPORT = new TranslationString("import");
        public static TranslationString HOME = new TranslationString("home");
        public static TranslationString AVAILABLE_SYSTEM_LANGUAGES = new TranslationString("available_sys_lang");
        public static TranslationString AVAILABLE_IMDB_LANGUAGES = new TranslationString("available_imdb_lang");
        public static TranslationString ENTER_IMDB_NAME = new TranslationString("enter_imdb_name");
        public static TranslationString NOT_COLLECTED_INFORMATION = new TranslationString("not_collected_info");
        public static TranslationString PLAY_TEXT = new TranslationString("play_text");
        public static TranslationString INCLUDE_SUB_DIRS = new TranslationString("include_sub_dirs");
    }

    public class Loc : ObservableObject
    {
        public List<string> Languages { get; private set; } = new List<string>();
        private Hashtable strings { get; set; }

        public Dictionary<string, string> IMDBLanguages { get; set; } = new Dictionary<string, string>();

        private string _selectedLang;
        public string SelectedLang 
        {
            get => _selectedLang;
            set {
                _selectedLang = value;
                AllPropertiesChanged();
            }
        }
        public string SelectedIMDBLang { get; set; }

        public static Loc Instance;

        public void Load()
        {
            Log.Logger.Information("Deserializing Translation JSON");
            strings = JsonConvert.DeserializeObject<Hashtable>(File.ReadAllText("./Resources/Translations.json"));
            Languages = (strings["languages"] as JArray).ToObject<List<string>>();

            SelectedLang = "EN";
            SelectedIMDBLang = "english";

            IMDBLanguages.Add("afrikaans", "af");
            IMDBLanguages.Add("albanian", "sq");
            IMDBLanguages.Add("arabic", "ar");
            IMDBLanguages.Add("armenian", "hy");
            IMDBLanguages.Add("azerbaijani", "az");
            IMDBLanguages.Add("basque", "eu");
            IMDBLanguages.Add("belarusian", "be");
            IMDBLanguages.Add("bengali", "bn");
            IMDBLanguages.Add("bulgarian", "bg");
            IMDBLanguages.Add("catalan", "ca");
            IMDBLanguages.Add("croatian", "hr");
            IMDBLanguages.Add("czech", "cs");
            IMDBLanguages.Add("danish", "da");
            IMDBLanguages.Add("dutch", "nl");
            IMDBLanguages.Add("english", "en");
            IMDBLanguages.Add("esperanto", "eo");
            IMDBLanguages.Add("estonian", "et");
            IMDBLanguages.Add("filipino", "tl");
            IMDBLanguages.Add("finnish", "fi");
            IMDBLanguages.Add("french", "fr");
            IMDBLanguages.Add("galician", "gl");
            IMDBLanguages.Add("german", "de");
            IMDBLanguages.Add("georgian", "ka");
            IMDBLanguages.Add("greek", "el");
            IMDBLanguages.Add("haitian creole", "ht");
            IMDBLanguages.Add("hebrew", "iw");
            IMDBLanguages.Add("hindi", "hi");
            IMDBLanguages.Add("hungarian", "hu");
            IMDBLanguages.Add("icelandic", "is");
            IMDBLanguages.Add("indonesian", "id");
            IMDBLanguages.Add("irish", "ga");
            IMDBLanguages.Add("italian", "it");
            IMDBLanguages.Add("japanese", "ja");
            IMDBLanguages.Add("korean", "ko");
            IMDBLanguages.Add("lao", "lo");
            IMDBLanguages.Add("latin", "la");
            IMDBLanguages.Add("latvian", "lv");
            IMDBLanguages.Add("lithuanian", "lt");
            IMDBLanguages.Add("macedonian", "mk");
            IMDBLanguages.Add("malay", "ms");
            IMDBLanguages.Add("maltese", "mt");
            IMDBLanguages.Add("norwegian", "no");
            IMDBLanguages.Add("polish", "pl");
            IMDBLanguages.Add("romanian", "ro");
            IMDBLanguages.Add("russian", "ru");
            IMDBLanguages.Add("serbian", "sr");
            IMDBLanguages.Add("slovak", "sk");
            IMDBLanguages.Add("slovenian", "sl");
            IMDBLanguages.Add("spanish", "es");
            IMDBLanguages.Add("swahili", "sw");
            IMDBLanguages.Add("swedish", "sv");
            IMDBLanguages.Add("tamil", "ta");
            IMDBLanguages.Add("telugu", "te");
            IMDBLanguages.Add("thai", "th");
            IMDBLanguages.Add("turkish", "tr");
            IMDBLanguages.Add("ukrainian", "uk");
            IMDBLanguages.Add("urdu", "ur");
            IMDBLanguages.Add("vietnamese", "vi");
            IMDBLanguages.Add("welsh", "cy");
            IMDBLanguages.Add("yiddish", "yi");
            IMDBLanguages.Add("burmese", "my");
            IMDBLanguages.Add("malayalam", "ml");

            IMDBLanguages.Add("farsi_persian", "fa");
            IMDBLanguages.Add("portuguese", "pt");
            IMDBLanguages.Add("brazillian-portuguese", "pt-BR");
            IMDBLanguages.Add("big_5_code", "b5c");
            IMDBLanguages.Add("chinese", "zh");
            IMDBLanguages.Add("chinese-bg-code", "zhbc");
            IMDBLanguages.Add("cambodian-khmer", "km");

            // Sort Languages
            SortedDictionary<string, string> sortDic = new SortedDictionary<string, string>(IMDBLanguages);
            IMDBLanguages = new Dictionary<string, string>(sortDic);

            Instance = this;
        }

        private string GetString(TranslationString str)
        {
            string res = (strings[str.Value] as JObject).Value<string>(SelectedLang);
            if (string.IsNullOrEmpty(res))
            {
                return (strings[str.Value] as JObject).Value<string>("EN");
            } else
            {
                return res;
            }
        }

        public string this[TranslationString key]
        {
            get
            {
                return GetString(key);
            }
        }
    }
}
