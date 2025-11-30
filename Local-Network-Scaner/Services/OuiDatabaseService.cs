using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Local_Network_Scaner.Model;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

namespace Local_Network_Scaner.Services
{
    public class OuiDatabaseService
    {
        public Dictionary<string, OuiRecord> OuiDatabase { get; private set; } = new Dictionary<string, OuiRecord>();

        public void LoadDatabaseCSV(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("OUI database file not found.", filePath);

            using var parser = new TextFieldParser(filePath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;

            // Skip header line
            parser.ReadLine();

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields == null || fields.Length < 3) continue;

                string assignment = fields[0].Trim();
                string oui = fields[1].Trim().ToUpper();
                string vendor = fields[2].Trim();
                string address = fields.Length >= 4 ? fields[3].Trim() : string.Empty;

                var record = new OuiRecord
                {
                    Oui = oui,
                    Vendor = vendor,
                    Address = address
                };

                OuiDatabase[oui] = record;
            }
        }

        public OuiRecord? GetVendor(string macAdress)
        {
            string normalized = macAdress.Replace(":", "").Replace("-", "").Replace(".", "").ToUpper();
            string oui = normalized.Length >= 6 ? normalized.Substring(0, 6) : normalized;

            return OuiDatabase.TryGetValue(oui, out var rec) ? rec : null;
        }
    }
}
