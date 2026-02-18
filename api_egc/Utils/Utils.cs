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

        public static String UsernameGenerate(string name, string lastName)
        {
            string cleanName = name.Trim().ToLower();
            string cleanLastName = lastName.Trim().ToLower();

            string[] names = cleanName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] lastNames = cleanLastName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string firstInitial = names.Length > 0 ? names[0][0].ToString() : "";
            string firstLastName = lastNames.Length > 0 ? lastNames[0] : "";
            string secondLastNameInitial = lastNames.Length > 1 ? lastNames[1][0].ToString() : "";

            return $"{firstInitial}{firstLastName}{secondLastNameInitial}";
        }

        public static DateTime getCurrentDateGMT6()
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, myZone);
        }

    }
}
