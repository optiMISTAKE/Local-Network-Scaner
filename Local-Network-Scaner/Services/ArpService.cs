using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Local_Network_Scanner.Model;

namespace Local_Network_Scanner.Services
{
    public class ArpService
    {
        public List<ArpEntry> GetArpTable()
        {
            List<ArpEntry> arpEntries = new List<ArpEntry>();

            var commandProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "arp",
                    Arguments = "-a",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            commandProcess.Start();
            string output = commandProcess.StandardOutput.ReadToEnd();
            commandProcess.WaitForExit();

            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Example line: "
                // 

                var match = Regex.Match(line.Trim(),
                @"(?<ip>\d+\.\d+\.\d+\.\d+)\s+(?<mac>([0-9a-f]{2}-){5}[0-9a-f]{2})\s+(?<type>\w+)",
                RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    arpEntries.Add(new ArpEntry
                    {
                        IPAddress = match.Groups["ip"].Value,
                        MACAddress = match.Groups["mac"].Value,
                        Type = match.Groups["type"].Value
                    });
                }
            }

            return arpEntries;
        }

        public string? GetMacAddress(string ipAddress)
        {
            var arpTable = GetArpTable();
            var entry = arpTable.FirstOrDefault(e => e.IPAddress == ipAddress);
            return entry?.MACAddress;
        }
    }
}
