using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace StoredProcsGeneratorTests
{
    [TestClass]
    public class CompanyTableTests : BaseTests
    {
        #region Company Update Tests
        [TestMethod]
        public void CompanyUpdateScriptTest()
        {
            var deleteScriptResult = CompanyUpdateDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Company Update - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("update", "Company"));
            Assert.AreEqual(-1, createScriptResult, "Company Update - Create script result should match.");
        }

        private int CompanyUpdateDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Company_Update')
                                BEGIN
	                            DROP PROCEDURE [dbo].[usp_test_Company_Update]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        [TestMethod]
        public void CompanyInsertScriptTest()
        {
            var deleteScriptResult = CompanyInsertDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Company Insert - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("insert", "Company"));
            Assert.AreEqual(-1, createScriptResult, "Company Insert - Create script result should match.");
        }

        private int CompanyInsertDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Company_Insert')
                                BEGIN
	                                DROP PROCEDURE [dbo].[usp_test_Company_Insert]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        [TestMethod]
        public void CompanyDeleteScriptTest()
        {
            var deleteScriptResult = CompanyDeleteDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Company Delete - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("delete", "Company"));
            Assert.AreEqual(-1, createScriptResult, "Company Delete - Create script result should match.");
        }

        private int CompanyDeleteDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Company_Delete')
                                BEGIN
	                                DROP PROCEDURE [dbo].[usp_test_Company_Delete]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        [TestMethod]
        public void CompanySearchScriptTest()
        {
            var deleteScriptResult = CompanySearchDeleteScript();
            Assert.AreEqual(-1, deleteScriptResult, "Company Search - Delete script result should match.");
            var createScriptResult = ExecuteDdlStatement(GetSqlScriptText("search", "Company"));
            Assert.AreEqual(-1, createScriptResult, "Company Search - Create script result should match.");
        }

        private int CompanySearchDeleteScript()
        {
            var deleteScript
                            = @"IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Company_Search')
                                BEGIN
	                                DROP PROCEDURE [dbo].[usp_test_Company_Search]
                                END";
            return ExecuteDdlStatement(deleteScript);
        }

        #endregion
    }
}
