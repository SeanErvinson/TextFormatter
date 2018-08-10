using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TextFormatter.Utilities;
using TextFormatter.WPF.ViewModels.Base;

namespace TextFormatter.WPF
{
    internal class FormatterViewModel : BaseViewModel
    {
        #region Private Properties

        /// <summary>
        /// Number of affected character
        /// </summary>
        private int _affectedCharacter;

        /// <summary>
        /// The input text area
        /// </summary>
        private string _inputTextArea;

        /// <summary>
        /// Value to be inserted
        /// </summary>
        private string _insertValue;

        /// <summary>
        /// State of case sensitivity
        /// </summary>
        private bool _isCaseSensitive;

        /// <summary>
        /// State of persistency
        /// </summary>
        private bool _isPersistent;

        /// <summary>
        /// The string to replace all occurrences of OldWord.
        /// </summary>
        private string _newWord;

        /// <summary>
        /// The string to be replaced.
        /// </summary>
        private string _oldWord;

        /// <summary>
        /// The output text area
        /// </summary>
        private string _outputTextArea;

        /// <summary>
        /// Value to be removed
        /// </summary>
        private string _removeValue;

        /// <summary>
        /// The type of the desired output array
        /// </summary>
        private ArrayFormat _selectedArrayType;

        /// <summary>
        /// The position where a character is to be inserted
        /// </summary>
        private InsertPosition _selectedPosition;

        /// <summary>
        /// Temporary text container
        /// </summary>
        private string _tempText;

        /// <summary>
        /// Reference the class for logging
        /// </summary>
        private readonly ILog _logger = LogManager.GetLogger(typeof(FormatterViewModel));

        #endregion Private Properties

        #region Public Properties

        /// <summary>
        /// Number of affected character
        /// </summary>
        public int AffectedCharacter
        {
            get { return _affectedCharacter; }
            set { SetField(ref _affectedCharacter, value, nameof(AffectedCharacter)); }
        }

        /// <summary>
        /// Enumerable of the array types
        /// </summary>
        public IEnumerable<ArrayFormat> ArrayTypeValues { get { return Enum.GetValues(typeof(ArrayFormat)).Cast<ArrayFormat>(); } }

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

        public IEnumerable<InsertPosition> InsertPositionValues { get { return Enum.GetValues(typeof(InsertPosition)).Cast<InsertPosition>(); } }

