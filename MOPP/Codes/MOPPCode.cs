using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOPP.Codes
{
    class MOPPCode
    {
        public enum CodeType {
            Halo2,
            Halo3,
            Halo3ODST,
            HaloOnline,
            NumTypes
        }

        public CodeType Type;
        public int Code;
        public long Position;
        public virtual List<UInt32> Arguments { get { return Parameters; } }
        protected List<UInt32> Parameters = null;
        public int ArgCount { get { return Parameters == null ? 0 : Parameters.Count;  } }
        bool Transposed;


        public MOPPCode(CodeType type, int code, long position, params UInt32[] arguements)
        {
            Transposed = false;
            Type = type;
            Code = code;
            Position = position;
            if (arguements != null)
            {
                Parameters = new List<UInt32>(arguements);
            }

        }

        public MOPPCode(CodeType type, int code, long position, List<UInt32> arguements)
        {
            Transposed = false;
            Type = type;
            Code = code;
            Position = position;
            if (arguements != null)
            {
                Parameters = new List<UInt32>(arguements);
            }

        }

        public object Clone(long position, List<UInt32> arguements)
        {
            var type = this.GetType();
            var instance = Activator.CreateInstance(type, this.Type, this.Code, position, arguements) as MOPPCode;

            return instance;
        }

        public virtual MOPPCode Transform(CodeType new_type)
        {
            // Create a complete instance
            MOPPCode code = this.Clone(this.Position, this.Parameters) as MOPPCode;

            for (int i = 0; i < code.Parameters.Count; i++)
            {
                code.Parameters[i] = code.Parameters[i];
            }
            code.Transposed = true;

            // Error Checking
            for (int i = 0; i < code.Parameters.Count; i++)
            {
                var arguement = code.Parameters[i];
                if (arguement < 0 || arguement > 255)
                {
                    Console.WriteLine($"0x{code.Code}@0x{code.Position}:Arg[{i}] is invalid ({arguement})");
                }
            }

            return code;
        }

        public override string ToString()
        {
            var str = $"{Code.ToString("X")}";
            for (var i = 0; i < Parameters.Count; i++)
            {
                str += $" {Parameters[i].ToString("X")}";
            }
            return str;
        }
    }
}
