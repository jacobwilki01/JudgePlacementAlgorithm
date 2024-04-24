using MvvmHelpers.Commands;
using JudgePlacement.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using JudgePlacement.Data;
using System.IO;

namespace JudgePlacement.ViewModel
{
    public class ApplicationControlViewModel : BaseViewModel
    {
        private string _importFilePath = string.Empty;

        public string ImportFilePath
        {
            get { return _importFilePath; }
            set { _importFilePath = value; NotifyPropertyChanged(); }
        }

        private string _exportFilePath = string.Empty;

        public string ExportFilePath
        {
            get { return _exportFilePath; }
            set { _exportFilePath = value; NotifyPropertyChanged(); }
        }

        public Command BrowseDataFile { get; }

        public Command BrowseExportFile { get; }

        public Command ExportCurrent { get; }

        public Command ImportTournament { get; }
        
        public Command UpdateCurrent { get; }

        private ObservableCollection<KeyValuePair<Tournament, string>> _loadedTournaments = new();

        public ReadOnlyObservableCollection<KeyValuePair<Tournament, string>> LoadedTournaments
        {
            get { return new(_loadedTournaments); }
        }

        private KeyValuePair<Tournament, string> _selectedTournamentPair;

        public KeyValuePair<Tournament, string> SelectedTournamentPair
        {
            get { return _selectedTournamentPair; }
            set 
            { 
                _selectedTournamentPair = value; 
                SelectedTournament = value.Key; 
                NotifyPropertyChanged(); 
            }
        }

        public Tournament SelectedTournament;

        private List<string> _loadedfiles = new();

        public ApplicationControlViewModel()
        {
            KeyValuePair<Tournament, string> blankSelection = new(new Tournament() { Name = "Placeholder Tournament" }, "None");

            _loadedTournaments.Add(blankSelection);
            SelectedTournamentPair = blankSelection;
            SelectedTournament = blankSelection.Key;

            BrowseDataFile = new(BrowseDataFileCommand);
            ImportTournament = new(ImportTournamentCommand);
        }

        public void BrowseDataFileCommand()
        {
            OpenFileDialog dialog = new();
            dialog.ShowDialog();

            if (!dialog.CheckPathExists)
                return;

            ImportFilePath = dialog.FileName;
        }

        public void ImportTournamentCommand()
        {
            string jsonString = File.ReadAllText(ImportFilePath);

            if (_loadedfiles.Contains(jsonString))
            {
                MessageBox.Show("Identical file has already been loaded!");
                return;
            }

            Tournament newTourn = TournamentJSONProcessor.CreateNewTournament(jsonString);

            foreach (KeyValuePair<Tournament, string> pair in _loadedTournaments)
            {
                Tournament pairTourn = pair.Key;

                if (pairTourn.Name.Equals(newTourn.Name) && pairTourn.Year.Equals(newTourn.Year))
                {
                    if (MessageBox.Show("A tournament with the same name and year has been loaded previously. If you are trying to load a duplicate, hit \"OK\". Otherwise, hit \"Cancel\" and select \"Update Existing\" instead.", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        newTourn.Name = newTourn.Name + " - Copy";
                        break;
                    }
                    else
                        return;
                }
            }

            KeyValuePair<Tournament, string> newSelection = new(newTourn, $"{newTourn.Year} {newTourn.Name}");

            _loadedfiles.Add(jsonString);
            _loadedTournaments.Add(newSelection);
            SelectedTournamentPair = newSelection;
            SelectedTournament = newTourn;
        }
    }
}
