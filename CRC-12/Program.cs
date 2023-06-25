using System;
using System.Collections;

namespace Seti_Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] sourceBytes;
            FileW data = new FileW();
            data.LoadData();
            sourceBytes = data.message;
            Console.WriteLine("Исходные байты:");
            for (int i = 0; i < sourceBytes.Length; i++)
            {
                Console.Write($"{sourceBytes[i]:X} ");
            }
            Console.WriteLine();

            var bits = new BitArray(sourceBytes);

            Console.WriteLine("Последовательность битов: ");
            for (int i = 0; i < bits.Length; i++)
            {
                Console.Write($"{Convert.ToInt16(bits[i])} ");
               
            }
            Console.WriteLine();

            //Задание 1
            bool checkSum = bits[0];
            for(int i=1; i<bits.Length; i++)
            {
                checkSum = checkSum ^ bits[i];
            }
            Console.WriteLine($"Контроль по паритету, контрольная сумма: {Convert.ToInt16(checkSum)}\n\n");

            //Задание 2
            int column = sourceBytes.Length;
            int row = 8;
            bool[,] bitsTable = new bool[column+ 1, row+1];
            int count = 0;
            for(int k=0; k<column; k++)
            {
                for (int j = 0; j < row; j++) {
                    bitsTable[k, j] = bits[count];
                    count++;
                }
            }
            for (int i = 0; i < row; i++)
            {
                checkSum = bitsTable[0, i];
                for (int j = 1; j < column; j++)
                {
                    checkSum ^= bitsTable[j, i];
                }
                bitsTable[column, i] = checkSum;
            }
            for (int i = 0; i < column; i++)
            {
                checkSum = bitsTable[i, 0];
                for (int j = 1; j < row; j++)
                {
                    checkSum ^= bitsTable[i, j];
                }
                bitsTable[i, row] = checkSum;
            }

            Console.WriteLine("Вертикальный и горизонтальный контроль по паритету:");
            for (int i = 0; i < column + 1; i++)
            {
                if (i != column)
                    Console.Write($"{i + 1}:\t\t");
                else
                    Console.Write("КС:\t\t");
                for (int j = 0; j < row; j++)
                {
                    Console.Write($"{Convert.ToInt16(bitsTable[i, j])} ");
                }
                if (i != column)
                {
                    Console.WriteLine($"\t КС: {Convert.ToInt16(bitsTable[i, row])}");
                }
            }
            Console.WriteLine("\n\n");

            //Задание 3
            BitArray register = new BitArray(12);
            uint pol = 0x80F;
             BitArray polynom = new BitArray(BitConverter.GetBytes(pol));

            BitArray frame = new BitArray(bits.Length+12);
            bool leftbyte;

            Console.WriteLine("CRC- 12 DECT:");
            for (int i=0; i<register.Length; i++)
            {
                register[i] = Convert.ToBoolean(0);
            }

            int counter = 0;
            for (int i=0; i<sourceBytes.Length; i++)
            {
                for(int j=0; j<8; j++)
                {
                    frame[counter] = bitsTable[(sourceBytes.Length-1-i),j];
                    counter++;
                }
            }

            frame.LeftShift(12);

            for (int i=0; i<frame.Length; i++)
            {
                Console.Write($"{Convert.ToInt16(frame[i])} ");
            }

            Console.WriteLine('\n');

            for (int i=0; i<frame.Length; i++)
            {
                leftbyte = register[11];
                register=register.LeftShift(1);
                register[0] = frame[frame.Length-1-i];
                if (leftbyte == true)
                {
                    for (int k=0; k<register.Length; k++)
                    {
                        register[k] = register[k] ^ polynom[k];

                    }
                }
                /*Console.WriteLine();
                for (int m=0; m<register.Length; m++)
                {
                    Console.Write($"{Convert.ToInt16(register[m])} ");
                }*/
            }
           // Console.WriteLine();
            Console.WriteLine(" КС:");
            for (int i = register.Length-1; i >=0; i--) {
                Console.Write($" {Convert.ToInt16 (register[i])} "); 
            }
         
        }
    }
}
