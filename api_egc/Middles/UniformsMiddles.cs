using api_egc.Models.Uniforms;
using System.Data;
using Microsoft.Data.SqlClient;

namespace api_egc.Middles
{
    public static class UniformsMiddles
    {
        public static List<Uniform> EXEC_SP_GET_UNIFORM(string connectionString, string name, int state)
        {
            List<Uniform> list = [];

            using SqlConnection conn = new(connectionString);
            conn.Open();

            using SqlCommand cmd = new("SP_GET_UNIFORM", conn);
            
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar, -1).Value = name;
            cmd.Parameters.Add("@state", SqlDbType.TinyInt).Value = state;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Uniform uniform = new()
                {
                    Id = Utils.Utils.GetValue<int>(reader, "Id"),
                    Name = Utils.Utils.GetValue<string>(reader, "Nombre"),
                    Price = Utils.Utils.GetValue<decimal>(reader, "Precio"),
                    State = Utils.Utils.GetValue<byte>(reader, "Estado")
                };

                list.Add(uniform);
            }

            return list;
        }

        public static void EXEC_SP_INSERT_UNIFORM(string connectionString, Uniform uniform)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            using SqlCommand cmd = new("SP_INSERT_UNIFORM", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar, -1).Value = uniform.Name;
            cmd.Parameters.Add("@price", SqlDbType.Decimal).Value = uniform.Price;

            cmd.ExecuteNonQuery();
        }

        public static void EXEC_SP_UPDATE_UNIFORM(string connectionString, Uniform uniform)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            using SqlCommand cmd = new("SP_UPDATE_UNIFORM", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = uniform.Id;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar, -1).Value = uniform.Name;
            cmd.Parameters.Add("@price", SqlDbType.Decimal).Value = uniform.Price;
            cmd.Parameters.Add("@state", SqlDbType.TinyInt).Value = uniform.State;

            cmd.ExecuteNonQuery();
        }
    }
}
