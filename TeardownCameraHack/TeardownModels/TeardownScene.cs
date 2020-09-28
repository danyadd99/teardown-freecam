using Squalr.Engine.Memory;

namespace TeardownCameraHack.TeardownModels
{
    public class TeardownScene
    {
        private readonly ulong _address;

        private TeardownLight _light;

        public TeardownLight FlashLight
        {
            get
            {
                if (_light != null)
                {
                    return _light;
                }
                return _light = new TeardownLight(Reader.Default.Read<ulong>(_address + 0xE8, out _));
            }
        }

        public int TotalProjectiles
        {
            get => Reader.Default.Read<int>(_address + 0xB0, out _);
            set => Writer.Default.Write(_address + 0xB0, value);
        }

        public int ProjectileArraySize
        {
            get => Reader.Default.Read<int>(_address + 0xB4, out _);
            set => Writer.Default.Write(_address + 0xB4, value);
        }

        public TeardownProjectile[] Projectiles
        {
            get
            {
                var totalProjectiles = TotalProjectiles;
                var projectiles = new TeardownProjectile[totalProjectiles];
                var projectileArrayAddress = Reader.Default.Read<ulong>(_address + 0xB8, out _);
                for (var projectileIndex = 0; projectileIndex < totalProjectiles; ++projectileIndex)
                {
                    projectiles[projectileIndex] = new TeardownProjectile(projectileArrayAddress + (ulong)(TeardownProjectile.StructureSize * projectileIndex));
                }
                return projectiles;
            }
        }

        public TeardownScene(ulong address)
        {
            _address = address;
        }
    }
}
