#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.MovieSpace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace MovieDatabase.MVVM.Model.Converter
{
    public class InformationProviderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<IMovieInfoProvider> providers = value as List<IMovieInfoProvider>;
            List<string> names = new List<string>();
            providers.ForEach(provider => { names.Add(provider.GetName()); });
            return names;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
