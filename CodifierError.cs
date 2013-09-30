using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Codifier.Error
{
    public static partial class TokenizerAPI
    {
      
    }

    public class CodifierException : Exception, ISerializable /* for future purposes */
    {
        public CodifierException(string message) : base(@"TokenizerException: " + message) { }
    }

}
