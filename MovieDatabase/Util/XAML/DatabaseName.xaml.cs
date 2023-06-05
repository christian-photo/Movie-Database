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
using System;
using System.ComponentModel;
using System.Windows;
using Wpf.Ui.Controls;

namespace MovieDatabase.Util.XAML
{
    /// <summary>
    /// Interaktionslogik für DatabaseName.xaml
    /// </summary>
    public partial class DatabaseName : UiWindow, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

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

        private DatabaseMediator _mediator;

        public DatabaseName(DatabaseMediator databaseMediator)
        {
            InitializeComponent();
            _mediator = databaseMediator;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string name = DBName.Text;
            if (string.IsNullOrEmpty(name))
            {
                name = "Database";
            }
            foreach (MovieDB db in _mediator.GetDatabases())
            {
                if (db.Name == name)
                {
                    name += $"___({Guid.NewGuid()})";
                    break;
                }
            }
            _mediator.AddDatabase(MovieDB.Create(name));
            Close();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }
    }
}
