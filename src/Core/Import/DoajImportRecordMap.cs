namespace QOAM.Core.Import
{
    using System;
    using System.Collections.Generic;
    using CsvHelper;
    using CsvHelper.Configuration;

    public sealed class DoajImportRecordMap : CsvClassMap<DoajImportRecord>
    {
        private static readonly HashSet<string> HasSealTrueValues = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) {"yes", "true"};

        public DoajImportRecordMap()
        {
            Map(m => m.Title).Name("Journal title");
            Map(m => m.URL).Name("Journal URL");
            Map(m => m.Publisher).Name("Publisher");
            Map(m => m.Language).Name("Full text language");
            Map(m => m.ISSN).ConvertUsing(MapISSN);
            Map(m => m.Subjects).Name("Keywords");
            Map(m => m.Country).Name("Country of publisher");
            Map(m => m.HasSeal).ConvertUsing(MapHasSeal);
        }

        private static bool MapHasSeal(ICsvReaderRow r)
        {
            return HasSealTrueValues.Contains(r.GetField("DOAJ Seal") ?? string.Empty);
        }

        private static string MapISSN(ICsvReaderRow r)
        {
            var eissn = r.GetField("Journal EISSN (online version)");
            return string.IsNullOrWhiteSpace(eissn) ? r.GetField("Journal ISSN (print version)") : eissn;
        }
    }
}