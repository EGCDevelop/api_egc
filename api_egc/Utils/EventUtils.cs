using System.Data;
using System.Data.SqlClient;
using api_egc.Models;

namespace api_egc.Utils
{
    public class EventUtils
    {
        public static int EXEC_SP_INSERTAREVENTO(string connectionString, string title, string description, DateTime eventDate,
            TimeSpan commandersEntry, bool onlyCommanders, TimeSpan membersEntry, string userCreate, bool generalBand)
        {
            using SqlConnection conn = new(connectionString);
            using SqlCommand cmd = new("sp_InsertarEvento", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            // Agregar parámetros de entrada
            cmd.Parameters.AddWithValue("@EVETitulo", title);
            cmd.Parameters.AddWithValue("@EVEDescripcion", description);
            cmd.Parameters.AddWithValue("@EVEFechaEvento", eventDate);
            cmd.Parameters.AddWithValue("@EVEHoraEntradaComandantes", commandersEntry);
            cmd.Parameters.AddWithValue("@EVESoloComandantes", onlyCommanders ? 1 : 0); // Convertir bool a bit
            cmd.Parameters.AddWithValue("@EVEHoraEntradaIntegrantes", membersEntry);
            cmd.Parameters.AddWithValue("@EVEUsuarioCreacion", userCreate);
            cmd.Parameters.AddWithValue("@EVEFechaCreacion", DateTime.Now); // Fecha de creación automática
            cmd.Parameters.AddWithValue("@EVEBandaGeneral", generalBand ? 1 : 0);

            // parametro de salida
            SqlParameter outputParam = new("@InsertedId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output,
            };

            cmd.Parameters.Add(outputParam);

            conn.Open();
            cmd.ExecuteNonQuery();

            return (int)outputParam.Value;
        }

        public static void EXEC_SP_INSERT_DETALLE_ASISTENCIA(string connectionString, long eventId, long squadId)
        {
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_INSERT_DETALLE_ASISTENCIA", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EDEVEId", SqlDbType.BigInt).Value = eventId;
                    cmd.Parameters.Add("@EDESCIdEscuadra", SqlDbType.BigInt).Value = squadId;

                    // Ejecutar el procedimiento
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Event> EXEC_SP_GET_EVENTS_BY_ID_SQUAD(string connectionString, long idSquad, DateTime fechaInicio,
            DateTime fechaFin )
        {
            List<Event> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_GET_EVENTS_BY_ID_SQUAD", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EscuadraId", SqlDbType.BigInt).Value = idSquad;
                    cmd.Parameters.Add("@FechaInicio", SqlDbType.DateTime).Value = fechaInicio;
                    cmd.Parameters.Add("@FechaFin", SqlDbType.DateTime).Value = fechaFin;



                    using SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Event e = new()
                        {
                            EVEId = Utils.GetValue<long>(reader, "EVEId"),
                            EVETitulo = Utils.GetValue<string>(reader, "EVETitulo"),
                            EVEDescripcion = Utils.GetValue<string>(reader, "EVEDescripcion"),
                            EVEFechaEvento = Utils.GetValue<DateTime>(reader, "EVEFechaEvento"),
                            EVEHoraEntradaComandantes = Utils.GetValue<TimeSpan>(reader, "EVEHoraEntradaComandantes"),
                            EVESoloComandantes = Utils.GetValue<int>(reader, "EVESoloComandantes"),
                            EVEHoraEntradaIntegrantes = Utils.GetValueNull<TimeSpan>(reader, "EVEHoraEntradaIntegrantes"),
                            EVEUsuarioCreacion = Utils.GetValue<string>(reader, "EVEUsuarioCreacion"),
                            EVEFechaCreacion = Utils.GetValue<DateTime>(reader, "EVEFechaCreacion"),
                            EVEUsuarioModificacion = Utils.GetValueNull<string>(reader, "EVEUsuarioModificacion"),
                            EVEFechaModificacon = Utils.GetValueNull<DateTime>(reader, "EVEFechaModificacon"),
                            EVEBandaGeneral = Utils.GetValue<int>(reader, "EVEBandaGeneral"),
                        };
                        list.Add(e);
                    }
                }
            }
            return list;
        }


        public static void EXEC_SP_DELETE_EVENTO(string connectionString, long eventId)
        {
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_DELETE_EVENTO", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EventId", SqlDbType.BigInt).Value = eventId;

                    // Ejecutar el procedimiento
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void EXEC_SP_DELETE_DETALLE_EVENTO(string connectionString, long eventId)
        {
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_DELETE_DETALLE_EVENTO", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EventId", SqlDbType.BigInt).Value = eventId;

                    // Ejecutar el procedimiento
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
