using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSGBlazorApp.Models
{
    [MessagePackObject]
    public class ByteDataModel
    {
        [Key(0)]
        public byte[] Data { get; set; }
    }

    [MessagePackObject]
    public class DataParamModel
    {
        [Key(0)]
        public Dictionary<string, Dictionary<string, dynamic>> Data { get; set; }
    }
}
