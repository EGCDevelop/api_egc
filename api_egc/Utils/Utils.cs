using System.Data.SqlClient;

namespace api_egc.Utils
{
    public class Utils
    {
        public static T GetValue<T>(SqlDataReader reader, string columnName) 
        {
            var value = reader[columnName];

            if(value == DBNull.Value)
            {
                return default(T)!;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
