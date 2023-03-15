# TwoPhaseCommitWithCSharpTransaction
ref: https://dotblogs.azurewebsites.net/supershowwei/2019/02/25/233819

core concept: setting transaction and commit the transaction later     
  
  
main calling code:
```CSharp   
string sql = "INSERT INTO TABLE1 (COLUMNS1,COLUMNS2)VALUES(@COLUMNS1,@COLUMNS2);"
TABLE1 insertData = new TABLE1{COLUMNS1="xxx",COLUMNS2="yyy"};
var newTransaction = new TransactionService(sql, TABLE1, connectstring, dbName);
transactionCoordinator.AddTransaction(newTransaction);  

transactionCoordinator.Finish(transactionCoordinator.Vote());
```
