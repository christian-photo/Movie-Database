#region "copyright"

/*
    Copyright © 2023 Christian Palm (christian@palm-family.de)
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using Microsoft.WindowsAPICodePack.Dialogs;
using MovieDatabase.Core;
using MovieDatabase.MovieSpace;
using MovieDatabase.MovieSpace.Database;
using MovieDatabase.Util;
using MovieDatabase.Util.XAML;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace MovieDatabase.MVVM.ViewModel
{
    public class ImportViewModel : ObservableObject
    {
        public static bool Split { get; private set; } = true;
        public static bool IncludeSubDirs { get; private set; } = true;

        public RelayCommand ImportFolders { get; set; }
        public RelayCommand ImportFiles { get; set; }
        public RelayCommand DownloadCovers { get; set; }
        public RelayCommand Splitting { get; set; }
        public RelayCommand SubDirs { get; set; }
        public RelayCommand SyncMovies { get; set; }
        public RelayCommand UpdateMovies { get; set; }
        public RelayCommand AddDatabase { get; set; }
        public RelayCommand RemoveDatabase { get; set; }
        public RelayCommand LoadDatabase { get; set; }

        private Visibility _visibility { get; set; } = Visibility.Hidden;
        private Brush _subDirsBrush = Brushes.Green;
        private Brush _splittingForeground { get; set; } = Brushes.Green;

        private string _state { get; set; } = Loc.Instance[TranslationString.NO_WORK];
        public static List<string> MovieExtensions { get; set; } = new List<string>();
        public static List<string> NotCollectedInformation { get; set; } = new List<string>();

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }

        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        private string _includeSubDirsText = Loc.Instance[TranslationString.INCLUDE_SUB_DIRS];
        public string IncludeSubDirsText
        {
            get { return _includeSubDirsText; }
            set
            {
                _includeSubDirsText = value;
                OnPropertyChanged();
            }
        }

        private string _importFoldersText = Loc.Instance[TranslationString.IMPORT_FOLDERS];
        public string ImportFoldersText
        {
            get { return _importFoldersText; }
            set
            {
                _importFoldersText = value;
                OnPropertyChanged();
            }
        }

        private string _importFilesText = Loc.Instance[TranslationString.IMPORT_FILES];
        public string ImportFilesText
        {
            get { return _importFilesText; }
            set
            {
                _importFilesText = value;
                OnPropertyChanged();
            }
        }

        private string _downloadCovers = Loc.Instance[TranslationString.DOWNLOAD_COVERS];
        public string DownloadCoversText
        {
            get { return _downloadCovers; }
            set
            {
                _downloadCovers = value;
                OnPropertyChanged();
            }
        }

        private string _titleByFileNameText = Loc.Instance[TranslationString.TITLE_BY_FILENAME];
        public string TitleByFileNameText
        {
            get { return _titleByFileNameText; }
            set
            {
                _titleByFileNameText = value;
                OnPropertyChanged();
            }
        }

        private string _availableSystemLanguages = Loc.Instance[TranslationString.AVAILABLE_SYSTEM_LANGUAGES];
        public string AvailableSystemLanguages
        {
            get { return _availableSystemLanguages; }
            set
            {
                _availableSystemLanguages = value;
                OnPropertyChanged();
            }
        }

        private List<string> _imdbLanguages = Utility.KeysToList(Loc.Instance.IMDBLanguages.Keys);
        public List<string> IMDBLanguages
        {
            get { return _imdbLanguages; }
            set
            {
                _imdbLanguages = value;
                OnPropertyChanged();
            }
        }

        private string _availableIMDBLanguages = Loc.Instance[TranslationString.AVAILABLE_IMDB_LANGUAGES];
        public string AvailableIMDBLanguages
        {
            get { return _availableIMDBLanguages; }
            set
            {
                _availableIMDBLanguages = value;
                OnPropertyChanged();
            }
        }

        private string _selectedIMDBLang = Loc.Instance.SelectedIMDBLang;
        public string SelectedIMDBLang
        {
            get { return _selectedIMDBLang; }
            set
            {
                _selectedIMDBLang = value;
                OnPropertyChanged();
            }
        }

        private List<ImportMovie> _syncedMovies = new List<ImportMovie>();
        public List<ImportMovie> SyncedMovies
        {
            get { return _syncedMovies; }
            set
            {
                _syncedMovies = value;
                OnPropertyChanged();
            }
        }

        private bool _isSyncing = false;
        public bool IsSyncing
        {
            get { return _isSyncing; }
            set
            {
                _isSyncing = value;
                OnPropertyChanged();
            }
        }

        public List<string> Languages { get; set; } = Loc.Instance.Languages;
        public string SelectedLang
        {
            get { return Loc.Instance.SelectedLang; }
            set
            {
                Loc.Instance.SelectedLang = value;
                MainViewModel.Instance.HomeVM.RaiseAllPropertiesChanged();
                MainViewModel.Instance.SearchVM.RaiseAllPropertiesChanged();
                MainViewModel.Instance.RaiseAllPropertiesChanged();
                MainViewModel.Instance.ImportVM.AllPropertiesChanged();
            }
        }

        public Brush SplittingForeground
        {
            get { return _splittingForeground; }
            set
            {
                _splittingForeground = value;
                OnPropertyChanged();
            }
        }

        public Brush SubDirsBrush
        {
            get { return _subDirsBrush; }
            set
            {
                _subDirsBrush = value;
                OnPropertyChanged();
            }
        }

        public List<string> Databases
        {
            get 
            {
                List<string> list = new List<string>();
                databaseMediator.GetDatabases().ForEach(database => list.Add(database.Name));
                return list;
            }
        }

        private int _selectedDBIndex;
        public int SelectedDatabase
        {
            get => _selectedDBIndex;
            set
            {
                _selectedDBIndex = value;
                OnPropertyChanged();
            }
        }

        private DatabaseMediator databaseMediator;

        public IMovieInfoProvider InfoProvider { get; set; }

        public ImportViewModel(DatabaseMediator mediator)
        {
            databaseMediator = mediator;

            InfoProvider = new ImdbProvider(MainWindow.informations.GetApiKey());

            ImportFolders = new RelayCommand(async o =>
            {
                List<string> Folders = FolderDialog();
                List<string> Files = new List<string>();
                List<Movie> NewMovies = new List<Movie>();

                if (Folders is null)
                {
                    return;
                }

                if (!Utility.IsConnectedToInternet())
                {
                    State = Loc.Instance[TranslationString.ERR_NO_CONN_FOLDER];
                    Log.Logger.Warning($"Can't download Information, not connected to the internet. Cancelling Task");
                    return;
                }

                Visibility = Visibility.Visible;

                if (IncludeSubDirs)
                {
                    foreach (string folder in Folders)
                    {
                        Files.AddRange(GetAllFiles(folder));
                    }
                }
                else
                {
                    foreach (string folder in Folders)
                    {
                        foreach (string file in DirSearch(folder))
                        {
                            Files.Add(file);
                        }
                    }
                }

                Log.Logger.Information($"Starting collecting Information! (Found {Files.Count} files)");
                foreach (string file in Files)
                {
                    if (CheckIfFileIsRegistered(file))
                    {
                        Log.Logger.Warning($"File {file} already registered! Skipping file");
                        continue;
                    }
                    Movie mov;
                    if (Split)
                    {
                        string title = GetRealTitle(Path.GetFileNameWithoutExtension(file));
                        mov = await RegisterMovie(file, title, InfoProvider);
                    }
                    else
                    {
                        mov = await RegisterMovie(file, Path.GetFileNameWithoutExtension(file), InfoProvider);
                    }
                    if (mov is null)
                    {
                        continue;
                    }
                    NewMovies.Add(mov);
                    databaseMediator.GetCurrentDatabase().Movies.Add(mov);
                }
                Visibility = Visibility.Hidden;
                State = Loc.Instance[TranslationString.NO_WORK];
                foreach (Movie mov in NewMovies)
                {
                    MainViewModel.Instance.HomeVM.AddMovieToView(mov.Info.Title, mov);
                }
                MainViewModel.Instance.SearchVM.RaiseAllPropertiesChanged();
                MainViewModel.Instance.HomeVM.RaiseAllPropertiesChanged();
            });

            ImportFiles = new RelayCommand(async o =>
            {
                List<string> files = FileDialog();
                if (files is null)
                {
                    return;
                }

                if (!Utility.IsConnectedToInternet())
                {
                    State = Loc.Instance[TranslationString.ERR_NO_CONN_FILE];
                    Log.Logger.Warning($"Can't download Information, not connected to the internet. Cancelling Task");
                    return;
                }

                Visibility = Visibility.Visible;

                Log.Logger.Information($"Starting collecting Information! (Found {files.Count} files)");
                foreach (string file in files)
                {
                    if (CheckIfFileIsRegistered(file))
                    {
                        Log.Logger.Warning($"File {file} already registered! Skipping file");
                        continue;
                    }
                    Movie mov;
                    if (Split)
                    {
                        string title = GetRealTitle(Path.GetFileNameWithoutExtension(file));
                        mov = await RegisterMovie(file, title, InfoProvider);
                    }
                    else
                    {
                        mov = await RegisterMovie(file, Path.GetFileNameWithoutExtension(file), InfoProvider);
                    }
                    if (mov is null)
                    {
                        continue;
                    }
                    databaseMediator.GetCurrentDatabase().Movies.Add(mov);
                }
                foreach (Movie mov in databaseMediator.GetCurrentDatabase().Movies)
                {
                    MainViewModel.Instance.HomeVM.AddMovieToView(mov.Info.Title, mov);
                }
                MainViewModel.Instance.SearchVM.RaiseAllPropertiesChanged();
                MainViewModel.Instance.HomeVM.RaiseAllPropertiesChanged();
                Visibility = Visibility.Hidden;
                State = Loc.Instance[TranslationString.NO_WORK];
            });

            DownloadCovers = new RelayCommand(async o =>
            {
                if (!Utility.IsConnectedToInternet())
                {
                    State = Loc.Instance[TranslationString.ERR_DOWNLOAD_COVER];
                    Log.Logger.Warning($"Can't download Information, not connected to the internet. Cancelling Task");
                    return;
                }
                Visibility = Visibility.Visible;
                try
                {
                    Directory.CreateDirectory(Path.Combine(ApplicationConstants.CoverFolder, databaseMediator.GetCurrentDatabase().GetID().ToString()));

                    foreach (Movie movie in databaseMediator.GetCurrentDatabase().Movies)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            string[] split = movie.Info.Image.Split('.'); // split[split.Length - 1] = the extension of the file

                            string imgSource = Path.Combine(ApplicationConstants.CoverFolder, databaseMediator.GetCurrentDatabase().GetID().ToString(), movie.Info.Uuid + "." + split[split.Length - 1]);

                            Log.Logger.Information($"Downloading cover for {movie.Info.Title}");
                            State = $"Downloading cover for {movie.Info.Title}";

                            byte[] data = await client.GetByteArrayAsync(movie.Info.Image);

                            await File.WriteAllBytesAsync(imgSource, data);

                            movie.Info.CoverPath = imgSource;
                        }
                    }
                    State = Loc.Instance[TranslationString.NO_WORK];
                    Visibility = Visibility.Hidden;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Something went wrong while downloading covers, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
                    Visibility = Visibility.Hidden;
                    State = "Error! Please check the logs!";
                    return;
                }
            });

            SyncMovies = new RelayCommand(o =>
            {
                Log.Logger.Information("Starting sync...");
                Visibility = Visibility.Visible;
                List<string> ScannedDirectories = new List<string>();
                List<string> MovieFilePaths = new List<string>();
                List<string> FilePaths = new List<string>();
                List<KeyValuePair<string, string>> changes = new List<KeyValuePair<string, string>>();

                foreach (Movie mov in databaseMediator.GetCurrentDatabase().Movies)
                {
                    MovieFilePaths.Add(mov.FilePath);
                }
                foreach (string path in MovieFilePaths)
                {
                    string dir = Path.GetDirectoryName(path);
                    if (ScannedDirectories.Contains(dir))
                    {
                        continue;
                    }
                    FilePaths = GetAllFiles(dir);
                    if (!FilePaths.Contains(path))
                    {
                        changes.Add(new KeyValuePair<string, string>(path, "deleted"));
                    }
                    ScannedDirectories.Add(dir);
                }
                ScannedDirectories.Clear();

                foreach (string file in FilePaths)
                {
                    string dir = Path.GetDirectoryName(file);
                    if (ScannedDirectories.Contains(dir))
                    {
                        continue;
                    }
                    if (!MovieFilePaths.Contains(file))
                    {
                        changes.Add(new KeyValuePair<string, string>(file, "new"));
                    }
                    ScannedDirectories.Add(dir);
                }

                List<ImportMovie> Imports = new List<ImportMovie>();

                foreach (KeyValuePair<string, string> change in changes)
                {
                    ImportMovie mov = new ImportMovie();
                    if (change.Value == "new")
                    {
                        mov.Name = change.Key;
                        mov.ShouldImport = true;
                        mov.Color = Brushes.Green;
                    }
                    else
                    {
                        mov.Name = change.Key;
                        mov.ShouldImport = true;
                        mov.Color = Brushes.Red;
                    }
                    Imports.Add(mov);
                }

                Log.Logger.Information($"Found {Imports.Count} changes while syncing");

                SyncedMovies = Imports;

                if (Imports.Count > 0)
                {
                    IsSyncing = true;

                    Visibility = Visibility.Hidden;
                }
            });

            UpdateMovies = new RelayCommand(async o =>
            {
                foreach (ImportMovie movie in SyncedMovies)
                {
                    if (movie.ShouldImport)
                    {
                        if (CheckIfFileIsRegistered(movie.Name))
                        {
                            Log.Logger.Warning($"File {movie.Name} already registered! Skipping file");
                            continue;
                        }
                        Movie mov;
                        if (Split)
                        {
                            string title = GetRealTitle(Path.GetFileNameWithoutExtension(movie.Name));
                            mov = await RegisterMovie(movie.Name, title, InfoProvider);
                        }
                        else
                        {
                            mov = await RegisterMovie(movie.Name, Path.GetFileNameWithoutExtension(movie.Name), InfoProvider);
                        }
                        if (mov is null)
                        {
                            continue;
                        }
                        databaseMediator.GetCurrentDatabase().Movies.Add(mov);
                    }
                }
                MainViewModel.Instance.SearchVM.RaiseAllPropertiesChanged();
                MainViewModel.Instance.HomeVM.RaiseAllPropertiesChanged();
                State = Loc.Instance[TranslationString.NO_WORK];
                SyncedMovies = new List<ImportMovie>();
                IsSyncing = false;
            });

            AddDatabase = new RelayCommand(o =>
            {
                new DatabaseName(databaseMediator).ShowDialog();
                MainViewModel.Instance.SearchVM.RaiseAllPropertiesChanged();
                MainViewModel.Instance.HomeVM.RaiseAllPropertiesChanged();
                AllPropertiesChanged();
            });

            RemoveDatabase = new RelayCommand(o =>
            {
                MovieDB selectedDB = databaseMediator.GetDatabases().Where(x => x.Name == Databases[SelectedDatabase]).First();
                if (selectedDB is null)
                    return;

                if (selectedDB.GetID() != databaseMediator.GetCurrentDatabase().GetID())
                {
                    databaseMediator.RemoveDatabase(selectedDB);
                    MainViewModel.Instance.SearchVM.RaiseAllPropertiesChanged();
                    MainViewModel.Instance.HomeVM.RaiseAllPropertiesChanged();
                    AllPropertiesChanged();
                }
            });

            LoadDatabase = new RelayCommand(o =>
            {
                MovieDB selectedDB = databaseMediator.GetDatabases().Where(x => x.Name == Databases[SelectedDatabase]).First();
                if (selectedDB is null)
                    return;

                if (databaseMediator.SetCurrentDatabase(selectedDB.GetID()))
                {
                    MainViewModel.Instance.SearchVM.ClearView();
                    MainViewModel.Instance.HomeVM.ClearView();

                    foreach (Movie movie in databaseMediator.GetCurrentDatabase().Movies)
                    {
                        MainViewModel.Instance.HomeVM.AddMovieToView(movie.Info.Title, movie);
                    }

                    if (databaseMediator.GetCurrentDatabase().Movies.Count > 0)
                        MainViewModel.Instance.HomeVM.SetSelectedMovie(databaseMediator.GetCurrentDatabase().Movies[0]);
                }

            });

            Splitting = new RelayCommand(o =>
            {
                Split = !Split;
                if (Split)
                {
                    SplittingForeground = Brushes.Green;
                }
                else
                {
                    SplittingForeground = Brushes.Red;
                }
            });

            SubDirs = new RelayCommand(o =>
            {
                IncludeSubDirs = !IncludeSubDirs;
                if (IncludeSubDirs)
                {
                    SubDirsBrush = Brushes.Green;
                }
                else
                {
                    SubDirsBrush = Brushes.Red;
                }
            });

            MovieExtensions.Add(".avi");
            MovieExtensions.Add(".mp4");
            MovieExtensions.Add(".mkv");
            MovieExtensions.Add(".mov");
            MovieExtensions.Add(".wmv");
            MovieExtensions.Add(".mpg");
            MovieExtensions.Add(".mp2");
            MovieExtensions.Add(".mpeg");
            MovieExtensions.Add(".mpv");
            MovieExtensions.Add(".m4v");
            MovieExtensions.Add(".ts");
        }

        private List<string> GetAllFiles(string folder)
        {
            List<string> files = new List<string>();
            foreach (string file in Directory.EnumerateFiles(folder))
            {
                if (MovieExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    files.Add(file);
                }
            }

            foreach (string directory in Directory.GetDirectories(folder))
            {
                files.AddRange(GetAllFiles(directory));
            }
            return files;
        }

        private static List<string> FolderDialog()
        {
            using (CommonOpenFileDialog cofd = new CommonOpenFileDialog())
            {
                cofd.IsFolderPicker = true;
                cofd.Multiselect = true;
                cofd.Title = "Select Folders";
                CommonFileDialogResult res = cofd.ShowDialog();

                if (res == CommonFileDialogResult.Ok)
                {
                    return cofd.FileNames.ToList();
                }
                return null;
            }
        }

        private static List<string> FileDialog()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Movie Files(*.avi;*.mp4;*.mkv;*.mov;*.wmv;*.mpg;*.mp2;*.mpeg;*.mpv;*.m4v;*.ts)|*.avi;*.mp4;*.mkv;*.mov;*.wmv;*.mpg;*.mp2;*.mpeg;*.mpv;*.m4v;*.ts";
                dialog.Title = "Select Movies";
                dialog.Multiselect = true;
                DialogResult res = dialog.ShowDialog();
                if (res == DialogResult.OK)
                {
                    return dialog.FileNames.ToList();
                }
                return null;
            }
        }

        public async Task<Movie> RegisterMovie(string file, string title, IMovieInfoProvider infoProvider)
        {
            Movie movie = new Movie(file);
            Log.Logger.Information($"Collecting Informations for {title}");

            State = Loc.Instance[TranslationString.COLLECTING_INFORMATION] + title;

            bool success = await movie.GetInfo(infoProvider, title);

            if (success)
            {
                return movie;
            }
            else
            {
                Log.Logger.Error($"Something went wrong while collecting Informations about {title}");
                NotCollectedInformation.Add(title);
                return null;
            }
        }

        private List<string> DirSearch(string sDir)
        {
            List<string> files = new List<string>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Something went wrong while cleaning up database {this}, Message: {ex.Message}\nStacktrace: {ex.StackTrace}");
            }

            return files;
        }

        private bool CheckIfFileIsRegistered(string file)
        {
            if (databaseMediator.GetCurrentDatabase().Movies.Where(x => x.FilePath == file).Any())
            {
                return true;
            }
            return false;
        }

        public static string GetRealTitle(string title)
        {
            string content = File.ReadAllText("./Resources/Regex.txt").Trim().Remove('\n');
            string result = Regex.Replace(title, content, "", RegexOptions.IgnoreCase);
            return result;
        }
    }
}