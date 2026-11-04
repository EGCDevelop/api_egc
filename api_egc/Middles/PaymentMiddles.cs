using Microsoft.Data.SqlClient;

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

                return 1;
            } 
            catch(SqlException ex)
            {
                throw new Exception(
                    $"EXEC_SP_GET_PAYMENT_NUMBER: Error de SQL Server al crear la orden. " +
                    $"Procedimiento: {ex.Procedure}. " +
                    $"Línea: {ex.LineNumber}. " +
                    $"Error SQL #{ex.Number}: {ex.Message}",
                    ex);
            }
            catch(Exception ex)
            {
                throw new Exception(
                    $"EXEC_SP_GET_PAYMENT_NUMBER: Error inesperado al crear la orden: {ex.Message}",
                    ex);
            }
        }
    }
}
