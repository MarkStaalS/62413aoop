using System.Linq;
using System.Net;

namespace webScraper.Models
{
    /// <summary>
    /// A class containing the relevant information regarding a single track
    /// </summary>
    class Track
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public int RecordId { get; set; }
        public void SetTrack(HtmlAgilityPack.HtmlNode htmlNode)
        {
            var trackInfo = htmlNode.ChildNodes.Where(node => node.GetAttributeValue("class", "")
                    .Contains("track")).ToList();
            Number = trackInfo[0].InnerText.ToString(); //needs to be string
            Name = WebUtility.HtmlDecode(trackInfo[trackInfo.Count - 2].InnerText.ToString());
            Duration = trackInfo[trackInfo.Count - 1].InnerText.ToString().Trim();
        }
    }
}
