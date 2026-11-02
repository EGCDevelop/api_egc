using api_egc.Models.Uniforms;
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

        public static void TransactionUniformsUpdated(string connectionString, List<Closet> closetSent)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                EXEC_SP_UPDATE_CLOSET(connection, transaction, closetSent);

                foreach (var cl in closetSent)
                {
                    if(cl.Id == 0)
                    {
                        EXEC_SP_INSERT_CLOSET(connection, transaction, cl);
                    }
                }

                transaction.Commit();
            } catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Fallo en la actualización de un closet: " + ex.Message);
            }
        }

        public static void EXEC_SP_INSERT_CLOSET(SqlConnection connection, SqlTransaction transaction, Closet closet)
        {
            using SqlCommand cmd = new("SP_INSERT_CLOSET", connection, transaction);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@uniformId", SqlDbType.Int).Value = closet.UniformId;
            cmd.Parameters.Add("@squadId", SqlDbType.BigInt).Value = closet.SquadId;

            cmd.ExecuteNonQuery();
        }

        public static void EXEC_SP_UPDATE_CLOSET(SqlConnection connection, SqlTransaction transaction, List<Closet> closetList)
        {
            string listId = string.Join(",", closetList.Select(x => x.UniformId.ToString()));

            using SqlCommand cmd = new("SP_UPDATE_CLOSET", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@listId", SqlDbType.NVarChar, -1).Value = listId;

            cmd.ExecuteNonQuery();
        }
    }
}
