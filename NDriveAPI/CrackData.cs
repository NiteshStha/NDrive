using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDriveAPI
{
    public class CrackData
    {
        public string GameName { get; set; }
        public DateTime CrackDate { get; set; }
        public bool Cracked => DateTime.Now > CrackDate;
    }
}
