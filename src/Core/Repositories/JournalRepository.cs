﻿using QOAM.Core.Helpers;

namespace QOAM.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Helpers;

    using LinqKit;

    using PagedList;

    using QOAM.Core.Repositories.Filters;

    public class JournalRepository : Repository<Journal>, IJournalRepository
    {
        public JournalRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public IList<Journal> All
        {
            get
            {
                return this.DbContext.Journals.OrderBy(j => j.Title).ToList();
            }
        }

        public Journal FindByIssn(string issn)
        {
            return this.DbContext.Journals.FirstOrDefault(j => j.ISSN.ToLower() == issn.ToLower());
        }

        public IQueryable<string> Titles(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Enumerable.Empty<string>().AsQueryable();
            } 

            return this.DbContext.Journals.Where(j => j.Title.ToLower().StartsWith(query.ToLower())).Select(j => j.Title);
        }

        public IQueryable<string> Publishers(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Enumerable.Empty<string>().AsQueryable();
            } 

            return this.DbContext.Journals.Where(j => j.Publisher.Name.ToLower().StartsWith(query.ToLower())).Select(j => j.Publisher.Name).Distinct();
        }

        public IQueryable<string> Issns(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Enumerable.Empty<string>().AsQueryable();
            } 

            return this.DbContext.Journals.Where(j => j.ISSN.ToLower().StartsWith(query.ToLower())).Select(j => j.ISSN);
        }

        public IQueryable<string> AllIssns
        {
            get
            {
                return this.DbContext.Journals.Select(j => j.ISSN);    
            }
        }

        public IPagedList<Journal> Search(JournalFilter filter)
        {
            var query = this.DbContext.Journals
                .Include(j => j.Publisher)
                .Include(j => j.Languages)
                .Include(j => j.Subjects)
                .Include(j => j.JournalScore);

            return query.Search(filter);
        }

        public IQueryable<Journal> SearchByISSN(IEnumerable<string> issns)
        {
            return this.DbContext.Journals.Where(j => issns.Contains(j.ISSN));
        }

        public IList<Journal> SearchAll(JournalFilter filter)
        {
            var query = this.DbContext.Journals.AsQueryable();

            if (filter.PublisherId.HasValue)
            {
                query = query.Where(j => j.PublisherId == filter.PublisherId.Value);
            }

            return query.ToList();
        }

        public Journal Find(int id)
        {
            return this.DbContext.Journals.Find(id);
        }

        public int ScoredJournalsCount()
        {
            return this.DbContext.Journals.Count(j => j.JournalScore.NumberOfBaseReviewers > 0);
        }

        public IList<Journal> AllIncluding(params Expression<Func<Journal, object>>[] includeProperties)
        {
            IQueryable<Journal> query = this.DbContext.Journals;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.ToList();
        }
    }
}