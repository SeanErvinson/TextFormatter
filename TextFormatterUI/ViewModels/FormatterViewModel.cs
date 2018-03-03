using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using StringManipulation;

namespace TextFormatterUI
{
    class FormatterViewModel : BaseViewModel
    {
        #region Public Properties
        /// <summary>
        /// The input text area
        /// </summary>
        public string InputTextArea { get; set; }

        /// <summary>
        /// The output text area
        /// </summary>
        public string OutputTextArea { get; set; }

        /// <summary>
        /// State of case sensitivity
        /// </summary>
        public bool IsCaseSensitive { get; set; } = false;

        /// <summary>
        /// State of persistency
        /// </summary>
        public bool IsPersistent { get; set; } = false;

        /// <summary>
        /// Word/Char to be removed
        /// </summary>
        public string RemoveTerm { get; set; } = null;

        /// <summary>
        /// The type of the desired output array
        /// </summary>
        public ArrayType SelectedArrayType { get; set; }

        /// <summary>
        /// The string to replace all occurrences of OldWord.
        /// </summary>
        public string NewWord { get; set; } = null;

        /// <summary>
        /// The string to be replaced.
        /// </summary>
        public string OldWord { get; set; } = null;

        /// <summary>
        /// Enumerable of the array types
        /// </summary>
        public IEnumerable<ArrayType> ArrayTypeValues { get { return Enum.GetValues(typeof(ArrayType)).Cast<ArrayType>(); } }
        
        #endregion

        #region Private Commands
        /// <summary>
        /// Copies the entire board to the clipboard
        /// </summary>
        private ICommand _copyToClipboardCommand;

        /// <summary>
        /// Save the text area
        /// </summary>
        private ICommand _saveCommand;

        /// <summary>
        /// Load from a file
        /// </summary>
        private ICommand _loadCommand;

        /// <summary>
        /// Clears the text area
        /// </summary>
        private ICommand _clearCommand;

        /// <summary>
        /// Remove all spaces on the textarea
        /// </summary>
        private ICommand _removeSpaceCommand;

        /// <summary>
        /// Remove all tabs on the textarea
        /// </summary>
        private ICommand _removeTabsCommand;

        /// <summary>
        /// Remove all linebreaks on the textarea
        /// </summary>
        private ICommand _removeLineBreaksCommand;

        /// <summary>
        /// Change text to upper
        /// </summary>
        private ICommand _allUpperCommand;

        /// <summary>
        /// Change text to lower
        /// </summary>
        private ICommand _allLowerCommand;

        /// <summary>
        /// Remove a specific word with the given word
        /// </summary>
        private ICommand _removeWordCommand;

        /// <summary>
        /// Replace a specific word with the given word
        /// </summary>
        private ICommand _replaceWordCommand;

        /// <summary>
        /// Converts the text into an array friendly format
        /// </summary>
        private ICommand _arrayParseCommand;

        /// <summary>
        /// Open a about window
        /// </summary>
        private ICommand _aboutCommand;
        
        #endregion

        #region Public Commands
        /// <summary>
        /// Copies the entire board to the clipboard
        /// </summary>
        public ICommand CopyToClipboardCommand
        {
            get
            {
                return _copyToClipboardCommand ?? (_copyToClipboardCommand = new RelayCommand(param => CopyClipboard(param)));
            }
        }

