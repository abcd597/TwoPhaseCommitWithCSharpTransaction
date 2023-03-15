using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for TransactionCoordinator
/// </summary>
public class TransactionCoordinator : IDisposable
{
    private List<TransactionService> _transactions = new List<TransactionService>();
    public TransactionCoordinator(){}
    /// <summary>
    /// 新建交易
    /// </summary>
    /// <param name="transaction">每一筆交易</param>
    public void AddTransaction(TransactionService transaction)
    {
        this._transactions.Add(transaction);
    }
    /// <summary>
    /// 各交易發出執行請求，但還未實際執行動作
    /// </summary>
    /// <returns>所有交易請求結果，如有一個失敗就視為全失敗</returns>
    public bool Vote()
    {
        var checkingTasks = this._transactions.Select(_transaction => _transaction.ExecuteTransactionAsync()).ToList();
        Task.WaitAll(checkingTasks.ToArray<Task>());
        return checkingTasks.All(_transactionTask => _transactionTask.Result);
    }
    /// <summary>
    /// 根據請求結果執行資料庫異動或回滾交易
    /// </summary>
    /// <param name="communicateResult"></param>
    public void Finish(bool communicateResult)
    {
        var tasks = communicateResult
            ? this._transactions.Select(_transaction => _transaction.CommitTransactionAsync()).ToArray()
            : this._transactions.Select(transaction => transaction.RollbackTransactionAsync()).ToArray();
        Task.WaitAll(tasks);
    }
    public void Dispose()
    {
        this._transactions.ForEach(_transaction => _transaction.Dispose());
    }
}