using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyectos
{
    public class Program
    {
        static void Main(string[] args)
        {
            using Lecturas L = new();

            //L.Encrypt2('a');
            //Console.WriteLine("Número de letras: " + L.ContarLetras());
            //Console.WriteLine("Número de digitos: " + L.ContarDigitos());
            //Console.WriteLine("Número de espacios: " + L.ContarEspacios());
            //Console.WriteLine("Es primer caracter es: " + L.PrimerCaracter());
            L.Palabras();
        }
    }
}