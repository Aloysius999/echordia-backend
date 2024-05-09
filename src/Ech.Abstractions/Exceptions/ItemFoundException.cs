using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Ech.Abstractions.Exceptions
{
    public class ItemFoundException : Exception
    {
        public ItemFoundException(string message) :
            base(message)
        {
        }
    }
}
