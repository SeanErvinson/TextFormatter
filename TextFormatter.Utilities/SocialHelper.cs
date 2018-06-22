using System;
using System.Diagnostics;

namespace TextFormatter.Utilities
{
    public static class SocialLinks
    {
        private static readonly string _githubLink = @"https://github.com/SeanErvinson";
        private static readonly string _linkedinLink = @"www.linkedin.com/in/sean-ervinson-ong-2550b3127";
        private static readonly string _twitterLink = @"https://twitter.com/ASean___";

        public static void Github()
        {
            ExecuteLinks(_githubLink);
        }

        public static void Linkedin()
        {
            ExecuteLinks(_linkedinLink);
        }

        public static void Twitter()
        {
            ExecuteLinks(_twitterLink);
        }

        private static void ExecuteLinks(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
