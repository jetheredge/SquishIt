using System.Linq;
using SquishIt.Framework;
using SquishIt.Framework.Base;

namespace SquishIt.Tests.Stubs {
    public interface IStubPreprocessor : IPreprocessor 
    {
        string CalledWith { get; }
    }

    public class StubGlobalPreprocessor : IStubPreprocessor
    {
        string calledWith;

        public bool ValidFor (string extension) 
        {
            return Extensions.Contains (extension);
        }

		public IProcessResult Process(string filePath, string content)
        {
            calledWith = content;
            return  new ProcessResult("globey");
        }

        public string[] Extensions 
        {
            get { return new[] { "global" }; }
        }

        public string CalledWith
        {
            get { return calledWith; }
        }
    }

    public class StubScriptPreprocessor : IStubPreprocessor
    {
        string calledWith;

        public bool ValidFor (string extension) 
        {
            return Extensions.Contains (extension);
        }

		public IProcessResult Process(string filePath, string content)
        {
            calledWith = content;
            return  new ProcessResult("scripty");
        }

        public string[] Extensions 
        {
            get { return new[] { "script" }; }
        }

        public string CalledWith
        {
            get { return calledWith; }
        }
    }

    public class StubStylePreprocessor : IStubPreprocessor
    {
        string calledWith;

        public bool ValidFor (string extension) 
        {
            return Extensions.Contains (extension);
        }

		public IProcessResult Process(string filePath, string content)
        {
            calledWith = content;
            return  new ProcessResult("styley");
        }

        public string[] Extensions 
        {
            get { return new[] { "style" }; }
        }

        public string CalledWith
        {
            get { return calledWith; }
        }
    }
}
