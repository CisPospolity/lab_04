using System;
using System.Collections.Generic;
using System.IO;

namespace lab_04
{
    class Program
    {
        //Użycie Dictionary aby łatwo przypisać rozszerzenie do odpowiedniego typu
        public static Dictionary<string, FileType> extentionType = new Dictionary<string, FileType>();

        static void InitDictionary()
        {
            extentionType.Add("png", FileType.image);
            extentionType.Add("webp", FileType.image);
            extentionType.Add("jpg", FileType.image);
            extentionType.Add("gif", FileType.image);
            extentionType.Add("tiff", FileType.image);

            extentionType.Add("ogg", FileType.audio);
            extentionType.Add("mp3", FileType.audio);
                
            extentionType.Add("mkv", FileType.video);
            extentionType.Add("mp4", FileType.video);
            extentionType.Add("webm", FileType.video);

            extentionType.Add("txt", FileType.document);
            extentionType.Add("doc", FileType.document);
            extentionType.Add("docx", FileType.document);
            extentionType.Add("xml", FileType.document);
            extentionType.Add("xmlx", FileType.document);
        }

        static void Main(string[] args)
        {
            InitDictionary();
            Type.Init();
            Extention.Init();
            BySize.Init();
            bool correctInput = false;

            DirectoryInfo di = null;

            FileInfo[] files = null;
            DirectoryInfo[] directories = null;
            while (!correctInput)
            {
                try
                {
                    Console.WriteLine("Enter target directory: ");
                    string input = Console.ReadLine();
                    di = new DirectoryInfo(input);
                    if(!Directory.Exists(input))
                    {
                        throw new ArgumentException();
                    }
                    correctInput = true;

                    files = di.GetFiles();
                    directories = di.GetDirectories();
                } catch
                {
                    Console.WriteLine("Invalid Path");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            List<File> filesToSort = new List<File>();

            Console.Clear();
            long filesSizes = 0;
            foreach (FileInfo file in files)
            {
                filesSizes += file.Length;
                File newFile = null;
                if (file.Name.Contains('.'))
                {
                    string[] fileName = file.Name.Split('.');
                    newFile = new File(fileName[0], fileName[1], file.Length);
                } else
                {
                    newFile = new File(file.Name, "None", file.Length);
                }
                Type.AddFileToList(newFile);
                Extention.AddFileToList(newFile);
                BySize.AddFileToList(newFile);
                CountsByFirstLetter.AddFile(newFile);
                filesToSort.Add(newFile);
            }


            Console.WriteLine("\t\t [Count]\t[Total size]");
            Console.WriteLine(String.Format("Directories:\t{0,5}", directories.Length));
            Console.WriteLine(String.Format("Files:\t{0,13}{1,17}", files.Length, ConvertBytes(filesSizes)));


            ShowFilesByTypes();
            ShowFilesByExt();
            ShowFilesBySize();
            ShowCountsByFirstLetter();

            filesToSort.Sort((x, y) => x.Name.CompareTo(y.Name));
            Console.WriteLine("\nFiles sorted by name:\n");
            foreach(File file in filesToSort)
            {
                Console.WriteLine(String.Format("{0,50}:{1,20}", file.Name + "." + file.Extention, ConvertBytes(file.Size)));
            }
            filesToSort.Sort((x, y) => x.Size.CompareTo(y.Size));
            Console.WriteLine("\nFiles sorted by Size:\n");
            foreach (File file in filesToSort)
            {
                Console.WriteLine(String.Format("{0,50} : {1,20}", file.Name+"."+file.Extention, ConvertBytes(file.Size)));
            }
        }

        static string ConvertBytes(long bytes)
        {

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double temp = bytes;
            int order = 0;
            while (temp >= 1024 && order < sizes.Length - 1)
            {
                order++;
                temp /= 1024;
            }
            return String.Format("{0:0.##} {1}", temp, sizes[order]);
        }

        static void ShowFilesByTypes()
        {
            Console.WriteLine("\nFiles by types:\n");
            
            Console.WriteLine("\t\t\t[Count]\t[Total size]\t[Avg size]\t[Min size]\t[Max size]");
            foreach(Type fileType in Type.types)
            {
                Console.WriteLine(String.Format("{0,10}{1,20}{2,10}{3,15}{4,16}{5,16}",
                                                fileType.type.ToString(),
                                                fileType.count,
                                                 ConvertBytes(fileType.totalSize),
                                                 ConvertBytes(fileType.avgSize),
                                                 ConvertBytes(fileType.minSize),
                                                 ConvertBytes(fileType.maxSize)
                                                ));

            }
        }

        static void ShowFilesByExt()
        {
            Console.WriteLine("\nFiles by extentions:\n");

            Console.WriteLine("\t\t\t[Count]\t[Total size]\t[Avg size]\t[Min size]\t[Max size]");
            foreach (Extention fileExt in Extention.extentions)
            {
                Console.WriteLine(String.Format("{0,10}{1,20}{2,10}{3,15}{4,16}{5,16}",
                                                fileExt.name,
                                                fileExt.count,
                                                 ConvertBytes(fileExt.totalSize),
                                                 ConvertBytes(fileExt.avgSize),
                                                 ConvertBytes(fileExt.minSize),
                                                 ConvertBytes(fileExt.maxSize)
                                                ));

            }
        }

        static void ShowFilesBySize()
        {
            Console.WriteLine("\nFiles by sizes:\n");

            Console.WriteLine("\t\t\t[Count]\t[Total size]\t[Avg size]\t[Min size]\t[Max size]");
            foreach (KeyValuePair<BySize,long> sizePair in BySize.sizes)
            {
                Console.WriteLine(String.Format("{0,15}{1,15}{2,10}{3,15}{4,16}{5,16}",
                                                sizePair.Key.name,
                                                sizePair.Key.count,
                                                 ConvertBytes(sizePair.Key.totalSize),
                                                 ConvertBytes(sizePair.Key.avgSize),
                                                 ConvertBytes(sizePair.Key.minSize),
                                                 ConvertBytes(sizePair.Key.maxSize)
                                                ));

            }
        }

        static void ShowCountsByFirstLetter()
        {
            Console.WriteLine("\nCounts by first letter:\n");

            foreach (KeyValuePair<char, int> count in CountsByFirstLetter.counts)
            {
                Console.WriteLine(String.Format("\'{0}\':{1}",
                                                count.Key,
                                                count.Value
                                                ));

            }
        }
    }

    class Type
    {
        //Użycie Listy aby trzyamć w jednym miejscu wszystkie typy
        public static List<Type> types = new List<Type>();
        public FileType type;
        public long count = 0;
        public long totalSize = 0;
        public bool containsZeroSize;
        public long avgSize
        {
            get
            {
                if(count == 0)
                {
                    return 0;
                } else
                {
                    return totalSize / count;
                }

            }
        }
        public long minSize = 0;
        public long maxSize = 0;

        public Type(FileType type)
        {
            this.type = type;
        }

        public static void Init()
        {
            types.Add(new Type(FileType.image));
            types.Add(new Type(FileType.audio));
            types.Add(new Type(FileType.video));
            types.Add(new Type(FileType.document));
            types.Add(new Type(FileType.other));
        }

        public static void AddFileToList(File file)
        {
            foreach(Type fileType in types)
            {
                if(file.Type == fileType.type)
                {
                    if (file.Size == 0 && !fileType.containsZeroSize)
                    {
                        fileType.containsZeroSize = true;
                    }
                    fileType.count++;
                    fileType.totalSize += file.Size;
                    if (fileType.minSize == 0 && !fileType.containsZeroSize)
                    {
                        fileType.minSize = file.Size;
                    }
                    fileType.minSize = Math.Min(fileType.minSize, file.Size);
                    fileType.maxSize = Math.Max(fileType.maxSize, file.Size);
                    break;
                }
            }
        }
    }
    class Extention
    {
        public static List<Extention> extentions = new List<Extention>();
        static Extention other;
        public string name;
        public long count = 0;
        public long totalSize = 0;
        private bool containsZeroSize = false;
        public long avgSize
        {
            get
            {
                if (count == 0)
                {
                    return 0;
                }
                else
                {
                    return totalSize / count;
                }

            }
        }
        public long minSize = 0;
        public long maxSize = 0;

        public Extention(string extentionName)
        {
            this.name = extentionName;
        }

        public static void Init()
        {
            extentions.Add(new Extention("jpg"));
            extentions.Add(new Extention("png"));
            extentions.Add(new Extention("gif"));
            extentions.Add(new Extention("doc"));
            extentions.Add(new Extention("txt"));
            extentions.Add(new Extention("mp3"));
            Extention other = new Extention("other");
            extentions.Add(other);
            Extention.other = other;
        }

        public static void AddFileToList(File file)
        {
            foreach (Extention fileExt in extentions)
            {
                if (fileExt.name == "other") continue;

                if (file.Extention.ToLower().Equals(fileExt.name.ToLower()))
                {
                    if(file.Size == 0 && !fileExt.containsZeroSize)
                    {
                        fileExt.containsZeroSize = true;
                    }
                    fileExt.count++;
                    fileExt.totalSize += file.Size;
                    if (fileExt.minSize == 0 && !fileExt.containsZeroSize)
                    {
                        fileExt.minSize = file.Size;
                    }
                    fileExt.minSize = Math.Min(fileExt.minSize, file.Size);
                    fileExt.maxSize = Math.Max(fileExt.maxSize, file.Size);
                    return;
                }
            }
            if (file.Size == 0 && !other.containsZeroSize)
            {
                other.containsZeroSize = true;
            }
            other.count++;
            other.totalSize += file.Size;
            if (other.minSize == 0 && !other.containsZeroSize)
            {
                other.minSize = file.Size;
            }
            other.minSize = Math.Min(other.minSize, file.Size);
            other.maxSize = Math.Max(other.maxSize, file.Size);
        }
    }

    class BySize
    {
        //Użycie Dictionary aby móc przypisać maksymalną wartość do danej grupy 
        public static Dictionary<BySize, long> sizes = new Dictionary<BySize, long>();
        public string name;
        public long count = 0;
        public long totalSize = 0;
        public long avgSize
        {
            get
            {
                if (count == 0)
                {
                    return 0;
                }
                else
                {
                    return totalSize / count;
                }

            }
        }
        public long minSize = 0;
        public long maxSize = 0;

        public BySize(string sizeName)
        {
            this.name = sizeName;
        }

        public static void Init()
        {
            sizes.Add(new BySize(". <= 1KB"), 1024);
            sizes.Add(new BySize("1KB < . <= 1MB"), 1048576);
            sizes.Add(new BySize("1MB < . <= 1GB"), 1073741824);
            sizes.Add(new BySize("1GB < ."), -1);
        }

        public static void AddFileToList(File file)
        {
            foreach (KeyValuePair<BySize, long> sizePair in sizes)
            {
                if (file.Size <= sizePair.Value || sizePair.Value == -1)
                {
                    sizePair.Key.count++;
                    sizePair.Key.totalSize += file.Size;
                    if (sizePair.Key.minSize == 0 && !sizes.ContainsValue(0))
                    {
                        sizePair.Key.minSize = file.Size;
                    }
                    sizePair.Key.minSize = Math.Min(sizePair.Key.minSize, file.Size);
                    sizePair.Key.maxSize = Math.Max(sizePair.Key.maxSize, file.Size);
                    break;
                }
            }
        }
    }

    class CountsByFirstLetter
    {
        //Użycie Dictionary aby łączyć dany symbol z wartością
        public static Dictionary<char, int> counts = new Dictionary<char, int>();
        
        public static void AddFile(File file)
        {
            char firstLetter = file.Name.ToUpper()[0];
            if(counts.ContainsKey(firstLetter))
            {
                counts[firstLetter]++;
            } else
            {
                counts.Add(firstLetter, 1);
            }
        }
    }

}
