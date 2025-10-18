using api_egc.Models.Instructors;
using System.Data;
using System.Data.SqlClient;

namespace api_egc.Utils
{
    public class InstructorUtils
    {
        public static List<EscuadrasInstructoresDTO> EXEC_SP_GET_ASSIGNED_SQUADS_INSTRUCTORS(string connectionString, long idInstructor)
        {
            List<EscuadrasInstructoresDTO> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_ASSIGNED_SQUADS_INSTRUCTORS", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@IdInstructor", SqlDbType.BigInt).Value = idInstructor;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    EscuadrasInstructoresDTO data = new()
                    {
                        Id = Utils.GetValue<int>(reader, "Id"),
                        IdEscuadra = Utils.GetValue<int>(reader, "IdEscuadra"),
                        IdInstructor = Utils.GetValue<long>(reader, "IdInstructor"),
                        Principal = Utils.GetValue<int>(reader, "Principal"),
                        Nombre = Utils.GetValue<string>(reader, "Nombre")
                    };

                    list.Add(data);
                }
            }
            return list;
        }

        public static void EXEC_SP_UPDATE_INSTRUCTOR_PROFILE(string connectionString, long id, string name, string lastName,
            string phone, string email)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlCommand cmd = new("SP_UPDATE_INSTRUCTOR_PROFILE", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = id;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
            cmd.Parameters.Add("@lastName", SqlDbType.NVarChar).Value = lastName;
            cmd.Parameters.Add("@phone", SqlDbType.VarChar, 8).Value = phone;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;

            // Ejecutar el procedimiento
            cmd.ExecuteNonQuery();
        }
    }
}
