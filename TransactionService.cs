using Aspose.Email.Tools.Logging;
using Dapper;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for TransactionService
/// </summary>
public class TransactionService : IDisposable
{
    private readonly IDbConnection connect;
    private readonly string sql;
    private readonly object parameters;
    private readonly string branchDB;
    private IDbTransaction transaction;
    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="sql">sql</param>
    /// <param name="parameters">sql參數</param>
    /// <param name="connectionString">連線字串</param>
    public TransactionService(string sql, object parameters, string connectionString)
    {
        this.connect = new SqlConnection(connectionString);
        this.sql = sql;
        this.parameters = parameters;
    }
    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="sql">sql</param>
    /// <param name="parameters">sql參數</param>
    /// <param name="connectionString">連線字串</param>
    /// <param name="branchDB">異動的DB，與連線字串裡不同時進行切換</param>
    public TransactionService(string sql, object parameters, string connectionString, string branchDB)//:this(sql,null,connString,branchDB)
    {
        this.connect = new SqlConnection(connectionString);
        this.sql = sql;
        this.branchDB = branchDB;
        this.parameters = parameters;
    }
    /// <summary>
    /// 執行交易但不提交
    /// </summary>
    /// <returns></returns>
    public Task<bool> ExecuteTransactionAsync()
    {

        return Task.Run(() =>
        {
            try
            {
                if (this.connect.State == ConnectionState.Closed)
                {
                    this.connect.Open();
                }
                if (null != this.branchDB && this.branchDB != this.connect.Database)
                {
                    this.connect.ChangeDatabase(this.branchDB);
                }
                this.transaction = this.connect.BeginTransaction();
                this.connect.Execute(this.sql, this.parameters, this.transaction);
                return true;
            }
            catch (Exception e)
            {
                
                NLog.LogManager.GetCurrentClassLogger().Error("CELL API ERROR!", e.Message);
            }
            return false;
        });
    }
    /// <summary>
    /// 交易提交
    /// </summary>
    /// <returns></returns>
    public Task CommitTransactionAsync()
    {
        return Task.Run(
            () =>
            {
                this.transaction?.Commit();
                this.Dispose();
            });
    }
    public Task RollbackTransactionAsync()
    {
        return Task.Run(
            () =>
            {
                this.transaction?.Rollback();
                this.Dispose();
            });
    }
    public void Dispose()
    {
        this.connect?.Dispose();
        this.transaction?.Dispose();
    }
}