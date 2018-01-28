using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Sample.Models;

namespace Sample.Services
{
    public class CsvData
    {
        public static List<Talk> LoadTalks()
        {
            using (var textReader = File.OpenText("PET-Talks.csv"))
            {
                using (var csv = new CsvReader(textReader))
                {
                    // Register mapping
                    csv.Configuration.RegisterClassMap<TalkMap>();
                    // Load once in memory
                    return csv.GetRecords<Talk>().ToList();
                }
            }
        }

        public static List<Speaker> LoadSpeakers()
        {
            using (var textReader = File.OpenText("PET-Speakers.csv"))
            {
                using (var csv = new CsvReader(textReader))
                {
                    // Load once in memory
                    return csv.GetRecords<Speaker>().ToList();
                }
            }
        }

        class TalkMap : ClassMap<Talk>
        {
            public TalkMap()
            {
                AutoMap();
                Map(x => x.Tags).ConvertUsing(row =>
                {
                    var tags = row.GetField<string>("Tags");
                    var stags = tags.Split(",");

                    return stags;
                });
            }
        }
    }
}