        /// <summary>
        /// Value to be inserted
        /// </summary>
        public string InsertValue
        {
            get { return _insertValue; }
            set { SetField(ref _insertValue, value, nameof(InsertValue)); }
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
        /// The output text area
        /// </summary>
        public string OutputTextArea
        {
            get { return _outputTextArea; }
            set { SetField(ref _outputTextArea, value, nameof(OutputTextArea)); }
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
        /// The position where a character is to be inserted
        /// </summary>
        public InsertPosition SelectedPosition
        {
            get { return _selectedPosition; }
            set { SetField(ref _selectedPosition, value, nameof(SelectedPosition)); }
        }

        #endregion Public Properties

        #region Private Commands

        /// <summary>
        /// Open a about window
        /// </summary>
        private ICommand _aboutCommand;

        /// <summary>
        /// Change text to lower
        /// </summary>
        private ICommand _allLowerCommand;

        /// <summary>
        /// Change text to upper
        /// </summary>
        private ICommand _allUpperCommand;

        /// <summary>
        /// Converts the text into an array friendly format
        /// </summary>
        private ICommand _arrayParseCommand;

        /// <summary>
        /// Clears the text area
        /// </summary>
        private ICommand _clearCommand;

        /// <summary>
        /// Copies the entire board to the clipboard
        /// </summary>
        private ICommand _copyToClipboardCommand;

        private ICommand _insertCommand;

        /// <summary>
        /// Load from a file
        /// </summary>
        private ICommand _loadCommand;

        /// <summary>
        /// Remove all linebreaks on the textarea
        /// </summary>
        private ICommand _removeLineBreaksCommand;

        /// <summary>
        /// Remove all spaces on the textarea
        /// </summary>
        private ICommand _removeSpaceCommand;

        /// <summary>
        /// Remove all tabs on the textarea
        /// </summary>
        private ICommand _removeTabsCommand;

        /// <summary>
        /// Remove a specific word with the given word
        /// </summary>
        private ICommand _removeWordCommand;

        /// <summary>
        /// Replace a specific word with the given word
        /// </summary>
        private ICommand _replaceWordCommand;

        /// <summary>
        /// Save the text area
        /// </summary>
        private ICommand _saveCommand;

        #endregion Private Commands

        #region Public Commands

        /// <summary>
        /// Open a about window
        /// </summary>
        public ICommand AboutCommand
        {
            get { return _aboutCommand ?? (_aboutCommand = new RelayCommand(param => AboutWindow())); }
        }

        /// <summary>
        /// Change text to lower
        /// </summary>
        public ICommand AllLowerCommand
        {
            get { return _allLowerCommand ?? (_allLowerCommand = new AsyncRelayCommand(() => ChangeLetterCaseAsync(LetterCase.Lower))); }
        }

        /// <summary>
        /// Change text to upper
        /// </summary>
        public ICommand AllUpperCommand
        {
            get { return _allUpperCommand ?? (_allUpperCommand = new AsyncRelayCommand(() => ChangeLetterCaseAsync(LetterCase.Upper))); }
        }

        /// <summary>
        /// Converts the text into an array friendly format
        /// </summary>
        public ICommand ArrayParseCommand
        {
            get { return _arrayParseCommand ?? (_arrayParseCommand = new AsyncRelayCommand(ParseArrayAsync)); }
        }

        /// <summary>
        /// Clears the text area
        /// </summary>
        public ICommand ClearCommand
        {
            get { return _clearCommand ?? (_clearCommand = new RelayCommand(ClearText)); }
        }

        /// <summary>
        /// Copies the entire board to the clipboard
        /// </summary>
        public ICommand CopyToClipboardCommand
        {
            get { return _copyToClipboardCommand ?? (_copyToClipboardCommand = new RelayCommand(CopyClipboard)); }
        }

        /// <summary>
        /// Insert a given value to either the start/ending position
        /// </summary>
        /// <returns></returns>
        public ICommand InsertCommand
        {
            get { return _insertCommand ?? (_insertCommand = new AsyncRelayCommand(InsertAsync)); }
        }

        /// <summary>
        /// Load from a file
        /// </summary>
        public ICommand LoadCommand
        {
            get { return _loadCommand ?? (_loadCommand = new AsyncRelayCommand(LoadFileAsync)); }
        }

        /// <summary>
        /// Remove all line breaks on the textarea
        /// </summary>
        public ICommand RemoveLineBreaksCommand
        {
            get { return _removeLineBreaksCommand ?? (_removeLineBreaksCommand = new AsyncRelayCommand(() => ReplaceAsync(@"\r\n?|\n", escapedCharacter : false))); }
        }

        /// <summary>
        /// Remove all spaces on the textarea
        /// </summary>
        public ICommand RemoveSpacesCommand
        {
            get { return _removeSpaceCommand ?? (_removeSpaceCommand = new AsyncRelayCommand(() => ReplaceAsync(@" "))); }
        }

        /// <summary>
        /// Remove all tabs on the textarea
        /// </summary>
        public ICommand RemoveTabsCommand
        {
            get { return _removeTabsCommand ?? (_removeTabsCommand = new AsyncRelayCommand(() => ReplaceAsync(@"\t", escapedCharacter: false))); }
        }

        /// <summary>
        /// Remove a specific word with the given word
        /// </summary>
        public ICommand RemoveWordCommand
        {
            get { return _removeWordCommand ?? (_removeWordCommand = new AsyncRelayCommand(() => ReplaceAsync(RemoveValue, caseSensitive: IsCaseSensitive))); }
        }

        /// <summary>
        /// Replace a specific word with the given word
        /// </summary>
        public ICommand ReplaceWordCommand
        {
            get { return _replaceWordCommand ?? (_replaceWordCommand = new AsyncRelayCommand(() => ReplaceAsync(OldWord, NewWord, IsCaseSensitive))); }
        }

        /// <summary>
        /// Save the text area
        /// </summary>
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new AsyncRelayCommand(SaveFileAsync)); }
        }

