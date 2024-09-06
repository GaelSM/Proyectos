using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lexico_1
{
    public class Lexico : Token, IDisposable
    {
        int lines;
        readonly StreamReader file;
        readonly StreamWriter logger;
        readonly StreamWriter assembly;
        public Lexico()
        {
            logger = new StreamWriter("./main.log");
            assembly = new StreamWriter("./main.asm");

            logger.AutoFlush = true;
            assembly.AutoFlush = true;

            if (File.Exists("./main.cpp"))
            {
                this.lines = 1;
                file = new StreamReader("./main.cpp");
            }
            else
            {
                throw new ErrorHandling("File main.cpp doesn´t exists", logger);
            }
        }

        public Lexico(string file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);

            if (!(Path.GetExtension(file) == ".cpp"))
            {
                throw new ErrorHandling("File doesn´t have correct extension .cpp");
            }

            logger = new StreamWriter("./" + fileName + ".log")
            {
                AutoFlush = true
            };

            if (!File.Exists(file))
            {
                throw new ErrorHandling("File " + file + " doesn´t exist", logger);
            }

            assembly = new StreamWriter("./" + fileName + ".asm")
            {
                AutoFlush = true
            };

            this.lines = 1;
            this.file = new StreamReader("./" + file);
        }

        public void Dispose()
        {
            logger.WriteLine("Number of lines: " + this.lines);
            logger.WriteLine("Destructor executed");

            file.Close();
            logger.Close();
            assembly.Close();
        }

        public void NextToken()
        {
            char currentSymbol;
            string word = "";

            //Leer espacios
            while (true)
            {
                currentSymbol = (char)file.Peek();

                if (currentSymbol == 10)
                {
                    this.lines++;
                }

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
                else if (word[0] == '<' && currentSymbol == '>')
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
            }

            else
            {
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