#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.MovieSpace.Database;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MovieDatabase.MVVM.Model.Converter
{
    public class DatabaseNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((MovieDB)value).Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MainWindow.databaseMediator.GetDatabases().Where(x => x.Name == value?.ToString()).First();
        }
    }
}
