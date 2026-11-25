using Microsoft.Data.SqlClient;
using System.Data;


namespace api_egc.Utils
{
    public static class SqlParameterExtensions
    {
        public static SqlParameter AddDecimal(this SqlParameterCollection parameters, string name, decimal value, byte precision = 18, byte scale = 2)
        {
            return parameters.Add(new SqlParameter(name, SqlDbType.Decimal)
            {
                Precision = precision,
                Scale = scale,
                Value = value
            });
        }
    }
}
