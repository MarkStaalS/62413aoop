using webScraper.Models;
using webScraper.DataOperations;
using System;
using System.Configuration;

namespace webScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            string BaseDir = @"C:\Users\@UserName\Desktop\test\"; //folder for the images usig username
            BaseDir = BaseDir.Replace("@UserName", Environment.UserName);
            scraperUtility Utility = new scraperUtility();
            Utility.Reset(BaseDir);
            Utility.CreateDir(BaseDir);

            Scraper discogsScraper = new Scraper();
            discogsScraper.GetRecords(20, BaseDir);
        }
    }
}
