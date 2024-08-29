using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lexico_1
{
    public class Lexico : Token, IDisposable
    {
        StreamReader file;
        StreamWriter logger;
        StreamWriter assembly;
        public Lexico() {
            if(File.Exists("./test.cpp")) {
                file = new StreamReader("./test.cpp");
            } else {
                throw new ErrorHandling("File test.cpp doesn´t exists", logger);
            }
            logger = new StreamWriter("./test.log");
            assembly = new StreamWriter("./test.asm");
        }
        public void Dispose() {
            file.Close();
            logger.Close();
            assembly.Close();
        }
    }
}