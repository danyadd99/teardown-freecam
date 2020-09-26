using Squalr.Engine.Memory;

namespace TeardownCameraHack
{
    public class TeardownCamera
    {
        private readonly ulong _cameraBaseAddress;

        public double Degrees;

        public bool IsFogEnabled
        {
            get => !Reader.Default.Read<bool>(_cameraBaseAddress + 0x1C, out _);
            set => Writer.Default.Write(_cameraBaseAddress + 0x1C, !value);
        }

        // valid range -1.0 through 0.0 (?)
        public float DrawDistance
        {
            get => Reader.Default.Read<float>(_cameraBaseAddress + 0x1AC, out _);
            set => Writer.Default.Write(_cameraBaseAddress + 0x1AC, value);
        }

        public float PositionX
        {
            get => Reader.Default.Read<float>(_cameraBaseAddress + 0x1E4, out _);
            set => Writer.Default.Write(_cameraBaseAddress + 0x1E4, value);
        }

        public float PositionY
        {
            get => Reader.Default.Read<float>(_cameraBaseAddress + 0x1E8, out _);
            set => Writer.Default.Write(_cameraBaseAddress + 0x1E8, value);
        }

        public float PositionZ
        {
            get => Reader.Default.Read<float>(_cameraBaseAddress + 0x1EC, out _);
            set => Writer.Default.Write(_cameraBaseAddress + 0x1EC, value);
        }

        public float Rotation1
        {
            get => Reader.Default.Read<float>(_cameraBaseAddress + 0x1B4, out _);
            set => Writer.Default.Write(_cameraBaseAddress + 0x1B4, value);
        }

        public float Rotation2
        {
            get => Reader.Default.Read<float>(_cameraBaseAddress + 0x1DC, out _);
            set => Writer.Default.Write(_cameraBaseAddress + 0x1DC, value);
        }

        public float Rotation3
        {
            get => Reader.Default.Read<float>(_cameraBaseAddress + 0x1BC, out _);
            set => Writer.Default.Write(_cameraBaseAddress + 0x1BC, value);
        }

        public float Rotation4
        {
            get => Reader.Default.Read<float>(_cameraBaseAddress + 0x1D4, out _);
            set => Writer.Default.Write(_cameraBaseAddress + 0x1D4, value);
        }

        public TeardownCamera(ulong pointer)
        {
            _cameraBaseAddress = Reader.Default.Read<ulong>(pointer, out _);
        }
    }
}
