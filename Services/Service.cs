using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbManager;
using Oracle.ManagedDataAccess.Client;

namespace EXRate_API_BOT.Services
{
    class Service
    {
        protected IDB GetConnection(string dbType = "MC")
        {

            OracleDB trans = OracleDB.getInstance(ProviderName.Oracle, dbType);

            return trans;
        }

        public DbResultDto GetCurrlist(string ydate)
        {
            IDB trans = GetConnection();

            DbResultDto dbRes;

            string sql = $"SELECT DISTINCT CURR FROM DST_MSTER1 WHERE ERDATE = '{ydate}' ";

            dbRes = trans.OpenSQL(sql);

            return dbRes;
        }

        public void InsertVal(Models.MSTER1 model)
        {
            IDB trans = GetConnection();

            DbResultDto dbRes;

            string plName = "PKG_APIEXR.UPSERT_EXR";

            List<IDataParameter> paramList;

            //Start transaction
            trans.BeginTransaction();

            ////////////
            //Add Data//
            ////////////
            paramList = new List<IDataParameter>
            {
                //Input parameters
                //Dynamic parameters
                trans.GenerateInParameter("IN_ERDATE", model.ERDATE, OracleDbType.Varchar2),
                trans.GenerateInParameter("IN_STYPE", model.STYPE, OracleDbType.Varchar2),
                trans.GenerateInParameter("IN_SCURR", model.SCURR, OracleDbType.Varchar2),
                trans.GenerateInParameter("IN_SRATE", model.SRATE, OracleDbType.Double),
                trans.GenerateInParameter("IN_BTYPE", model.BTYPE, OracleDbType.Varchar2),
                trans.GenerateInParameter("IN_BCURR", model.BCURR, OracleDbType.Varchar2),
                trans.GenerateInParameter("IN_BRATE", model.BRATE, OracleDbType.Double),
            };
            
            dbRes = trans.ExecPL(plName, paramList, out IDataParameterCollection outParam);
            
            //Check error
            if (dbRes.IsError)
            {
                trans.RollbackTransaction();
            }
            else
            {
                trans.CommitTransaction();

                //dbRes.ErrMsg = ((OracleParameter)outParam["OUT_MSG_VAL"]).Value.ToString();
                //dbRes.ErrColName = ((OracleParameter)outParam["OUT_MSG_KEY"]).Value.ToString();
            }

            trans.EndTransaction();
        }
    }
}
