using System;
using System.Diagnostics;

namespace TextFormatter.Models
{
    public class SocialLinks
    {
        private static string twitterLink = @"https://twitter.com/ASean___";
        private static string linkedinLink = @"www.linkedin.com/in/sean-ervinson-ong-2550b3127";
        private static string githubLink = @"https://github.com/SeanErvinson";

        public static void Twitter()
        {
            ExecuteLinks(twitterLink);
        }

        public static void Linkedin()
        {
            ExecuteLinks(linkedinLink);
        }

        public static void Github()
        {
            ExecuteLinks(githubLink);
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
