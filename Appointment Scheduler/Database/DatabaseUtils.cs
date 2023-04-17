using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;

namespace Appointment_Scheduler.Database
{
    public class DatabaseUtils
    {
        private string? _connectionString;
        private IConfiguration? _configuration;
        private SqlConnection? _connection;
        private static byte[] _salt = new byte[] { 1,7,2,1,8,7,1,2,9,2,3,3,4,4,2,2,1,2,4,7,1,4,2,2,9,1,1,4,2,2,9,2,0,4,6,1,8,2,2,3,0,2,3,7}; 
        public DatabaseUtils(IConfiguration configuration)
        {
            try
            {
                _configuration = configuration;
                _connectionString = _configuration.GetConnectionString("Appointment_Scheduler");
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR WHEN ESTABLISIHING DATABASE CONNECTION " + ex.Message);
            }
        }


        ~DatabaseUtils()
        {
            _connection.Close();
        }

        public   string _getHashedPassowrd(string password) {
            Console.WriteLine("salt: "+_salt);
            foreach (var i in _salt) Console.Write(i);
            Console.WriteLine();
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: _salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        private int _getNoOfUsers()
        {
            int users = 0;
            try
            {
                string query = "select count(*) from Users";
                SqlCommand command = new(query, _connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        users = reader.GetInt32(0);
                }
            } catch (Exception ec)
            {
                Console.WriteLine("ERROR WHEN CREATING USER ID :"+ec.Message);
            }
            return users;
        }

        private int _getNoOfAppointments()
        {
            int appointments = 0;
            try
            {
                string query = "select top(1) id from Appointments order by createdon asc;";
                SqlCommand command = new(query, _connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        appointments = reader.GetInt32(0);
                }
            }
            catch (Exception ec)
            {
                Console.WriteLine("ERROR WHEN FETCHING NO OF APPOINTMENT ID :" + ec.Message);
            }
            return appointments;
        }

        public int CreateUser(string username, string email,string password)
        {
            int id = _getNoOfUsers() + 1;
            try {
                string hashedPassword = _getHashedPassowrd(password);
                string createQurey = $"insert into Users values({id},'{username}','{email}','{hashedPassword}')";

                SqlCommand command = new(createQurey, _connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex) {
                Console.WriteLine("ERROR WHEN CREATING USER :" + ex.Message);
                if (ex.Message.Contains("CK_email"))
                    return -1;
                if (ex.Message.Contains("UQ__Users__72E12F1B793B4E92"))
                    return -2;
              
            }
            return id;
        }

        public List<Models.Appointment> GetAppointmentsOfUser(int id)
        {
            List<Models.Appointment>appointments = new List<Models.Appointment>();

            string query = $"select id,title,description,date from Appointments where user_id={id}";

            try {
                SqlCommand command = new(query, _connection);
                using(SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read())
                    {
                        Models.Appointment appointment = new Models.Appointment();
                        appointment.Id = reader.GetInt32(0);
                        appointment.Title = reader.GetString(1);
                        appointment.Description = reader.GetString(2);
                        appointment.Date=reader.GetDateTime(3);
                        appointments.Add(appointment);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("ERROR WHEN FETCHING APPOINTMENTS :"+ex.Message); }

            appointments.Sort((x,y)=> DateTime.Compare(x.Date,y.Date));

            return appointments;

        }

        public Models.User GetUserById(int id)
        {
            Models.User user = new Models.User();
            string query = $"select name,email,password from Users where id={id}";

            try
            {
                SqlCommand command = new(query, _connection);
                using (SqlDataReader reader=command.ExecuteReader()) {
                    while (reader.Read())
                    {
                        user.Id = id;
                        user.Name = reader.GetString(0);
                        user.Email = reader.GetString(1);
                        user.Password = reader.GetString(2);

                    }
                }
            }catch(Exception ex) { Console.WriteLine("ERROR WHEN FETCHING USER BY ID " + ex.Message); }

            return user;

        }

        public Models.User GetUserByEmail(string email)
        {
            Models.User user = new Models.User();
            string query = $"select id, name,password from Users where email='{email}'";

            try
            {
                SqlCommand command = new(query, _connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.Id = reader.GetInt32(0);
                        user.Name = reader.GetString(1);
                        user.Password = reader.GetString(2);

                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("ERROR WHEN FETCHING USER BY EMAIL " + ex.Message); }

            return user;
        }

        public bool CreateAppointment(string title, string description, string date, int userId)
        {
            int id = _getNoOfAppointments() + 1;
            string todayDate =DateTime.Now.ToString("yyyy/MM/dd");
            Console.WriteLine(todayDate);
            string query = $"insert into Appointments values({id},'{title}','{description}','{date}',{userId},'{todayDate}')";
            try
            {
                SqlCommand sqlCommand = new(query, _connection);
                sqlCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR WHEN CREATING APPOINTMENT " + ex.Message);
            }
            return false;
        }

        public void UpdateAppointment(int id,string title,string description, string date)
        {
            string query = $"update Appointments set title='{title.Trim()}',description='{description.Trim()}',date='{date.Trim()}' where id={id}";
            Console.WriteLine(query);
            try {
                SqlCommand command = new(query, _connection);
                command.ExecuteNonQuery();
            }catch(Exception ex) { Console.WriteLine("ERROR WHEN UPDATING APPOINTMENT "+ex.Message); }
        }

        public void DeleteAppointment(int userId,int appointmentId) {
            string query = $"DELETE FROM Appointments where id={appointmentId}";
            try {
                SqlCommand command = new(query, _connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex) { Console.WriteLine("ERROR WHEN DELETING APPOINTMENT "+ex.Message); }
        }
    }
}
