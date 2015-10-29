﻿namespace QOAM.Core.Cleanup
{
    using System;
    using Repositories;
    using Validation;

    public class UnpublishedScoreCardsCleanup : ICleanup
    {
        private readonly IBaseScoreCardRepository baseScoreCardRepository;
        private readonly IValuationScoreCardRepository valuationScoreCardRepository;
        private readonly UnpublishedScoreCardsCleanupSettings settings;

        public UnpublishedScoreCardsCleanup(
            IBaseScoreCardRepository baseScoreCardRepository,
            IValuationScoreCardRepository valuationScoreCardRepository,
            UnpublishedScoreCardsCleanupSettings settings)
        {
            Requires.NotNull(baseScoreCardRepository, nameof(baseScoreCardRepository));
            Requires.NotNull(valuationScoreCardRepository, nameof(valuationScoreCardRepository));
            Requires.NotNull(settings, nameof(settings));

            this.settings = settings;
            this.valuationScoreCardRepository = valuationScoreCardRepository;
            this.baseScoreCardRepository = baseScoreCardRepository;
        }

        public void Cleanup()
        {
            var toBeRemovedWindow = TimeSpan.FromDays(settings.NumberOfDaysBeforeArchiving);

            this.baseScoreCardRepository.RemoveUnpublishedScoreCards(toBeRemovedWindow);
            this.valuationScoreCardRepository.RemoveUnpublishedScoreCards(toBeRemovedWindow);
        }
    }
}