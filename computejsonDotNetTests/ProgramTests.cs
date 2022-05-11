using Microsoft.VisualStudio.TestTools.UnitTesting;
using Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Filter.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void IfArgsAreMathSubstTest1()
        {
            string thisJson = "{\"productId\":\"163939\",\"isbn13\":10,\"isbn10\":5}";
            string[] filterText = { "@@isbn13", "-", "@@isbn10" };
            int record = 1;

            var actual = Program.IfArgsAreMath(thisJson, filterText, record);
            var expected = "5";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IfArgsAreMathAdditTest2()
        {
            string thisJson = "{\"productId\":\"163939\",\"isbn13\":10,\"isbn10\":5}";
            string[] filterText = { "@@isbn13", "+", "@@isbn10" };
            int record = 1;

            var actual = Program.IfArgsAreMath(thisJson, filterText, record);
            var expected = "15";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IfArgsAreMathDivisTest3()
        {
            string thisJson = "{\"productId\":\"163939\",\"isbn13\":10,\"isbn10\":5}";
            string[] filterText = { "@@isbn13", "/", "@@isbn10" };
            int record = 1;

            var actual = Program.IfArgsAreMath(thisJson, filterText, record);
            var expected = "2";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IfArgsAreMathMultipTest4()
        {
            string thisJson = "{\"productId\":\"163939\",\"isbn13\":10,\"isbn10\":5}";
            string[] filterText = { "@@isbn13", "*", "@@isbn10" };
            int record = 1;

            var actual = Program.IfArgsAreMath(thisJson, filterText, record);
            var expected = "50";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IfArgsAreMathTest()
        {
            string thisJson = "{\"productId\":\"163939\",\"isbn13\":10,\"isbn10\":5}";
            string[] filterText = { "@@isbn13", "*", "@@isbn10", "-", "@@isbn10" };
            int record = 1;

            var actual = Program.IfArgsAreMath(thisJson, filterText, record);
            var expected = "45";

            Assert.AreEqual(expected, actual);
        }
    }
}