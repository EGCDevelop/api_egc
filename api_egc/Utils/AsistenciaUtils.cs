using api_egc.Models;
using System.Data;
using System.Data.SqlClient;

namespace api_egc.Utils
{
    public class AsistenciaUtils
    {

        public static IntegranteDto EXEC_SP_GET_INTEGRANTE_BY_ID(string connectionString, long idIntegrante)
        {
            IntegranteDto integrante = null;

            using(SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using(SqlCommand cmd = new("SP_GET_INTEGRANTE_BY_ID", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = idIntegrante;

                    using SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        integrante = new()
                        {
                            INTIdIntegrante = Utils.GetValue<long>(reader, "INTIdIntegrante"),
                            INTNombres = Utils.GetValue<string>(reader, "INTNombres"),
                            INTApellidos = Utils.GetValue<string>(reader, "INTApellidos"),
                            INTEdad = Utils.GetValue<int>(reader, "INTEdad"),
                            INTTelefono = Utils.GetValue<string>(reader, "INTTelefono"),
                            INTESTIdEstablecimiento = Utils.GetValue<long>(reader, "INTESTIdEstablecimiento"),
                            INTEstablecimientoNombre = Utils.GetValue<string>(reader, "INTEstablecimientoNombre"),
                            INTCARIdCarrera = Utils.GetValue<long>(reader, "INTCARIdCarrera"),
                            INTCarreraNombre = Utils.GetValue<string>(reader, "INTCarreraNombre"),
                            INTGRAIdGrado = Utils.GetValue<long>(reader, "INTGRAIdGrado"),
                            INTGradoNombre = Utils.GetValue<string>(reader, "INTGradoNombre"),
                            INTSeccion = Utils.GetValue<string>(reader, "INTSeccion"),
                            INTESCIdEscuadra = Utils.GetValue<long>(reader, "INTESCIdEscuadra"),
                            INTEsNuevo = Utils.GetValue<int>(reader, "INTEsNuevo"),
                            INTNombreEncargado = Utils.GetValue<string>(reader, "INTNombreEncargado"),
                            INTTelefonoEncargado = Utils.GetValue<string>(reader, "INTTelefonoEncargado"),
                            INTEstadoIntegrante = Utils.GetValue<int>(reader, "INTEstadoIntegrante"),
                            INTPUIdPuesto = Utils.GetValue<long>(reader, "INTPUIdPuesto")
                        };
                    }
                }
            }
            return integrante!;
        }


        public static void EXEC_SP_INSERT_ASISTENCIA(string connectionString, long id, long eventId, long idRegister)
        {
            DateTime date = Utils.getCurrentDateGMT6();

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new("SP_INSERT_ASISTENCIA", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@IdIntegrante", SqlDbType.BigInt).Value = id;
                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = date;
                    cmd.Parameters.Add("@EventId", SqlDbType.BigInt).Value = eventId;
                    cmd.Parameters.Add("@IdRegistro", SqlDbType.BigInt).Value = idRegister;
                    cmd.Parameters.Add("@Extraordinaria", SqlDbType.Bit).Value = 0;

                    // Ejecutar el procedimiento
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool EXEC_SP_GET_ASISTENCIA_BY_INTEGRANTE(string connectionString, long id, long eventId)
        {
            DateTime date = Utils.getCurrentDateGMT6();
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlCommand cmd = new("SP_GET_ASISTENCIA_BY_INTEGRANTE", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;
            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = date;
            cmd.Parameters.Add("@eventId", SqlDbType.BigInt).Value = eventId;

            // Ejecutar el procedimiento y obtener el resultado
            object result = cmd.ExecuteScalar();

            // Si `result` no es null, significa que existe asistencia
            return result != null;
        }


        public static List<AsistenciaDto> EXEC_SP_GET_ASISTENCIA(string connectionString, long IdEscruadra, DateTime date, long EventId)
        {
            List<AsistenciaDto> list = [];

            using(SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using(SqlCommand cmd = new("SP_GET_ASISTENCIA", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@IdEscruadra", SqlDbType.BigInt).Value = IdEscruadra;
                    cmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;
                    cmd.Parameters.Add("@EventId", SqlDbType.BigInt).Value = EventId;

                    using SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        AsistenciaDto asistencia = new()
                        {
                            INTIdIntegrante = Utils.GetValue<long>(reader, "INTIdIntegrante"),
                            INTNombres = Utils.GetValue<string>(reader, "INTNombres"),
                            INTApellidos = Utils.GetValue<string>(reader, "INTApellidos"),
                            ASIIdAsistencia = Utils.GetValueNull<long?>(reader, "ASIIdAsistencia"),
                            ASIFechaAsistencia = Utils.GetValueNull<DateTime?>(reader, "ASIFechaAsistencia"),
                            Asistencia = Utils.GetValue<int>(reader, "Asistencia"),
                            ASIEVEId = Utils.GetValue<long>(reader, "ASIEVEId"),
                            ASIINTIdIntegranteRegistro = Utils.GetValue<long>(reader, "ASIINTIdIntegranteRegistro"),
                            ASIEsExtraordinaria = Utils.GetValue<int>(reader, "ASIEsExtraordinaria"),
                            ASIComentario = Utils.GetValueNull<string>(reader, "ASIComentario"),
                            ASIFechaRegistroExtraordinaria = Utils.GetValueNull<DateTime?>(reader, "ASIFechaRegistroExtraordinaria"),
                        };

                        list.Add(asistencia);
                    }

                }
            }
            return list;
        }

        public static bool EXEC_SP_VALIDATE_EVENT_EXIST(string connectionString, long squadId, long eventId)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlCommand cmd = new("SP_VALIDATE_EVENT_EXIST", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@SquadId", SqlDbType.BigInt).Value = squadId;
            cmd.Parameters.Add("@EventId", SqlDbType.BigInt).Value = eventId;

            // Ejecutamos el procedimiento almacenado y obtenemos el resultado
            object result = cmd.ExecuteScalar();

            // Si `result` es 1, el evento existe; si es 0, no existe
            return result != null && Convert.ToInt32(result) == 1;
        }
    }
}
