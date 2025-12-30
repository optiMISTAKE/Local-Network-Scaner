using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Local_Network_Scanner.Model;
using Microsoft.VisualBasic.FileIO;

namespace Local_Network_Scanner.Services
{
    public class BluetoothUuidService
    {
        public Dictionary<string, BluetoothRecord> BluetoothUuidDatabase { get; private set; } = new Dictionary<string, BluetoothRecord>();

        public void LoadBluetoothUuidDatabase(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Bluetooth database file not found.", filePath);

            using var parser = new TextFieldParser(filePath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;

            // Skip header line
            parser.ReadLine();

            while(!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields == null || fields.Length < 3) continue;
                string allocationType = fields[0].Trim();
                string uuid = fields[1].Trim().ToUpper();
                string allocatedFor = fields[2].Trim();
                var record = new BluetoothRecord
                {
                    AllocationType = allocationType,
                    uuid = uuid,
                    AllocatedFor = allocatedFor
                };
                BluetoothUuidDatabase[uuid] = record;
            }
        }

        public BluetoothRecord? GetBluetoothRecord(string uuid)
        {
            string normalized = uuid.Replace("-", "").ToUpper();
            return BluetoothUuidDatabase.TryGetValue(normalized, out var rec) ? rec : null;
        }
    }
}
