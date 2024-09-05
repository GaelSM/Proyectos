using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lexico_1
{
    public class Lexico : Token, IDisposable
    {
        readonly StreamReader file;
        readonly StreamWriter logger;
        readonly StreamWriter assembly;
        public Lexico()
        {
            logger = new StreamWriter("./test.log");
            assembly = new StreamWriter("./test.asm");

            logger.AutoFlush = true;
            assembly.AutoFlush = true;

            if (File.Exists("./test.cpp"))
            {
                file = new StreamReader("./test.cpp");
            }
            else
            {
                throw new ErrorHandling("File test.cpp doesn´t exists", logger);
            }
        }

        public Lexico(string fileName)
        {
            logger = new StreamWriter("./" + fileName + ".log");
            assembly = new StreamWriter("./" + fileName + ".asm");

            logger.AutoFlush = true;
            assembly.AutoFlush = true;
        }

        public void Dispose()
        {
            file.Close();
            logger.Close();
            assembly.Close();
        }

        public int LineCounter()
        {
            int counter = 0;
            while (!file.EndOfStream)
            {
                file.ReadLine();
                counter++;
            }
            return counter;
        }

        public void NextToken()
        {
            char currentSymbol;
            string word = "";

            //Leer espacios
            while (true)
            {
                currentSymbol = (char)file.Peek();
                if (char.IsWhiteSpace(currentSymbol))
                {
                    file.Read();
                }
                else
                {
                    break;
                }
            }

            word += currentSymbol;
            file.Read();

            //Identificador
            if (char.IsLetter(currentSymbol))
            {
                setClasification(Tipos.Indentificador);
                while (true)
                {
                    currentSymbol = (char)file.Peek();
                    if (char.IsLetterOrDigit(currentSymbol))
                    {
                        word += currentSymbol;
                        file.Read();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //Número
            else if (char.IsDigit(currentSymbol))
            {
                setClasification(Tipos.Numero);
                while (true)
                {
                    currentSymbol = (char)file.Peek();
                    if (char.IsDigit(currentSymbol))
                    {
                        word += currentSymbol;
                        file.Read();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //Fin de sentencia
            else if (currentSymbol == ';')
            {
                setClasification(Tipos.FinSentencia);
            }

            // Inicio de bloque
            else if (currentSymbol == '{')
            {
                setClasification(Tipos.InicioBloque);
            }

            //Fin de Bloque
            else if (currentSymbol == '}')
            {
                setClasification(Tipos.FinBloque);
            }

            //Operador de termino
            else if (currentSymbol == '+' || currentSymbol == '-')
            {
                currentSymbol = (char)file.Peek();
                //Operador de incremento
                if (currentSymbol == word[0] || currentSymbol == '=')
                {
                    setClasification(Tipos.IncrementoTermino);
                    word += currentSymbol;
                    file.Read();
                }

                //Operador de flecha
                else if (word == "-" && currentSymbol == '>')
                {
                    setClasification(Tipos.Flecha);
                    word += currentSymbol;
                    file.Read();
                }
                else
                {
                    setClasification(Tipos.OperadorTermino);
                }
            }

            //Operador ternario
            else if (currentSymbol == '?')
            {
                setClasification(Tipos.OperadorTernario);
            }

            //Operador de asignación (=)
            else if (currentSymbol == '=')
            {
                currentSymbol = (char)file.Peek();

                //Operador relacional (==)
                if (currentSymbol == word[0])
                {
                    setClasification(Tipos.Relacional);
                    word += currentSymbol;
                    file.Read();
                }
                else
                {
                    setClasification(Tipos.Asignacion);
                }
            }

            //Operador lógico (!)
            else if (currentSymbol == '!')
            {
                currentSymbol = (char)file.Peek();

                //Operador relacional (!=)
                if (currentSymbol == '=')
                {
                    setClasification(Tipos.Relacional);
                    word += currentSymbol;
                    file.Read();
                }
                else
                {
                    setClasification(Tipos.Logico);
                }
            }

            //Operador lógico (&&) o (||)
            else if (currentSymbol == '&' || currentSymbol == '|')
            {
                currentSymbol = (char)file.Peek();

                if (currentSymbol == word[0])
                {
                    word += currentSymbol;
                    setClasification(Tipos.Logico);
                    file.Read();
                }
            }

            //Operador relacional (>) o (<)
            else if (currentSymbol == '>' || currentSymbol == '<')
            {
                currentSymbol = (char)file.Peek();

                //Operador relacional (>=) o (<=)
                if (currentSymbol == '=')
                {
                    word += currentSymbol;
                    file.Read();
                }

                setClasification(Tipos.Relacional);
            }

            //Operador de factor (/) (*) (%)
            else if (currentSymbol == '/' || currentSymbol == '*' || currentSymbol == '%')
            {
                currentSymbol = (char)file.Peek();

                //Operador de incremento (/=) (*=) (%=)
                if (currentSymbol == '=')
                {
                    setClasification(Tipos.IncrementoFactor);
                    word += currentSymbol;
                    file.Read();
                }
                else
                {
                    setClasification(Tipos.OperadorFactor);
                }
            }

            else if (currentSymbol == '$')
            {
                setClasification(Tipos.Caracter);

                currentSymbol = (char)file.Peek();

                if (char.IsDigit(currentSymbol))
                {
                    setClasification(Tipos.Moneda);
                    
                    word += currentSymbol;
                    file.Read();

                    while (true)
                    {
                        currentSymbol = (char) file.Peek();

                        if(char.IsDigit(currentSymbol)) {
                            word += currentSymbol;
                            file.Read();
                        } else {
                            break;
                        }
                    }
                }
            }

            else {
                setClasification(Tipos.Caracter);
            }

            setContent(word);
            logger.WriteLine(word + " -> " + getClasification());
        }

        public void GetAllTokens()
        {
            while (!file.EndOfStream)
            {
                NextToken();
            }
        }
    }
}