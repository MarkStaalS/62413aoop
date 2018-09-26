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
                //all relevant information for each item in our list above
            
                foreach (var listItem in listItems)
                {
                    string recordUrlPath = listItem.GetAttributeValue("href", "").ToString();
                    //string recName = listItem.InnerHtml;
                    Program.getRecordInfo(url + recordUrlPath);
                }

                //next page
               urlPath = htmlDoc.DocumentNode
                    .SelectSingleNode("//*[@id=\"pjax_container\"]/div[3]/form/div[1]/ul/li[2]/a")
                    .GetAttributeValue("href", "");

                Console.WriteLine($"\n *** next page: {urlPath} *** \n");
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static (string genre, string imgUrl, string artist, string recordName) getRecordInfo(string url)
        {
            //scrape single record
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = new HtmlDocument();
            //Tuple
            (string genre, string imgUrl, string artist, string recordName) recordInfo;

            try
            {
                htmlDoc = web.Load(url);
            }
            catch (Exception)
            {
                return (null, null, null, null);
            }

            ////using node reference, could use xPath but thet would require further formating of the data
            //var genre_ = htmlDoc.DocumentNode.Descendants("a")
            //    .Where(node => node.GetAttributeValue("href", "")
            //    .Contains("/genre/")).ToList();

            ////Using xPath
            //var pageContentNodes = htmlDoc.DocumentNode
            //    .SelectSingleNode("//*[@id=\"page_content\"]/div[1]/div[1]/a")
            //    .ChildNodes.ToList();
            //string[] imgUrl_ = pageContentNodes[pageContentNodes.Count - 2].InnerHtml.Split('\"');
            
            //var profileTitleNodes = htmlDoc.DocumentNode
            //    .SelectSingleNode("//*[@id=\"profile_title\"]")
            //    .ChildNodes.ToList();

            recordInfo.genre = getGenre(htmlDoc);
            recordInfo.imgUrl = getImgUrl(htmlDoc);
            recordInfo.artist = getArtist(htmlDoc);
            recordInfo.recordName = getRecordName(htmlDoc);

            Console.WriteLine($"url: {recordInfo.imgUrl} \n" +
                $"record name: {recordInfo.recordName} \n" +
                $"artist: {recordInfo.artist} \n" +
                $"genre: {recordInfo.genre}\n");

            return recordInfo;
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
        private static string getRecordName(HtmlDocument htmlDoc)
        {
            string recordName;
            var profileTitleNodes = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@id=\"profile_title\"]")
                .ChildNodes.ToList();
            recordName = profileTitleNodes.Last().InnerText.Trim();
            return recordName;
        }
    }
}
