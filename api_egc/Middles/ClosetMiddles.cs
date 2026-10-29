using api_egc.Models;
using System.Data;
using System.Data.SqlClient;

namespace api_egc.Middles
{
    public static class ClosetMiddles
    {
        public static List<Closet> EXEC_SP_GET_CLOSET_BY_SQUAD(string connectionString, long squadId)
        {
            List<Closet> dataList = [];

            using SqlConnection conn = new(connectionString);
            conn.Open();

            using SqlCommand cmd = new("SP_GET_CLOSET_BY_SQUAD", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@squadId", SqlDbType.BigInt).Value = squadId;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Closet closet = new()
                {
                    Id = Utils.Utils.GetValue<int>(reader, "Id"),
                    SquadId = Utils.Utils.GetValue<int>(reader, "CLOUNIId"),
                    UniformId = Utils.Utils.GetValue<int>(reader, "IdEscuadra"),
                    Estado = Utils.Utils.GetValue<byte>(reader, "Estado")
                };

                dataList.Add( closet );
            }

            return dataList;
        }

        public static void EXEC_SP_INSERT_CLOSET(string connectionString, Closet closet)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            using SqlCommand cmd = new("SP_INSERT_CLOSET", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@uniformId", SqlDbType.Int).Value = closet.UniformId;
            cmd.Parameters.Add("@squadId", SqlDbType.BigInt).Value = closet.SquadId;

            cmd.ExecuteNonQuery();
        }
    }
}
