using System;
using System.Collections.Generic;
using System.Text;

namespace Lab3Cifrado
{
    class Model
    {
        public class Persona
        {
            public string name { get; set; }
            public string dpi { get; set; }
            public string datebirth { get; set; }
            public string address { get; set; }
            public List<string> companies { get; set; }

        }
    }
}
