using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderlandEncrypter
{
	internal class Program
	{
		private static void PauseKeyPress()
		{
#if DEBUG
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
#endif
		}

		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("WonderlandEncrypter <folder>");
				PauseKeyPress();
				Environment.Exit(1);
			}

			string path = args[0].Replace("\\", "/");
			string outPath = Path.Combine(path, "Output").Replace("\\", "/");

			string[] dirs = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).ToArray();
			foreach (string file in dirs) 
			{
				try
				{
					RenameFile(file.Replace('\\', '/'), path, outPath);
				}
				catch (UnknownExtensionException ex)
				{
					Console.WriteLine("Error: Unknown File Extension: " + ex.Message + " for " + file);
				}
			}
		}

		private static string CryptString(string input, bool encrypt = false)
		{
			StringBuilder sb = new StringBuilder();

			foreach (char c in input)
			{
				if (!char.IsLetter(c))
				{
					sb.Append(c);
					continue;
				}

				if (encrypt)
				{
					char newChar = (char)((byte)c + (byte)1);
					if (newChar == 0x7B)
						newChar = 'a';
					if (newChar == 0x5B)
						newChar = 'A';
					sb.Append(newChar);
				}
				else
				{
					char newChar = (char)((byte)c - (byte)1);
					if (newChar == 0x60)
						newChar = 'z';
					if (newChar == 0x40)
						newChar = 'Z';
					sb.Append(newChar);
				}
			}

			return sb.ToString();
		}

		private static void RenameFile(string file, string path, string outputPath)
		{
			string localPath = file.Replace(path, "");
			if (localPath.StartsWith("/"))
				localPath = localPath.Substring(1);

			if (localPath.StartsWith("Data/Editor/"))
				return;

			//Console.WriteLine("Path: " + localPath);

			int lastIndex = localPath.LastIndexOf("/");
			string fileDir;
			if (lastIndex == -1)
				fileDir = "";
			else
				fileDir = localPath.Substring(0, lastIndex);

			if (fileDir == "Data")
				return;

			string fileName = Path.GetFileNameWithoutExtension(localPath);
			string extension = Path.GetExtension(localPath);

			string encryptedName = CryptString(fileName, true);
			string encryptedExtension = DetectExtension(file);

			if (fileDir == "")
				Console.WriteLine("Processing: " + "<rootdir>" + " | " + fileName + extension + " | " + encryptedName + encryptedExtension);
			else
				Console.WriteLine("Processing: " + fileDir + " | " + fileName + extension + " | " + encryptedName + encryptedExtension);

			string outPath = Path.Combine(outputPath, fileDir);
			Directory.CreateDirectory(outPath);
			string outFile = Path.Combine(outPath, encryptedName + encryptedExtension).Replace("\\", "/");

			File.Copy(file, outFile, true);
		}

		private static string DetectExtension(string file)
		{
			string extension = Path.GetExtension(file).ToLower();
			if (extension == ".bmp" || extension == ".png" || extension == ".jpg"
				|| extension == ".wav" || extension == ".ogg")
			{
				return ".wdf";
			}
			else if (extension == ".b3d")
			{
				return ".wd1";
			}
			else if (extension == ".md2")
			{
				return ".wd2";
			}
			else if (extension == ".3ds")
			{
				return ".wd3";
			}
			else
			{
				throw new UnknownExtensionException(extension);
			}
		}
	}
}
