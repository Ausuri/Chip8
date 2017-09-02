using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace Chip8Emulator
{
    class clsChip8
    {
        
        ushort opcode, I, pc, scale, stackPointer;
        ushort[] stack = new ushort[16];
        byte[] memory = new byte[4096];
        byte[] V, key = new byte[16];
        byte[] gfx = new byte[64 * 32];
        byte soundTimer, delayTimer, length = new byte();
        Boolean drawFlag;
        Random rnd = new Random();
        byte[] chip8_fontset = new byte[80]{
          0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
          0x20, 0x60, 0x20, 0x20, 0x70, // 1
          0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
          0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
          0x90, 0x90, 0xF0, 0x10, 0x10, // 4
          0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
          0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
          0xF0, 0x10, 0x20, 0x40, 0x40, // 7
          0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
          0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
          0xF0, 0x90, 0xF0, 0x90, 0x90, // A
          0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
          0xF0, 0x80, 0x80, 0x80, 0xF0, // C
          0xE0, 0x90, 0x90, 0x90, 0xE0, // D
          0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
          0xF0, 0x80, 0xF0, 0x80, 0x80};  // F



        void Chip8()
        {

        }

        private void setKeys()
        {
            throw new NotImplementedException();
        }

        private void drawGraphics()
        {

        }

        private void emulateCycle()
        {
            // Fetch opcode
            opcode = (byte)(memory[pc] << 8 | memory[pc + 1]);

            // Decode opcode
            switch (opcode & 0xF000)
            {
                case 0x0000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x00E0:
                            foreach (byte x in gfx)
                                gfx[x] = 0;
                            pc += 2;
                            break;

                        case 0x00EE:
                            Array.Clear(gfx, 0, 64 * 32);
                            pc += 2;
                            break;

                        default:
                            break;
                    }
                    break;

                case 0x1000:
                    pc = (byte)(opcode & 0x0FFF);
                    break;

                case 0x2000:
                    stack[stackPointer] = pc;
                    stackPointer++;
                    pc = (byte)(opcode & 0x0FFF);
                    break;

                case 0x3000:
                    if ((V[(opcode & 0x0F00) >> 8]) == (byte)(opcode & 0x00FF))
                        pc += 4;
                    else
                        pc += 2;
                    break;

                case 0x4000:
                    if ((V[(opcode & 0x0F00) >> 8]) != (byte)(opcode & 0x00FF))
                        pc += 4;
                    else
                        pc += 2;
                    break;

                case 0x5000:
                    if ((V[(opcode & 0x0F00) >> 8]) == (V[(opcode & 0x00F0) >> 4]))
                        pc += 4;
                    else
                        pc += 2;
                    break;

                case 0x6000:
                    V[(opcode & 0x0F00) >> 8] = (byte)(opcode & 0x00FF);
                    pc += 2;
                    break;

                case 0x7000:
                    V[(opcode & 0x0F00) >> 8] += (byte)(opcode & 0x00FF);
                    pc += 2;
                    break;

                case 0x8000:

                    switch (opcode & 0x000F)
                    {
                        case 0x0000:
                            V[(opcode & 0x0F00) >> 8] = V[(opcode & 0x00F0) >> 4];
                            pc += 2;
                            break;

                        case 0x0001:
                            V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] | V[(opcode & 0x00F0) >> 4]);
                            pc += 2;
                            break;

                        case 0x0002:
                            V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] & V[(opcode & 0x00F0) >> 4]);
                            pc += 2;
                            break;

                        case 0x0003:
                            V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] ^ V[(opcode & 0x00F0) >> 4]);
                            pc += 2;
                            break;

                        case 0x0004:
                            if (V[(opcode & 0x0F00) >> 8] + V[(opcode & 0x00F0) >> 4] > 255)
                            {
                                V[0xF] = 1;
                                V[(opcode & 0x0F00) >> 8] = 255;
                            }
                            else
                                V[(opcode & 0x0F00) >> 8] += V[(opcode & 0x00F0) >> 4];
                            pc += 2;
                            break;

                        case 0x0005:
                            if (V[(opcode & 0x0F00) >> 8] > V[(opcode & 0x00F0) >> 4])
                                V[0xF] = 1;
                            V[(opcode & 0x0F00) >> 8] -= V[(opcode & 0x00F0) >> 4];
                            pc += 2;
                            break;

                        case 0x0006:
                            if ((V[(opcode & 0x0F00) >> 8] & 1) == 1)
                                V[0xF] = 1;
                            V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] >> 1);
                            pc += 2;
                            break;

                        case 0x0007:
                            if (V[(opcode & 0x00F0) >> 4] > V[(opcode & 0x0F00) >> 8])
                                V[0xF] = 1;
                            V[(opcode & 0x00F0) >> 4] -= V[(opcode & 0x0F00) >> 8];
                            pc += 2;
                            break;
                        case 0x000E:
                            if ((V[(opcode & 0x0F00) >> 8] & 1) == 1)
                                V[0xF] = 1;
                            V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] << 1);
                            pc += 2;
                            break;
                    }
                    break;

                case 0x9000:
                    if (V[(opcode & 0x0F00) >> 8] != V[(opcode & 0x00F0) >> 4])
                        pc += 2;
                    pc += 2;
                    break;

                case 0xA000:
                    I = (byte)(opcode & 0x0FFF);
                    pc += 2;
                    break;

                case 0xB000:
                    pc = (byte)(opcode & 0x0FFF);
                    pc += V[0];
                    break;

                case 0xC000:
                    byte rNumber = generateRandom();
                    V[(opcode & 0xF00) >> 8] = (byte)(rNumber & (opcode & 0x00FF));
                    pc += 2;
                    break;

                case 0xD000:
                    ushort xPoint = V[(opcode & 0x0F00) >> 8];
                    ushort yPoint = V[(opcode & 0x00F0) >> 4];
                    ushort height = (byte)(opcode & 0x000F);
                    ushort pixel;

                    V[0xF] = 0;
                    for (int yline = 0; yline < height; yline++)
                    {
                        pixel = memory[I + yline];
                        for (int xline = 0; xline < 8; xline++)
                        {
                            if ((pixel & (0x80 >> xline)) != 0)
                            {
                                if (gfx[(xPoint + xline + ((yPoint + yline) * 64))] == 1)
                                    V[0xF] = 1;
                                gfx[xPoint + xline + ((yPoint + yline) * 64)] ^= 1;
                            }
                        }
                    }
                    drawFlag = true;
                    pc += 2;
                    break;

                case 0xE000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x009E:
                            if (key[V[(opcode & 0x0F00) >> 8]] == 1)
                                pc += 4;
                            else
                                pc += 2;
                            break;

                        case 0x00A1:
                            if (key[V[(opcode & 0x0F00) >> 8]] == 0)
                                pc += 4;
                            else
                                pc += 2;
                            break;
                    }
                    break;

                case 0xF000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x0007:
                            V[(opcode & 0xF00) >> 8] = delayTimer;
                            pc += 2;
                            break;

                        case 0x000A:
                            break;

                        case 0x0015:
                            delayTimer = V[(opcode & 0xF00) >> 8];
                            pc += 2;
                            break;

                        case 0x0018:
                            soundTimer = V[(opcode & 0xF00) >> 8];
                            pc += 2;
                            break;

                        case 0x001E:
                            I = (byte)(I + V[(opcode & 0xF00) >> 8]);
                            pc += 2;
                            break;

                        case 0x0029:
                            stackPointer = (byte)((opcode & 0x0F00 >> 8) * 5);
                            break;

                        case 0x0033:
                            memory[I] = (byte)(V[((opcode & 0x0F00) >> 8)] / 100);
                            memory[I + 1] = (byte)((V[((opcode & 0x0F00) >> 8)] / 10) % 10);
                            memory[I + 2] = (byte)((V[((opcode & 0x0F00) >> 8)] % 100) % 10);
                            pc += 2;
                            break;

                        case 0x0055:
                            length = (byte)((opcode & 0xF00) >> 8);
                            for (int i = 0; i <= length; i++)
                                memory[I + i] = V[i];
                            pc += 2;
                            length = 0;

                            break;

                        case 0x0065:
                            length = (byte)((opcode & 0xF00) >> 8);
                            for (int i = 0; i <= length; i++)
                                V[i] = memory[I + i];
                            pc += 2;
                            length = 0;
                            break;


                    }
                    break;

                default:
                    Console.WriteLine("Unknown opcode: 0x%X\n", opcode);
                    break;
            }

            // Update timers
            if (delayTimer > 0)
                --delayTimer;

            if (soundTimer > 0)
            {
                if (soundTimer == 1)
                    Console.WriteLine("Beep\n");
                --soundTimer;
            }
        }

        public void initialize()
        {
            pc = 0x200;  // Program counter starts at 0x200
            opcode = 0;      // Reset current opcode   
            I = 0;      // Reset index register
            stackPointer = 0;      // Reset stack pointer

            // Load fontset
            for (int i = 0; i < 80; ++i)
                memory[i] = chip8_fontset[i];
        }

        private byte generateRandom()
        {
            byte rNumber = new byte();
            rNumber = (byte)rnd.Next(0, 255);
            return rNumber;
        }

        public void loadGame(string p)
        {
            throw new NotImplementedException();
        }

        private void setupInput()
        {
            throw new NotImplementedException();
        }

        private void setupGraphics()
        {
            throw new NotImplementedException();
        }
    }
}