using System;
using webScraper.Models;
using webScraper.DataOperations;

namespace webScraper
{
    class scraperUtility
    {
        //Utilities for development
        public void printRecordInfo(record record)
        {
            Console.WriteLine($"url: \t\t{record.url} \n" +
                        $"title: \t\t{record.name} \n" +
                        $"artist: \t{record.artist} \n" +
                        $"label: \t\t{record.label} \n" +
                        $"country: \t{record.country} \n" +
                        $"released: \t{record.released} \n" +
                        $"genre: \t\t{record.genre} \n" +
                        $"Tracks:");
            printTracks(record);
            Console.WriteLine();
        }
        public void printTracks(record record)
        {
            foreach (track track in record.tracklist)
            {
                Console.WriteLine($"\t {track.number}" +
                    $"\t {track.name}" +
                    $"\t {track.duration}");
            }
        }
        public void createDir(string BaseDirectory)
        {
            System.IO.Directory.CreateDirectory(BaseDirectory); // Creates directory if it not already exists.
        }
        public void Reset(string BaseDirrectory)
        {
            recordsDataAccessLayer recordsDataAccessLayer = new recordsDataAccessLayer();
            recordsDataAccessLayer.ResetDatabase();
            try
            {
                System.IO.Directory.Delete(BaseDirrectory, true); // Deletes directory
            }
            catch
            {
            }
        }
    }
}
