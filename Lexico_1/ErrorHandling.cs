using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lexico_1
{
    public class ErrorHandling : Exception
    {
        public ErrorHandling(string message) : base(message) {}
        public ErrorHandling(string message, StreamWriter logger) : base(message) {
            logger.WriteLine("Error: " + message);
        }

        public ErrorHandling(string message, StreamWriter logger, int line) : base(message + " on line " + line) {
            logger.WriteLine("Error: " + message + " on line " + line);
        }
    }
}