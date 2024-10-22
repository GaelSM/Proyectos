using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lexico_3
{
    public class ErrorHandling : Exception
    {
        public ErrorHandling(string message) : base("Error: " + message) { }
        public ErrorHandling(string message, StreamWriter logger) : base("Error: " + message)
        {
            logger.WriteLine("Error: " + message);
        }

        public ErrorHandling(string message, StreamWriter logger, int line) : base("Error: " + message + " en la linea " + line)
        {
            logger.WriteLine("Error: " + message + " en la linea " + line);
        }
    }
}