using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.ConnectionManager
{
    public interface IDataContextHelper
    {
        public string ConnetionString { get; }
        public string providerName { get; }

        public OnlineQuizConnDB GetPPContextHelper(bool enableAutoSelect = true);
    }
}
