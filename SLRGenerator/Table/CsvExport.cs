using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using SLRGenerator.Types;

namespace SLRGenerator.Table
{
    public static class CsvExport
    {
        public static void SaveToCsv(IList<TableRule> rules)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {Delimiter = ";", SanitizeForInjection = true};
            using var csv = new CsvWriter(new StreamWriter("table.csv"), config);
            csv.WriteField("");
            foreach (var item in rules.First().Values) csv.WriteField(item.Key);
            csv.NextRecord();
            var okFlag = false;
            foreach (var rule in rules)
            {
                csv.WriteField(rule.Key);
                foreach (var value in rule.Values)
                {
                    if (!okFlag)
                    {
                        okFlag = true;
                        csv.WriteField("OK");
                        continue;
                    }

                    csv.WriteField(value.Value.ToString());
                }

                csv.NextRecord();
            }
        }
    }
}