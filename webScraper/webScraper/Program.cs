using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;
using webScraper.DataOperations;
using webScraper.Models;
using System.Text.RegularExpressions;

namespace webScraper
{
    class Program
    {
        static void Main(string[] args)
        {

            string BaseDir = @"C:\Users\ham-d\Desktop\test\"; //folder for the images

            scraperUtility utility = new scraperUtility();
            utility.Reset(BaseDir);

            scraper discogsScraper = new scraper();
            discogsScraper.getRecords(10, BaseDir);

            //TODO 
            //sql use correct id numbering without jumping
            //Ryg datamodel(record.cs) og dal ud i en Dll så de kan bruges af en anden applikation
            //Konfiguration
        }
    }
}
