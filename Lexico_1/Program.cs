using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico_1
{
    public class Program
    {
        static void Main(string[] args) {
            using Lexico token = new();

            token.setContent("Hola");
            token.setClasification(Token.Tipos.Indentificador);

            Console.WriteLine(token.getContent() + " " + token.getClasification());
        }
    }
}