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
            scraperUtility utility = new scraperUtility();
            utility.Reset(BaseDir);
            utility.createDir(BaseDir);

            scraper discogsScraper = new scraper();
            discogsScraper.getRecords(20, BaseDir);
        }
    }
}
