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
using MovieDatabase.Util;
using Serilog;
using System.Windows;

namespace MovieDatabase.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private string _searchText = Loc.Instance[TranslationString.SEARCH];
        public string SearchText
        {
            get {  return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        private string _importText = Loc.Instance[TranslationString.IMPORT];
        public string ImportText
        {
            get {  return _importText; }
            set
            {
                _importText = value;
                OnPropertyChanged(nameof(ImportText));
            }
        }

        private string _homeText = Loc.Instance[TranslationString.HOME];
        public string HomeText
        {
            get {  return _homeText; }
            set
            {
                _homeText = value;
                OnPropertyChanged(nameof(HomeText));
            }
        }

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand SearchViewCommand { get; set; }
        public RelayCommand ImportViewCommand { get; set; }

        public HomeViewModel HomeVM { get; set; }
        public SearchViewModel SearchVM { get; set; }
        public ImportViewModel ImportVM { get; set; }

        private object _currentview;
        public object CurrentView
        {
            get { return _currentview; }
            set
            {
                _currentview = value;
                OnPropertyChanged();
            }
        }

        private readonly DatabaseMediator databaseMediator;

        public static MainViewModel Instance { get; set; }
        public MainViewModel(DatabaseMediator mediator)
        {
            Instance = this;
            databaseMediator = mediator;

            HomeVM = new HomeViewModel(databaseMediator);
            SearchVM = new SearchViewModel(databaseMediator);
            ImportVM = new ImportViewModel(databaseMediator);

            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });

            SearchViewCommand = new RelayCommand(o =>
            {
                CurrentView = SearchVM;
            });

            ImportViewCommand = new RelayCommand(o =>
            {
                CurrentView = ImportVM;
            });
        }

        public void RaiseAllPropertiesChanged()
        {
            AllPropertiesChanged();
        }
    }
}