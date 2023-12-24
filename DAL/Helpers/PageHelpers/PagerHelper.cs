using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Helpers.PageHelpers
{
    public class PagerHelper
    {
    

        #region Define as Singleton
        private static PagerHelper _Instance;

        public static PagerHelper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new PagerHelper();
                }

                return (_Instance);
            }
        }

        public PagerHelper()
        {
            this.AjaxEnabled = false;
        }
        #endregion


        public const string? CURRENT_PAGE_NUMBER = "C_PG_N";
        public const string? ORDER = "ORDER";
        public const string? ORDER_BY = "ORDER_BY";
        public const string? ORDER_FOR_PAGER = "ORDER_FOR_PAGER";
        public const string? TOTAL_RECORDS = "TOTAL_REC_COUNT";

        //public const int DirectlyNavigablePageCount = 10;

        public int DirectlyNavigablePageCount
        {
            get
            {

                if (CurrentPage > 0)
                {
                    if (CurrentPage > 9999) return 3;
                    if (CurrentPage > 999) return 4;
                    if (CurrentPage > 0) return 5;

                    return 5;
                }
                else
                {
                    return 5;
                }
            }
        }

        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }

        //public int DirectlyNavigablePageCount
        //{
        //   get
        //    {
        //        if(directlyNavigablePageCount == 0)
        //            return (Constants.ITEMS_PER_PAGE);
        //        else
        //            return directlyNavigablePageCount;
        //    }
        //    set 
        //    {
        //        directlyNavigablePageCount = value;
        //    }
        //}

        //int directlyNavigablePageCount = 0;

        public int RecordsPerPage
        {
            get
            {
                if (recordPerpage == 0)
                    return (0);
                else
                    return recordPerpage;
            }
            set
            {
                recordPerpage = value;
            }
        }
        int recordPerpage = 0;

        //public Views ParentView { get; set; }
        public string? OrderBy { get; set; }
        public string? OrderType { get; set; }

        public int TotalPages
        {
            get
            {
                return ((int)Math.Ceiling((double)TotalRecords / RecordsPerPage));
            }
        }

        public int StartPageNumber
        {
            get
            {
                return Math.Abs((((((int)Math.Ceiling((double)CurrentPage / DirectlyNavigablePageCount))
                                   * DirectlyNavigablePageCount) - DirectlyNavigablePageCount) + 1));
            }
        }

        public int EndPageNumber
        {
            get
            {
                if (StartPageNumber + DirectlyNavigablePageCount > TotalPages)
                {
                    return (TotalPages);
                }
                return (StartPageNumber + DirectlyNavigablePageCount - 1);
            }
        }

        public bool ShowFirstPrevLinks
        {
            get
            {
                if (TotalPages == 0 || TotalPages < CurrentPage)
                    return false;
                return (StartPageNumber > DirectlyNavigablePageCount);
            }
        }

        public bool ShowNextLastLinks
        {
            get
            {
                if (TotalPages == 0 || TotalPages < CurrentPage)
                    return false;
                return (EndPageNumber < TotalPages);
            }
        }

        public static string? ToggleOrder(string? order)
        {
            return (order == "ASC" ? "DESC" : "ASC");
        }

        public bool IsForSearchPage { get; set; }

        public bool AjaxEnabled { get; set; }
        public string? OnClientClickAjaxCall { get; set; }
        public string? GetAjaxRelatedAttributes()
        {
            if (!AjaxEnabled)
                return "";
            else
                return string.Format("onclick='{0};'", this.OnClientClickAjaxCall);
        }


        public PagerHelper? MakePaginationObject(int ListCount = 0, int TotalRecords = 0, int ITEMS_PER_PAGE = 10, int PageNo = 1)
        {
            PagerHelper? helper = new PagerHelper();

            try
            {
                if (ListCount > 0 && TotalRecords > 0)
                {
                    helper.TotalRecords = TotalRecords;
                    helper.RecordsPerPage = ITEMS_PER_PAGE;
                    helper.CurrentPage = PageNo;
                }
                else
                {
                    helper.TotalRecords = 0;
                    helper.RecordsPerPage = 0;
                    helper.CurrentPage = 0;
                }


                return helper;
            }
            catch (Exception)
            {


                return helper;
            }

        }
    }
}
