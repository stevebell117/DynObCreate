using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynObCreate;
using System.Data;

namespace DynObCreateTestSuite
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Code", typeof(string));
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Validated", typeof(bool));

            table.Rows.Add("Code1", 1, true);
            table.Rows.Add("Code2", 2, false);

            API obj = new API(table);

            dynamic dynObj = obj.GetDynamicObject();

            string code = dynObj.Code?[0];

            Assert.AreEqual("Code1", code);
        }
    }
}
