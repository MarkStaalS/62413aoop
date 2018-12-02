using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using webScraper.DataOperations;
using webScraper.Models;

namespace webScraper
{
    /// <summary>
    /// Class that scrapes the required amount of records from discogs.com, adds them to a database and downloads the albumcover if present to a specified directory.
    /// </summary>
    class Scraper
    {
        public void GetRecords(int maxCtr, string BaseDirectory)
        {
            // scrapes website and gets record information and link to image
            string url = @"https://www.discogs.com"; //Base url
            string urlPath = @"/search/?type=release";//path url first page

            scraperUtility utility = new scraperUtility();
            RecordsDataAccessLayer RecordsDataAccessLayer = new RecordsDataAccessLayer();
            int ctr = 0;
            int errorCtr = 0;
            int pageCtr = 0;
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
                        urlPath = "/search/?type=release&page=" + pageCtr;
                        //Iterates to next page
                        pageCtr++;
                        Console.WriteLine("Failed to load web page");
                    }
                }
                
                //Explain lambda functions
                var recordList = htmlDoc.DocumentNode.Descendants("div") //Html attributes containing list of records
                    .Where(node => node.GetAttributeValue("id", "")
                    .Equals("search_results")).ToList();

                var listItems = recordList[0].Descendants("a") //List of record links
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("search_result_title")).ToList();
                //list
                foreach (var listItem in listItems)
                {
                    if (ctr >= maxCtr)
                        break;
                    //set record
                    //addd to list
                    string recordUrlPath = listItem.GetAttributeValue("href", "").ToString(); //url to record page

                    using (Record recordObj = new Record())
                    {
                        recordObj.SetRecord(url + recordUrlPath, recordUrlPath, recordObj);

                        if (recordObj.Genre == null)
                        {
                            Console.WriteLine("Error: Genre = Null");
                            errorCtr++;
                        }
                        else
                        {
                            //Checks weather or not the record has been added and weather to download cover image
                            if (RecordsDataAccessLayer.InsertRecord(recordObj))
                            {
                                //utility.PrintRecordInfo(recordObj);

                                string id = RecordsDataAccessLayer.GetLatestId();
                                //If there is no albumcover the path will be "-"
                                string path;
                                if (recordObj.ImgUrl == "thumbnail_border")
                                {
                                    path = "-";
                                }
                                else
                                {
                                    path = DownloadImage(recordObj.ImgUrl, id, BaseDirectory);
                                }
                                RecordsDataAccessLayer.UpdateRecordPtah(id, path); //Update file path
                                RecordsDataAccessLayer.InsertTrackList(id, recordObj);
                                ctr++;
                                Console.WriteLine("SUCC: " + recordObj.Name);
                            }
                            else
                            {
                                errorCtr++;
                            }
                        }
                    }
                }
                urlPath = GetNextPage(htmlDoc);
                pageCtr++;
                Console.WriteLine($"Next page: {urlPath} count:{ctr}");
            }
            Console.WriteLine($"Error rate {errorCtr} / {ctr}");
            Console.ReadLine();
        }
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
                    Console.WriteLine("There was an error downloading a cover.");
                    return null;
                }
            }

        }
        private static string GetNextPage(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode
                    .SelectSingleNode("//*[@id=\"pjax_container\"]/div[3]/form/div[1]/ul/li[2]/a")
                    .GetAttributeValue("href", "");
        }
    }
}
