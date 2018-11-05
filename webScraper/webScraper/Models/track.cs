using System.Linq;
using System.Net;

namespace webScraper.Models
{
    class track
    {
        public string number { get; set; }
        public string name { get; set; }
        public string duration { get; set; }
        public int recordId { get; set; }

        public void setTrack(HtmlAgilityPack.HtmlNode htmlNode)
        {
            var trackInfo = htmlNode.ChildNodes.Where(node => node.GetAttributeValue("class", "")
                    .Contains("track")).ToList();
            number = trackInfo[0].InnerText.ToString(); //needs to be string
            name = WebUtility.HtmlDecode(trackInfo[trackInfo.Count - 2].InnerText.ToString());
            duration = trackInfo[trackInfo.Count - 1].InnerText.ToString().Trim();
        }
    }
}
