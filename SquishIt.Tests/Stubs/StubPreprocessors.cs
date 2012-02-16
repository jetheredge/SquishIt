using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquishIt.Framework;

namespace SquishIt.Tests.Stubs {
    public interface IStubPreprocessor : IPreprocessor 
    {
        bool WasCalled { get; }
    }

    public class StubGlobalPreprocessor : IStubPreprocessor
    {
        bool wasCalled;

        public bool ValidFor (string extension) 
        {
            return Extensions.Contains (extension);
        }

        public string Process (string filePath, string content)
        {
            wasCalled = true;
            return "globey";
        }

        public string[] Extensions 
        {
            get { return new[] { "global" }; }
        }

        public bool WasCalled
        {
            get { return wasCalled; }
        }
    }

    public class StubScriptPreprocessor : IStubPreprocessor
    {
        bool wasCalled;

        public bool ValidFor (string extension) 
        {
            return Extensions.Contains (extension);
        }

        public string Process (string filePath, string content)
        {
            wasCalled = true;
            return "scripty";
        }

        public string[] Extensions 
        {
            get { return new[] { "script" }; }
        }

        public bool WasCalled
        {
            get { return wasCalled; }
        }
    }

    public class StubStylePreprocessor : IStubPreprocessor
    {
        bool wasCalled;

        public bool ValidFor (string extension) 
        {
            return Extensions.Contains (extension);
        }

        public string Process (string filePath, string content)
        {
            wasCalled = true;
            return "styley";
        }

        public string[] Extensions 
        {
            get { return new[] { "style" }; }
        }

        public bool WasCalled
        {
            get { return wasCalled; }
        }
    }
}
