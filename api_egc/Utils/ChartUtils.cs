using api_egc.Models;
using System.Data;
using System.Data.SqlClient;

namespace api_egc.Utils
{
    public class ChartUtils
    {
        public static List<AttendanceChartDTO> EXEC_SP_GET_DATA_FROM_ATTENDANCE_CHART(string connectionString, long eventId, 
            long squadId)
        {
            List<AttendanceChartDTO> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_GET_DATA_FROM_ATTENDANCE_CHART", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EVEId", SqlDbType.BigInt).Value = eventId;
                    cmd.Parameters.Add("@ESCIdEscuadra", SqlDbType.BigInt).Value = squadId;

                    using SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        AttendanceChartDTO attendance = new()
                        {
                            ESCIdEscuadra = Utils.GetValue<long>(reader, "ESCIdEscuadra"),
                            ESCNombre = Utils.GetValue<string>(reader, "ESCNombre"),
                            TotalIntegrantes = Utils.GetValue<int>(reader, "TotalIntegrantes"),
                            Asistencias = Utils.GetValue<int>(reader, "Asistencias"),
                            Faltan = Utils.GetValue<int>(reader, "Faltan"),
                            EventoId = Utils.GetValue<long>(reader, "EventoId"),
                            NombreEvento = Utils.GetValue<string>(reader, "NombreEvento"),
                        };
                        list.Add(attendance);
                    }
                }
            }
            return list;
        }
    }
}
