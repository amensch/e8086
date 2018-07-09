
namespace KDS.e8086
{
    public enum SegmentOverrideState
    {
        UseCS,
        UseDS,
        UseSS,
        UseES,
        NoOverride
    };

    public interface IBus
    {
        ushort CS { get; set; }  // code segment
        ushort DS { get; set; }  // data segment
        ushort SS { get; set; }  // stack segment
        ushort ES { get; set; }  // extra segmemt
        ushort IP { get; set; }  // instruction pointer
        SegmentOverrideState SegmentOverride { get; set; }
        bool UsingBasePointer { get; set; }
        RAM ram { get; set; }

        void StoreString8(int offset, byte data);
        void StoreString16(int offset, ushort data);
        void SaveData(int word_size, int offset, int value);
        void SaveData8(int offset, byte value);
        void SaveData16(int offset, ushort value);

        ushort PopStack(int offset);
        void PushStack(int offset, ushort value);

        byte NextIP();

        void MoveString8(int src_offset, int dst_offset);
        void MoveString16(int src_offset, int dst_offset);

        byte GetDestString8(int offset);
        ushort GetDestString16(int offset);

        int GetData(int word_size, int offset);
        byte GetData8(int offset);
        ushort GetData16(int offset);
        ushort GetData16(int segment, int offset);
    }
}
