using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace lab_04
{
    public enum FileType
    {
        image,
        audio,
        video,
        document,
        other
    }
    class File
    {
        private string name;
        private string extention;
        private long size;
        private FileType fileType;

        public string Name
        {
            get => name;
            set
            {
                name = value;
            }
        }

        public string Extention
        {
            get => extention;
            set
            {
                extention = value;
                SetFileType(value);
            }
        }

        public long Size
        {
            get => size;
            set
            {
                size = value;
            }
        }
        

        public FileType Type
        {
            get => fileType;

        }

        private void SetFileType(string extention)
        {
            if(Program.extentionType.TryGetValue(extention, out FileType value)) {
                fileType = value;
            } else
            {
                fileType = FileType.other;
            }
        }

        public File(string name, string extention, long size)
        {
            Name = name;
            Extention = extention;
            Size = size;
        }

    }
}
