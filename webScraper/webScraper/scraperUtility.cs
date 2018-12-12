using System;
using webScraper.Models;
using webScraper.DataOperations;

namespace webScraper
{
    class ScraperUtility : IDisposable
    {
        public void PrintRecordInfo(Record record)
        {
            Console.WriteLine($"title: \t\t{record.Name} \n" +
                        $"artist: \t{record.Artist} \n" +
                        $"label: \t\t{record.Label} \n" +
                        $"country: \t{record.Country} \n" +
                        $"released: \t{record.Released} \n" +
                        $"genre: \t\t{record.Genre} \n" +
                        $"Tracks:");
            PrintTracks(record);
            Console.WriteLine();
        }
        public void PrintTracks(Record record)
        {
            foreach (Track track in record.Tracklist)
            {
                Console.WriteLine($"\t {track.Number}" +
                    $"\t {track.Name}" +
                    $"\t {track.Duration}");
            }
        }
        public void CreateDir(string BaseDirectory)
        {
            System.IO.Directory.CreateDirectory(BaseDirectory); //Creates directory if one does not already exist.
        }
        public void Reset(string BaseDirrectory)
        {
            RecordsDataAccessLayer recordsDataAccessLayer = new RecordsDataAccessLayer();
            recordsDataAccessLayer.ResetDatabase();
            try
            {
                System.IO.Directory.Delete(BaseDirrectory, true); //Deletes directory
            }
            catch
            {
            }
        }
        public void Dispose()
        {
        }
    }
}
