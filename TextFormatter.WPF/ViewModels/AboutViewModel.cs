using System.Windows.Input;
using TextFormatter.Utilities;
using TextFormatter.WPF.ViewModels.Base;

namespace TextFormatter.WPF
{
    class AboutViewModel : BaseViewModel
    {
        private ICommand _twitterCommand;
        private ICommand _githubCommand;
        private ICommand _linkedinCommand;

        public ICommand TwitterCommand
        {
            get
            {
                return _twitterCommand ?? (_twitterCommand = new RelayCommand(parama => SocialLinks.Twitter()));
            }
        }

        public ICommand GithubCommand
        {
            get
            {
                return _githubCommand ?? (_githubCommand = new RelayCommand(param => SocialLinks.Github()));
            }
        }

        public ICommand LinkedinCommand
        {
            get
            {
                return _linkedinCommand ?? (_linkedinCommand = new RelayCommand(param => SocialLinks.Linkedin()));
            }
        }
    }
}
