using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Processing.QueryPipelines
{
    internal class CsvMaker
    {
        public byte[] CreateCsv(string[] exportFields, object inputResult)
        {
            var searchResult = (object[])inputResult;
            var lines = new List<string>();
            IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(searchResult[0].GetType()).OfType<PropertyDescriptor>();
            var header = string.Join(
                ",",
                props.ToList()
                    .Where(x => exportFields.Contains(x.Name))
                    .Select(x => x.Name));
            lines.Add(header);
            var valueLines = searchResult.Select(row =>
                string.Join(",", header.Split(',').Select(a => row.GetType().GetProperty(a)?.GetValue(row, null))));
            lines.AddRange(valueLines);
            var csv = lines.ToArray();

            var bytes = new List<byte[]>();

            foreach (var s in csv)
            {
                bytes.Add(System.Text.Encoding.UTF8.GetBytes(s));
                bytes.Add(System.Text.Encoding.UTF8.GetBytes(Environment.NewLine));
            }

            var result = bytes.SelectMany(x => x).ToArray();

            return result;
        }

        public string CreateFileName(string fileName)
        {
            return
                $"{fileName}_{Clock.Now.Date.Year}_{Clock.Now.Date.Month}_{Clock.Now.Date.Day}_{Clock.Now.Hour}_{Clock.Now.Minute}.csv";
        }
    }
}