        #endregion Public Commands

        #region Helper Methods

        /// <summary>
        /// Open the about page
        /// </summary>
        private void AboutWindow()
        {
            var aboutWindow = new About
            {
                DataContext = new AboutViewModel()
            };
            aboutWindow.ShowDialog();
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
            AffectedCharacter = _tempText.Length - Regex.Matches(_tempText, @"\s|\d").Count;
            UpdatePersistency();
        }

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
            AffectedCharacter = 0;
        }

        /// <summary>
        /// Copies the text to the clipboard
        /// </summary>
        /// <param name="parameter">Which textarea to be copied</param>
        private static void CopyClipboard(object parameter)
        {
            var textContent = parameter as string;
            Clipboard.SetText(textContent);
        }

        /// <summary>
        /// Insert a given value to either the start/ending position
        /// </summary>
        /// <returns></returns>
        public async Task InsertAsync()
        {
            if (string.IsNullOrEmpty(InsertValue))
                return;
            OutputTextArea = await StringHelper.InsertAsync(_tempText, InsertValue, SelectedPosition);
            UpdatePersistency();
        }

        /// <summary>
        /// Load a text file to the input textarea
        /// </summary>
        private async Task LoadFileAsync()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text file |*.txt;*.json;*.xml;*.rtf|All Files|*.*",
                Title = "Open a text file",
                DefaultExt = "txt"
            };

            if (openFileDialog.ShowDialog() == true)
                InputTextArea = await IOHelper.LoadFileAsync(openFileDialog.FileName);
        }

        /// <summary>
        /// Convert string into a array friendly format
        /// </summary>
        private async Task ParseArrayAsync()
        {
            OutputTextArea = await StringHelper.ArrayStructureAsync(_tempText, SelectedArrayType);
            AffectedCharacter = StringHelper.AffectedCharacter;
            UpdatePersistency();
        }

        /// <summary>
        /// Replace specific character with given replacement
        /// </summary>
        /// <param name="pattern">Character/Word to be replace</param>
        /// <param name="replacement">Replace with. Default value is empty</param>
        /// <param name="caseSensitive">Case sensitivity. Default value is false</param>
        /// <returns></returns>
        public async Task ReplaceAsync(string pattern, string replacement = "", bool caseSensitive = false, bool escapedCharacter = true)
        {
            if (pattern == null || replacement == null)
                return;
            OutputTextArea = await StringHelper.ReplaceAsync(_tempText, pattern, replacement, caseSensitive, escapedCharacter);
            AffectedCharacter = StringHelper.AffectedCharacter;
            UpdatePersistency();
        }

        /// <summary>
        /// Save file to destination
        /// </summary>
        /// <param name="parameter">Which textarea is to be saved</param>
        private static async Task SaveFileAsync(object parameter)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text file (*.txt)|*.txt|All Files|*.*",
                Title = "Please select a location to save",
                DefaultExt = "txt"
            };

            var textContent = parameter as string;

            if (saveFileDialog.ShowDialog() == true)
                await IOHelper.SaveFileAsync(saveFileDialog.FileName, textContent);
        }

        /// <summary>
        /// Notifies the output text to be the input text
        /// </summary>
        private void UpdatePersistency()
        {
            if (IsPersistent && OutputTextArea != null)
                _tempText = OutputTextArea;
        }

        #endregion Helper Methods
    }
}