using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace webScraper.Models
{
    /// <summary>
    /// Class for setting and transfering record data
    /// </summary>
    /// Implements IDisposable interface (collection of abstract members) allowing for the garbage collector to dispose of the object
    class Record : IDisposable
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public string ImgUrl { get; set; }
        public string PathUrl { get; set; }
        public string Country { get; set; }
        public string Label { get; set; }
        public string Released { get; set; }
        public List<Track> Tracklist { get; set; }

        //scrape single record
        public void SetRecord(string url, string urlPath, Record record)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc = web.Load(url);
                record.Label = GetLabel(htmlDoc);
                record.Country = GetCountry(htmlDoc);
                record.Released = GetReleased(htmlDoc);
                record.Tracklist = GetTracks(htmlDoc);
                record.Name = GetName(htmlDoc);
                record.Artist = GetArtist(htmlDoc);
                record.Genre = GetGenre(htmlDoc);
                record.ImgUrl = GetImgUrl(htmlDoc);
                record.PathUrl = "PathUrl"; // placeholder
            }
            catch 
            {
            }
        }
        private static string GetGenre(HtmlDocument htmlDoc)
        {
            return WebUtility.HtmlDecode(htmlDoc.DocumentNode.Descendants("a")
             .Where(node => node.GetAttributeValue("href", "")
             .Contains("/genre/")).First().InnerHtml.ToString());
        }
        private static string GetImgUrl(HtmlDocument htmlDoc)
        {
            var pageContentNodes = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"page_content\"]/div[1]/div[1]/a")
                .ChildNodes.ToList();
            string[] imgUrl_ = pageContentNodes[pageContentNodes.Count - 2].InnerHtml.Split('\"');
            return imgUrl_[1];
        }
        private static string GetArtist(HtmlDocument htmlDoc)
        {
            var profileTitleNodes = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"profile_title\"]")
                .ChildNodes.ToList();
            char[] separators = new char[] { ';', ',', '\r', '\t', '\n', '\\', '\'' };//unwanted characters
            string result = Regex.Escape(WebUtility.HtmlDecode(profileTitleNodes[1].InnerText.Trim()));
            //Run through escaped string removing unwanted characters
            foreach (char c in separators)
            {
                result = result.Replace(c.ToString(), "");
            }
            return result.Trim();
        }
        private static string GetName(HtmlDocument htmlDoc)
        {
            var profileTitleNodes = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"profile_title\"]")
                .ChildNodes.ToList();
            char[] separators = new char[] {';', ',', '\r', '\t', '\n', '\\', '\'' };//unwanted characters
            string result = Regex.Escape(WebUtility.HtmlDecode(profileTitleNodes[profileTitleNodes.Count - 2].InnerText.Trim()));
            //Run through escaped string removing unwanted characters
            foreach (char c in separators) 
            {
                result = result.Replace(c.ToString(), "");
            }
            return result.Trim();
        }
        private static string GetLabel(HtmlDocument htmlDoc)
        {
            return WebUtility.HtmlDecode(htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"page_content\"]/div[1]/div[3]/div[2]/a")
                .InnerText.ToString());
        }
        private static string GetCountry(HtmlDocument htmlDoc)
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
        private static string GetReleased(HtmlDocument htmlDoc)
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
        private static List<Track> GetTracks(HtmlDocument htmlDoc)
        {
            //Get tracks from the record and returns them in a list
            List<Track> trackList = new List<Track>();
            var htmlTrackList = htmlDoc.DocumentNode.Descendants("tr")
                .Where(node => node.GetAttributeValue("class", "")
                    .Contains("track")).ToList();
            foreach (HtmlAgilityPack.HtmlNode htmlNode in htmlTrackList)
            {
                Track track = new Track();
                track.SetTrack(htmlNode);
                trackList.Add(track);
            }
            return trackList;
        }
        //IDisposable Interface
        public void Dispose()
        {
        }
    }
}
