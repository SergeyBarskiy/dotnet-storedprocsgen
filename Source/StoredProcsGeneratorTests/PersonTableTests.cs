using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace StoredProcsGeneratorTests
{
    [TestClass]
    public class PersonTableTests : BaseTests
    {
        #region Person Update Tests
        [TestMethod]
        public void PersonUpdateScriptTest()
        {
            var deleteScriptResult = PersonUpdateDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Person Update - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("update", "Person"));
            Assert.AreEqual(-1, createScriptResult, "Person Update - Create script result should match.");
        }

        private int PersonUpdateDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Person_Update')
                                BEGIN
	                            DROP PROCEDURE [dbo].[usp_test_Person_Update]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        [TestMethod]
        public void PersonInsertScriptTest()
        {
            var deleteScriptResult = PersonInsertDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Person Insert - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("insert", "Person"));
            Assert.AreEqual(-1, createScriptResult, "Person Insert - Create script result should match.");
        }

        private int PersonInsertDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Person_Insert')
                                BEGIN
	                                DROP PROCEDURE [dbo].[usp_test_Person_Insert]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        [TestMethod]
        public void PersonDeleteScriptTest()
        {
            var deleteScriptResult = PersonDeleteDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Person Delete - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("delete", "Person"));
            Assert.AreEqual(-1, createScriptResult, "Person Delete - Create script result should match.");
        }

        private int PersonDeleteDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Person_Delete')
                                BEGIN
	                                DROP PROCEDURE [dbo].[usp_test_Person_Delete]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        [TestMethod]
        public void PersonSearchScriptTest()
        {
            var deleteScriptResult = PersonSearchDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Person Search - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("search", "Person"));
            Assert.AreEqual(-1, createScriptResult, "Person Search - Create script result should match.");
        }

        private int PersonSearchDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Person_Search')
                                BEGIN
	                                DROP PROCEDURE [dbo].[usp_test_Person_Search]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        #endregion
    }
}
