using KDS.Loader;

namespace KDS.e8086Disassembler
{
    interface ICodeDisassembler
    {
        string Disassemble(ICodeLoader loader);
    }
}
