using System.Data;
using System.Data.SqlClient;
using api_egc.Models;

namespace api_egc.Utils
{
    public class GeneralMethodsUtils
    {
        public static List<Escuadras> EXEC_SP_GET_ESCUADRA(string connectionString)
        {
            List<Escuadras> list = [];

            using(SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_ESCUADRA", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Escuadras escuadras = new()
                    {
                        ESCIdEscuadra = Utils.GetValue<long>(reader, "ESCIdEscuadra"),
                        ESCNombre = Utils.GetValue<string>(reader, "ESCNombre")
                    };

                    list.Add(escuadras);
                }
            }

            return list;
        }

        public static List<IntegrantePerYearDto> EXEC_SP_GET_BY_INSERT_PER_YEAR(string connectionString)
        {
            List<IntegrantePerYearDto> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_BY_INSERT_PER_YEAR", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IntegrantePerYearDto integrantePerYearDto = new()
                    {
                        INTIdIntegrante = Utils.GetValue<long>(reader, "INTIdIntegrante"),
                        INTESCIdEscuadra = Utils.GetValue<long>(reader, "INTESCIdEscuadra"),
                        INTPUIdPuesto = Utils.GetValue<long>(reader, "INTPUIdPuesto")
                    };

                    list.Add(integrantePerYearDto);
                }
            }

            return list;
        }

        public static void EXEC_SP_INSERT_MEMEBER_PER_YEAR(string connectionString, long IPAINTIdIntegrante, 
            long IPAESCIdEscuadra, long IPAPUIdPuesto)
        {
            int year =DateTime.Now.Year;

            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlCommand cmd = new("SP_INSERT_MEMEBER_PER_YEAR", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@IPAINTIdIntegrante", SqlDbType.BigInt).Value = IPAINTIdIntegrante;
            cmd.Parameters.Add("@IPAESCIdEscuadra", SqlDbType.BigInt).Value = IPAESCIdEscuadra;
            cmd.Parameters.Add("@IPAPUIdPuesto", SqlDbType.BigInt).Value = IPAPUIdPuesto;
            cmd.Parameters.Add("@IPAAnio", SqlDbType.SmallInt).Value = year;

            // Ejecutar el procedimiento
            cmd.ExecuteNonQuery();
        }
    }
}
