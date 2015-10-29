﻿namespace QOAM.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Web.Mvc;
    using QOAM.Core;
    using QOAM.Core.Repositories;
    using QOAM.Website.Helpers;
    using QOAM.Website.Models;
    using QOAM.Website.ViewModels.Score;

    using Validation;

    [RoutePrefix("score")]
    public class ScoreController : ApplicationController
    {
        private const int SubjectTruncationLength = 90;
        
        private readonly IScoreCardVersionRepository scoreCardVersionRepository;
        private readonly IJournalRepository journalRepository;
        private readonly ILanguageRepository languageRepository;
        private readonly ISubjectRepository subjectRepository;
        private readonly IQuestionRepository questionRepository;
        private readonly IBaseJournalPriceRepository baseJournalPriceRepository;
        private readonly GeneralSettings generalSettings;
        private readonly IValuationJournalPriceRepository valuationJournalPriceRepository;
        private readonly IInstitutionRepository institutionRepository;

        public ScoreController(IBaseScoreCardRepository baseScoreCardRepository, IBaseJournalPriceRepository baseJournalPriceRepository, IValuationScoreCardRepository valuationScoreCardRepository, IValuationJournalPriceRepository valuationJournalPriceRepository, IScoreCardVersionRepository scoreCardVersionRepository, IJournalRepository journalRepository, ILanguageRepository languageRepository, ISubjectRepository subjectRepository, IQuestionRepository questionRepository, GeneralSettings generalSettings, IUserProfileRepository userProfileRepository, IInstitutionRepository institutionRepository, IAuthentication authentication)
            : base(baseScoreCardRepository, valuationScoreCardRepository, userProfileRepository, authentication)
        {
            Requires.NotNull(baseJournalPriceRepository, nameof(baseJournalPriceRepository));
            Requires.NotNull(valuationJournalPriceRepository, nameof(valuationJournalPriceRepository));
            Requires.NotNull(scoreCardVersionRepository, nameof(scoreCardVersionRepository));
            Requires.NotNull(journalRepository, nameof(journalRepository));
            Requires.NotNull(languageRepository, nameof(languageRepository));
            Requires.NotNull(subjectRepository, nameof(subjectRepository));
            Requires.NotNull(questionRepository, nameof(questionRepository));
            Requires.NotNull(institutionRepository, nameof(institutionRepository));
            Requires.NotNull(generalSettings, nameof(generalSettings));
            
            this.scoreCardVersionRepository = scoreCardVersionRepository;
            this.valuationJournalPriceRepository = valuationJournalPriceRepository;
            this.journalRepository = journalRepository;
            this.languageRepository = languageRepository;
            this.subjectRepository = subjectRepository;
            this.questionRepository = questionRepository;
            this.baseJournalPriceRepository = baseJournalPriceRepository;
            this.institutionRepository = institutionRepository;
            this.generalSettings = generalSettings;
        }

        [HttpGet, Route("")]
        public ActionResult Index(IndexViewModel model)
        {
            model.Languages = this.languageRepository.All.ToSelectListItems("<All languages>");
            model.Disciplines = this.subjectRepository.All.ToSelectListItems("<All disciplines>", SubjectTruncationLength);
            model.Journals = this.journalRepository.Search(model.ToFilter());

            return this.View(model);
        }

        [HttpGet, Route("basescorecard/{id:int}")]
        [Authorize]
        public ViewResult BaseScoreCard(int id)
        {
            var scoreCard = this.baseScoreCardRepository.Find(id, this.Authentication.CurrentUserId);

            if (scoreCard == null)
            {
                scoreCard = this.CreateNewBaseScoreCard(id);

                this.baseScoreCardRepository.InsertOrUpdate(scoreCard);
                this.baseScoreCardRepository.Save();
            }

            var journalPrice = this.baseJournalPriceRepository.Find(id, this.Authentication.CurrentUserId);

            if (journalPrice == null)
            {
                journalPrice = this.CreateNewBaseJournalPrice(scoreCard);

                this.baseJournalPriceRepository.InsertOrUpdate(journalPrice);
                this.baseJournalPriceRepository.Save();
            }

            var scoreViewModel = scoreCard.ToBaseScoreCardViewModel();
            scoreViewModel.Price = journalPrice.ToViewModel();
            scoreViewModel.Currencies = ((Currency[])Enum.GetValues(typeof(Currency))).Select(c => new KeyValuePair<Currency, string>(c, c.GetName()));

            return this.View(scoreViewModel);
        }

        [HttpPost, Route("basescorecard/{id:int}")]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public ActionResult BaseScoreCard(int id, BaseScoreCardViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var scoreCard = this.baseScoreCardRepository.Find(id, this.Authentication.CurrentUserId);
            model.UpdateScoreCard(scoreCard, this.generalSettings.ScoreCardLifeTime);

            // It is important to note that the JournalScore of the Journal is updated in a 
            // trigger in the datavaluation and not in the code. This is done to prevent concurrency 
            // issues from leading to incorrect totals and averages for the journal score

            this.baseScoreCardRepository.InsertOrUpdate(scoreCard);
            this.baseScoreCardRepository.Save();

            var journalPrice = this.baseJournalPriceRepository.Find(id, this.Authentication.CurrentUserId);
            model.UpdateJournalPrice(journalPrice);

            this.baseJournalPriceRepository.InsertOrUpdate(journalPrice);
            this.valuationJournalPriceRepository.Save();

            return this.Json(true);
        }

        [HttpGet, Route("valuationscorecard/{id:int}")]
        [Authorize]
        public ViewResult ValuationScoreCard(int id)
        {
            var scoreCard = this.valuationScoreCardRepository.Find(id, this.Authentication.CurrentUserId);
            var journalPrice = this.valuationJournalPriceRepository.Find(id, this.Authentication.CurrentUserId);

            if (scoreCard == null)
            {
                scoreCard = this.CreateNewValuationScoreCard(id);

                this.valuationScoreCardRepository.InsertOrUpdate(scoreCard);
                this.valuationScoreCardRepository.Save();
            }

            var scoreViewModel = scoreCard.ToValuationScoreCardViewModel();
            scoreViewModel.Price = journalPrice.ToViewModel();
            scoreViewModel.Currencies = ((Currency[])Enum.GetValues(typeof(Currency))).Select(c => new KeyValuePair<Currency, string>(c, c.GetName()));

            return this.View(scoreViewModel);
        }

        [HttpPost, Route("valuationscorecard/{id:int}")]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public ActionResult ValuationScoreCard(int id, ValuationScoreCardViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var scoreCard = this.valuationScoreCardRepository.Find(id, this.Authentication.CurrentUserId);

            model.UpdateScoreCard(scoreCard, this.generalSettings.ScoreCardLifeTime);

            // It is important to note that the JournalScore of the Journal is updated in a 
            // trigger in the datavaluation and not in the code. This is done to prevent concurrency 
            // issues from leading to incorrect totals and averages for the journal score

            this.valuationScoreCardRepository.InsertOrUpdate(scoreCard);
            this.valuationScoreCardRepository.Save();

            var journalPrice = this.valuationJournalPriceRepository.Find(id, this.Authentication.CurrentUserId) ?? this.CreateNewValuationJournalPrice(scoreCard);

            model.UpdateJournalPrice(journalPrice);

            if (model.HasPrice)
            {
                this.valuationJournalPriceRepository.InsertOrUpdate(journalPrice);
            }
            else
            {
                this.valuationJournalPriceRepository.Delete(journalPrice);
            }

            this.valuationJournalPriceRepository.Save();

            return this.Json(true);
        }

        [HttpGet, Route("requestvaluation/{id:int}")]
        [Authorize]
        public ViewResult RequestValuation(int id)
        {
            var journal = this.journalRepository.Find(id);
            var model = new RequestValuationViewModel
                            {
                                JournalId = id, 
                                JournalTitle = journal.Title, 
                                JournalISSN = journal.ISSN, 
                                EmailBody = Resources.RequestValuation.Body
                                                .Replace("<<JournalTitle>>", journal.Title), 
                                EmailSubject = Resources.RequestValuation.Subject
                                                .Replace("<<JournalTitle>>", journal.Title)
                                                .Replace("<<JournalISSN>>", journal.ISSN),
                            };

            var currentUser = this.UserProfileRepository.Find(this.Authentication.CurrentUserId);
            if (currentUser != null)
            {
                model.EmailFrom = currentUser.Email;
            }

            return this.View(model);
        }

        [HttpPost, Route("requestvaluation/{id:int}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RequestValuation(RequestValuationViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            model.IsKnownEmailAddress = this.IsKnownEmailAddress(model.EmailTo);
            model.HasKnownEmailDomain = this.HasKnownEmailDomain(model.EmailTo);

            if (!model.IsKnownEmailAddress && !model.HasKnownEmailDomain)
            {
                this.ModelState.AddModelError("", "Sorry, the domain name in the email address of the addressee does not match the name of an academic institution known to us. If you want this institution to be included in our list, please enter it’s name and web address in our contact box and we will respond promptly.");

                return this.View(model);
            }

            var email = model.ToRequestValuationEmail();
            email.Url = model.IsKnownEmailAddress 
                ? this.Url.Action("Login", "Account", new { ReturnUrl = this.Url.Action("ValuationScoreCard", new { id = model.JournalId }), loginAddress = model.EmailTo }, this.Request.Url.Scheme) 
                : this.Url.Action("Register", "Account", new { loginAddress = model.EmailTo, addLink = this.Url.Action("ValuationScoreCard", new { id = model.JournalId }) }, this.Request.Url.Scheme);

            try
            {
                email.Send();

                return this.RedirectToAction("RequestValuationResult", new { success = true });
            }
            catch
            {
                return this.RedirectToAction("RequestValuationResult");
            }
        }

        [HttpGet, Route("requestvaluationresult")]
        public ActionResult RequestValuationResult(bool success = false)
        {
            this.ViewBag.Success = success;

            return this.View();
        }

        [HttpGet, Route("basescorecard/details/{id:int}")]
        public ActionResult BaseScoreCardDetails(int id)
        {
            var scoreCard = this.baseScoreCardRepository.Find(id);

            if (scoreCard.UserProfileId == this.Authentication.CurrentUserId)
            {
                return this.RedirectToAction("BaseScoreCard", scoreCard.JournalId);
            }

            if (scoreCard.State != ScoreCardState.Published)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            this.ViewBag.JournalPrice = this.baseJournalPriceRepository.Find(scoreCard.JournalId, scoreCard.UserProfileId);

            return this.View(scoreCard);
        }

        [HttpGet, Route("valuationscorecard/details/{id:int}")]
        public ActionResult ValuationScoreCardDetails(int id)
        {
            var scoreCard = this.valuationScoreCardRepository.Find(id);

            if (scoreCard.UserProfileId == this.Authentication.CurrentUserId)
            {
                return this.RedirectToAction("ValuationScoreCard", scoreCard.JournalId);
            }

            if (scoreCard.State != ScoreCardState.Published)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            this.ViewBag.JournalPrice = this.valuationJournalPriceRepository.Find(scoreCard.JournalId, scoreCard.UserProfileId);

            return this.View(scoreCard);
        }

        private BaseScoreCard CreateNewBaseScoreCard(int id)
        {
            return new BaseScoreCard { DateStarted = DateTime.Now, UserProfileId = this.Authentication.CurrentUserId, Version = this.scoreCardVersionRepository.FindCurrent(), Journal = this.journalRepository.Find(id), QuestionScores = this.questionRepository.BaseScoreCardQuestions.Select(q => new BaseQuestionScore { Question = q, Score = Score.Undecided }).ToList(), Score = new BaseScoreCardScore(), };
        }

        private ValuationScoreCard CreateNewValuationScoreCard(int id)
        {
            return new ValuationScoreCard { DateStarted = DateTime.Now, UserProfileId = this.Authentication.CurrentUserId, Version = this.scoreCardVersionRepository.FindCurrent(), Journal = this.journalRepository.Find(id), QuestionScores = this.questionRepository.ValuationScoreCardQuestions.Select(q => new ValuationQuestionScore { Question = q, Score = Score.Undecided }).ToList(), Score = new ValuationScoreCardScore(), };
        }

        private BaseJournalPrice CreateNewBaseJournalPrice(BaseScoreCard scoreCard)
        {
            return new BaseJournalPrice { BaseScoreCardId = scoreCard.Id, JournalId = scoreCard.JournalId, UserProfileId = this.Authentication.CurrentUserId, DateAdded = DateTime.Now, };
        }

        private ValuationJournalPrice CreateNewValuationJournalPrice(ValuationScoreCard scoreCard)
        {
            return new ValuationJournalPrice { ValuationScoreCardId = scoreCard.Id, JournalId = scoreCard.JournalId, UserProfileId = this.Authentication.CurrentUserId };
        }

        private bool HasKnownEmailDomain(string address)
        {
            var addressList = address.Split(new char[] { ',', ';' });

            return addressList.Select(a => new MailAddress(a)).Select(mailAddress => this.institutionRepository.All.FirstOrDefault(i => mailAddress.Host.Contains(i.ShortName))).All(institution => institution != null);
        }

        private bool IsKnownEmailAddress(string address)
        {
            var addressList = address.Split(new[] { ',', ';' });

            return addressList.All(a => !string.IsNullOrEmpty(a) && this.UserProfileRepository.FindByEmail(a) != null);
        }
    }
}