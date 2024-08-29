using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Lexico_1
{
    public class Token
    {
        public enum Tipos
        {
            Indentificador, Numero, Caracter
        }
        private string content;
        private Tipos clasification;
        public Token()
        {
            this.content = "";
            this.clasification = Tipos.Indentificador;
        }

        public void setContent(string content) {
            this.content = content;
        }

        public void setClasification(Tipos clasification) {
            this.clasification = clasification;
        }

        public string getContent() {
            return this.content;
        }

        public Tipos getClasification() {
            return this.clasification;
        }
    }
}