using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Docs
{
    public class Recursion
    {
        class Directory
        {
            public string Name { get; set; }
            public IEnumerable<Directory> Directories { get; set; }
            public IEnumerable<File> Files { get; set; }
        }

        class File
        {
            public string Name { get; set; }
        }

        [Fact]
        public void Validate()
        {
            var type = typeof(Directory);

            var dataContractStore = new DataContractStore();

            var dataContract = new
            {
                Name = typeof(string),
                Directories = new[] { dataContractStore.Get("dir") },
                Files = new[]
                {
                    new
                    {
                        Name = typeof(string)
                    }
                }
            };

            dataContractStore.Add("dir", dataContract);

            new DataContractValidator().Validate(type, dataContract);
        }
    }
}