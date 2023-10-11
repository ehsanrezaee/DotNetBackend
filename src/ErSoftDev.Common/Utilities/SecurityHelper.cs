using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ErSoftDev.Common.Utilities
{
    public static class SecurityHelper
    {
        #region Genearte Md5
        public static string GetMd5(string str)
        {
            str = Hash64(str);
            var textBytes = Encoding.Default.GetBytes(str);
            try
            {
                var cryptHandler = new MD5CryptoServiceProvider();
                var hash = cryptHandler.ComputeHash(textBytes);
                var ret = "";
                foreach (var a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                return "";
            }

        }
        private static string Hash64(string str)
        {
            str = "!#-&'()" + str + "*+,%$./";
            try
            {
                var encDataByte = Encoding.UTF8.GetBytes(str);
                return Convert.ToBase64String(encDataByte);
            }
            catch
            {
                return str;
            }
        }
        #endregion

        public static string StringValidation(string inputStr)
        {
            var mode = "H";
            if (inputStr == null || string.IsNullOrEmpty(inputStr))
                return inputStr;

            inputStr = inputStr.ToLower();
            //For all string 
            inputStr = inputStr.Trim();
            inputStr = inputStr.Replace(",", "_");
            inputStr = inputStr.Replace("'", "_");
            inputStr = inputStr.Replace("\"", "_");
            inputStr = inputStr.Replace("ي", "ی");
            inputStr = inputStr.Replace("ی", "ی");
            inputStr = inputStr.Replace("ك", "ک");
            inputStr = inputStr.Replace("ڪ", "ک");
            inputStr = inputStr.Replace("۱", "1");
            inputStr = inputStr.Replace("۲", "2");
            inputStr = inputStr.Replace("۳", "3");
            inputStr = inputStr.Replace("۴", "4");
            inputStr = inputStr.Replace("۵", "5");
            inputStr = inputStr.Replace("۶", "6");
            inputStr = inputStr.Replace("۷", "7");
            inputStr = inputStr.Replace("۸", "8");
            inputStr = inputStr.Replace("۹", "9");
            inputStr = inputStr.Replace("۰", "0");

            switch (mode)
            {
                case "H":
                    inputStr = inputStr.Replace("'", "''");
                    inputStr = inputStr.Replace(">", ">");
                    inputStr = inputStr.Replace("<", "<");
                    inputStr = inputStr.Replace("/*", " ");
                    inputStr = inputStr.Replace("*/", " ");
                    inputStr = inputStr.ToLower().Replace("script", "BLOCKED");
                    inputStr = inputStr.ToLower().Replace("xp_", " ");
                    inputStr = inputStr.ToLower().Replace("union", "_union");
                    inputStr = inputStr.ToLower().Replace("having", "_having");
                    inputStr = inputStr.ToLower().Replace("insert", "_insert");
                    inputStr = inputStr.ToLower().Replace("select", "_select");
                    inputStr = inputStr.ToLower().Replace("delete", "_delete");
                    inputStr = inputStr.ToLower().Replace("update", "_update");
                    inputStr = inputStr.ToLower().Replace("drop", "_drop");
                    inputStr = inputStr.ToLower().Replace("exec", "_exec");
                    inputStr = inputStr.ToLower().Replace("sp_", "_sp_");
                    inputStr = inputStr.ToLower().Replace("shutdown", "_shutdown");
                    inputStr = inputStr.ToLower().Replace("javascript\\:", "BLOCKED ");
                    inputStr = inputStr.ToLower().Replace("vbscript\\:", "BLOCKED ");

                    break;
                case "M":
                    inputStr = inputStr.Replace("'", "''");
                    inputStr = inputStr.ToLower().Replace("script", "BLOCKED");
                    inputStr = inputStr.ToLower().Replace("javascript\\:", "BLOCKED ");
                    inputStr = inputStr.ToLower().Replace("vbscript\\:", "BLOCKED ");

                    break;
                case "L":
                    inputStr = inputStr.Replace("'", "''");
                    break;
            }

            string functionReturnValue = inputStr;
            return functionReturnValue;
        }

    }
}

