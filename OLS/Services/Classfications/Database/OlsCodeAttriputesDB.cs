using OLS.Services.Classfications.Database.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database
{
    public class OlsCodeAttriputesDB
    {
        public ClassA_DB classA_DB { get; set; }
        public ClassB_DB classB_DB { get; set; }
        public ClassC_DB classC_DB { get; set; }
        public ClassD_DB classD_DB { get; set; }

        public OlsCodeAttriputesDB()
        {
            classA_DB = new ClassA_DB();
            classB_DB = new ClassB_DB();
            classC_DB = new ClassC_DB();
            classD_DB = new ClassD_DB();
        }
    }
}
