using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSUStudentManagement.CommonCode
{
    public class CommonHelper
    {

        #region Define as Singleton
        private static CommonHelper _Instance;
        public static CommonHelper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new CommonHelper();
                }

                return (_Instance);
            }
        }
        private CommonHelper()
        {
        }
        #endregion



        //--generate RandomNumber
        public string? GeneratRandomNumber()
        {
            Random rnd = new Random();
            string? RandomNumber = (rnd.Next(100000, 999999)).ToString();
            return RandomNumber;
        }

        //--generate New GUID ID
        public string? GeneratNewGUID()
        {
            string? NewGUID = "";
            Guid obj = Guid.NewGuid();
            NewGUID = obj.ToString();

            return NewGUID;
        }
    }
}