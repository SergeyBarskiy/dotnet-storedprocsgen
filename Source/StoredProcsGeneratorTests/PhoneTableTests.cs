using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace StoredProcsGeneratorTests
{
    [TestClass]
    public class PhoneTableTests : BaseTests
    {
        #region Phone Update Tests
        [TestMethod]
        public void PhoneUpdateScriptTest()
        {
            var deleteScriptResult = PhoneUpdateDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Phone Update - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("update", "Phone"));
            Assert.AreEqual(-1, createScriptResult, "Phone Update - Create script result should match.");
        }

        private int PhoneUpdateDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Phone_Update')
                                BEGIN
	                            DROP PROCEDURE [dbo].[usp_test_Phone_Update]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        [TestMethod]
        public void PhoneInsertScriptTest()
        {
            var deleteScriptResult = PhoneInsertDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Phone Insert - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("insert", "Phone"));
            Assert.AreEqual(-1, createScriptResult, "Phone Insert - Create script result should match.");
        }

        private int PhoneInsertDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Phone_Insert')
                                BEGIN
	                                DROP PROCEDURE [dbo].[usp_test_Phone_Insert]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        [TestMethod]
        public void PhoneDeleteScriptTest()
        {
            var deleteScriptResult = PhoneDeleteDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Phone Delete - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("delete", "Phone"));
            Assert.AreEqual(-1, createScriptResult, "Phone Delete - Create script result should match.");
        }

        private int PhoneDeleteDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Phone_Delete')
                                BEGIN
	                                DROP PROCEDURE [dbo].[usp_test_Phone_Delete]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        [TestMethod]
        public void PhoneSearchScriptTest()
        {
            var deleteScriptResult = PhoneSearchDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Phone Search - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("search", "Phone"));
            Assert.AreEqual(-1, createScriptResult, "Phone Search - Create script result should match.");
        }

        private int PhoneSearchDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Phone_Search')
                                BEGIN
	                                DROP PROCEDURE [dbo].[usp_test_Phone_Search]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        #endregion
    }
}
