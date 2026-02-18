using api_egc.Models.Instructors;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace api_egc.Utils
{
    public class InstructorUtils
    {
        public static List<EscuadrasInstructoresDTO> EXEC_SP_GET_ASSIGNED_SQUADS_INSTRUCTORS(string connectionString, long idInstructor)
        {
            List<EscuadrasInstructoresDTO> list = [];

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using SqlCommand cmd = new("SP_GET_ASSIGNED_SQUADS_INSTRUCTORS", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@IdInstructor", SqlDbType.BigInt).Value = idInstructor;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    EscuadrasInstructoresDTO data = new()
                    {
                        Id = Utils.GetValue<int>(reader, "Id"),
                        IdEscuadra = Utils.GetValue<int>(reader, "IdEscuadra"),
                        IdInstructor = Utils.GetValue<long>(reader, "IdInstructor"),
                        Principal = Utils.GetValue<int>(reader, "Principal"),
                        Nombre = Utils.GetValue<string>(reader, "Nombre")
                    };

                    list.Add(data);
                }
            }
            return list;
        }

        public static void EXEC_SP_UPDATE_INSTRUCTOR_PROFILE(string connectionString, long id, string name, string lastName,
            string phone, string email)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            using SqlCommand cmd = new("SP_UPDATE_INSTRUCTOR_PROFILE", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = id;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
            cmd.Parameters.Add("@lastName", SqlDbType.NVarChar).Value = lastName;
            cmd.Parameters.Add("@phone", SqlDbType.VarChar, 8).Value = phone;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;

            // Ejecutar el procedimiento
            cmd.ExecuteNonQuery();
        }

        public static void TRANSACTION_UPDATE_INSTRUCTOR(string connectionString, InstructorPayload payload)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                EXEC_SP_UPDATE_INSTRUCTOR(connection, transaction, payload);
                DeactivateOldSquads(connection, transaction, payload.Id, payload.Squads);

                List<int> squadsIds = EXEC_SP_GET_IDS_SQUADS_INSTRUCTORS(connection, transaction, payload.Id);

                var newSquads = payload.Squads.Where(s => !squadsIds.Contains(s.SquadId)).ToList();

                foreach (var squad in newSquads)
                {
                    EXEC_INSERT_ASIGNED_SQUADS_INSTRUCTOR(connection, transaction, squad, (int)payload.Id);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Fallo en la actualización integral del instructor: " + ex.Message);
            }
        }

        public static void EXEC_SP_UPDATE_INSTRUCTOR(SqlConnection connection, SqlTransaction transaction, InstructorPayload payload)
        {
           
            using SqlCommand cmd = new("SP_UPDATE_INSTRUCTOR", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = payload.Id;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = payload.Name;
            cmd.Parameters.Add("@lastName", SqlDbType.NVarChar).Value = payload.LastName;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = payload.Email;
            cmd.Parameters.Add("@phone", SqlDbType.VarChar, 8).Value = payload.Tel;
            cmd.Parameters.Add("@position", SqlDbType.BigInt).Value = payload.Position;
            cmd.Parameters.Add("@area", SqlDbType.NVarChar, 100).Value = payload.Area;
            cmd.Parameters.Add("@rol", SqlDbType.Int).Value = payload.Rol;
            cmd.Parameters.Add("@state", SqlDbType.Int).Value = payload.State;

            // Ejecutar el procedimiento
            cmd.ExecuteNonQuery();
        }

        private static void DeactivateOldSquads(SqlConnection conn, SqlTransaction trans, int instructorId, List<AssignedSquadRequest> squads)
        {
            string idsString = string.Join(",", squads.Select(s => s.SquadId));

            using SqlCommand cmd = new("SP_UPDATE_SQUADS_INSTRUCTORS", conn, trans);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = instructorId;
            cmd.Parameters.Add("@squadIdsList", SqlDbType.NVarChar, 100).Value = idsString;

            cmd.ExecuteNonQuery();
        }

        public static List<int> EXEC_SP_GET_IDS_SQUADS_INSTRUCTORS(SqlConnection connection, SqlTransaction transaction, long id)
        {
            List<int> ids = new List<int>();
            using (SqlCommand cmd = new SqlCommand("SP_GET_IDS_SQUADS_INSTRUCTORS", connection, transaction))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = id;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ids.Add(Convert.ToInt32(reader["Id"]));
                    }
                }
            }
            return ids;
        }

        public static List<Instructor> EXEC_SP_GET_INSTRUCTORS_BY_FILTERS(string connectionString, int state, 
            int puesto, string like, int usuarioId)
        {
            List<Instructor> dataList = [];

            using(SqlConnection conn = new(connectionString))
            {
                conn.Open();

                using SqlCommand cmd = new("SP_GET_INSTRUCTORS_BY_FILTERS", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Like", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(like) ? DBNull.Value : like;
                cmd.Parameters.Add("@state", SqlDbType.Int).Value = state;
                cmd.Parameters.Add("@puesto", SqlDbType.Int).Value = puesto;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Instructor inst = new()
                    {
                        Id = Utils.GetValue<long>(reader, "Id"),
                        Nombre = Utils.GetValue<string>(reader, "Nombre"),
                        Apellido = Utils.GetValue<string>(reader, "Apellido"),
                        Telefono = Utils.GetValue<string>(reader, "Telefono"),
                        Correo = Utils.GetValue<string>(reader, "Correo"),
                        Estado = Utils.GetValue<int>(reader, "Estado"),
                        Usuario = Utils.GetValue<string>(reader, "Usuario"),
                        IdPuesto = Utils.GetValue<int>(reader, "IdPuesto"),
                        Area = Utils.GetValue<string>(reader, "Area"),
                        Rol = Utils.GetValue<int>(reader, "Rol"),
                        Escuadras = Utils.GetValue<string>(reader, "Escuadras"),
                    };

                    dataList.Add(inst);
                }
            }

            return dataList;
        }

        public static void TRANSACTION_INSERT_INSTRUCTOR(string connectionString, InstructorPayload instructor)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                int instructorId = EXEC_SP_INSERT_INSTRUCTOR(connection, transaction, instructor);

                // 2. Insertamos cada escuadra usando el método separado
                foreach (var squad in instructor.Squads)
                {
                    EXEC_INSERT_ASIGNED_SQUADS_INSTRUCTOR(connection, transaction, squad, instructorId);
                }

                // Si todo salió bien, confirmamos la transacción global
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error en la transacción: " + ex.Message);
            }
        }

        private static int EXEC_SP_INSERT_INSTRUCTOR(SqlConnection conn, SqlTransaction trans, InstructorPayload instructor)
        {
            using SqlCommand cmd = new SqlCommand("SP_INSERT_INSTRUCTOR", conn, trans);
            cmd.CommandType = CommandType.StoredProcedure;

            string hashedPassword = PasswordHasher.HashPassword(instructor.Password);
            string userGenerated = Utils.UsernameGenerate(instructor.Name, instructor.LastName);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = instructor.Name;
            cmd.Parameters.Add("@lastName", SqlDbType.NVarChar, 100).Value = instructor.LastName;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = instructor.Email;
            cmd.Parameters.Add("@tel", SqlDbType.VarChar, 10).Value = instructor.Tel;
            cmd.Parameters.Add("@username", SqlDbType.NVarChar, 100).Value = userGenerated;
            cmd.Parameters.Add("@password", SqlDbType.NVarChar, 1000).Value = hashedPassword;
            cmd.Parameters.Add("@position", SqlDbType.BigInt).Value = instructor.Position;
            cmd.Parameters.Add("@area", SqlDbType.NVarChar, 50).Value = instructor.Area;
            cmd.Parameters.Add("@rol", SqlDbType.Int).Value = instructor.Rol;
            cmd.Parameters.Add("@state", SqlDbType.Int).Value = instructor.State;

            // parametro de salida
            SqlParameter outputParam = new("@InsertedId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output,
            };

            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();
            return (int)outputParam.Value;
        }

        private static void EXEC_INSERT_ASIGNED_SQUADS_INSTRUCTOR(SqlConnection conn, SqlTransaction trans, 
            AssignedSquadRequest squad, int instructorId)
        {
            using SqlCommand cmd = new SqlCommand("SP_INSERT_ASIGNED_SQUADS_INSTRUCTOR", conn, trans);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@squadId", SqlDbType.BigInt).Value = squad.SquadId;
            cmd.Parameters.Add("@instructorId", SqlDbType.BigInt).Value = instructorId;
            cmd.Parameters.Add("@state", SqlDbType.Int).Value = 1; // 1 = Activo
            cmd.Parameters.Add("@isPrincipal", SqlDbType.Int).Value = squad.IsPrincipal ? 1 : 0;

            cmd.ExecuteNonQuery();
        }
    }
}
