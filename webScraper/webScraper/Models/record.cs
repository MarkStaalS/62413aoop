using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webScraper.Models
{
    class record : IDisposable
    {
        public string name { get; set; }
        public string artist { get; set; }
        public string genre { get; set; }
        public string url { get; set; }
        public string pathUrl { get; set; }

        public record()
        {
        }

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing == true)
            {
                name = null;
                artist = null;
                genre = null;
                url = null;
                pathUrl = null;
            }
            else
            {

            }
        }
        #endregion
    }
}
