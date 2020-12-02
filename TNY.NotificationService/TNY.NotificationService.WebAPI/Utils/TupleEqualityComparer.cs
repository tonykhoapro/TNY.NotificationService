using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TNY.NotificationService.WebAPI.Utils
{
    public class TupleEqualityComparer : IEqualityComparer<Tuple<string, string>>
    {

        public bool Equals(Tuple<string, string> lhs, Tuple<string, string> rhs)
        {
            return
              StringComparer.InvariantCultureIgnoreCase.Equals(lhs.Item1, rhs.Item1)
           && StringComparer.InvariantCultureIgnoreCase.Equals(lhs.Item2, rhs.Item2);
        }


        public int GetHashCode(Tuple<string, string> tuple)
        {
            return StringComparer.CurrentCultureIgnoreCase.GetHashCode(tuple.Item1)
                 ^ tuple.Item2.GetHashCode();
        }
    }
}