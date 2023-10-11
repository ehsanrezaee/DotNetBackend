using System.Text;
using Microsoft.Data.SqlClient;

namespace ErSoftDev.Common.Utilities
{
    public static class SqlExtensions
    {
        public static string GetArguments(this object[] parameters, string spName = "")
        {
            var sbParamList = new StringBuilder();
            if (spName.HasValue()) sbParamList.Append(spName + " ");
            foreach (SqlParameter item in parameters)
            {
                sbParamList.Append(item.ParameterName);
                sbParamList.Append(",");
            }
            return sbParamList.Remove(sbParamList.Length - 1, 1).ToString();
        }
        public static string GetArguments(this SqlParameter[] parameters, string spName = "")
        {
            var sbParamList = new StringBuilder();
            if (spName.HasValue()) sbParamList.Append(spName + " ");
            foreach (var item in parameters)
            {
                sbParamList.Append(item.ParameterName);
                sbParamList.Append(",");
            }
            return sbParamList.Remove(sbParamList.Length - 1, 1).ToString();
        }
    }
}
