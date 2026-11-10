using api_egc.Models.Uniforms;
using api_egc.Models.Uniforms.DTO;
using Microsoft.Data.SqlClient;
using System.Data;

namespace api_egc.Middles
{
    public static class PaymentMiddles
    {
        public static int EXEC_SP_GET_PAYMENT_NUMBER(string connectionString, long orderId)
        {
            try
            {
                using SqlConnection connection = new(connectionString);
                connection.Open();
                
                using SqlCommand cmd = new("SP_GET_PAYMENT_NUMBER", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@orderId", SqlDbType.BigInt).Value = orderId;

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToInt32(result);
            } catch(SqlException ex)
            {
                throw new Exception(
                    $"EXEC_SP_GET_PAYMENT_NUMBER: Error de SQL Server al crear la orden. " +
                    $"Procedimiento: {ex.Procedure}. " +
                    $"Línea: {ex.LineNumber}. " +
                    $"Error SQL #{ex.Number}: {ex.Message}",
                    ex);
            } catch(Exception ex)
            {
                throw new Exception(
                    $"EXEC_SP_GET_PAYMENT_NUMBER: Error inesperado al crear la orden: {ex.Message}",
                    ex);
            }
        }

        public static void TRANSACTION_SAVE_PAYMENT_HISTORY(string connectionString, SavePaymentHistory data)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {

                // insertamos el encabezado de un pago
                long paymentHistoryId = EXEC_SP_INSERT_PAYMENT_HISTORY(connection, transaction, data.Payment);

                // insertamos el registro de entrada de dinero
                data.Inflow.PaymentHistoryId = paymentHistoryId;
                EXEC_SP_INSERT_PAYMENT_HISTORY_DETAILS(connection, transaction, data.Inflow);

                // insertamos el registro de salida de dinero
                if(data.Outflow != null)
                {
                    data.Outflow.PaymentHistoryId = paymentHistoryId;
                    EXEC_SP_INSERT_PAYMENT_HISTORY_DETAILS(connection, transaction, data.Outflow);
                }

                decimal newAmountPaid = data.Order.AmountPaid + data.Payment.PaidAmount;
                decimal newAmountPending = data.Order.AmountPending - data.Payment.PaidAmount;
                data.Order.AmountPaid = newAmountPaid;
                data.Order.AmountPending = newAmountPending;
                data.Order.FinancialState = data.Order.AmountPending == 0 ? 3 : 2;

                OrderMiddles.EXEC_SP_UPDATE_ORDER(connection, transaction, data.Order);

                string fullLogMessage = Utils.Utils.BuildPaymentHistoryDescription(data);
                EXEC_SP_INSERT_PAYMENT_LOGS(connection, transaction, fullLogMessage, data.Payment.UserReceived);

                transaction.Commit();
            }
            catch (SqlException ex)
            {
                transaction.Rollback();

                throw new Exception(
                    $"TRANSACTION_SAVE_PAYMENT_HISTORY: Error de SQL Server al crear la orden. " +
                    $"Procedimiento: {ex.Procedure}. " +
                    $"Línea: {ex.LineNumber}. " +
                    $"Error SQL #{ex.Number}: {ex.Message}",
                    ex);
            }
            catch (InvalidCastException ex)
            {
                transaction.Rollback();

                throw new Exception(
                    "TRANSACTION_SAVE_PAYMENT_HISTORY: El tipo de dato devuelto por el procedimiento " +
                    "SP_INSERT_NEW_ORDER no coincide con el esperado. " +
                    "Verifica si @InsertedID está declarado como INT o BIGINT en SQL Server.",
                    ex);
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw new Exception(
                    $"TRANSACTION_SAVE_PAYMENT_HISTORY: Error inesperado al crear la orden: {ex.Message}",
                    ex);
            }
        }

        public static long EXEC_SP_INSERT_PAYMENT_HISTORY(SqlConnection connection, SqlTransaction transaction, PaymentHistory history)
        {
            SqlParameter outputParam = new("@InsertedID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            using SqlCommand cmd = new("SP_INSERT_PAYMENT_HISTORY", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@PaidAmount", SqlDbType.Decimal).Value = history.PaidAmount;
            cmd.Parameters.Add("@CashReceived", SqlDbType.Decimal).Value = history.CashReceived;
            cmd.Parameters.Add("@ChancheMoney", SqlDbType.Decimal).Value = history.ChancheMoney;
            cmd.Parameters.Add("@PaidDate", SqlDbType.DateTime).Value = history.PaidDate;
            cmd.Parameters.Add("@PaymentNumber", SqlDbType.SmallInt).Value = history.PaymentNumber;
            cmd.Parameters.Add("@Observations", SqlDbType.DateTime).Value = string.IsNullOrEmpty(history.Observations) ? DBNull.Value : history.Observations;
            cmd.Parameters.Add("@OrderId", SqlDbType.BigInt).Value = history.OrderId;
            cmd.Parameters.Add("@UserReceived", SqlDbType.BigInt).Value = history.UserReceived;
            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();

            return (long)outputParam.Value;
        }

        public static void EXEC_SP_INSERT_PAYMENT_HISTORY_DETAILS(SqlConnection connection, SqlTransaction transaction, PaymentHistoryDetails detail)
        {
            using SqlCommand cmd = new("SP_INSERT_PAYMENT_HISTORY_DETAILS", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@PaymentHistoryId", SqlDbType.BigInt).Value = detail.PaymentHistoryId;
            cmd.Parameters.Add("@TypeMovement", SqlDbType.TinyInt).Value = detail.TypeMovement;
            cmd.Parameters.Add("@NumberOneBills", SqlDbType.Int).Value = detail.NumberOneBills;
            cmd.Parameters.Add("@NumberFiveBills", SqlDbType.Int).Value = detail.NumberFiveBills;
            cmd.Parameters.Add("@NumberTenBills", SqlDbType.Int).Value = detail.NumberTenBills;
            cmd.Parameters.Add("@NumberTwentyBills", SqlDbType.Int).Value = detail.NumberTwentyBills;
            cmd.Parameters.Add("@NumberFiftyBills", SqlDbType.Int).Value = detail.NumberFiftyBills;
            cmd.Parameters.Add("@NumberOneHundredBills", SqlDbType.Int).Value = detail.NumberOneHundredBills;
            cmd.Parameters.Add("@TwoHundredBills", SqlDbType.Int).Value = detail.TwoHundredBills;

            cmd.ExecuteNonQuery();
        }

        public static void EXEC_SP_INSERT_PAYMENT_LOGS(SqlConnection connection, SqlTransaction transaction, string action, long instructorId)
        {
            using SqlCommand cmd = new("SP_INSERT_PAYMENT_LOGS", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Action", SqlDbType.NVarChar, -1).Value = action;
            cmd.Parameters.Add("@ActionUserId", SqlDbType.BigInt).Value = instructorId;

            cmd.ExecuteNonQuery();
        }
    }
}
