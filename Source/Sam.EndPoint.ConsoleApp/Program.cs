using Sam.FileTableFramework.Extentions;
using Sam.Persistence;
using System;

var connectionStrings = "Data Source =.; Initial Catalog = db1; Integrated Security = true";
var db = new DatabaseContext();
db.UseSqlServer(connectionStrings);
db.Migrate();

var result = await db.Table1.CountAsync();

Console.WriteLine($"Count Files in Table1 : {result}");
Console.ReadKey();
