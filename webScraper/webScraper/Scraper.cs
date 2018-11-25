using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using webScraper.DataOperations;
using webScraper.Models;

namespace webScraper
{
    class scraper
    {
        public void getRecords(int maxCtr, string BaseDirectory)
        {
            // scrapes website and gets record information and link to image
            string url = @"https://www.discogs.com"; //Base url
            string urlPath = @"/search/?type=release";//path url first page

            scraperUtility utility = new scraperUtility();
            recordsDataAccessLayer recordsDataAccessLayer = new recordsDataAccessLayer();
            int ctr = 0;
            while (ctr < maxCtr)
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument htmlDoc = web.Load(url + urlPath);

                var recordList = htmlDoc.DocumentNode.Descendants("div") //Html attributes containing list of records
                    .Where(node => node.GetAttributeValue("id", "")
                    .Equals("search_results")).ToList();

                var listItems = recordList[0].Descendants("a") //List of record links
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("search_result_title")).ToList();

                foreach (var listItem in listItems)
                {
                    if (ctr >= maxCtr)
                        break;

                    string recordUrlPath = listItem.GetAttributeValue("href", "").ToString(); //url to record page

                    using (record recordObj = new record())
                    {
                        recordObj.setRecord(url + recordUrlPath, recordUrlPath, recordObj);

                        if (recordObj.genre == null || recordObj.imgUrl == "thumbnail_border")
                        {
                            Console.WriteLine("Error\n");
                        }
                        else
                        {
                            //Checks weather or not the record has been added and weather to download cover image
                            if (recordsDataAccessLayer.InsertRecord(recordObj))
                            {
                                utility.printRecordInfo(recordObj);

                                string id = recordsDataAccessLayer.GetLatestId();
                                recordsDataAccessLayer.updateRecordPtah(id, downloadImage(recordObj.imgUrl, id, BaseDirectory)); //Update file path
                                recordsDataAccessLayer.insertTrackList(id, recordObj);
                                ctr++;
                            }
                        }
                    }
                }
                urlPath = getNextPage(htmlDoc);

                Console.WriteLine($"\n *** next page: {urlPath} count:{ctr}*** \n");
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }
        private static string downloadImage(string imgUrl, string id, string BaseDirectory)
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
        private static string getNextPage(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode
                    .SelectSingleNode("//*[@id=\"pjax_container\"]/div[3]/form/div[1]/ul/li[2]/a")
                    .GetAttributeValue("href", "");
        }
    }
}
