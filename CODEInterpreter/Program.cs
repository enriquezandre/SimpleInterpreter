using Antlr4;
using Antlr4.Runtime;
using CODEInterpreter.Content;
using static System.Net.Mime.MediaTypeNames;
using CommonTokenStream = Antlr4.Runtime.CommonTokenStream;

var fileName = "Content\\test.ss"; //args[0]?

var fileContents = File.ReadAllText(fileName);

var inputStream = new AntlrInputStream(fileContents);

var simpleLexer = new SimpleLexer(inputStream);
var commonTokenStream = new CommonTokenStream(simpleLexer);
var simpleParser = new SimpleParser(commonTokenStream);
//simpleParser.AddErrorListener();
var simpleContext = simpleParser.program();
var visitor = new SimpleVisitor();
visitor.Visit(simpleContext);