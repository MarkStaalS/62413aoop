using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webScraper.Models
{
    class record
    {
        public string name { get; set; }
        public string artist { get; set; }
        public string genre { get; set; }
        public string imgUrl { get; set; }
        public string url { get; set; }
        public string pathUrl { get; set; }
        public string country { get; set; }
        public string label { get; set; }
        public string released { get; set; }
        public List<track> tracklist { get; set; }

        public void setRecord(string url, string urlPath, record record)
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
        }
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
            foreach (HtmlAgilityPack.HtmlNode htmlNode in htmlTrackList)
            {
                track track = new track();
                track.setTrack(htmlNode);
                trackList.Add(track);
            }
            return trackList;
        }
    }
}
