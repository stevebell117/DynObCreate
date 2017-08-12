using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynObCreate
{
    public class DynamicObjectCreate
    {
        private dynamic obj;

        /// <summary>
        /// Pass a DataTable into this API Constructor to instantiate the Dynamic Object.
        /// </summary>
        /// <param name="sqlTable"></param>
        public DynamicObjectCreate(DataTable sqlTable)
        {
            SetDynamicObject(sqlTable);
        }

        /// <summary>
        /// <para>Pass a SQL Query into this API Constructor to instantiate the Dynamic Object.</para>
        /// <para>Note: Does not currently check against SQL Injection</para>
        /// </summary>
        /// <param name="sqlString">SQL string passed in to be executed</param>
        /// <param name="sqlConnectionString">The SQL Connection string used to query the database</param>
        /// <param name="sqlType">Determines if the sqlString is a query (Default), Stored Proc, or the Table Name (TableDirect). Note: TableDirect only works for OLEDB</param>
        public DynamicObjectCreate(string sqlString, string sqlConnectionString, CommandType sqlType = CommandType.Text)
        {
            using(SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand(sqlString, conn))
                {
                    command.CommandType = sqlType;
                    
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();

                        dataTable.Load(reader);

                        SetDynamicObject(dataTable);
                    }
                }
            }
        }

        private void SetDynamicObject(DataTable sqlTable)
        {
            var tempObj = new ExpandoObject() as IDictionary<string, Object>;

            if (sqlTable.Rows.Count > 0) //If there's no data no need to create the object. Just return an Empty IDictionary
            {
                int columnIndex = 0;

                foreach (DataColumn col in sqlTable.Columns)
                {
                    string columnName = col.ColumnName;

                    tempObj.Add(columnName, null);

                    tempObj[columnName] = Helper.Lists.InstantiateDynamicList(sqlTable.Rows[0].ItemArray[columnIndex++]);
                }

                foreach (DataRow row in sqlTable.Rows)
                {
                    int rowIndex = 0;

                    foreach (object item in row.ItemArray)
                    {
                        string columnName = tempObj.ElementAt(rowIndex++).Key;

                        IList listPtr = (IList)tempObj[columnName];

                        listPtr?.Add(item);
                    }
                }
            }

            obj = tempObj;
        }

        /// <summary>
        /// <para>This will return a fully instantiated dynamic object with each column in the SQL DataTable returned with data.</para>
        /// <para>Any column name will have the data accessible via a "object.ColumnName" call, and the data will be contained in a typed list.</para>
        /// </summary>
        /// <returns>Dynamic Object of type IDictionary<string_IList></returns>
        public dynamic GetDynamicObject()
        {
            return obj;
        }
    }
}