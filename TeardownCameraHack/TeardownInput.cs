using Squalr.Engine.Memory;

namespace TeardownCameraHack
{
    public class TeardownInput
    {
        private readonly ulong _address;

        public int MouseScreenPositionX
        {
            get => Reader.Default.Read<int>(_address + 0x0, out _);
            set => Writer.Default.Write(_address + 0x0, value);
        }

        public int MouseScreenPositionY
        {
            get => Reader.Default.Read<int>(_address + 0x4, out _);
            set => Writer.Default.Write(_address + 0x4, value);
        }

        public int ClickedWindowPositionX
        {
            get => Reader.Default.Read<int>(_address + 0x428, out _);
            set => Writer.Default.Write(_address + 0x428, value);
        }

        public int ClickedWindowPositionY
        {
            get => Reader.Default.Read<int>(_address + 0x42C, out _);
            set => Writer.Default.Write(_address + 0x42C, value);
        }

        public TeardownInput(ulong address)
        {
            _address = address;
        }
    }
}
