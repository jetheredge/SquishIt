using System.Linq;
using SquishIt.Framework;
using SquishIt.Framework.Base;

namespace SquishIt.Tests.Stubs
{
    public interface IStubPreprocessor : IPreprocessor
    {
        string CalledWith { get; }
    }

    public class StubGlobalPreprocessor : Preprocessor, IStubPreprocessor
    {
        string calledWith;

        public override IProcessResult Process(string filePath, string content)
        {
            calledWith = content;
            return new ProcessResult("globey");
        }

        public override string[] Extensions
        {
            get { return new[] { ".global" }; }
        }

        public string CalledWith
        {
            get { return calledWith; }
        }
    }

    public class StubScriptPreprocessor : Preprocessor, IStubPreprocessor
    {
        string calledWith;

        public override IProcessResult Process(string filePath, string content)
        {
            calledWith = content;
            return new ProcessResult("scripty");
        }

        public override string[] Extensions
        {
            get { return new[] { ".script" }; }
        }

        public override string[] IgnoreExtensions { get { return new[] { ".js" }; } }

        public string CalledWith
        {
            get { return calledWith; }
        }
    }

    public class StubStylePreprocessor : Preprocessor, IStubPreprocessor
    {
        string calledWith;

        public override IProcessResult Process(string filePath, string content)
        {
            calledWith = content;
            return new ProcessResult("styley");
        }

        public override string[] Extensions
        {
            get { return new[] { ".style" }; }
        }

        public override string[] IgnoreExtensions { get { return new[] { ".css" }; } }

        public string CalledWith
        {
            get { return calledWith; }
        }
    }
}
