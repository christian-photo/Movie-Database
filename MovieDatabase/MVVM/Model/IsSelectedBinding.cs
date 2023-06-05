#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using MovieDatabase.Core;
using System.Windows.Data;

namespace MovieDatabase.MVVM.Model
{
    public class IsSelectedBinding : Binding
    {

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
            }
        }

        public IsSelectedBinding()
        {
            Initialize();
        }

        public IsSelectedBinding(string path) : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
            Mode = BindingMode.OneWay;
        }
    }
}
