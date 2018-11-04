namespace webScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            string BaseDir = @"C:\Users\ham-d\Desktop\test\"; //folder for the images

            scraperUtility utility = new scraperUtility();
            utility.Reset(BaseDir);

            scraper discogsScraper = new scraper();
            discogsScraper.getRecords(10, BaseDir);
        }
    }
}
