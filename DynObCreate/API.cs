using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynObCreate
{
    public class API
    {
        private dynamic obj;

        public API(DataTable sqlTable)
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