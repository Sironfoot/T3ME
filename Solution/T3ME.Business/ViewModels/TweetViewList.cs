using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Business.ViewModels
{
    public class TweetViewList
    {
        public TweetViewList(string pageTitle, int totalRecords, int pageNumber, int recordsPerPage)
        {
            this.PageTitle = pageTitle;
            this.TotalRecords = totalRecords;
            this.PageNumber = pageNumber;
            this.RecordsPerPage = recordsPerPage;

            this.Tweets = new List<TweetView>();
        }

        public string PageTitle { get; protected set; }

        public int TotalRecords { get; protected set; }
        public int PageNumber { get; protected set; }
        public int RecordsPerPage { get; protected set; }

        public string SearchTerm { get; set; }

        public IList<TweetView> Tweets { get; protected set; }
    }
}