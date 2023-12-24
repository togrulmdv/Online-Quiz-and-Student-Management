using DAL.ConnectionManager;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DBContext
{
    public class DataContextHelper : IDataContextHelper
    {
        private readonly IConfiguration _configuration;


        public DataContextHelper(IConfiguration configuration)
        {
            _configuration = configuration;

            ConnetionString = _configuration.GetConnectionString("OnlineQuizConn");
            providerName = "System.Data.SqlClient";
        }
        public string ConnetionString { get; }
        public string providerName { get; }

        public OnlineQuizConnDB GetPPContextHelper(bool enableAutoSelect = true)
        {

            return (GetNewDataContext(ConnetionString, providerName, enableAutoSelect));
        }

        private static OnlineQuizConnDB GetNewDataContext(string ConnetionString, string providerName, bool enableAutoSelect)
        {
            OnlineQuizConnDB repository = new OnlineQuizConnDB(ConnetionString, providerName);
            repository.EnableAutoSelect = enableAutoSelect;
            //repository.ELHelperInstance = elHelperInstance;



            return (repository);
        }
    }
}
