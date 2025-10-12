using api_egc.Models;
using api_egc.Models.Instructors;
using System.Data;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                using (SqlCommand cmd = new("SP_GET_ASSIGNED_SQUADS_INSTRUCTORS", connection))
                {
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
            }
            return list;
        }
    }
}
