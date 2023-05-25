namespace AC_lab5_;

using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static int Main(string[] args)
    {
        List<string> directoryPath = GetParameterValue(args, "--directory", "--filetype");
        List<string> fileType = GetParameterValue(args, "--filetype", "--directory");

        if (directoryPath.Count > 0)
        {
            Console.WriteLine("The name of the catalog/catalogs:");
            foreach (string path in directoryPath)
            {
                Console.WriteLine(path);
            }

            if (fileType.Count > 0)
            {
                Console.WriteLine("The type of the file/files to synchronize:");
                foreach (string type in fileType)
                {
                    Console.WriteLine(type);
                }

                if (Directory.Exists(directoryPath[0]) && Directory.Exists(directoryPath[1]))
                {
                    foreach (var ft  in fileType)
                    {
                        for (int i = 0; i + 1 < directoryPath.Count; i++)
                        {
                            if (Directory.Exists(directoryPath[i]) && Directory.Exists(directoryPath[i + 1]))
                            {
                                Console.WriteLine($"Syn. data of type {ft} in catalogs {directoryPath[i]} and {directoryPath[i+1]}"); 
                                string[]? first = SynDataInOneCat(directoryPath[i], ft);
                            // Console.WriteLine("Files:");
                            // string[] files = Directory.GetFiles(directoryPath[i]);
                            // foreach (string s in files)
                            // {
                            //     Console.WriteLine(s);
                            // }
                                string[]? second = SynDataInOneCat(directoryPath[i+1], ft);
                                if (first == null || second == null)
                                {
                                    Console.WriteLine($"There is no {ft} type of file in {directoryPath[i]} or {directoryPath[i+1]}");
                                }
                                else
                                {
                                    string filei = GetFileFromDirectory(directoryPath[i], ft);
                                    string filei1 = GetFileFromDirectory(directoryPath[i+1], ft);
                                    DateTime fileidate = File.GetLastWriteTime(filei);
                                    DateTime filei1date = File.GetLastWriteTime(filei1);
                                    if (fileidate > filei1date)
                                    {
                                        byte[] sourceFileData = File.ReadAllBytes(filei);
                                        string[] filestypeofFT = Directory.GetFiles(directoryPath[i+1], ft, SearchOption.AllDirectories);
                                        ReplaceWithMaxDate(filestypeofFT, sourceFileData, fileidate);
                                    }
                                    else
                                    {
                                        byte[] sourceFileData = File.ReadAllBytes(filei1);
                                        string[] filestypeofFT = Directory.GetFiles(directoryPath[i], ft, SearchOption.AllDirectories);
                                        ReplaceWithMaxDate(filestypeofFT, sourceFileData, filei1date);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("There is no such catalog/catalogs to syn.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("To synchronize data needs at least two existed catalogs");
                    return 3;
                }
            }
            else
            {
                Console.WriteLine("It's needed to point the type of files to synchronize --filetype.");
                return 2;
            }
        }
        else
        {
            Console.WriteLine("It's needed to point the catalog/catalogs --directory.");
            return 1;
        }

        return 0;
    }
    static List<string> GetParameterValue(string[] args, string parameterName, string parameterName2)
    {
        List<string> listofpar = new List<string>();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == parameterName)
            {
                while (i+1<args.Length && args[i+1]!=parameterName2)
                {
                    i = i + 1;
                    listofpar.Add(args[i]);
                }
            }
        }
        return listofpar;
    }

    static string []? SynDataInOneCat(string directoryPath, string fileType)
    {
        string[] filestypeofFT = Directory.GetFiles(directoryPath, fileType, SearchOption.AllDirectories);
        if (filestypeofFT.Length > 0)
        {
            DateTime maxDateTime = DateTime.MinValue;
            string maxDate = FindMaxDate(filestypeofFT, ref maxDateTime);
            byte[] sourceFileData = File.ReadAllBytes(maxDate);
            return ReplaceWithMaxDate(filestypeofFT, sourceFileData, maxDateTime);
        }
        else
        {
            return null;
        }
    }
    static string FindMaxDate(string [] filestypeofFT, ref DateTime maxDateTime)
    {
        DateTime[] dateTimes = new DateTime[7];

        int IndOfMax=0;

        for (int i = 0; i < filestypeofFT.Length; i++)
        {
            dateTimes[i] = File.GetLastWriteTime(filestypeofFT[i]);
            if (dateTimes[i] > maxDateTime)
            {
                maxDateTime = dateTimes[i];
                IndOfMax = i;
            }
        }
        return filestypeofFT[IndOfMax];
    }
    static string [] ReplaceWithMaxDate(string[] filestypeofFT, byte[] sourceFileData, DateTime maxDateTime)
    {
        foreach (string filePath in filestypeofFT)
        {
            File.WriteAllBytes(filePath, sourceFileData);
            File.SetLastWriteTime(filePath, maxDateTime);
        }
        return filestypeofFT;
    }
    static string GetFileFromDirectory(string directoryPath, string fileType)
    {
        string[] files = Directory.GetFiles(directoryPath, fileType, SearchOption.AllDirectories);
        return files[0];
    }
}