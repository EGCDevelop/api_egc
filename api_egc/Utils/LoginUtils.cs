using System.Data;
using System.Data.SqlClient;
using api_egc.Models;
using Newtonsoft.Json.Linq;

namespace api_egc.Utils
{
    public class LoginUtils
    {

        public static Versiones EXEC_SP_VERSION_APP(string connectionString)
        {
            Versiones version = null;
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_VERSION_APP", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    version = new()
                    {
                        VERIdVersion = Utils.GetValue<long>(reader, "VERIdVersion"),
                        VERNumero = Utils.GetValue<string>(reader, "VERNumero"),
                        VERDescripcion = Utils.GetValue<string>(reader, "VERDescripcion"),
                        VERObligatoria = Utils.GetValue<int>(reader, "VERObligatoria"),
                    };
                }
            }

            return version!;
        }


        public static void EXEC_SP_UPDATE_PASSWORD(string connectionString, string username, string password)
        {

            string hashedPassword  = PasswordHasher.HashPassword(password);
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_UPDATE_PASSWORD", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 500).Value = hashedPassword;

                    // Ejecutar el procedimiento
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static Member EXEC_SP_GET_MEMBER_BY_USERNAME(string connectionString, string username)
        {
            Member member = null;

            using (SqlConnection  connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_GET_MEMBER_BY_USERNAME", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = username;

                    using SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        member = new()
                        {
                            INTIdIntegrante = Utils.GetValue<long>(reader, "INTIdIntegrante"),
                            INTNombres = Utils.GetValue<string>(reader, "INTNombres"),
                            INTApellidos = Utils.GetValue<string>(reader, "INTApellidos"),
                            INTESCIdEscuadra = Utils.GetValue<long>(reader, "INTESCIdEscuadra"),
                            INTPUIdPuesto = Utils.GetValue<long>(reader, "INTPUIdPuesto"),
                            INTPassword = Utils.GetValue<string>(reader, "INTPassword")
                        };
                    }
                }
            }

            return member!;
        }

        public static void EXEC_SP_UPDATE_TOKEN(string connectionString, string username, string token)
        {
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_UPDATE_TOKEN", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@Token", SqlDbType.NVarChar, 1000).Value = token;

                    // Ejecutar el procedimiento
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void EXEC_SP_INSERT_BITACORA(string connectionString, long id)
        {
            DateTime date = DateTime.Now;

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_INSERT_BITACORA", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@IdIntegrante", SqlDbType.BigInt).Value = id;
                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = date;

                    // Ejecutar el procedimiento
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
