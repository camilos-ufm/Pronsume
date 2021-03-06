using System;
using MySql.Data.MySqlClient;

// MYSQL TUTORIAL: https://zetcode.com/csharp/mysql/

namespace ProyectoFinal
{
    public class SqlConnector
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Db { get; set; }
        public string Cs { get; set; }

        public MySqlConnection ConnectionMysql { get; set; }


        public SqlConnector(string server, string user, string password, String db)
        {
            Server = server;
            User = user;
            Password = password;
            Db = db;
            Cs = @$"server={server};userid={user};password={password};database={db}";
            ConnectionMysql = new MySqlConnection(Cs);
            ConnectionMysql.Open();
        }





        public void sqlConnect()
        {
            // string cs = @$"server={this.Server};userid={this.User};password={this.Password};database={this.Db}";

            using var con = new MySqlConnection(Cs);
            con.Open();

            var stm = "SELECT VERSION()";
            var cmd = new MySqlCommand(stm, con);

            var version = cmd.ExecuteScalar().ToString();
            Console.WriteLine($"MySQL version: {version}");
        }

        public void createTable()
        {

            using var con = new MySqlConnection(Cs);
            con.Open();

            using var cmd = new MySqlCommand();
            cmd.Connection = con;


            // cmd.CommandText = "CREATE TABLE IF NOT EXISTS leads";
            // cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS leads(id INTEGER PRIMARY KEY AUTO_INCREMENT,
                    name TEXT, phone TEXT, date TEXT, city TEXT, value TEXT, thread_name TEXT, produced_by TEXT, timestamp TIMESTAMP, time_to_produce INT, time_to_consume INT)";
            cmd.ExecuteNonQuery();

            // cmd.CommandText = $"INSERT INTO leads(name, phone, date, city, thread_name) VALUES({person.name}, {person.phone}, {person.date}, {person.city}. {thread_name})";
            // cmd.ExecuteNonQuery();

            // Console.WriteLine("Table leads created");

        }
        public void insertIntoTable(Person person, String thread_name)
        {
            if (IsDebug.isDebug)
                Console.WriteLine($"To insert: {person.name} in {thread_name}");

            using var cmd = new MySqlCommand();
            cmd.Connection = ConnectionMysql;
            Random rnd = new Random();

            int r = rnd.Next();

            cmd.CommandText = $"INSERT INTO leads(name, phone, date, city, value, thread_name, produced_by, time_to_produce, time_to_consume) VALUES('{person.name.ToString().Replace("'", "\'")}', '{person.phone.ToString().Replace("'", "\'")}', '{person.date.ToString().Replace("'", "\'")}', '{person.city.ToString().Replace("'", "\'")}', '{r.ToString()}', '{thread_name.ToString()}', '{person.produced_by.ToString()}', '{person.time_to_produce.ToString()}', '{person.time_to_consume.ToString()}')";
            cmd.ExecuteNonQuery();
            if (IsDebug.isDebug)
                Console.WriteLine($"Inserted {person.name}");

        }

    }
}