using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using TextFormatter.Models.TextManipulate;
using TextFormatter.WPF.ViewModels.Base;

namespace TextFormatter.WPF
{
    class FormatterViewModel : BaseViewModel
    {
        #region Private Properties

        /// <summary>
        /// The input text area
        /// </summary>
        private string _inputTextArea;

        /// <summary>
        /// The output text area
        /// </summary>
        private string _outputTextArea;

        /// <summary>
        /// State of case sensitivity
        /// </summary>
        private bool _isCaseSensitive = false;

        /// <summary>
        /// State of persistency
        /// </summary>
        private bool _isPersistent = false;

        /// <summary>
        /// Value to be removed
        /// </summary>
        private string _removeValue = null;

        /// <summary>
        /// The type of the desired output array
        /// </summary>
        private ArrayFormat _selectedArrayType;

        /// <summary>
        /// The string to replace all occurrences of OldWord.
        /// </summary>
        private string _newWord;

        /// <summary>
        /// The string to be replaced.
        /// </summary>
        private string _oldWord;

        /// <summary>
        /// Temporary text container
        /// </summary>
        private string _tempText;

        #endregion

        #region Public Properties
        /// <summary>
        /// The input text area
        /// </summary>
        public string InputTextArea
        {
            get { return _inputTextArea; }
            set
            {
                SetField(ref _inputTextArea, value, nameof(InputTextArea));
                _tempText = InputTextArea;
            }
        }

        /// <summary>
        /// The output text area
        /// </summary>
        public string OutputTextArea
        {
            get { return _outputTextArea; }
            set { SetField(ref _outputTextArea, value, nameof(OutputTextArea)); }
        }

        /// <summary>
        /// State of case sensitivity
        /// </summary>
        public bool IsCaseSensitive
        {
            get { return _isCaseSensitive; }
            set { SetField(ref _isCaseSensitive, value, nameof(IsCaseSensitive)); }
        }

        /// <summary>
        /// State of persistency
        /// </summary>
        public bool IsPersistent
        {
            get { return _isPersistent; }
            set { SetField(ref _isPersistent, value, nameof(IsPersistent)); }
        }

        /// <summary>
        /// Value to be removed
        /// </summary>
        public string RemoveValue
        {
            get { return _removeValue; }
            set { SetField(ref _removeValue, value, nameof(RemoveValue)); }
        }

        /// <summary>
        /// The type of the desired output array
        /// </summary>
        public ArrayFormat SelectedArrayType
        {
            get { return _selectedArrayType; }
            set { SetField(ref _selectedArrayType, value, nameof(SelectedArrayType)); }
        }

        /// <summary>
        /// The string to replace all occurrences of OldWord.
        /// </summary>
        public string NewWord
        {
            get { return _newWord; }
            set { SetField(ref _newWord, value, nameof(NewWord)); }
        }

        /// <summary>
        /// The string to be replaced.
        /// </summary>
        public string OldWord
        {
            get { return _oldWord; }
            set { SetField(ref _oldWord, value, nameof(OldWord)); }
        }

        /// <summary>
        /// Enumerable of the array types
        /// </summary>
        public IEnumerable<ArrayFormat> ArrayTypeValues { get { return Enum.GetValues(typeof(ArrayFormat)).Cast<ArrayFormat>(); } }

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
            get { return _copyToClipboardCommand ?? (_copyToClipboardCommand = new RelayCommand(param => CopyClipboard(param))); }
        }

