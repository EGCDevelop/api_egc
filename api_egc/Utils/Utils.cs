using api_egc.Models.Uniforms;
using api_egc.Models.Uniforms.DTO;
using Microsoft.Data.SqlClient;
using System.Text;

namespace api_egc.Utils
{
    public class Utils
    {
        public static T GetValue<T>(SqlDataReader reader, string columnName) 
        {
            var value = reader[columnName];

            if(value == DBNull.Value)
            {
                return default(T)!;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T GetValueNull<T>(SqlDataReader reader, string columnName)
        {
            var value = reader[columnName];

            // Si el valor es DBNull, devolvemos `default(T)`
            if (value == DBNull.Value)
            {
                return default;
            }

            // Si es un tipo nullable, convertimos el valor a su tipo subyacente
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
            {
                return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)));
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static String UsernameGenerate(string name, string lastName)
        {
            string cleanName = name.Trim().ToLower();
            string cleanLastName = lastName.Trim().ToLower();

            string[] names = cleanName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] lastNames = cleanLastName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string firstInitial = names.Length > 0 ? names[0][0].ToString() : "";
            string firstLastName = lastNames.Length > 0 ? lastNames[0] : "";
            string secondLastNameInitial = lastNames.Length > 1 ? lastNames[1][0].ToString() : "";

            return $"{firstInitial}{firstLastName}{secondLastNameInitial}";
        }

        public static DateTime getCurrentDateGMT6()
        {
            //DateTime utcNow = DateTime.UtcNow;
            //TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            //return TimeZoneInfo.ConvertTimeFromUtc(utcNow, myZone);
            return DateTime.UtcNow.AddHours(-6);
        }

        public static string BuildPaymentHistoryDescription(SavePaymentHistory data)
        {
            ArgumentNullException.ThrowIfNull(data);
            ArgumentNullException.ThrowIfNull(data.Payment);
            ArgumentNullException.ThrowIfNull(data.Inflow);

            var payment = data.Payment;
            var builder = new StringBuilder();

            builder.Append($"El usuario {payment.UserReceived} recibió un pago ");
            builder.Append($"por Q{payment.PaidAmount:N2} ");
            builder.Append($"para el pedido #{payment.OrderId}. ");

            builder.Append($"Efectivo recibido: Q{payment.CashReceived:N2}. ");

            if (payment.ChancheMoney > 0)
            {
                builder.Append($"Vuelto entregado: Q{payment.ChancheMoney:N2}. ");
            }
            else
            {
                builder.Append("No se entregó vuelto. ");
            }

            builder.Append($"Pago número {payment.PaymentNumber}. ");

            if (!string.IsNullOrWhiteSpace(payment.Observations))
            {
                builder.Append($"Observaciones: {payment.Observations}. ");
            }

            builder.Append("Billetes recibidos: ");
            builder.Append(BuildBillsDescription(data.Inflow));
            builder.Append(". ");

            if (data.Outflow is not null)
            {
                builder.Append("Billetes devueltos como vuelto: ");
                builder.Append(BuildBillsDescription(data.Outflow));
                builder.Append(". ");
            }
            else
            {
                builder.Append("No se registraron billetes devueltos como vuelto.");
            }

            return builder.ToString();
        }


        private static string BuildBillsDescription(PaymentHistoryDetails details)
        {
            var bills = new List<string>();

            AddBillDescription(bills, details.NumberOneBills, "Q1");
            AddBillDescription(bills, details.NumberFiveBills, "Q5");
            AddBillDescription(bills, details.NumberTenBills, "Q10");
            AddBillDescription(bills, details.NumberTwentyBills, "Q20");
            AddBillDescription(bills, details.NumberFiftyBills, "Q50");
            AddBillDescription(bills, details.NumberOneHundredBills, "Q100");
            AddBillDescription(bills, details.TwoHundredBills, "Q200");

            return bills.Count > 0
                ? string.Join(", ", bills)
                : "ninguno";
        }

        private static void AddBillDescription(
    List<string> bills,
    int quantity,
    string denomination)
        {
            if (quantity > 0)
            {
                bills.Add($"{quantity} billete(s) de {denomination}");
            }
        }

    }
}
