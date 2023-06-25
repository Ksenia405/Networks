using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Seti_Lab3
{
    class FileW
    {
        string filename;
        public byte[] message;
        public FileW() {
            filename = @"C:\Users\Ksenia\source\repos\Seti_Lab3\data.txt";
        }
        public void LoadData()
        {
                if (File.Exists(filename))
                {
                    Console.WriteLine("Файл найден");
                }

                 message = File.ReadAllBytes(filename);
        }
    }
}
