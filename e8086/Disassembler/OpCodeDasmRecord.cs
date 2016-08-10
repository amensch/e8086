using System;

namespace KDS.e8086
{
    public class OpCodeDasmRecord
    {
        public ushort op { get; set; }
        public string addr_byte { get; set; }
        public string next_bytes { get; set; }
        public string op_name { get; set; }
        public string op_fmt_1 { get; set; }
        public string op_fmt_2  { get; set; }

        public OpCodeDasmRecord(ushort _op, string _addr_byte, string _next_bytes, string _op_name, string _op_fmt_1, string _op_fmt_2)
        {
            op = _op;
            addr_byte = _addr_byte;
            next_bytes = _next_bytes;
            op_name = _op_name;
            op_fmt_1 = _op_fmt_1;
            op_fmt_2 = _op_fmt_2;
        }
    }
}
