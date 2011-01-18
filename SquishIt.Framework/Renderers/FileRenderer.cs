using System;
using SquishIt.Framework.Files;

namespace SquishIt.Framework.Renderers
{
    public class FileRenderer: IRenderer
    {
        protected IFileWriterFactory fileWriterFactory;

        public FileRenderer(IFileWriterFactory fileWriterFactory)
        {            
            this.fileWriterFactory = fileWriterFactory;
        }

        public void Render(string content, string outputFile)
        {
            WriteFiles(content, outputFile);
        }

        protected void WriteFiles(string output, string outputFile)
        {
            if (outputFile != null)
            {
                using (var fileWriter = fileWriterFactory.GetFileWriter(outputFile))
                {
                    fileWriter.Write(output);
                }
            }
            else
            {
                Console.WriteLine(output);
            }
        }
    }
}