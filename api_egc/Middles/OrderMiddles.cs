using api_egc.Models.Uniforms;
using api_egc.Models.Uniforms.DTO;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    GrandTotal = Utils.Utils.GetValue<decimal>(reader, "GranTotal"),
                    AmountPaid = Utils.Utils.GetValue<decimal>(reader, "MontoAbonado"),
                    AmountPending = Utils.Utils.GetValue<decimal>(reader, "MontoPendiente"),
                    MemberId = Utils.Utils.GetValue<long>(reader, "IdIntegrante"),
                    CreationDate = Utils.Utils.GetValue<DateTime>(reader, "FechaCreacion"),
                    FullDeliveryDate = Utils.Utils.GetValueNull<DateTime>(reader, "FechaEntregaCompleta"),
                    InstructorId = Utils.Utils.GetValue<long>(reader, "INSId"),
                    FinancialState = Utils.Utils.GetValue<int>(reader, "EstadoFinanciero"),
                    DeliveryState = Utils.Utils.GetValue<int>(reader, "EstadoEntrega")
                };

                data.Add(order);
            }
            
            return data;
        }

        public static UniformOrder EXEC_SP_GET_ORDER_BY_ID(string connectionString, long orderId)
        {
            try
            {
                UniformOrder? order =  null;

                using SqlConnection connection = new(connectionString);
                connection.Open();

                using SqlCommand cmd = new("SP_GET_ORDER_BY_ID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@orderId", SqlDbType.BigInt).Value = orderId;

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    order = new()
                    {
                        Id = Utils.Utils.GetValue<int>(reader, "Id"),
                        GrandTotal = Utils.Utils.GetValue<decimal>(reader, "GranTotal"),
                        AmountPaid = Utils.Utils.GetValue<decimal>(reader, "MontoAbonado"),
                        AmountPending = Utils.Utils.GetValue<decimal>(reader, "MontoPendiente"),
                        MemberId = Utils.Utils.GetValue<long>(reader, "IdIntegrante"),
                        CreationDate = Utils.Utils.GetValue<DateTime>(reader, "FechaCreacion"),
                        FullDeliveryDate = Utils.Utils.GetValueNull<DateTime>(reader, "FechaEntregaCompleta"),
                        InstructorId = Utils.Utils.GetValue<long>(reader, "INSId"),
                        FinancialState = Utils.Utils.GetValue<int>(reader, "EstadoFinanciero"),
                        DeliveryState = Utils.Utils.GetValue<int>(reader, "EstadoEntrega")
                    };
                }

                return order!;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"EXEC_SP_GET_ORDER_BY_ID: Error inesperado: {ex.Message}",
                    ex);
            }
        }

        public static List<UniformOrderDetails> EXEC_SP_GET_ORDER_DETAILS(string connectionString, long orderId)
        {
            List<UniformOrderDetails> data = [];

            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlCommand cmd = new("SP_GET_ORDER_DETAILS", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@orderId", SqlDbType.BigInt).Value = orderId;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                UniformOrderDetails detail = new()
                {
                    Id = Utils.Utils.GetValue<int>(reader, "Id"),
                    OrderId = Utils.Utils.GetValue<long>(reader, "OrderId"),
                    UniformId = Utils.Utils.GetValue<int>(reader, "UniformId"),
                    DeliveryState = Utils.Utils.GetValue<byte>(reader, "DeliveryState"),
                    UserDelivery = Utils.Utils.GetValue<long>(reader, "UserDelivery"),
                    DeliveryDate = Utils.Utils.GetValueNull<DateTime>(reader, "DeliveryDate"),
                    DeliveryReversalComments = Utils.Utils.GetValueNull<string>(reader, "DeliveryReversalComments"),
                    StateActive = Utils.Utils.GetValue<byte>(reader, "StateActive")
                };

                data.Add(detail);
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

                transaction.Commit();
            }
            catch (SqlException ex) 
            {
                transaction.Rollback();

                throw new Exception(
                    $"TRANSACTION_SAVE_ORDER: Error de SQL Server al crear la orden. " +
                    $"Procedimiento: {ex.Procedure}. " +
                    $"Línea: {ex.LineNumber}. " +
                    $"Error SQL #{ex.Number}: {ex.Message}",
                    ex);
            }
            catch(InvalidCastException ex)
            {
                transaction.Rollback();

                throw new Exception(
                    "TRANSACTION_SAVE_ORDER: El tipo de dato devuelto por el procedimiento " +
                    "SP_INSERT_NEW_ORDER no coincide con el esperado. " +
                    "Verifica si @InsertedID está declarado como INT o BIGINT en SQL Server.",
                    ex);
            }
            catch(Exception ex)
            {
                transaction.Rollback();

                throw new Exception(
                    $"TRANSACTION_SAVE_ORDER: Error inesperado al crear la orden: {ex.Message}",
                    ex);
            }
        }

        public static void TRANSACTION_ORDER_DETAIL(string connectionString, UniformOrderDetails details, UniformOrder order)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                EXEC_SP_UPDATE_ORDER_DETAIL(connection, transaction, details);

                // si el estado del detalle esta entregado, cambiamos el estado de entrega del pedidos a 2 - entrega parcial
                if(details.DeliveryState == 2)
                {
                    order.DeliveryState = 2;
                    OrderMiddles.EXEC_SP_UPDATE_ORDER(connection, transaction, order);
                }

                string logMessage = $"El usuario {details.UserDelivery} entrego el implemento con id de detalle: {details.UniformId} " +
                    $"| Fecha de entrega: {details.DeliveryDate} |";
                PaymentMiddles.EXEC_SP_INSERT_PAYMENT_LOGS(connection, transaction, logMessage, details.UserDelivery);

                transaction.Commit();
            }
            catch (SqlException ex)
            {
                transaction.Rollback();

                throw new Exception(
                    $"TRANSACTION_ORDER_DETAIL: Error de SQL Server al crear la orden. " +
                    $"Procedimiento: {ex.Procedure}. " +
                    $"Línea: {ex.LineNumber}. " +
                    $"Error SQL #{ex.Number}: {ex.Message}",
                    ex);
            }
            catch (InvalidCastException ex)
            {
                transaction.Rollback();

                throw new Exception(
                    "TRANSACTION_ORDER_DETAIL: El tipo de dato devuelto por el procedimiento " +
                    "EXEC_SP_UPDATE_ORDER_DETAIL no coincide con el esperado. "+
                    ex);
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw new Exception(
                    $"TRANSACTION_SAVE_ORDER: Error inesperado al crear la orden: {ex.Message}",
                    ex);
            }
        }

        public static long EXEC_SP_INSERT_NEW_ORDER(SqlConnection connection, SqlTransaction transaction, UniformOrder uniform)
        {
            SqlParameter outputParam = new("@InsertedID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
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
            SqlParameter outputParam = new("@InsertedID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            using SqlCommand cmd = new("SP_INSERT_ORDER_DETAILS", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@OrderId", SqlDbType.BigInt).Value = orderId;
            cmd.Parameters.Add("@UniformId", SqlDbType.Int).Value = details.UniformId;
            cmd.Parameters.Add("@DeliveryState", SqlDbType.TinyInt).Value = details.DeliveryState;
            cmd.Parameters.Add("@UserDelivery", SqlDbType.BigInt).Value = details.UserDelivery == 0 ? DBNull.Value : details.UserDelivery;
            cmd.Parameters.Add("@DeliveryReversalComments", SqlDbType.NVarChar, -1).Value = string.IsNullOrEmpty(details.DeliveryReversalComments) ? DBNull.Value : details.DeliveryDate;
            cmd.Parameters.Add("@StateActive", SqlDbType.TinyInt).Value = details.StateActive;
            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();

            return (long)outputParam.Value;
        }

        public static void EXEC_SP_UPDATE_ORDER(SqlConnection connection, SqlTransaction transaction, UniformOrder order)
        {
            using SqlCommand cmd = new("SP_UPDATE_ORDER", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@OrderId", SqlDbType.BigInt).Value = order.Id;
            cmd.Parameters.Add("@AmountPaid", SqlDbType.Decimal).Value = order.AmountPaid;
            cmd.Parameters.Add("@AmountPending", SqlDbType.Decimal).Value = order.AmountPending;
            cmd.Parameters.Add("@FullDeliveryDate", SqlDbType.DateTime).Value = order.FullDeliveryDate;
            cmd.Parameters.Add("@FinancialState", SqlDbType.TinyInt).Value = order.FinancialState;
            cmd.Parameters.Add("@DeliveryState", SqlDbType.TinyInt).Value = order.DeliveryState;

            cmd.ExecuteNonQuery();
        }

        public static void EXEC_SP_UPDATE_ORDER_DETAIL(SqlConnection connection, SqlTransaction transaction, UniformOrderDetails details)
        {
            using SqlCommand cmd = new("SP_UPDATE_ORDER_DETAIL", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = details.Id;
            cmd.Parameters.Add("@DeliveryState", SqlDbType.TinyInt).Value = details.DeliveryState;
            cmd.Parameters.Add("@UserDelivery", SqlDbType.BigInt).Value = details.UserDelivery;
            cmd.Parameters.Add("@DeliveryDate", SqlDbType.DateTime).Value = details.DeliveryDate;
            cmd.Parameters.Add("@DeliveryReversalComments", SqlDbType.NVarChar, -1).Value = string.IsNullOrEmpty(details.DeliveryReversalComments) ? DBNull.Value : details.DeliveryReversalComments;
            cmd.Parameters.Add("@StateActive", SqlDbType.TinyInt).Value = details.StateActive;

            cmd.ExecuteNonQuery();
        }
    }
}
