using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Linq.Expressions;

namespace Lexico_3
{
    public class Lexico : Token, IDisposable
    {
        int lines;
        const int F = -1;
        const int E = -2;
        readonly StreamReader file;
        readonly StreamWriter logger;
        readonly StreamWriter assembly;
        readonly int[,] TRAND = {
            {0, 1, 2, 3},
            {F, 1, 1, F},
            {F, F, 2, F},
            {F, F, F, F},
        };
        
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

        private int Column(char c)
        {
            if (char.IsWhiteSpace(c))
            {
                return 0;
            }
            else if (char.IsLetter(c))
            {
                return 1;
            }
            else if (char.IsDigit(c))
            {
                return 2;
            }
            return 3;
        }

        private void Classify(int state)
        {
            switch (state)
            {
                case 1: setClasification(Tipos.Indentificador); break;
                case 2: setClasification(Tipos.Numero); break;
                case 3: setClasification(Tipos.Caracter); break;
            }
        }

        public void NextToken()
        {
            char c;
            string buffer = "";
            int state = 0;

            while (state >= 0)
            {
                if (state == 0)
                {
                    buffer = "";
                }

                c = (char)file.Peek();

                state = TRAND[state, Column(c)];
                Classify(state);

                if (state >= 0)
                {
                    file.Read();
                    if (c == '\n')
                    {
                        lines++;
                    }

                    if (state > 0)
                    {
                        buffer += c;
                    }
                }
            }

            if (state == E)
            {
                string message;

                if (getClasification() == Tipos.Numero)
                {
                    message = "Léxico, se espera un dígito";
                }
                else if (getClasification() == Tipos.Cadena)
                {
                    message = "Léxico, se espera a que se cierre la cadena";
                }
                else if (getClasification() == Tipos.Caracter)
                {
                    message = "Léxico, caracter invalido";
                }
                else
                {
                    message = "No se ha cerrado el comentario";
                }

                throw new ErrorHandling(message, logger, lines);
            }

            setContent(buffer);
            logger.WriteLine(buffer + " ---- " + getClasification());
        }

        public void GetAllTokens()
        {
            while (!file.EndOfStream)
            {
                NextToken();
            }
        }

        public bool EndOfFile()
        {
            return file.EndOfStream;
        }
    }
}

/*

    EXPRESIÓN REGULAR
    Es un método formal el cual a través de una secuencia de 
    carácteres define un patrón de búsqueda.

    a) Reglas BNF
    b) Reglas BNF extendidas
    c) Operaciones aplicadas al lenguaje

        Operaciones Aplicadas al Lenguaje (OAF)

        1. Concactenación simple. (.)
        2. Concatenación exponencial. (^)
        3. Cerradura de Kleene. (*)
        4. Cerradura positiva. (+)
        5. Cerradura Epsilon. (?)
        6. Operador  (|)
        7. Parentesis, agrupación. ()

        L = {A, B, C, D, ..., Z, a, b, c, d, ... , z}
        D = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}

        1. L.D, LD
        2. L^3 = LLL, D^5 = DDDDD
        3. L* = Cero o más
        4. L+ = Una o más
        5. L? = Cero o una vez (Opcional)
        6. L | D = Letra o Digíto
        7. (LD)L? = Letra seguido de dígito y una letra opcional

        Producción Gramátical

        Clasificación del token -> Expresion regular

        Identificador -> L(L|D)*
        Número -> D+(.D+)?(E(+|-)?D+)?
        Fin de Sentencia -> ;
        Inicio de Bloque -> {
        Fin de Bloque -> }
        Operador Ternario -> ?
        Operador de Término -> +|-
        Operador de Factor -> *|/|%
        Incremento de Término -> (+|-)((+|-)|=)
        Incremento de Factor -> (*|/|%)=
        Operador Lógico -> &&||||!
        Operador Relacional -> >=?|<(>|=)?|==|!=
        Puntero -> ->
        Asignación -> =
        Cadena -> C*
        Caracter -> 'C'|#D*|Lambda

    AUTÓMATA
    Modelo matemático que representa una expresión regular a través
    de una grafo que consiste en un conjunto de estados bien definidos, 
    un estado inicial, un alfabeto de entrada y una función de transición.

*/