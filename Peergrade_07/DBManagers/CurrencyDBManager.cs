using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using IHW4.Models;

namespace IHW4.DBManagers
{
    /// <summary>
    /// Набор методов для взаимодействия с БД
    /// </summary>
    public static class CurrencyDBManager
    {
        private static SQLiteConnection _connection;
        private static SQLiteCommand command;

        private static bool Connect(string fileName)
        {
            try
            {
                _connection = new SQLiteConnection("Data Source=" + fileName + ";Version=3; FailIfMissing=False");
                _connection.Open();
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Access exception: {ex.Message}");
                return false;
            }
        }

        static CurrencyDBManager()
        {
            if (Connect("db.sqlite"))
            {
                command = new SQLiteCommand(_connection);
                command.CommandText = "CREATE TABLE IF NOT EXISTS currency (" +
                                      "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                                      "name1 TEXT NOT NULL," +
                                      "name2 TEXT NOT NULL," +
                                      "rate DECIMAL NOT NULL);";
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Добавление нового курса валют
        /// </summary>
        /// <param name="rate"> Информация о курсе </param>
        /// <returns> Индикатор операции добавления строки в таблицу </returns>
        public static int AddCurrencyRate(CurrencyRate rate)
        {
            if (GetRate(rate.From, rate.To) != null)
            {
                return 1;
            }
            
            command.CommandText = "INSERT INTO currency (name1, name2, rate)" +
                                  "VALUES (:name1, :name2, :rate)";
            command.Parameters.AddWithValue("name1", rate.From);
            command.Parameters.AddWithValue("name2", rate.To);
            command.Parameters.AddWithValue("rate", rate.Rate);
            
            try
            {
                command.ExecuteNonQuery();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Получение курса валют
        /// </summary>
        /// <returns> Список курсов </returns>
        public static List<CurrencyRate> GetList()
        {
            var list = new List<CurrencyRate>();
            command.CommandText = "SELECT * FROM currency";
            using var reader = command.ExecuteReader();
            if (!reader.HasRows) return list;
            
            while (reader.Read())  
            {
                list.Add(new CurrencyRate((string) reader.GetValue(1), 
                    (string) reader.GetValue(2), 
                    (decimal) reader.GetValue(3)));
            }

            return list;
        }

       /// <summary>
       /// Получение информации о курсе двух валют
       /// </summary>
       /// <param name="name1"> Обозначение 1-ой валюты </param>
       /// <param name="name2"> Обозначение 2-ой валюты </param>
       /// <returns> Курс </returns>
        public static CurrencyRate GetRate(string name1, string name2)
        {
            command.CommandText = "SELECT * FROM currency WHERE (name1 = :name1 AND name2 = :name2) OR (name1 = :name2 AND name2 = :name1)";
            command.Parameters.AddWithValue("name1", name1);
            command.Parameters.AddWithValue("name2", name2);

            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            if (data.Rows.Count == 0)
            {
                return null;
            }

            var currency1 = data.Select()[0].Field<string>("name1");
            var currency2 = data.Select()[0].Field<string>("name2");
            var rate = data.Select()[0].Field<decimal>("rate");
            
            return name1 == currency1 ? 
                            new CurrencyRate(currency1, currency2, rate) : 
                            new CurrencyRate(currency2, currency1, 1 / rate);
        }

       /// <summary>
       /// Обновление курса
       /// </summary>
       /// <param name="rate"> Информация о курсе </param>
       /// <returns> Индикатор операции </returns>
       public static bool UpdateRate(CurrencyRate rate)
       {
           command.CommandText = "UPDATE currency SET name1 = :name1, name2 = :name2, rate = :rate " +
                                 "WHERE (name1 = :name1 AND name2 = :name2) " +
                                 "OR (name1 = :name2 AND name2 = :name1)";
           command.Parameters.AddWithValue("name1", rate.From);
           command.Parameters.AddWithValue("name2", rate.To);
           command.Parameters.AddWithValue("rate", rate.Rate);
           
           try
           {
               command.ExecuteNonQuery();
               return true;
           } 
           catch
           {
               return false;
           }
       }
    }
}
