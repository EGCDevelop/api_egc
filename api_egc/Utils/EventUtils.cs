using System.Data;
using System.Data.SqlClient;
using api_egc.Models;

namespace api_egc.Utils
{
    public class EventUtils
    {
        public static int EXEC_SP_INSERTAREVENTO(string connectionString, string title, string description, DateTime eventDate,
            TimeSpan commandersEntry, bool onlyCommanders, TimeSpan membersEntry, string userCreate, bool generalBand,
            int idEstado)
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
            cmd.Parameters.AddWithValue("@EVEFechaCreacion", Utils.getCurrentDateGMT6()); // Fecha de creación automática
            cmd.Parameters.AddWithValue("@EVEBandaGeneral", generalBand ? 1 : 0);
            cmd.Parameters.AddWithValue("@EVEIdEstado", idEstado);

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
            DateTime fechaFin, int idEstado )
        {
            List<Event> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_GET_EVENTS_BY_ID_SQUAD", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EscuadraId", SqlDbType.BigInt).Value = idSquad == 11 ? 0 : idSquad;
                    cmd.Parameters.Add("@FechaInicio", SqlDbType.DateTime).Value = fechaInicio;
                    cmd.Parameters.Add("@FechaFin", SqlDbType.DateTime).Value = fechaFin;
                    cmd.Parameters.Add("@IdEstado", SqlDbType.Int).Value = idEstado;

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
                            EVEIdEstado = Utils.GetValue<int>(reader, "EVEIdEstado"),
                            ListadoEscuadras = Utils.GetValueNull<string>(reader, "ListadoEscuadras"),
                            EVEActivo = Utils.GetValueNull<int>(reader, "EVEActivo"),
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

        public static List<Escuadras> EXEC_SP_GET_ESCUADRAS_BY_EVENT(string connectionString, long id)
        {
            List<Escuadras> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_ESCUADRAS_BY_EVENT", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

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

        public static void TRANSACTION_END_EVENT(string connectionString, int eventId, string username, 
            DateTime endDate, string commentExit)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                EXEC_SP_UPDATE_END_EVENTO(connection, transaction, eventId, endDate, username);
                EXEC_SP_UPDATE_END_ATTENDANCE(connection, transaction, eventId, endDate, username, commentExit);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Fallo en la actualización integral del instructor: " + ex.Message);
            }

        }

        public static void EXEC_SP_UPDATE_END_EVENTO(SqlConnection connection, SqlTransaction transaction,
            long eventId, DateTime endDate, string username)
        {
            using SqlCommand cmd = new("SP_UPDATE_END_EVENTO", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@eventId", SqlDbType.BigInt).Value = eventId;
            cmd.Parameters.Add("@endDate", SqlDbType.DateTime).Value = endDate;
            cmd.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;

            cmd.ExecuteNonQuery();
        }

        public static void EXEC_SP_UPDATE_END_ATTENDANCE(SqlConnection connection, SqlTransaction transaction,
            long eventId, DateTime endDate, string username, string commentExit)
        {
            using SqlCommand cmd = new("SP_UPDATE_END_ATTENDANCE", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@eventId", SqlDbType.BigInt).Value = eventId;
            cmd.Parameters.Add("@endDate", SqlDbType.DateTime).Value = endDate;
            cmd.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
            cmd.Parameters.Add("@commentExit", SqlDbType.NVarChar, 1000).Value = commentExit;

            cmd.ExecuteNonQuery();
        }
    }
}
