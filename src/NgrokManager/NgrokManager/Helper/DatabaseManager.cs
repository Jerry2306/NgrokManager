using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NgrokManager.Helper
{
    public class DatabaseManager : IDisposable
    {
        public bool IsOpen { get; private set; }
        public bool IsClosed { get => !IsOpen; }

        private MySqlConnection _sqlCon { get; set; }
        private MySqlCommand _command { get; set; }
        private MySqlDataReader _dataReader { get; set; }
        public DatabaseManager(string connectionString)
        {
            IsOpen = false;
            _sqlCon = new MySqlConnection(connectionString);
            _sqlCon.StateChange += _sqlCon_StateChange;
        }

        private void _sqlCon_StateChange(object sender, StateChangeEventArgs e)
        {
            if (e.CurrentState == ConnectionState.Closed)
                IsOpen = false;
            else if (e.CurrentState == ConnectionState.Open)
                IsOpen = true;
        }

        public void Open()
        {
            try
            {
                _sqlCon.Open();
                IsOpen = true;
            }
            catch (Exception exc)
            {
                throw new Exception($"Datenbankverbindung konnte nicht geöffnet werden: {exc.Message}", exc);
            }
        }

        public void Close()
        {
            try
            {
                _sqlCon.Close();
                IsOpen = false;
            }
            catch (Exception exc)
            {
                throw new Exception($"Datenbankverbindung konnte nicht geschlossen werden: {exc.Message}", exc);
            }
        }

        public List<List<object>> RunQuery(string sql, int columnCnt)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentException("Parameter 'sql' darf nicht leer sein");

            List<List<object>> result = new List<List<object>>();

            try
            {
                if (IsClosed)
                    Open();

                _command = new MySqlCommand(sql, _sqlCon);

                _dataReader = _command.ExecuteReader();

                while (_dataReader.Read())
                {
                    int cnt = result.Count;
                    result.Add(new List<object>());

                    for (int i = 0; i < columnCnt; i++)
                    {
                        result[cnt].Add(_dataReader.GetValue(i));
                    }
                }
                _dataReader.Close();
            }
            catch (Exception exc)
            {
                throw new Exception($"Die Abfrage({sql}, {columnCnt}) konnte nicht ausgeführt werden: {exc.Message}", exc);
            }

            return result;
        }

        public int RunNonQuery(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentException("Parameter 'sql' darf nicht leer sein");

            int result = -1;
            try
            {
                if (IsClosed)
                    Open();

                _command = new MySqlCommand(sql, _sqlCon);

                result = _command.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                throw new Exception($"Der Befehl({sql}) konnte nicht ausgeführt werden: {exc.Message}", exc);
            }

            return result;
        }

        #region Dispose

        private bool _isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isCalledByDispose)
        {
            if (_isDisposed)
                return;

            if (isCalledByDispose)
            {
                _sqlCon?.Close();
                _sqlCon?.Dispose();
                _sqlCon = null;

                _command?.Dispose();
                _command = null;

                _dataReader?.Close();
                _dataReader = null;
            }

            _isDisposed = true;
        }

        ~DatabaseManager()
        {
            Dispose(false);
        }
        #endregion
    }
}
