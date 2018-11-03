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
    }
}
