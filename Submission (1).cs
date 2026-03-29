using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;

/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 **/

namespace ConsoleApp1
{
    public class Submission
    {
        public static string xmlURL = "https://raw.githubusercontent.com/megh280105/CSE-445/main/NationalParks.xml";
        public static string xmlErrorURL = "https://raw.githubusercontent.com/megh280105/CSE-445/main/NationalParksErrors.xml";
        public static string xsdURL = "https://raw.githubusercontent.com/megh280105/CSE-445/main/NationalParks.xsd";

        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            StringBuilder errors = new StringBuilder();

            try
            {
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(null, xsdUrl);

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = schemas;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;

                settings.ValidationEventHandler += delegate (object sender, ValidationEventArgs e)
                {
                    if (e.Exception != null)
                    {
                        errors.AppendLine(
                            e.Severity.ToString() + ": " +
                            e.Message +
                            " Line: " + e.Exception.LineNumber +
                            ", Position: " + e.Exception.LinePosition
                        );
                    }
                    else
                    {
                        errors.AppendLine(e.Severity.ToString() + ": " + e.Message);
                    }
                };

                using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
                {
                    while (reader.Read())
                    {
                    }
                }

                if (errors.Length == 0)
                {
                    return "No errors are found";
                }

                return errors.ToString().Trim();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                string xmlContent = DownloadContent(xmlUrl);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);

                return jsonText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Helper method to download content from URL
        private static string DownloadContent(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }
}