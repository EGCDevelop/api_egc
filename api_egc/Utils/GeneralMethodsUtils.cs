using System.Data;
using System.Data.SqlClient;
using api_egc.Models;
using api_egc.Models.Instructors;

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

        public static List<Establishment> EXEC_SP_GET_ESTABLECIMIENTO(string connectionString)
        {
            List<Establishment> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_ESTABLECIMIENTO", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Establishment establishment = new()
                    {
                        ESTIdEstablecimiento = Utils.GetValue<long>(reader, "ESTIdEstablecimiento"),
                        ESTNombreEstablecimiento = Utils.GetValue<string>(reader, "ESTNombreEstablecimiento")
                    };

                    list.Add(establishment);
                }
            }
            return list;
        }

        public static List<Carrera> EXEC_SP_GET_CARRERA(string connectionString)
        {
            List<Carrera> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_CARRERA", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Carrera carrera = new()
                    {
                        CARIdCarrera = Utils.GetValue<long>(reader, "CARIdCarrera"),
                        CARNombreCarrera = Utils.GetValue<string>(reader, "CARNombreCarrera")
                    };

                    list.Add(carrera);
                }
            }

            return list;
        }

        public static List<Grado> EXEC_SP_GET_GRADO(string connectionString)
        {
            List<Grado> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_GRADO", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Grado grado = new()
                    {
                        GRAIdGrado = Utils.GetValue<long>(reader, "GRAIdGrado"),
                        GRANombreGrado = Utils.GetValue<string>(reader, "GRANombreGrado")
                    };

                    list.Add(grado);
                }
            }

            return list;
        }
        public static List<Position> EXEC_SP_GET_POSITION(string connectionString)
        {
            List<Position> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_POSITION", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Position puesto = new()
                    {
                        PUIdPuesto = Utils.GetValue<long>(reader, "PUIdPuesto"),
                        PUNombre = Utils.GetValue<string>(reader, "PUNombre")
                    };

                    list.Add(puesto);
                }
            }

            return list;
        }

        public static List<IntegrantePerYearDto> EXEC_SP_GET_BY_INSERT_PER_YEAR(string connectionString)
        {
            List<IntegrantePerYearDto> list = [];
            int year = DateTime.Now.Year;

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_BY_INSERT_PER_YEAR", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;

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

        public static List<InstructorPositions> EXEC_SP_GET_INSTRUCTOR_POSITIONS(string connectionString)
        {
            List<InstructorPositions> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_INSTRUCTOR_POSITIONS", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    InstructorPositions positions = new()
                    {
                        Id = Utils.GetValue<int>(reader, "Id"),
                        Descripcion = Utils.GetValue<string>(reader, "Descripcion")
                    };

                    list.Add(positions);
                }
            }

            return list;
        }
    }
}
