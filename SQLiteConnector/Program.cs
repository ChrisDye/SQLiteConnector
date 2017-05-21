using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace SQLiteConnector
{
    public class Program
    {
        /* TODO: I think these methods should return an exception object 
        * so the class that calls this dll can do with the exceptions as needed, 
        * such as display to a user
        */
        // TODO: This class may be over commented? Perhaps recheck once base methods completed

        SQLiteConnection DBConnection;

        /// <summary> Default Constructor </summary>
        public void SQLiteConnector()
        {
            if (!File.Exists("DefaultDB.sqlite"))
            { SQLiteConnection.CreateFile("DefaultDB.sqlite"); }
        }

        /// <summary> 
        /// <para>Constructor with args</para>
        /// <para>First arg should be the dbname to connect(If found) or create(If not found)</para>
        /// </summary>
        public void SQLiteConnector(string[] args)
        {
            string DBName = args[0];
            // Create DB if it does not exist
            if (!File.Exists(DBName+".sqlite"))
            {
                SQLiteConnection.CreateFile(DBName + ".sqlite");
            }
            DBConnection = new SQLiteConnection("Data Source=" + DBName + ".sqlite;Version=3;");
        }

        /// <summary>
        /// Opens the connection to the chosen DB
        /// </summary>
        public void OpenDBConnection()
        {
            DBConnection.Open();
        }

        /// <summary>
        /// Closes the connection to the chosen DB
        /// </summary>
        public void CloseDBConnection()
        {
            DBConnection.Close();
        }

        /// <summary> Creates table with specified table name and columns </summary>
        public void CreateTable(string TableName, string[] ColumnNames, string[] ColumnDataTypes)
        {
            if (ColumnNames.Length != ColumnDataTypes.Length || !DoesTableExist(TableName))
            {
                // TODO: some sort of logging to explain inconsistency (Log file maybe?)
                return;
            }
            string SQLScript = "create table " + TableName + "(";
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                SQLScript += ColumnNames[i] + " " + ColumnDataTypes[i] + ",";
            }
            SQLScript = SQLScript.TrimEnd(',') + ")";

            SQLiteCommand CreateComm = new SQLiteCommand(SQLScript, DBConnection);
            CreateComm.ExecuteNonQuery();
        }

        /// <summary> Single line insert into table with specified table name and columns </summary>
        public void InsertIntoTable(string TableName, string[] ColumnNames, string[] ColumnValues)
        {
            // TODO: Multiline inserts
            if (ColumnNames.Length != ColumnValues.Length || !DoesTableExist(TableName))
            {
                // TODO: some sort of logging to explain inconsistency (Log file maybe?)
                return;
            }
            string SQLScript = "insert into " + TableName + "(";
            foreach (string ColName in ColumnNames)
            {
                SQLScript += ColName + ",";
            }
            SQLScript = SQLScript.TrimEnd(',') + ") values (";
            foreach (string ColVal in ColumnValues)
            {
                SQLScript += ColVal + ",";
            }
            SQLScript = SQLScript.TrimEnd(',') + ")";
            SQLiteCommand InsertComm = new SQLiteCommand(SQLScript, DBConnection);
            InsertComm.ExecuteNonQuery();
        }
        
        /// <summary> Update table with specified table name and columns, currently only supports simple where clause </summary>
        public void UpdateTable(string TableName, string[] ColumnNames, string[] ColumnValues, string WhereCol = "", string WhereVal = "")
        {
            if (ColumnNames.Length != ColumnValues.Length || !DoesTableExist(TableName))
            {
                // TODO: some sort of logging to explain inconsistency (Log file maybe?)
                return;
            }
            string SQLScript = "Update " + TableName + " set ";
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                SQLScript += ColumnNames[i] + " = " + ColumnValues[i] + ", ";
            }
            SQLScript = SQLScript.TrimEnd(',');
            if (WhereCol.Trim() != string.Empty)
            {
                SQLScript += " where " + WhereCol + " = " + WhereVal;
            }
            SQLiteCommand InsertComm = new SQLiteCommand(SQLScript, DBConnection);
            InsertComm.ExecuteNonQuery();
        }

        private bool DoesTableExist(string TableName)
        {
            SQLiteCommand CheckComm = new SQLiteCommand("select name from sqlite_master where type='table' and name = '" + TableName + "'", DBConnection);
            if (CheckComm.ExecuteNonQuery() > 0) { return true; }
            return false;
        }
    }
}
