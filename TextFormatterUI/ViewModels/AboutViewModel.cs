using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SocialMediaLinks;

namespace TextFormatterUI
{
    class AboutViewModel : BaseViewModel
    {
        private ICommand _twitterCommand;
        private ICommand _githubCommand;
        private ICommand _linkedinCommand;
        private ICommand _facebookCommand;

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


        public ICommand FacebookCommand
        {
            get { return _facebookCommand; }
        }

    }
}
