#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace MovieDatabase.Core.Utility.PlotPopup
{
    /// <summary>
    /// Interaktionslogik für PlotPopup.xaml
    /// </summary>
    public partial class PlotPopup : UiWindow
    {
        private string _plot = "";
        public string Plot
        {
            get { return _plot; }
            set
            {
                _plot = value;
                PlotText.Text = value;
            }
        }
        public PlotPopup()
        {
            InitializeComponent();
        }

        private void CloseWindow(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
