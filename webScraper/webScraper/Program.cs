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
            string BaseDir = @"C:\Users\" + Environment.UserName + @"\Desktop\test\"; //folder for the images usig username
            //BaseDir = BaseDir.Replace("@UserName", Environment.UserName);

            //User interface
            int x = 0;
            bool looping = true;
            Console.WriteLine("Please enter command: " +
                    "\nFor additional informaiton enter 'info'");
            while (looping)
            {
                string input = Console.ReadLine();
                if (input.Contains("info"))
                {
                    Console.WriteLine("\ninfo \t\t print additional information" +
                            "\ndir -p \t print dir for image folder" +
                            "\ndir -c \t\t change dir for image folder, this will reset current dir" +
                            "\nscrape x \t command to scrape x records" +
                            "\nreset \t\t resets database and image folder" +
                            "\nexit \t\t closes application\n");
                }
                else if (input.Equals("dir -p"))
                {
                    Console.WriteLine(BaseDir);
                }
                else if (input.Equals("dir -c"))
                {
                    using (ScraperUtility Utility = new ScraperUtility())
                    {
                        Utility.Reset(BaseDir);
                    }
                    Console.WriteLine("Please enter new directory");
                    BaseDir = Console.ReadLine();
                }
                else if (input.Contains("scrape"))
                {
                    using (ScraperUtility Utility = new ScraperUtility())
                    {
                        Utility.CreateDir(BaseDir);
                    }
                    Int32.TryParse(input.Replace("scrape", ""), out x);
                    Console.WriteLine("Scraping records, please wait ...");
                    using (Scraper DiscogsScraper = new Scraper())
                    {
                        DiscogsScraper.GetRecords(x, BaseDir);
                    }
                }
                else if (input.Equals("reset"))
                {
                    using (ScraperUtility Utility = new ScraperUtility())
                    {
                        Utility.Reset(BaseDir);
                    }
                    Console.WriteLine("Database and dir has been reset");
                }
                else if (input.Equals("exit"))
                {
                    looping = false;
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
        }
    }
}
