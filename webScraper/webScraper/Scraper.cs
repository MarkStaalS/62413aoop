using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using webScraper.DataOperations;
using webScraper.Models;

namespace webScraper
{
    /// <summary>
    /// Class that scrapes the required amount of records from discogs.com, adds them to a database and downloads the albumcover if present to a specified directory.
    /// </summary>
    class Scraper : IDisposable
    {
        public void GetRecords(int maxCtr, string BaseDirectory)
        {
            // scrapes website and gets record information and link to image
            string url = @"https://www.discogs.com"; //Base url
            string urlPath = @"/search/?type=release";//path url first page
            int ctr = 0;
            int pageCtr = 0;
            Stopwatch stopwatch = new Stopwatch();
            RecordsDataAccessLayer RecordsDataAccessLayer= new RecordsDataAccessLayer();
            stopwatch.Start();
            while (ctr < maxCtr)
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument htmlDoc = new HtmlDocument();
                //Handels failed loading of pages
                //Will loop until loading of page successfull
                for (; ; )
                {
                    try
                    {
                        htmlDoc = web.Load(url + urlPath);
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Failed to load web page");
                        urlPath = "/search/?type=release&page=" + pageCtr;
                        pageCtr++; //Iterates to next page
                    }
                }
                
                //Lambda functions used together with LINQ query, taking the Html node as input and returning in this case a list
                var recordList = htmlDoc.DocumentNode.Descendants("div") //Html attributes containing list of records
                    .Where(node => node.GetAttributeValue("id", "")
                    .Equals("search_results")).ToList();

                var listItems = recordList[0].Descendants("a") //List of record links
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("search_result_title")).ToList();
                //Loops through every item in the list, coresponding with the records on a single web page
                foreach (var listItem in listItems)
                {
                    //Checks that only the specified ammount of records will be scraped
                    if (ctr >= maxCtr)
                        break;
                    string recordUrlPath = listItem.GetAttributeValue("href", "").ToString(); //url to record page

                    using (Record Record = new Record())
                    {
                        //Call SetRecord to fill the record object with required informaiton
                        Record.SetRecord(url + recordUrlPath, recordUrlPath, Record);
                        //If the record genre is empty the record will not be entered into the database
                        if (Record.Genre == null)
                        {
                            Console.WriteLine("Error: Genre = Null");
                        }
                        else
                        {
                            //Checks weather or not the record has been added and weather to download cover image
                            if (RecordsDataAccessLayer.InsertRecord(Record))
                            {
                                string id = RecordsDataAccessLayer.GetLatestId();
                                //If there is no album cover the path will be "-"
                                string path;
                                if (Record.ImgUrl == "thumbnail_border")
                                {
                                    path = "-";
                                }
                                else
                                {
                                    //If the record dose not use a stock image the image will be downloaded to the specified folder
                                    path = DownloadImage(Record.ImgUrl, id, BaseDirectory);
                                }
                                RecordsDataAccessLayer.UpdateRecordPtah(id, path); //Update file path
                                RecordsDataAccessLayer.InsertTrackList(id, Record); //All tracks of the record will be inserted into the database
                                ctr++;
                                //notifies the user thet the record has been successfully added to the database
                                Console.WriteLine($"Success: {Record.Name} - {Record.Artist}");
                            }
                        }
                    }
                }
                //Get url path for the next page
                urlPath = GetNextPage(htmlDoc);
                pageCtr++; //used if unabel to get page (to iterate to next page)
            }
            stopwatch.Stop();
            Console.WriteLine($"Done, records scraped: {ctr}, Time elapsed: {stopwatch.Elapsed}");
        }
        //Using web client to download album cover to specified folder
        private static string DownloadImage(string imgUrl, string id, string BaseDirectory)
        {
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                try
                {
                    web.DownloadFile(imgUrl, BaseDirectory + id + ".jpg");
                    return BaseDirectory + id + ".jpg";
                }
                catch (System.ArgumentException)
                {
                    Console.WriteLine("Error: Downloading cover.");
                    return null;
                }
            }

        }
        //Get url path for the next page
        private static string GetNextPage(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode
                    .SelectSingleNode("//*[@id=\"pjax_container\"]/div[3]/form/div[1]/ul/li[2]/a")
                    .GetAttributeValue("href", "");
        }

        public void Dispose()
        {
        }
    }
}
