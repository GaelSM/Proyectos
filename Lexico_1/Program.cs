using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico_1
{
    public class Program : Token
    {
        static void Main(string[] args)
        {
            try
            {
                using Lexico token = new ("main.cpp");
                token.GetAllTokens();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}