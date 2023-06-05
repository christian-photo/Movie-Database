#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.Core;
using MovieDatabase.MovieSpace.Database;
using MovieDatabase.MVVM.ViewModel;
using MovieDatabase.Util;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;

namespace MovieDatabase
{
    /// <summary>
    /// Interaktionslogik für APIKeyPrompt.xaml
    /// </summary>
    public partial class APIKeyPrompt : UiWindow, INotifyPropertyChanged
    {
        private string _text = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public string Text
        {
            get {  return _text; }
            set
            {
                _text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        private string _submit = Loc.Instance[TranslationString.SUBMIT];
        public string Submit
        {
            get {  return _submit; }
            set
            {
                _submit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Submit)));
            }
        }
        public APIKeyPrompt()
        {
            InitializeComponent();
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (await CheckAPIKey(API_Key.Text))
            {
                MainWindow.informations.SetApiKey(API_Key.Text);
                Close();
            }
        }

        private async Task<bool> CheckAPIKey(string key)
        {
            bool isValid = false;
            Text = "Validating, plase wait...";
            Log.Logger.Information("Validating api key...");
            try
            {
                HttpClient client = new HttpClient(Utility.FastClientHandler);
                string response = await client.GetStringAsync("https://imdb-api.com/en/API/Search/" + key + "/inception 2010");
                client.Dispose();
                JObject res = JObject.Parse(response);
                JToken Value = res["errorMessage"];
                if (!Value.Value<string>().Equals("Invalid API Key"))
                {
                    isValid = true;
                } else
                {
                    Text = "Invalid!";
                }
            } catch (Exception ex)
            {
                Log.Logger.Error($"Something went wrong while cleaning up database {this}, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
            }
            Log.Logger.Information($"Validation result: {isValid}");
            return isValid;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
