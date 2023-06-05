#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using System;
using System.IO;

namespace MovieDatabase.Core
{
    public class ApplicationConstants
    {
        public static readonly string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MovieDatabase");
        public static readonly string DatabaseFolder = Path.Combine(DataPath, "Databases");
        public static readonly string CoverFolder = Path.Combine(DataPath, "Covers");
        public static readonly string LogFolder = Path.Combine(DataPath, "Logs");
        public static readonly string DatabaseExtension = "mvdb";
    }
}