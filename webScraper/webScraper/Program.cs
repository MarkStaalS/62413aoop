using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;
using webScraper.DataOperations;
using webScraper.Models;
using System.Text.RegularExpressions;

namespace webScraper
{
    class Program
    {
        static void Main(string[] args)
        {

            string BaseDir = @"C:\Users\ham-d\Desktop\test\"; //folder for the images

            //Reset(BaseDir);
            getRecords(30, BaseDir);
            //TODO 
            //sql use correct id numbering without jumping
            //Ryg datamodel(record.cs) og dal ud i en Dll så de kan bruges af en anden applikation
            //Konfiguration
        }

        private static void getRecords(int maxCtr, string BaseDirectory)
        {
            //webScraper
            // scrapes website and gets record information and link to image

            string url = @"https://www.discogs.com"; //Base url
            string urlPath = @"/search/?type=release";//path url first page

            recordsDataAccessLayer recordDAL = new recordsDataAccessLayer();
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

                    record recordObj = new record();
                    recordObj = setRecordObj(url + recordUrlPath, recordUrlPath, recordObj);

                    if (recordObj.genre == null || recordObj.imgUrl == "thumbnail_border")
                    {
                        Console.WriteLine("Error\n");
                    }
                    else
                    {
                        //Checks weather or not the record has been added and weather to download cover image
                        if (recordDAL.InsertRecord(recordObj))
                        {
                            printRecordInfo(recordObj);

                            string id = recordDAL.GetLatestId();
                            recordDAL.updateRecordPtah(id, downloadImage(recordObj.imgUrl, id, BaseDirectory)); //Update file path
                            recordDAL.insertTrackList(id, recordObj);
                            ctr++;
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
            System.IO.Directory.CreateDirectory(BaseDirectory); // Creates directory if it not already exists.

            using(WebClient web = new WebClient())
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
        private static record setRecordObj(string url, string urlPath, record record)
        {
            //scrape single record
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = new HtmlDocument();
            htmlDoc = web.Load(url);
            record.label = getLabel(htmlDoc);
            record.country = getCountry(htmlDoc);
            record.released = getReleased(htmlDoc);
            record.tracklist = getTracks(htmlDoc);
            record.name = getRecordTitle(htmlDoc);
            record.artist = getArtist(htmlDoc);
            record.genre = getGenre(htmlDoc);
            record.url = urlPath;
            record.imgUrl = getImgUrl(htmlDoc);
            record.pathUrl = "PathUrl"; // placeholder
            return record;
        }
        private static string getNextPage(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode
                    .SelectSingleNode("//*[@id=\"pjax_container\"]/div[3]/form/div[1]/ul/li[2]/a")
                    .GetAttributeValue("href", "");
        }
        #region GetRecordData
        private static string getGenre(HtmlDocument htmlDoc)
        {
               return htmlDoc.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("href", "")
                .Contains("/genre/")).First().InnerHtml.ToString();
        }
        private static string getImgUrl(HtmlDocument htmlDoc)
        {
            var pageContentNodes = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"page_content\"]/div[1]/div[1]/a")
                .ChildNodes.ToList();
            string[] imgUrl_ = pageContentNodes[pageContentNodes.Count - 2].InnerHtml.Split('\"');
            return imgUrl_[1];
        }
        private static string getArtist(HtmlDocument htmlDoc)
        {
            var profileTitleNodes = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"profile_title\"]")
                .ChildNodes.ToList();
            return profileTitleNodes[1].InnerText.Trim();
        }
        private static string getRecordTitle(HtmlDocument htmlDoc)
        {
            var profileTitleNodes = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"profile_title\"]")
                .ChildNodes.ToList();
            return profileTitleNodes[profileTitleNodes.Count - 2].InnerText.Trim();
        }
        private static string getLabel(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"page_content\"]/div[1]/div[3]/div[2]/a")
                .InnerText.ToString();
        }
        private static string getCountry(HtmlDocument htmlDoc)
        {
            try
            {
                return htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"page_content\"]/div[1]/div[3]/div[6]/a")
                .InnerText.Trim().ToString();
            }
            catch (Exception)
            {
                return "-";
            }
        }
        private static string getReleased(HtmlDocument htmlDoc)
        {
            try
            {
                return htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"page_content\"]/div[1]/div[3]/div[8]/a")
                .InnerText.Trim().ToString();
            }
            catch (Exception)
            {
                return "-";
            }
        }
        private static List<track> getTracks(HtmlDocument htmlDoc)
        {
            //Get tracks from the record and returns them in a list
            List<track> trackList = new List<track>();
            var htmlTrackList = htmlDoc.DocumentNode.Descendants("tr")
                .Where(node => node.GetAttributeValue("class", "")
                    .Contains("track")).ToList();

            foreach (var t in htmlTrackList)
            {
                track track = new track();
                var trackInfo = t.ChildNodes.Where(node => node.GetAttributeValue("class", "")
                    .Contains("track")).ToList();
                track.number = trackInfo[0].InnerText.ToString(); //needs to be string
                track.name = trackInfo[trackInfo.Count - 2].InnerText.ToString();
                track.duration = trackInfo[trackInfo.Count - 1].InnerText.ToString().Trim();
                //Console.WriteLine($"{track.number}, {track.name}, {track.duration}");
                trackList.Add(track);
            }
            return trackList;
        }
        #endregion
        private static void printRecordInfo(record record)
        {
            Console.WriteLine($"url: \t\t{record.url} \n" +
                        $"title: \t\t{record.name} \n" +
                        $"artist: \t{record.artist} \n" +
                        $"label: \t\t{record.label} \n" +
                        $"country: \t{record.country} \n" +
                        $"released: \t{record.released} \n" +
                        $"genre: \t\t{record.genre} \n" +
                        $"Tracks:");
            printTracks(record);
            Console.WriteLine();
        }
        private static void printTracks(record record)
        {
            foreach (track track in record.tracklist)
            {
                Console.WriteLine($"\t {track.number}" +
                    $"\t {track.name}" +
                    $"\t {track.duration}");
            }
        }
        private static void Reset(string BaseDirrectory)
        {
            recordsDataAccessLayer recordsDataAccessLayer = new recordsDataAccessLayer();
            recordsDataAccessLayer.ResetDatabase();
            try
            {
                System.IO.Directory.Delete(BaseDirrectory, true); // Deletes directory
            }
            catch 
            {
            }
        }
    }
}
