#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.Core.Utility.EditPopup;
using MovieDatabase.Core.Utility.PlotPopup;
using MovieDatabase.MVVM.ViewModel;
using MovieDatabase.Util;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovieDatabase.MVVM.View
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public static SearchView Instance { get; set; }
        private static ListView ListView { get; set; }

        public SearchView()
        {
            InitializeComponent();
            Instance = this;
            ListView = Panel_Mini;
        }

        private void SearchMovie(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainViewModel.Instance.SearchVM.Search();
            }
        }

        public static void Reload()
        {
            ListView.Items.Refresh();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlotPopup p = new PlotPopup();
            p.Plot = MainViewModel.Instance.SearchVM.SelectedMovie.Info.Plot;
            p.ShowDialog();
        }

        private void Edit(object sender, MouseButtonEventArgs e)
        {
            new EditPopup(MainViewModel.Instance.SearchVM.SelectedMovie, MainViewModel.Instance.ImportVM.GetSelectedProvider()).ShowDialog();
        }
    }
}
