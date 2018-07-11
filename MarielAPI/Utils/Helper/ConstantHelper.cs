using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarielAPI.Helper
{
    public class ConstantHelper
    {

        public const string apiVersion = "api/v1/";
        public static readonly int pageSize = 20;

        public const string accountEndPoint = apiVersion + "accounts";
        public const string loanReuqestEndPoint = apiVersion + "loanrequests";



        public static readonly int document_ItemTableID = 15;
        public static readonly int document_ItemGroupTableID = 16;

        public static readonly int TABLE_ACCOUNT_ID = 8090;

        public static readonly string LOG_FOLDER = "logs";
        public static readonly string UPLOAD_FOLDER = "uploads";

        public static readonly string ALREADY_REQUEST_LOAN = "You already requested a loan!";


        public static readonly string INVALID_PIN = "Invalid PIN!";
        public static readonly string PIN_EXPIRED = "PIN Expired !";
        public static readonly string PENDING_SMS = "Please wait 30 more minutes to send sms again !";
        public static readonly string INVALID_PHONE = "Invalid Phone Number !";
    }
}