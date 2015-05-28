using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfig.Models
{
    public class NullExceptionPreventer : DynamicObject
    {
        // all member access to a NullExceptionPreventer will return a new NullExceptionPreventer
        // this allows for infinite nesting levels: var s = Obj1.foo.bar.bla.blubb; is perfectly valid
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new NullExceptionPreventer();
            return true;
        }

        public static implicit operator string(NullExceptionPreventer nep)
        {
            return string.Empty;
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public static implicit operator string[](NullExceptionPreventer nep)
        {
            return new string[] { };
        }
        
        public static implicit operator bool(NullExceptionPreventer nep)
        {
            return false;
        }
        public static implicit operator bool[](NullExceptionPreventer nep)
        {
            return new bool[] { };
        }
        public static implicit operator int[](NullExceptionPreventer nep)
        {
            return new int[] { };
        }
        public static implicit operator long[](NullExceptionPreventer nep)
        {
            return new long[] { };
        }
        public static implicit operator int(NullExceptionPreventer nep)
        {
            return 0;
        }
        public static implicit operator long(NullExceptionPreventer nep)
        {
            return 0;
        }

        public static implicit operator bool?(NullExceptionPreventer nep)
        {
            return null;
        }
        public static implicit operator int?(NullExceptionPreventer nep)
        {
            return null;
        }
        public static implicit operator long?(NullExceptionPreventer nep)
        {
            return null;
        }
    }
}
