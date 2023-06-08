#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

namespace MovieDatabase.MovieSpace
{
    public class MovieInfo
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Image { get; set; }
        public string ReleaseDate { get; set; }
        public string RuntimeStr { get; set; }
        public string Plot { get; set; }
        public string Awards { get; set; }
        public string Actors { get; set; }
        public string Director { get; set; }
        public string Genres { get; set; }
        public string Languages { get; set; }
        public string ImDbRating { get; set; }
        public string MetacriticRating { get; set; }
        public string RottenTomatoesRating { get; set; }
        public string CoverPath { get; set; }
        public string Uuid { get; set; }
    }
}
