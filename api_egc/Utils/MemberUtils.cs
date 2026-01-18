using System.Data;
using System.Data.SqlClient;
using api_egc.Models;

namespace api_egc.Utils
{
    public class MemberUtils
    {
        public static List<MemberDTO> EXEC_SP_GET_INTEGRANTE_LIKE(string connectionString, int year, string like,
            long squadId, long schoolId, int isNew, int memberState)
        {
            List<MemberDTO> list = [];

            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new("SP_GET_INTEGRANTE_LIKE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Anio", SqlDbType.Int).Value = year;
                    cmd.Parameters.Add("@Like", SqlDbType.NVarChar).Value = like;
                    cmd.Parameters.Add("@IdEscuadra", SqlDbType.BigInt).Value = squadId;
                    cmd.Parameters.Add("@IdEstablecimiento", SqlDbType.BigInt).Value = schoolId;
                    cmd.Parameters.Add("@EsNuevo", SqlDbType.Int).Value = isNew;
                    cmd.Parameters.Add("@EstadoIntegrante", SqlDbType.Int).Value = memberState;

                    using SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        MemberDTO member = new() {
                            INTIdIntegrante = Utils.GetValue<long>(reader, "INTIdIntegrante"),
                            INTNombres = Utils.GetValue<string>(reader, "INTNombres"),
                            INTApellidos = Utils.GetValue<string>(reader, "INTApellidos"),
                            INTEdad = 1,
                            INTTelefono = Utils.GetValueNull<string>(reader, "INTTelefono"),
                            INTESTIdEstablecimiento = Utils.GetValue<long>(reader, "INTESTIdEstablecimiento"),
                            ESTNombreEstablecimiento = Utils.GetValue<string>(reader, "ESTNombreEstablecimiento"),
                            INTEstablecimientoNombre = Utils.GetValueNull<string>(reader, "INTEstablecimientoNombre"),
                            INTCARIdCarrera = Utils.GetValue<long>(reader, "INTCARIdCarrera"),
                            CARNombreCarrera = Utils.GetValue<string>(reader, "CARNombreCarrera"),
                            INTCarreraNombre = Utils.GetValueNull<string>(reader, "INTCarreraNombre"),
                            INTGRAIdGrado = Utils.GetValue<long>(reader, "INTGRAIdGrado"),
                            GRANombreGrado = Utils.GetValue<string>(reader, "GRANombreGrado"),
                            INTGradoNombre = Utils.GetValueNull<string>(reader, "INTGradoNombre"),
                            INTSeccion = Utils.GetValue<string>(reader, "INTSeccion"),
                            INTESCIdEscuadra = Utils.GetValue<long>(reader, "INTESCIdEscuadra"),
                            ESCNombre = Utils.GetValue<string>(reader, "ESCNombre"),
                            INTEsNuevo = Utils.GetValue<int>(reader, "INTEsNuevo"),
                            INTNombreEncargado = Utils.GetValueNull<string>(reader, "INTNombreEncargado"),
                            INTTelefonoEncargado = Utils.GetValueNull<string>(reader, "INTTelefonoEncargado"),
                            INTEstadoIntegrante = Utils.GetValue<int>(reader, "INTEstadoIntegrante"),
                            INTPUIdPuesto = Utils.GetValue<long>(reader, "INTPUIdPuesto"),
                            PUNombre = Utils.GetValue<string>(reader, "PUNombre"),
                            INTUsuario = Utils.GetValueNull<string>(reader, "INTUsuario")
                        };

                        list.Add(member);
                    }
                }
            }
            return list;
        }

        public static List<MemberDTO> EXEC_SP_GET_INTEGRANTE_FOR_INSTRUCTOR(string connectionString, int year, string like,
            string? squadId, long? schoolId, int? isNew, int? memberState, int? career, int? positionId)
        {
            List<MemberDTO> list = [];

            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();

                using SqlCommand cmd = new("SP_GET_INTEGRANTE_FOR_INSTRUCTOR", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Anio", SqlDbType.Int).Value = year;
                cmd.Parameters.Add("@Like", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(like) ? DBNull.Value : like;
                cmd.Parameters.Add("@IdEscuadra", SqlDbType.NVarChar, -1).Value = (object)squadId ?? DBNull.Value;
                cmd.Parameters.Add("@IdEstablecimiento", SqlDbType.BigInt).Value = schoolId;
                cmd.Parameters.Add("@EsNuevo", SqlDbType.Int).Value = isNew == 3 ? 0 : isNew == 0 ? 2 : isNew;
                cmd.Parameters.Add("@EstadoIntegrante", SqlDbType.Int).Value = memberState;
                cmd.Parameters.Add("@IdCarrera", SqlDbType.Int).Value = career;
                cmd.Parameters.Add("@PositionId", SqlDbType.Int).Value = positionId;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    MemberDTO member = new()
                    {
                        INTIdIntegrante = Utils.GetValue<long>(reader, "INTIdIntegrante"),
                        INTNombres = Utils.GetValue<string>(reader, "INTNombres"),
                        INTApellidos = Utils.GetValue<string>(reader, "INTApellidos"),
                        INTEdad = Utils.GetValue<int>(reader, "INTEdad"),
                        INTTelefono = Utils.GetValueNull<string>(reader, "INTTelefono"),
                        INTESTIdEstablecimiento = Utils.GetValue<long>(reader, "INTESTIdEstablecimiento"),
                        ESTNombreEstablecimiento = Utils.GetValue<string>(reader, "ESTNombreEstablecimiento"),
                        INTEstablecimientoNombre = Utils.GetValueNull<string>(reader, "INTEstablecimientoNombre"),
                        INTCARIdCarrera = Utils.GetValue<long>(reader, "INTCARIdCarrera"),
                        CARNombreCarrera = Utils.GetValue<string>(reader, "CARNombreCarrera"),
                        INTCarreraNombre = Utils.GetValueNull<string>(reader, "INTCarreraNombre"),
                        INTGRAIdGrado = Utils.GetValue<long>(reader, "INTGRAIdGrado"),
                        GRANombreGrado = Utils.GetValue<string>(reader, "GRANombreGrado"),
                        INTGradoNombre = Utils.GetValueNull<string>(reader, "INTGradoNombre"),
                        INTSeccion = Utils.GetValue<string>(reader, "INTSeccion"),
                        INTESCIdEscuadra = Utils.GetValue<long>(reader, "INTESCIdEscuadra"),
                        ESCNombre = Utils.GetValue<string>(reader, "ESCNombre"),
                        INTEsNuevo = Utils.GetValue<int>(reader, "INTEsNuevo"),
                        INTNombreEncargado = Utils.GetValueNull<string>(reader, "INTNombreEncargado"),
                        INTTelefonoEncargado = Utils.GetValueNull<string>(reader, "INTTelefonoEncargado"),
                        INTEstadoIntegrante = Utils.GetValue<int>(reader, "INTEstadoIntegrante"),
                        INTPUIdPuesto = Utils.GetValue<long>(reader, "INTPUIdPuesto"),
                        PUNombre = Utils.GetValue<string>(reader, "PUNombre"),
                        INTUsuario = Utils.GetValue<string>(reader, "INTUsuario")
                    };

                    list.Add(member);
                }
            }
            return list;
        }


        public static void EXEC_SP_UPDATE_MEMBER(string connectionString, long id, string firstName, string lastName, string cellPhone,
            long squadId, long positionId, long isActive, long isAncient, long establecimientoId, string anotherEstablishment,
            long courseId, string courseName, long degreeId, string section, string fatherName, string fatherCell,
            int age, string username, string password)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlCommand cmd = new("SP_UPDATE_MEMBER", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@INTIdIntegrante", SqlDbType.BigInt).Value = id;
            cmd.Parameters.Add("@INTNombres", SqlDbType.NVarChar).Value = firstName;
            cmd.Parameters.Add("@INTApellidos", SqlDbType.NVarChar).Value = lastName;
            cmd.Parameters.Add("@Age", SqlDbType.Int).Value = age;
            cmd.Parameters.Add("@INTTelefono", SqlDbType.VarChar).Value = cellPhone;
            cmd.Parameters.Add("@INTESCIdEscuadra", SqlDbType.BigInt).Value = squadId;
            cmd.Parameters.Add("@INTPUIdPuesto", SqlDbType.BigInt).Value = positionId;
            cmd.Parameters.Add("@INTEstadoIntegrante", SqlDbType.Bit).Value = isActive;
            cmd.Parameters.Add("@INTEsNuevo", SqlDbType.Bit).Value = isAncient;

            cmd.Parameters.Add("@INTESTIdEstablecimiento", SqlDbType.BigInt).Value = establecimientoId;
            cmd.Parameters.Add("@INTEstablecimientoNombre", SqlDbType.NVarChar).Value = anotherEstablishment;

            cmd.Parameters.Add("@INTCARIdCarrera", SqlDbType.BigInt).Value = courseId;
            cmd.Parameters.Add("@INTCarreraNombre", SqlDbType.NVarChar).Value = courseName;
            cmd.Parameters.Add("@INTGRAIdGrado", SqlDbType.BigInt).Value = degreeId;
            cmd.Parameters.Add("@INTSeccion", SqlDbType.NVarChar).Value = section;
            cmd.Parameters.Add("@INTNombreEncargado", SqlDbType.NVarChar).Value = fatherName;
            cmd.Parameters.Add("@INTTelefonoEncargado", SqlDbType.VarChar).Value = fatherCell;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = String.IsNullOrEmpty(username) ? DBNull.Value : username;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 100).Value = String.IsNullOrEmpty(password) ? DBNull.Value : password;

            // Ejecutar el procedimiento
            cmd.ExecuteNonQuery();
        }

        public static void EXEC_SP_INSERT_MEMBER(string connectionString, string firstName, string lastName, int years,
            string cellPhone, long establecimientoId, string? anotherEstablishment, long courseId, string? courseName,
            long degreeId, string? degreeName, string section, long squadId, long positionId,
            bool isAncient, string? fatherName, string? fatherCell, bool isActive, string? username)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlCommand cmd = new("SP_INSERT_MEMBER", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@INTNombres", SqlDbType.NVarChar).Value = firstName;
            cmd.Parameters.Add("@INTApellidos", SqlDbType.NVarChar).Value = lastName;
            cmd.Parameters.Add("@INTEdad", SqlDbType.Int).Value = years;

            cmd.Parameters.Add("@INTTelefono", SqlDbType.VarChar).Value = cellPhone;
            cmd.Parameters.Add("@INTESTIdEstablecimiento", SqlDbType.BigInt).Value = establecimientoId;
            cmd.Parameters.Add("@INTEstablecimientoNombre", SqlDbType.NVarChar).Value = (object?)anotherEstablishment ?? DBNull.Value;

            cmd.Parameters.Add("@INTCARIdCarrera", SqlDbType.BigInt).Value = courseId;
            cmd.Parameters.Add("@INTCarreraNombre", SqlDbType.NVarChar).Value = (object?)courseName ?? DBNull.Value;

            cmd.Parameters.Add("@INTGRAIdGrado", SqlDbType.BigInt).Value = degreeId;
            cmd.Parameters.Add("@INTGradoNombre", SqlDbType.NVarChar).Value = (object?)degreeName ?? DBNull.Value;

            cmd.Parameters.Add("@INTSeccion", SqlDbType.VarChar).Value = section;
            cmd.Parameters.Add("@INTESCIdEscuadra", SqlDbType.BigInt).Value = squadId;
            cmd.Parameters.Add("@INTEsNuevo", SqlDbType.Bit).Value = isAncient;

            cmd.Parameters.Add("@INTNombreEncargado", SqlDbType.NVarChar).Value = (object?)fatherName ?? DBNull.Value;
            cmd.Parameters.Add("@INTTelefonoEncargado", SqlDbType.VarChar).Value = (object?)fatherCell ?? DBNull.Value;

            cmd.Parameters.Add("@INTEstadoIntegrante", SqlDbType.Bit).Value = isActive;
            cmd.Parameters.Add("@INTPUIdPuesto", SqlDbType.BigInt).Value = positionId;
            cmd.Parameters.Add("@INTUsuario", SqlDbType.VarChar).Value = (object?)username ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }

        public static void EXEC_SP_UPDATE_MEMBER_STATE(string connectionString, long memberId, int state, string comment)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlCommand cmd = new("SP_UPDATE_MEMBER_STATE", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@INTIdIntegrante", SqlDbType.BigInt).Value = memberId;
            cmd.Parameters.Add("@State", SqlDbType.Int).Value = state;
            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 1000).Value = String.IsNullOrEmpty(comment) ? DBNull.Value : comment;


            cmd.ExecuteNonQuery();
        }
    }
}
