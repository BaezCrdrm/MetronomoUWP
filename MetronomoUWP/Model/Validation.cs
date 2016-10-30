using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetronomoUWP.Model
{
    public class Validation
    {
        public bool IsNumeric(string s)
        {
            int output;
            return int.TryParse(s, out output);
        }
    }
}
