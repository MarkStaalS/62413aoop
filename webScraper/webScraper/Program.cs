﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Http;
using System.Net;
using System.IO;


namespace webScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            getHtmlAsync();
            Console.ReadLine();
        }

        private static async void getHtmlAsync()
        {
            //webScraper
            // scrapes website and gets record information and link to image
            //base url
            string url = @"https://www.discogs.com";
            //path url first page
            string urlPath = @"/search/?type=release";
            int ctr = 0;
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\Users\ham-d\Desktop\tst.txt", true))
            {

                for (int i = 0; i <= 1; i++)
                {

                    var httpClient = new HttpClient();
                    var html = await httpClient.GetStringAsync(url + urlPath);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    // try using IDisposable, maby by inhereting from the original class
                    //using ( HtmlDocument nd = new HtmlDocument)

                    //Using html atributes and nodes to get required information
                    //gets the part of the html document contaning the list
                    var recordList = htmlDoc.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("id", "")
                        .Equals("search_results")).ToList();
                    //divides the above section into bits for each record and puts 
                    //them in a list
                    var listItems = recordList[0].Descendants("a")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("search_result_title")).ToList();
                    //gets website links and then visits the website to get
                    foreach (var listItem in listItems)
                    {
                        string recordUrlPath = listItem.GetAttributeValue("href", "").ToString();
                        (string genre, string imgUrl, string artist, string recordTitle) recordInfo =
                            Program.getRecordInfo(url + recordUrlPath);
                        if (recordInfo.genre == null || recordInfo.imgUrl == "thumbnail_border")
                        {
                            Console.WriteLine("Error\n");
                        }
                        else
                        {
                            //file.WriteLine("");
                            //file.WriteLine(recordInfo.recordTitle);
                            //file.WriteLine(recordInfo.artist);
                            //file.WriteLine(recordInfo.genre);
                            //file.WriteLine(recordInfo.imgUrl); 
                            ctr++;
                            //printRecordInfo(recordInfo.genre,
                            //    recordInfo.imgUrl,
                            //    recordInfo.artist,
                            //    recordInfo.recordTitle);
                        }
                    }
                    //next page
                    urlPath = getNextPage(htmlDoc);

                    Console.WriteLine($"\n *** next page: {urlPath} count:{ctr}*** \n");
                }
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }
        private static void printRecordInfo(string genre, string imgUrl, string artist, string recordTitle)
        {
            Console.WriteLine($"url: {imgUrl} \n" +
                        $"record title: {recordTitle} \n" +
                        $"artist: {artist} \n" +
                        $"genre: {genre}\n");
        }
        private static string getNextPage(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode
                    .SelectSingleNode("//*[@id=\"pjax_container\"]/div[3]/form/div[1]/ul/li[2]/a")
                    .GetAttributeValue("href", "");
        }
        private static (string genre, string imgUrl, string artist, string recordTitle) getRecordInfo(string url)
        {
            //scrape single record
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = new HtmlDocument();

            try
            {
                htmlDoc = web.Load(url);
                return (getGenre(htmlDoc),
                    getImgUrl(htmlDoc), 
                    getArtist(htmlDoc), 
                    getRecordTitle(htmlDoc));
            }
            catch (Exception)
            {
                return (null, null, null, null);
            }
        }
        private static string getGenre(HtmlDocument htmlDoc)
        {
            var genre_ = htmlDoc.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("href", "")
                .Contains("/genre/")).ToList();
            return genre_[0].InnerHtml;
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
    }
}
