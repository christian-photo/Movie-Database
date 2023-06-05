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
using System.Windows.Controls;

namespace MovieDatabase.MVVM.View
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            Panel_Mini.Items.Refresh();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PlotPopup p = new PlotPopup();
            p.Plot = MainViewModel.Instance.HomeVM.SelectedMovie.Info.Plot;
            p.ShowDialog();
        }

        private void Edit(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new EditPopup(MainViewModel.Instance.HomeVM.SelectedMovie, MainViewModel.Instance.ImportVM.InfoProvider).ShowDialog();
        }
    }
}