﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Indexeur
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                File.WriteAllText("files.csv", "");
                Console.WriteLine("Successffully written to files.csv, the results will be stored here.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("WARNING : couldn't write to files.csv, the index won't be saved. Please fix your authorizations before continuing.");
            }

            Console.Write("Directory to index : ");
            string path = Console.ReadLine();

            StringBuilder files_csv = new StringBuilder("file name\n");
            StringBuilder failed_directories_csv = new StringBuilder("directory,reason\n");
            Stack<string> directories = new Stack<string>();
            directories.Push(path);

            Console.WriteLine("Building index");

            ulong file_count = 0;
            ulong failed_directories_count = 0;
            while (directories.Count > 0)
            {
                string d = directories.Pop();

                try
                {
                    foreach (string file in Directory.GetFiles(d))
                    {
                        files_csv.Append('"');
                        files_csv.Append(file);
                        files_csv.Append("\"\n");

                        file_count += 1;

                        if (file_count % 1000 == 0)
                            Console.WriteLine(file_count.ToString() + " files indexed");
                    }

                    foreach (string directory in Directory.GetDirectories(d))
                        directories.Push(directory);
                }
                catch (UnauthorizedAccessException)
                {
                    failed_directories_csv.Append('"');
                    failed_directories_csv.Append(d);
                    failed_directories_csv.Append("\",authorization\n");

                    failed_directories_count += 1;
                }
                catch (DirectoryNotFoundException)
                {

                }
                catch (PathTooLongException)
                {
                    failed_directories_csv.Append('"');
                    failed_directories_csv.Append(d);
                    failed_directories_csv.Append("\",path too long\n");

                    failed_directories_count += 1;
                }
            }

            Console.WriteLine(file_count.ToString() + " files have been indexed");
            Console.WriteLine(failed_directories_count.ToString() + " access to directories have failed");

            try
            {
                File.WriteAllText("files.csv", files_csv.ToString());
                Console.WriteLine("Index has been written in files.csv");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Authorization required to write the output in files.csv");
            }

            try
            {
                File.WriteAllText("failed_directories.csv", failed_directories_csv.ToString());
                Console.WriteLine("Failed directories have been written in failed_directories.csv");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Authorization required to write the output in failed_directories.csv");
            }

            Console.WriteLine("Press any key to end the program");
            Console.ReadKey();
        }
    }
}
