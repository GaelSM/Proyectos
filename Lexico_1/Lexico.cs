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
                lines = 1;
                file = new StreamReader("./main.cpp");
            }
            else
            {
                throw new ErrorHandling("File main.cpp doesn´t exists", logger);
            }
        }

        public Lexico(string file)
        {
            if (!(Path.GetExtension(file) == ".cpp"))
            {
                throw new ErrorHandling("File doesn´t have correct extension .cpp");
            }

            string fileName = Path.GetFileNameWithoutExtension(file);

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

            lines = 1;
            this.file = new StreamReader("./" + file);
        }

        public void Dispose()
        {
            file.Close();
            logger.Close();
            assembly.Close();
        }

        public void NextToken()
        {
            char currentSymbol;
            string word = "";

            //Leer espacios
            while (char.IsWhiteSpace(currentSymbol = (char) file.Peek()))
            {
                if (currentSymbol == 10)
                {
                    lines++;
                }

                file.Read();
            }

            word += currentSymbol;
            file.Read();

            //Identificador
            if (char.IsLetter(currentSymbol))
            {
                setClasification(Tipos.Indentificador);
                while (char.IsLetterOrDigit(currentSymbol = (char) file.Peek()))
                {
                    word += currentSymbol;
                    file.Read();
                }
            }

            //Número
            else if (char.IsDigit(currentSymbol))
            {
                setClasification(Tipos.Numero);

                while (char.IsDigit(currentSymbol = (char) file.Peek()))
                {
                    word += currentSymbol;
                    file.Read();
                }

                //Parte fraccionaria
                if (currentSymbol == '.')
                {
                    word += currentSymbol;
                    file.Read();

                    currentSymbol = (char) file.Peek();

                    if (!char.IsDigit(currentSymbol))
                    {
                        throw new ErrorHandling("Invalid number", logger, lines);
                    }

                    while (char.IsDigit(currentSymbol = (char) file.Peek()))
                    {
                        word += currentSymbol;
                        file.Read();
                    }
                }

                //Parte científica
                if (char.ToLower(currentSymbol = (char) file.Peek()) == 'e')
                {
                    word += currentSymbol;
                    file.Read();

                    currentSymbol = (char) file.Peek();

                    if(currentSymbol == '+' || currentSymbol == '-') {
                        word += currentSymbol;
                        file.Read();
                    }

                    if(!char.IsDigit((char) file.Peek())) {
                        throw new ErrorHandling("Invalid number", logger, lines);
                    }

                    while (char.IsDigit(currentSymbol = (char)file.Peek()))
                    {
                        word += currentSymbol;
                        file.Read();
                    }
                }
            }

            // Fin de sentencia
            else if (currentSymbol == ';')
            {
                setClasification(Tipos.FinSentencia);
            }

            // Inicio de bloque
            else if (currentSymbol == '{')
            {
                setClasification(Tipos.InicioBloque);
            }

            // Fin de Bloque
            else if (currentSymbol == '}')
            {
                setClasification(Tipos.FinBloque);
            }

            // Operador de termino
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

            // Operador ternario
            else if (currentSymbol == '?')
            {
                setClasification(Tipos.OperadorTernario);
            }

            // Operador de asignación (=)
            else if (currentSymbol == '=')
            {
                currentSymbol = (char)file.Peek();

                //Operador relacional (==)
                if (currentSymbol == word[0])
                {
                    setClasification(Tipos.OperadorRelacional);
                    word += currentSymbol;
                    file.Read();
                }
                else
                {
                    setClasification(Tipos.Asignacion);
                }
            }

            // Operador lógico (!)
            else if (currentSymbol == '!')
            {
                currentSymbol = (char)file.Peek();

                //Operador relacional (!=)
                if (currentSymbol == '=')
                {
                    setClasification(Tipos.OperadorRelacional);
                    word += currentSymbol;
                    file.Read();
                }
                else
                {
                    setClasification(Tipos.OperadorLogico);
                }
            }

            // Operador lógico (&&) o (||)
            else if (currentSymbol == '&' || currentSymbol == '|')
            {
                setClasification(Tipos.Caracter);

                currentSymbol = (char)file.Peek();

                if (currentSymbol == word[0])
                {
                    word += currentSymbol;
                    setClasification(Tipos.OperadorLogico);
                    file.Read();
                }
            }

            // Operador relacional (>) o (<)
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

                setClasification(Tipos.OperadorRelacional);
            }

            // Operador de factor (/) (*) (%)
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

            // Moneda
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

            // Cadena
            else if (currentSymbol == '"')
            {
                setClasification(Tipos.Cadena);

                if ((currentSymbol = (char)file.Peek()) == '"')
                {
                    word += currentSymbol;
                    file.Read();
                }
                else
                {

                    while (!((currentSymbol = (char)file.Peek()) == '"'))
                    {
                        if (file.EndOfStream)
                        {
                            throw new ErrorHandling("String not closed", logger, lines);
                        }

                        word += currentSymbol;
                        file.Read();
                    }

                    word += currentSymbol;
                    file.Read();
                }
            }

            // Caracter
            else if (currentSymbol == '\'')
            {
                setClasification(Tipos.Caracter);

                currentSymbol = (char) file.Read();
                word += currentSymbol;

                currentSymbol = (char) file.Peek();

                if(currentSymbol != '\'') {
                    throw new ErrorHandling("Invalid caracter", logger, lines);
                }

                word += currentSymbol;
                file.Read();
            }

            // Caracter
            else if(currentSymbol == '#') {
                setClasification(Tipos.Caracter);

                while(char.IsDigit(currentSymbol = (char) file.Peek())) {
                    word += currentSymbol;
                    file.Read();
                }
            }

            else
            {
                setClasification(Tipos.Caracter);
            }

            setContent(word);
            logger.WriteLine(word + " ---- " + getClasification());
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