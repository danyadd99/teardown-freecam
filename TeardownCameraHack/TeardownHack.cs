using System;
using System.Diagnostics;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using Squalr.Engine.Memory;
using Squalr.Engine.OS;

namespace TeardownCameraHack
{
    public class TeardownHack
    {
        private static readonly float TickRate = 1.0f / 60.0f;
        private static readonly float NormalCameraSpeed = 5.0f;
        private static readonly float FastCameraSpeed = 25.0f;
        private static readonly float TurnSpeed = (float)Math.PI * 0.5f;
        private static readonly float LightColorChangeAmount = 25.0f;
        private static readonly float FireSizeChangeAmount = 1.0f;

        private readonly InputSimulator _inputSimulator;
        private readonly ulong _teardownBaseAddress;

        public TeardownHack(Process teardownProcess)
        {
            _inputSimulator = new InputSimulator();
            _teardownBaseAddress = (ulong)teardownProcess.MainModule.BaseAddress;
            Processes.Default.OpenedProcess = teardownProcess;
        }

        public void Start()
        {
            DisplayInstructions();
            ApplyPatches();
            HackLoop();
        }

        private void DisplayInstructions()
        {
            Console.WriteLine("Teardown Camera Hack by Xorberax");
            Console.WriteLine("Use WASD/QE/ZX/Shift to move.");
            Console.WriteLine("Use Up/Down arrows to change fire size.");
            Console.WriteLine("Use 1,2,3,4,5,6 to change the flashlight color.");
            Console.WriteLine("Use 7 to change the projectile type.");
        }

        private void HackLoop()
        {
            var settings = new TeardownSettings(_teardownBaseAddress);
            var input = new TeardownInput(Reader.Default.Read<ulong>(_teardownBaseAddress + 0x3E8E10, out _));
            var scene = new TeardownScene(Reader.Default.Read<ulong>(Reader.Default.Read<ulong>(_teardownBaseAddress + 0x3E8B60, out _), out _));
            var camera = new TeardownCamera(_teardownBaseAddress + 0x003E2528);
            float localX = 0;
            float localZ = 0;
            camera.RotationY = 0;
            var stopwatch = Stopwatch.StartNew();
            while (true)
            {
                var deltaTime = stopwatch.ElapsedMilliseconds / 1000.0f;
                if (deltaTime < TickRate)
                {
                    continue;
                }
                stopwatch.Restart();

                var shouldUseFastCameraSpeed = _inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT);
                var cameraMovementSpeed = shouldUseFastCameraSpeed ? FastCameraSpeed : NormalCameraSpeed;

                // camera position
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_W))
                {
                    // localZ += cameraMovementSpeed * deltaTime;
                    localZ += (float)Math.Cos(camera.RotationY) * cameraMovementSpeed * deltaTime;
                    localX += (float)Math.Sin(camera.RotationY) * cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_S))
                {
                    // localZ -= cameraMovementSpeed * deltaTime;
                    localZ -= (float)Math.Cos(camera.RotationY) * cameraMovementSpeed * deltaTime;
                    localX -= (float)Math.Sin(camera.RotationY) * cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_A))
                {
                    // localX += cameraMovementSpeed * deltaTime;
                    localZ -= (float)Math.Sin(camera.RotationY) * cameraMovementSpeed * deltaTime;
                    localX += (float)Math.Cos(camera.RotationY) * cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_D))
                {
                    localZ += (float)Math.Sin(camera.RotationY) * cameraMovementSpeed * deltaTime;
                    localX -= (float)Math.Cos(camera.RotationY) * cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_Q))
                {
                    camera.PositionY += cameraMovementSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_E))
                {
                    camera.PositionY -= cameraMovementSpeed * deltaTime;
                }
                
                camera.PositionX = localX * (float)Math.Cos(camera.RotationY) - localZ * (float)Math.Sin(camera.RotationY);
                camera.PositionZ = localX * (float)Math.Sin(camera.RotationY) + localZ * (float)Math.Cos(camera.RotationY);

                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_H))
                {
                    Thread.Sleep(500);
                    camera.RotationY += 1.57f;
                }
                // camera rotation
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_Z))
                {
                    camera.RotationY += TurnSpeed * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_X))
                {
                    camera.RotationY -= TurnSpeed * deltaTime;
                }

                // settings
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.UP))
                {
                    settings.FireSize += FireSizeChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.DOWN))
                {
                    settings.FireSize -= FireSizeChangeAmount * deltaTime;
                }

                // light color
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_1))
                {
                    scene.Light.Red -= LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_2))
                {
                    scene.Light.Red += LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_3))
                {
                    scene.Light.Green -= LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_4))
                {
                    scene.Light.Green += LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_5))
                {
                    scene.Light.Blue -= LightColorChangeAmount * deltaTime;
                }
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_6))
                {
                    scene.Light.Blue += LightColorChangeAmount * deltaTime;
                }

                // change projectile type
                if (_inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_7))
                {
                    Console.Beep(500, 200); // HACK: utilize the beep to notify the player that the type changed, and to delay the keystrokes, preventing the types from cycling quickly -- replace this with a keypress/key-up check instead
                    settings.BulletType = (TeardownProjectileType)(((byte)settings.BulletType + 1) % Enum.GetValues(typeof(TeardownProjectileType)).Length);
                }
            }
        }

        private void ApplyPatches()
        {
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x312D1, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }); // pause camera path
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x2E750, new byte[] { 0x90, 0x90, 0x90, 0x90 }); // prevent camera position assignment
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x2E73C, new byte[] { 0x90, 0x90, 0x90, 0x90 }); // prevent camera rotation assignment
            Writer.Default.WriteBytes(_teardownBaseAddress + 0x2E74C, new byte[] { 0x90, 0x90, 0x90, 0x90 }); // prevent camera rotation assignment
            // Writer.Default.WriteBytes(_teardownBaseAddress + 0xC6989, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); // prevent light position assignment
            // Writer.Default.WriteBytes(_teardownBaseAddress + 0xC698E, new byte[] { 0x90, 0x90, 0x90 }); // prevent light position assignment
            // Writer.Default.WriteBytes(_teardownBaseAddress + 0xC6989, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); // prevent player position assignment
            // Writer.Default.WriteBytes(_teardownBaseAddress + 0xC698E, new byte[] { 0x90, 0x90, 0x90 }); // prevent player position assignment
        }
    }
}
