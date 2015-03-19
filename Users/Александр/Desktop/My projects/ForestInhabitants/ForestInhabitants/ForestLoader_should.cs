using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace ForestInhabitants
{
    public class ForestLoader_should
    {
        [TestCase]
        public void work_with_null_stream_reader()
        {
            Assert.Throws<NullReferenceException>(() => new ForestLoader(null).Load());
        }

        [TestCase]
        public void throw_exception_with_not_correct_symbols()
        {
            Assert.Throws<KeyNotFoundException>(() => new ForestLoader(new StreamReader("FakeSymbols.txt")).Load());
        }

        [TestCase]
        public void not_work_with_not_exists_files()
        {
            Assert.Throws<FileNotFoundException>(() => new ForestLoader(new StreamReader(Path.GetRandomFileName())).Load());
        }
    }
}
