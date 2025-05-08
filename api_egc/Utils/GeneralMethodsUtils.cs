using System.Data;
using System.Data.SqlClient;
using api_egc.Models;

namespace api_egc.Utils
{
    public class GeneralMethodsUtils
    {
        public static List<Escuadras> EXEC_SP_GET_ESCUADRA(string connectionString)
        {
            List<Escuadras> list = [];

            using(SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_ESCUADRA", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Escuadras escuadras = new()
                    {
                        ESCIdEscuadra = Utils.GetValue<long>(reader, "ESCIdEscuadra"),
                        ESCNombre = Utils.GetValue<string>(reader, "ESCNombre")
                    };

                    list.Add(escuadras);
                }
            }

            return list;
        }
    }
}
