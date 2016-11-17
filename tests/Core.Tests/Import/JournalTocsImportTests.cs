﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Moq;
using QOAM.Core.Import;
using QOAM.Core.Repositories;
using QOAM.Core.Tests.Import.Resources;
using Xunit;

namespace QOAM.Core.Tests.Import
{
    public class JournalTocsImportTests
    {
        Mock<IJournalTocsClient> _client;

        [Fact]
        public void ParseJournals_fetches_a_batch_of_journal_xml_and_converts_it_to_Journals()
        {
            // Arrange
            var journalTocsImport = CreateJournalTocsImport();
            _client.Setup(x => x.DownloadJournals("update")).Returns(GetJournalTocsFirst500Xml());
            // Act
            var journals = journalTocsImport.ParseJournals();

            // Assert
            Assert.Equal(500, journals.Count);
        }


        private static Journal GetExpectedJournal()
        {
            return new Journal
                   {
                       Title = "Postepy Rehabilitacji",
                       ISSN = "0860-6161",
                       Link = "http://www.asla.org.au/pubs/access/access.htm",
                       Publisher = new Publisher { Name = "L E D S Publishing Co. Inc." },
                       Country = new Country { Name = "Poland" },
                       Languages = new List<Language> { new Language { Name = "English" }, new Language { Name = "Polish" } },
                       Subjects = new List<Subject> { new Subject { Name = "medical sciences-physical medicine and rehabilitation" } },
                       OpenAccess = false
                   };
        }

        private static void AssertEqualJournal(Journal expectedJournal, Journal parsedJournal)
        {
            Assert.Equal(expectedJournal.Title, parsedJournal.Title);
            Assert.Equal(expectedJournal.ISSN, parsedJournal.ISSN);
            Assert.Equal(expectedJournal.Link, parsedJournal.Link);
            Assert.Equal(expectedJournal.Publisher.Name, parsedJournal.Publisher.Name);
            Assert.Equal(expectedJournal.Country.Name, parsedJournal.Country.Name);
            Assert.Equal(expectedJournal.Languages.Select(l => l.Name), parsedJournal.Languages.Select(l => l.Name));
            Assert.Equal(expectedJournal.Subjects.Select(s => s.Name), parsedJournal.Subjects.Select(s => s.Name));
            Assert.Equal(expectedJournal.OpenAccess, parsedJournal.OpenAccess);
        }

        static string GetJournalTocsFirst500Xml()
        {
            return ResourceReader.GetContentsOfResource("JournalTocs-setup-first-500.xml");
        }

        static string GetJournalTocsNextXml()
        {
            return ResourceReader.GetContentsOfResource("JournalTocs-setup-next-500.xml");
        }

        static string GetJournalTocsNoMoreItemsNotice()
        {
            return ResourceReader.GetContentsOfResource("JournalTocs-no-more-items-notice.xml");
        }

        JournalTocsImport CreateJournalTocsImport()
        {
            _client = new Mock<IJournalTocsClient>();
            //var journalTocsSettings = new JournalTocsSettings();;
            return new JournalTocsImport(_client.Object, Mock.Of<IBlockedISSNRepository>());
        }
    }
}