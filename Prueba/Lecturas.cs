using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Proyectos
{
    public class Lecturas : IDisposable
    {
        readonly StreamReader archivo;
        readonly StreamWriter log;
        public Lecturas()
        {
            archivo = new StreamReader("C:/Users/gaels/Documents/ITQ/IV/Lenguajes y Autómatas I/Prueba/prueba.cpp");
            log = new StreamWriter("C:/Users/gaels/Documents/ITQ/IV/Lenguajes y Autómatas I/Prueba/prueba.log");
        }

        public Lecturas(string nombre)
        {
            archivo = new StreamReader(nombre);
            log = new StreamWriter("prueba.log");
        }

        public void Dispose()
        {
            archivo.Close();
            log.Close();
        }

        public void Display()
        {
            while (!archivo.EndOfStream)
            {
                Console.Write((char)archivo.Read());
            }
        }

        public void Copy()
        {
            while (!archivo.EndOfStream)
            {
                log.Write((char)archivo.Read());
            }
        }

        public void Encrypt()
        {
            char letter;

            while (!archivo.EndOfStream)
            {
                letter = (char)archivo.Read();

                if (char.IsLetter(letter))
                {
                    log.Write((char)(letter + 2));
                }
                else
                {
                    log.Write(letter);
                }
            }
        }

        public void Encrypt2(char v)
        {
            int letter;
            int[] vocals = [97, 101, 105, 111, 117];
            bool isVocal;

            while (!archivo.EndOfStream)
            {
                letter = archivo.Read();
                isVocal = vocals.Contains(letter) || vocals.Contains(letter + 32);

                if (isVocal)
                {
                    log.Write(v);
                }
                else
                {
                    log.Write((char)letter);
                }
            }
        }

        public void DesEncrypt()
        {
            char letter;

            while (!archivo.EndOfStream)
            {
                letter = (char)archivo.Read();

                if (char.IsLetter(letter))
                {
                    log.Write((char)(letter - 2));
                }
                else
                {
                    log.Write(letter);
                }
            }
        }

        public int ContarLetras()
        {
            int counter = 0;
            while (!archivo.EndOfStream)
            {
                if (char.IsLetter((char)archivo.Read()))
                {
                    counter++;
                }
            }
            return counter;
        }

        public int ContarDigitos()
        {
            int counter = 0;
            while (!archivo.EndOfStream)
            {
                if (char.IsDigit((char)archivo.Read()))
                {
                    counter++;
                }
            }
            return counter;
        }

        public int ContarEspacios()
        {
            int counter = 0;
            while (!archivo.EndOfStream)
            {
                if (char.IsWhiteSpace((char)archivo.Read()))
                {
                    counter++;
                }
            }
            return counter;
        }

        public char PrimerCaracter()
        {
            char c = ' ';
            int letter;

            while (!archivo.EndOfStream)
            {
                letter = archivo.Read();

                if (!char.IsWhiteSpace((char)letter))
                {
                    c = (char)letter;
                    return c;
                }
            }

            return c;
        }

        public void Palabras()
        {
            string word = "";
            char currentSimbol;

            while (!this.finArchivo())
            {
                currentSimbol = (char)archivo.Read();

                if(!char.IsWhiteSpace(currentSimbol)) {
                    if(char.IsLetter(currentSimbol) || char.IsDigit(currentSimbol)) {
                        word += currentSimbol;
                    }
                } else {
                    if(word != "") {
                        log.WriteLine(word);
                        word = "";
                    }
                }
            }

            log.WriteLine(word);
        }

        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}