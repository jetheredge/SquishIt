using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JavascriptBundler.Minifiers
{
    public class NullMinifier: IFileCompressor
    {
        public string Compress(string file)
        {
            using (var sr = new StreamReader(file))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
