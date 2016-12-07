using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

using QRCoder;

namespace qrtool
{
    class Program
    {
        static Bitmap GenerateQRCode(byte[] data, int ppm = 4)
        {
            using (var generator = new QRCodeGenerator())
            using (var qr_data = generator.CreateQRCode(data))
            using (var qr_code = new QRCode(qr_data))
                return qr_code.GetGraphic(ppm);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("qrtool v1.0");
            Console.WriteLine("SciresM, 2016");
            Console.WriteLine();
            if (args.Length < 1)
            {
                Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} input.bin [-o output.png] [-ppm n]");
                return;
            }

            var in_fn = args[0];
            var out_fn = Path.Combine(Path.GetDirectoryName(in_fn), Path.GetFileNameWithoutExtension(in_fn) + ".png");
            var ppm = 4;

            for (var i = 1; i < args.Length - 1; i++)
            {
                if (args[i] == "-o")
                    out_fn = args[i + 1];
                else if (args[i] == "-ppm")
                {
                    try
                    {
                        ppm = int.Parse(args[i + 1]);
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine($"Error: {args[i + 1]} is an invalid ppm.");
                        return;
                    }
                }
            }

            var dat = new byte[0];

            try
            {
                dat = File.ReadAllBytes(in_fn);
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is IOException)
            {
                Console.WriteLine($"Error: could not read {in_fn}.");
                return;
            }

            if (dat.Length > 0x1000)
            {
                Console.WriteLine($"Error: {in_fn} is too big to make a QR code!");
                return;
            }

            try
            {
                using (var qr = GenerateQRCode(dat, ppm))
                {
                    qr.Save(out_fn, ImageFormat.Png);
                }
                Console.WriteLine($"Saved QR code to {out_fn}!");
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is IOException)
            {
                Console.WriteLine($"Error: Failed to save QR code to {out_fn}.");
                return;
            }
        }
    }
}
