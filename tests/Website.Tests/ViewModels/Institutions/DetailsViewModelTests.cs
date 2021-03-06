﻿namespace QOAM.Website.Tests.ViewModels.Institutions
{
    using System.Web.Helpers;

    using QOAM.Core.Repositories.Filters;
    using Website.ViewModels.Institutions;
    using Xunit;

    public class DetailsViewModelTests
    {
        [Fact]
        public void ConstructorSetsSortByToNumberOfJournalScoreCards()
        {
            // Arrange
            var indexViewModel = new DetailsViewModel();

            // Act

            // Assert
            Assert.Equal(UserProfileSortMode.NumberOfBaseJournalScoreCards, indexViewModel.SortBy);
        }

        [Fact]
        public void ConstructorSetsSortDirectionToDescending()
        {
            // Arrange
            var indexViewModel = new DetailsViewModel();

            // Act

            // Assert
            Assert.Equal(SortDirection.Descending, indexViewModel.Sort);
        }
    }
}