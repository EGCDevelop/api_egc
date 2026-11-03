using api_egc.Models.Uniforms;
using api_egc.Models.Uniforms.DTO;
using System.Data;
using Microsoft.Data.SqlClient;

namespace api_egc.Middles
{
    public static class OrderMiddles
    {
        public static void TRANSACTION_SAVE_ORDER(string connectionString, SaveOrdenDTO ordenDTO)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                EXEC_SP_INSERT_NEW_ORDER(connection, transaction, ordenDTO.Uniform);
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.RollbackAsync();
                }
                catch (Exception rollbackException)
                {
                    throw new Exception(
                        $"Error al revertir la transacción: {rollbackException.Message}",
                        rollbackException);
                }

                throw new Exception(
                    $"EXEC_TRANSACTION_SP_CAP_UPDATE_TRAINING: " +
                    $"Error al crear solicitud: {ex.Message}",
                    ex);
            }
        }

        public static long EXEC_SP_INSERT_NEW_ORDER(SqlConnection connection, SqlTransaction transaction, UniformOrder uniform)
        {
            SqlParameter outputParam = new("@InsertedID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            using SqlCommand cmd = new("SP_INSERT_NEW_ORDER", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@GranTotal", SqlDbType.Decimal).Value = uniform.GrandTotal;
            cmd.Parameters.Add("@AmountPaid", SqlDbType.Decimal).Value = uniform.AmountPaid;
            cmd.Parameters.Add("@AmountPending", SqlDbType.Decimal).Value = uniform.AmountPending;
            cmd.Parameters.Add("@MemberId", SqlDbType.BigInt).Value = uniform.MemberId;
            cmd.Parameters.Add("@CreationDate", SqlDbType.DateTime).Value = uniform.CreationDate;
            cmd.Parameters.Add("@InstructorId", SqlDbType.BigInt).Value = uniform.InstructorId;
            cmd.Parameters.Add("@FinancialState", SqlDbType.TinyInt).Value = uniform.FinancialState;
            cmd.Parameters.Add("@DeliveryState", SqlDbType.TinyInt).Value = uniform.DeliveryState;
            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();

            return (long)outputParam.Value;
        }
    }
}
