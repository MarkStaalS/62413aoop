using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Http;


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
            
            for (int i = 0; i <= 3; i++)
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

                    printRecordInfo(recordInfo.imgUrl,
                        recordInfo.recordTitle,
                        recordInfo.artist,
                        recordInfo.genre);
                }
                //next page
                urlPath = getNextPage(htmlDoc);

                Console.WriteLine($"\n *** next page: {urlPath} *** \n");
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
            string urlPath;
            urlPath = htmlDoc.DocumentNode
                    .SelectSingleNode("//*[@id=\"pjax_container\"]/div[3]/form/div[1]/ul/li[2]/a")
                    .GetAttributeValue("href", "");
            return urlPath;
        }
        private static (string genre, string imgUrl, string artist, string recordTitle) getRecordInfo(string url)
        {
            //scrape single record
            (string genre, string imgUrl, string artist, string recordTitle) recordInfo;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = new HtmlDocument();

            try
            {
                htmlDoc = web.Load(url);
                recordInfo.genre = getGenre(htmlDoc);
                recordInfo.imgUrl = getImgUrl(htmlDoc);
                recordInfo.artist = getArtist(htmlDoc);
                recordInfo.recordTitle = getRecordTitle(htmlDoc);
                return recordInfo;
            }
            catch (Exception)
            {
                return (null, null, null, null);
            }
        }
        private static string getGenre(HtmlDocument htmlDoc)
        {
            string genre;
            //using node reference, could use xPath but thet would require further formating of the data
            var genre_ = htmlDoc.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("href", "")
                .Contains("/genre/")).ToList();
            genre = genre_[0].InnerHtml;
            return genre;
        }
        private static string getImgUrl(HtmlDocument htmlDoc)
        {
            string imgUrl  ="";
            //Using xPath
            var pageContentNodes = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"page_content\"]/div[1]/div[1]/a")
                .ChildNodes.ToList();
            string[] imgUrl_ = pageContentNodes[pageContentNodes.Count - 2].InnerHtml.Split('\"');
            imgUrl = imgUrl_[1]; ;
            return imgUrl;
        }
        private static string getArtist(HtmlDocument htmlDoc)
        {
            string artist;
            var profileTitleNodes = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"profile_title\"]")
                .ChildNodes.ToList();
            artist = profileTitleNodes[1].InnerText.Trim();
            return artist;
        }
        private static string getRecordTitle(HtmlDocument htmlDoc)
        {
            string recordTitle;
            var profileTitleNodes = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"profile_title\"]")
                .ChildNodes.ToList();
            recordTitle = profileTitleNodes.Last().InnerText.Trim();
            return recordTitle;
        }
    }
}
