using api_egc.Models.Uniforms;
using api_egc.Models.Uniforms.DTO;
using Microsoft.Data.SqlClient;
using System.Data;

namespace api_egc.Middles
{
    public static class OrderMiddles
    {
        public static List<UniformOrder> EXEC_SP_GET_ORDER_BY_FILTERS(string connectionString, long memberId, 
            int? FinancialState = 0, int? DeliveryState = 0)
        {
            List<UniformOrder> data = [];

            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlCommand cmd = new("SP_GET_ORDER_BY_FILTERS", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@memberId", SqlDbType.BigInt).Value = memberId;
            cmd.Parameters.Add("@FinancialState", SqlDbType.TinyInt).Value = FinancialState;
            cmd.Parameters.Add("@DeliveryState", SqlDbType.TinyInt).Value = DeliveryState;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                UniformOrder order = new()
                {
                    Id = Utils.Utils.GetValue<int>(reader, "Id"),
                    GrandTotal = Utils.Utils.GetValue<decimal>(reader, "GrandTotal"),
                    AmountPaid = Utils.Utils.GetValue<decimal>(reader, "AmountPaid"),
                    AmountPending = Utils.Utils.GetValue<decimal>(reader, "AmountPending"),
                    MemberId = Utils.Utils.GetValue<long>(reader, "MemberId"),
                    CreationDate = Utils.Utils.GetValue<DateTime>(reader, "CreationDate"),
                    FullDeliveryDate = Utils.Utils.GetValue<DateTime>(reader, "FullDeliveryDate"),
                    InstructorId = Utils.Utils.GetValue<long>(reader, "InstructorId"),
                    FinancialState = Utils.Utils.GetValue<int>(reader, "FinancialState"),
                    DeliveryState = Utils.Utils.GetValue<int>(reader, "DeliveryState")
                };

                data.Add(order);
            }
            
            return data;
        }

        public static void TRANSACTION_SAVE_ORDER(string connectionString, SaveOrdenDTO ordenDTO)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                // insetamos el encabezado de una orden
                long orderId = EXEC_SP_INSERT_NEW_ORDER(connection, transaction, ordenDTO.Uniform);

                // insertamos los detalles de una orden
                foreach (UniformOrderDetails detail in ordenDTO.Details)
                {
                    detail.DeliveryState = 1;
                    detail.StateActive = 1;
                    EXEC_SP_INSERT_ORDER_DETAILS(connection, transaction, detail, orderId);
                }
            }
            catch (Exception ex) 
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackException)
                {
                    throw new Exception(
                        $"Error al revertir la transacción: {rollbackException.Message}",
                        rollbackException);
                }
                throw new Exception(
                    $"TRANSACTION_SAVE_ORDER: " +
                    $"Error al crear orden: {ex.Message}",
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

        public static long EXEC_SP_INSERT_ORDER_DETAILS(SqlConnection connection, SqlTransaction transaction, UniformOrderDetails details,
            long orderId)
        {
            SqlParameter outputParam = new("@InsertedID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            using SqlCommand cmd = new("SP_INSERT_ORDER_DETAILS", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@OrderId", SqlDbType.BigInt).Value = orderId;
            cmd.Parameters.Add("@UniformId", SqlDbType.Int).Value = details.UniformId;
            cmd.Parameters.Add("@DeliveryState", SqlDbType.TinyInt).Value = details.DeliveryState;
            cmd.Parameters.Add("@UserDelivery", SqlDbType.BigInt).Value = details.UserDelivery;
            cmd.Parameters.Add("@DeliveryReversalComments", SqlDbType.NVarChar, -1).Value = string.IsNullOrEmpty(details.DeliveryReversalComments) ? DBNull.Value : details.DeliveryDate;
            cmd.Parameters.Add("@StateActive", SqlDbType.TinyInt).Value = details.StateActive;
            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();

            return (long)outputParam.Value;
        }
    }
}
