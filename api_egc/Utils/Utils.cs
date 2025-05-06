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

        public static T GetValueNull<T>(SqlDataReader reader, string columnName)
        {
            var value = reader[columnName];

            // Si el valor es DBNull, devolvemos `default(T)`
            if (value == DBNull.Value)
            {
                return default;
            }

            // Si es un tipo nullable, convertimos el valor a su tipo subyacente
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
            {
                return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)));
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
