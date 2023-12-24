using DAL.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DAL.Helpers.PageHelpers
{
    public class Constants : IConstants
    {
        private readonly IConfiguration _configuration;
        public Constants(IConfiguration configuration)
        {

            this._configuration = configuration;
        }



        public int ITEMS_PER_PAGE()
        {
            try
            {
                string? itemValue = _configuration.GetSection("AppSetting").GetSection("ListingItemsPerPage").Value;
                int pageSize = !String.IsNullOrEmpty(itemValue) ? Convert.ToInt32(itemValue) : 10;

                if (pageSize == 0 || pageSize < 1)
                {
                    return 10;
                }
                else
                {
                    return pageSize;
                }
            }
            catch (Exception ex)
            {

                return 10;
            }
        }



    }

    public interface IConstants
    {

        public int ITEMS_PER_PAGE();
    }

}