using System;
using MySql.Data.MySqlClient;

// MYSQL TUTORIAL: https://zetcode.com/csharp/mysql/

namespace ProyectoFinal
{
    public class sql_connector
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Db { get; set; }
        public string Cs { get; set; }



        public sql_connector(string server, string user, string password, String db)
        {
            Server = server;
            User = user;
            Password = password;
            Db = db;
            Cs = @$"server={server};userid={user};password={password};database={db}";
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


            cmd.CommandText = "DROP TABLE IF EXISTS cars";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE cars(id INTEGER PRIMARY KEY AUTO_INCREMENT,
                    name TEXT, price INT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Audi',52642)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Mercedes',57127)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Skoda',9000)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Volvo',29000)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Bentley',350000)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Citroen',21000)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Hummer',41400)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO cars(name, price) VALUES('Volkswagen',21600)";
            cmd.ExecuteNonQuery();

            Console.WriteLine("Table cars created");

        }

    }
}