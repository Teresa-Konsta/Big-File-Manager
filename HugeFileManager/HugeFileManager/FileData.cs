using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeFileManager
{
    public class FileData
    {
        public string DataItem { get; set; }

        public static List<FileData> fileDataList = new List<FileData>();

        public static void GetData(string dataItem)
        {
            fileDataList.Add(
                new FileData
                {
                    DataItem = dataItem
                });
        }
    }
}
