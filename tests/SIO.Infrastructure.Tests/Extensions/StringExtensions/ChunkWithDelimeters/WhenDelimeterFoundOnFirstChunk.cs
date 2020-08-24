using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SIO.Infrastructure.Extensions;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.Tests.Extensions.StringExtensions.ChunkWithDelimeters
{
    public class WhenDelimeterFoundOnFirstChunk : ChunkWithDelimetersSpecification
    {
        private const char _delimeter = '.';
        private const int _length = 123;
        private const string _text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Sit amet nisl suscipit adipiscing bibendum est ultricies. Eget nulla facilisi etiam dignissim diam quis enim lobortis scelerisque. Senectus et netus et malesuada. Elit duis tristique sollicitudin nibh sit amet commodo. Orci ac auctor augue mauris augue neque. Ullamcorper dignissim cras tincidunt lobortis feugiat vivamus at augue. Quisque id diam vel quam elementum pulvinar etiam. Condimentum id venenatis a condimentum vitae sapien pellentesque habitant. Mi eget mauris pharetra et ultrices neque ornare. Turpis egestas sed tempus urna et pharetra pharetra massa. Sit amet massa vitae tortor condimentum.";
        protected override Task<IEnumerable<string>> Given()
        {
            return Task.FromResult(_text.ChunkWithDelimeters(_length, _delimeter));
        }

        protected override Task When()
        {
            return Task.CompletedTask;
        }

        [Then]
        public void FirstChunkShouldEndWithDelimeter()
        {
            Result.First().Should().EndWith(_delimeter.ToString());
        }
    }
}