        /// <summary>
        /// Save the text area
        /// </summary>
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(param => SaveFile(param))); }
        }

        /// <summary>
        /// Load from a file
        /// </summary>
        public ICommand LoadCommand
        {
            get { return _loadCommand ?? (_loadCommand = new AsyncRelayCommand(() => LoadFileAsync())); }
        }

        /// <summary>
        /// Clears the text area
        /// </summary>
        public ICommand ClearCommand
        {
            get { return _clearCommand ?? (_clearCommand = new RelayCommand(param => ClearText(param))); }
        }

        /// <summary>
        /// Remove all spaces on the textarea
        /// </summary>
        public ICommand RemoveSpacesCommand
        {
            get { return _removeSpaceCommand ?? (_removeSpaceCommand = new AsyncRelayCommand(() => RemoveAsync(@" "))); }
        }

        /// <summary>
        /// Remove all tabs on the textarea
        /// </summary>
        public ICommand RemoveTabsCommand   
        {
            get { return _removeTabsCommand ?? (_removeTabsCommand = new AsyncRelayCommand(() => RemoveAsync(@"\t"))); }
        }

        /// <summary>
        /// Remove all line breaks on the textarea
        /// </summary>
        public ICommand RemoveLineBreaksCommand
        {
            get { return _removeLineBreaksCommand ?? (_removeLineBreaksCommand = new AsyncRelayCommand(() => RemoveAsync(@"\r\n?|\n"))); }
        }

        /// <summary>
        /// Change text to upper
        /// </summary>
        public ICommand AllUpperCommand
        {
            get { return _allUpperCommand ?? (_allUpperCommand = new AsyncRelayCommand(() => ChangeLetterCaseAsync(LetterCase.Upper))); }
        }

        /// <summary>
        /// Change text to lower
        /// </summary>
        public ICommand AllLowerCommand
        {
            get { return _allLowerCommand ?? (_allLowerCommand = new AsyncRelayCommand(() => ChangeLetterCaseAsync(LetterCase.Lower))); }
        }

        /// <summary>
        /// Remove a specific word with the given word
        /// </summary>
        public ICommand RemoveWordCommand
        {
            get { return _removeWordCommand ?? (_removeWordCommand = new AsyncRelayCommand(() => RemoveAsync(RemoveValue, caseSensitive: IsCaseSensitive))); }
        }

        /// <summary>
        /// Replace a specific word with the given word
        /// </summary>
        public ICommand ReplaceWordCommand
        {
            get { return _replaceWordCommand ?? (_replaceWordCommand = new AsyncRelayCommand(() => RemoveAsync(OldWord, NewWord, IsCaseSensitive))); }
        }

        /// <summary>
        /// Converts the text into an array friendly format
        /// </summary>
        public ICommand ArrayParseCommand
        {
            get { return _arrayParseCommand ?? (_arrayParseCommand = new AsyncRelayCommand(() => ParseArrayAsync())); }
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
            if (parameter as string == nameof(InputTextArea))
                InputTextArea = string.Empty;
            else
                OutputTextArea = string.Empty;
            _tempText = InputTextArea;
        }

        /// <summary>
        /// Save file to destination
        /// </summary>
        /// <param name="parameter">Which textarea is to be saved</param>
        private void SaveFile(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text file (*.txt)|*.txt|All Files|*.*",
                Title = "Please select a location to save",
                DefaultExt = "txt"
            };

            string textContent = parameter as string;

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, textContent);
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
        private async Task LoadFileAsync()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text file |*.txt;*.json;*.xml;*.rtf|All Files|*.*",
                Title = "Open a text file",
                DefaultExt = "txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                StringBuilder stringBuilder = new StringBuilder();
                var file = openFileDialog.OpenFile();
                using (var fileStream = new StreamReader(file))
                {
                    string line;
                    while((line = await fileStream.ReadLineAsync()) != null)
                        stringBuilder.AppendLine(line);

                    InputTextArea = stringBuilder.ToString();
                }
            }
        }

        /// <summary>
        /// Convert string to Lower or Upper case
        /// </summary>
        /// <param name="caseType">The type of letter case</param>
        /// <returns></returns>
        public async Task ChangeLetterCaseAsync(LetterCase caseType)
        {
            if (string.IsNullOrEmpty(InputTextArea))
                return;
            switch (caseType)
            {
                case LetterCase.Upper:
                    OutputTextArea = await Task.Run(() => _tempText.ToUpper());
                    break;
                case LetterCase.Lower:
                    OutputTextArea = await Task.Run(() => _tempText.ToLower());
                    break;
            }
            UpdatePersistency();
        }

        /// <summary>
        /// Replace specific character with given replacement
        /// </summary>
        /// <param name="pattern">Character/Word to be replace</param>
        /// <param name="replacement">Replace with. Default value is empty</param>
        /// <param name="caseSensitive">Case sensitivity. Default value is false</param>
        /// <returns></returns>
        public async Task RemoveAsync(string pattern, string replacement = "", bool caseSensitive = false)
        {
            OutputTextArea = await StringManipulate.Replace(_tempText, pattern, replacement, caseSensitive);
            UpdatePersistency();
        }

        /// <summary>
        /// Convert string into a array friendly format
        /// </summary>
        private async Task ParseArrayAsync()
        {
            OutputTextArea = await StringManipulate.ArrayFormat(_tempText, SelectedArrayType);
            UpdatePersistency();
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

        /// <summary>
        /// Notifies the output text to be the input text
        /// </summary>
        private void UpdatePersistency()
        {
            if (IsPersistent && OutputTextArea != null)
                _tempText = OutputTextArea;
        }

        #endregion

    }
}
