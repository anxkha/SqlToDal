using System;
using System.Linq;
using SqlToDal.Generation;

var parser = new Parser { ProcedurePrefix = "usp_" };
var procedures = parser.Parse("C:\\Repos\\SqlToDal\\src\\TestDatabase\\bin\\Debug\\TestDatabase.dacpac");

var generator = new Generator { Namespace = "SqlToDal.Generated" };
var classes = procedures.Select(generator.GenerateProcedure).ToArray();
Console.ReadKey();