        /// <summary>
        /// Save the text area
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(param => SaveFile(param)));
            }
        }

        /// <summary>
        /// Load from a file
        /// </summary>
        public ICommand LoadCommand
        {
            get
            {
                return _loadCommand ?? (_loadCommand = new RelayCommand(param => LoadFile()));
            }
        }

        /// <summary>
        /// Clears the text area
        /// </summary>
        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new RelayCommand(param => ClearText(param)));
            }
        }

        /// <summary>
        /// Remove all spaces on the textarea
        /// </summary>
        public ICommand RemoveSpacesCommand
        {
            get
            {
                return _removeSpaceCommand ?? (_removeSpaceCommand = new RelayCommand(param => RemoveSpaces()));
            }
        }

        /// <summary>
        /// Remove all tabs on the textarea
        /// </summary>
        public ICommand RemoveTabsCommand
        {
            get
            {
                return _removeTabsCommand ?? (_removeTabsCommand = new RelayCommand(param => RemoveTabs()));
            }
        }

        /// <summary>
        /// Remove all line breaks on the textarea
        /// </summary>
        public ICommand RemoveLineBreaksCommand
        {
            get
            {
                return _removeLineBreaksCommand ?? (_removeLineBreaksCommand = new RelayCommand(param => RemoveLineBreaks()));
            }
        }

        /// <summary>
        /// Change text to upper
        /// </summary>
        public ICommand AllUpperCommand
        {
            get
            {
                return _allUpperCommand ?? (_allUpperCommand = new RelayCommand(param => ToUpper()));
            }
        }

        /// <summary>
        /// Change text to lower
        /// </summary>
        public ICommand AllLowerCommand
        {
            get
            {
                return _allLowerCommand ?? (_allLowerCommand = new RelayCommand(param => ToLower()));
            }
        }

        /// <summary>
        /// Remove a specific word with the given word
        /// </summary>
        public ICommand RemoveWordCommand
        {
            get
            {
                return _removeWordCommand ?? (_removeWordCommand = new RelayCommand(param => RemoveWord()));
            }
        }

        /// <summary>
        /// Replace a specific word with the given word
        /// </summary>
        public ICommand ReplaceWordCommand
        {
            get
            {
                return _replaceWordCommand ?? (_replaceWordCommand = new RelayCommand(param => ReplaceWord()));
            }
        }

        /// <summary>
        /// Converts the text into an array friendly format
        /// </summary>
        public ICommand ArrayParseCommand
        {
            get { return _arrayParseCommand ?? (_arrayParseCommand = new RelayCommand(param => ParseArray())); }
        }

        /// <summary>
        /// Open a about window
        /// </summary>
        public ICommand AboutCommand
        {
            get { return _aboutCommand ?? (_aboutCommand = new RelayCommand(param => AboutWindow())); }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Clear the textarea
        /// </summary>
        /// <param name="parameter">Which textarea to be cleared base on the name of the textbox (needs to be improved)</param>
        private void ClearText(object parameter)
        {
            if(parameter as string == nameof(InputTextArea))
            {
                InputTextArea = string.Empty;
                OnPropertyChanged("InputTextArea");
            }
            else
            {
                OutputTextArea = string.Empty;
                OnPropertyChanged("OutputTextArea");
            }
        }

        /// <summary>
        /// Save file to destination
        /// </summary>
        /// <param name="parameter">Which textarea is to be saved</param>
        private void SaveFile(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|All Files|*.*";
            saveFileDialog.Title = "Please select a location to save";
            saveFileDialog.DefaultExt = "txt";

            string textContent = parameter as string;

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, (string)textContent);
            }
        }

        /// <summary>
        /// Copies the text to the clipboard
        /// </summary>
        /// <param name="parameter">Which textarea to be copied</param>
        private void CopyClipboard(object parameter)
        {
            string textContent = parameter as string;
            Clipboard.SetText(textContent);
        }

        /// <summary>
        /// Load a text file to the input textarea
        /// </summary>
        private void LoadFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt|All Files|*.*";
            openFileDialog.Title = "Open a text file";
            openFileDialog.DefaultExt = "txt";

            if(openFileDialog.ShowDialog() == true)
            {
                StringBuilder stringBuilder = new StringBuilder();
                var file = openFileDialog.OpenFile();
                string line;
                using (var fileStream = new StreamReader(file))
                {
                    while((line = fileStream.ReadLine()) != null)
                    {
                        stringBuilder.AppendLine(line);
                    }
                    InputTextArea = stringBuilder.ToString();
                    OnPropertyChanged("InputTextArea");
                }
            }
        }

        /// <summary>
        /// Remove all the spaces
        /// </summary>
        private void RemoveSpaces()
        {
            OutputTextArea = StringManipulate.Replace(InputTextArea, " ", string.Empty);
            OnPropertyChanged("OutputTextArea");
            if (IsPersistent && OutputTextArea != null)
            {
                InputTextArea = OutputTextArea;
                OnPropertyChanged("InputTextArea");
            }
        }

        /// <summary>
        /// Remove all the tabs
        /// </summary>
        private void RemoveTabs()
        {
            OutputTextArea = StringManipulate.Replace(InputTextArea, "\t", string.Empty);
            OnPropertyChanged("OutputTextArea");
            if (IsPersistent && OutputTextArea != null)
            {
                InputTextArea = OutputTextArea;
                OnPropertyChanged("InputTextArea");
            }
        }

        /// <summary>
        /// Remove all the linebreak
        /// </summary>
        private void RemoveLineBreaks()
        {
            OutputTextArea = StringManipulate.Replace(InputTextArea, "\r?\n|\r", string.Empty);
            OnPropertyChanged("OutputTextArea");
            if (IsPersistent && OutputTextArea != null)
            {
                InputTextArea = OutputTextArea;
                OnPropertyChanged("InputTextArea");
            }
        }

        /// <summary>
        /// Convert string to Uppercase
        /// </summary>
        private void ToUpper()
        {
            if (string.IsNullOrEmpty(InputTextArea))
                return;
            OutputTextArea = InputTextArea.ToUpper();
            OnPropertyChanged("OutputTextArea");
            if (IsPersistent && OutputTextArea != null)
            {
                InputTextArea = OutputTextArea;
                OnPropertyChanged("InputTextArea");
            }
        }

        /// <summary>
        /// Convert string to Lowercase
        /// </summary>
        private void ToLower()
        {
            if (string.IsNullOrEmpty(InputTextArea))
                return;
            OutputTextArea = InputTextArea.ToLower();
            OnPropertyChanged("OutputTextArea");
            if (IsPersistent && OutputTextArea != null)
            {
                InputTextArea = OutputTextArea;
                OnPropertyChanged("InputTextArea");
            }
        }

        /// <summary>
        /// Remove a specific word from the given word
        /// </summary>
        private void RemoveWord()
        {
            OutputTextArea = StringManipulate.Replace(InputTextArea, RemoveTerm, string.Empty, IsCaseSensitive);
            OnPropertyChanged("OutputTextArea");
            if (IsPersistent && OutputTextArea != null)
            {
                InputTextArea = OutputTextArea;
                OnPropertyChanged("InputTextArea");
            }
        }

        /// <summary>
        /// Replace all occurance of the word from the given word
        /// </summary>
        private void ReplaceWord()
        {
            OutputTextArea = StringManipulate.Replace(InputTextArea, OldWord, NewWord, IsCaseSensitive);
            OnPropertyChanged("OutputTextArea");
            if (IsPersistent && OutputTextArea != null)
            {
                InputTextArea = OutputTextArea;
                OnPropertyChanged("InputTextArea");
            }
        }

        /// <summary>
        /// Convert string into a array friendly format
        /// </summary>
        private void ParseArray()
        {
            OutputTextArea = StringManipulate.ArrayFormat(InputTextArea, SelectedArrayType);
            OnPropertyChanged("OutputTextArea");
            if (IsPersistent && OutputTextArea != null)
            {
                InputTextArea = OutputTextArea;
                OnPropertyChanged("InputTextArea");
            }
        }

        /// <summary>
        /// Open the about page
        /// </summary>
        private void AboutWindow()
        {
            About aboutWindow = new About
            {
                DataContext = new AboutViewModel()
            };
            aboutWindow.ShowDialog();
        }

       

        #endregion

    }
}
