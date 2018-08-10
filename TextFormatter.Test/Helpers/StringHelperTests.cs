using System.Threading.Tasks;
using TextFormatter.Utilities;
using Xunit;

namespace TextFormatter.Test.Helpers
{
    public class StringHelperTests
    {
        [Theory]
        [InlineData("a b", "ab")]
        [InlineData("ab ", "ab")]
        [InlineData(" ab ", "ab")]
        [InlineData(" ab\t ", "ab\t")]
        [InlineData(" ab\t\n ", "ab\t\n")]
        public async Task ShouldReplaceWhitespaceWithEmpty(string givenInput, string expectedOutput)
        {
            string actual = await StringHelper.ReplaceAsync(givenInput, @" ", "");
            Assert.Equal(expectedOutput, actual);
        }

        [Theory]
        [InlineData("a\tb", "ab")]
        [InlineData("ab\t\tc", "abc")]
        [InlineData("ab\t\n\t\n", "ab\n\n")]
        [InlineData("\t\tab", "ab")]
        public async Task ShouldReplaceTabsWithEmpty(string givenInput, string expectedOutput)
        {
            string actual = await StringHelper.ReplaceAsync(givenInput, @"\t", "", escapedCharacter: false);
            Assert.Equal(expectedOutput, actual);
        }

        [Theory]
        [InlineData("bc", "a", InsertPosition.Start, "abc")]
        [InlineData("ab", "c", InsertPosition.End, "abc")]
        public async Task ShouldInsertToPositionCorrectly(string givenInput, string insertValue, InsertPosition position, string expectedOutput)
        {
            string actual = await StringHelper.InsertAsync(givenInput, insertValue, position);
            Assert.Equal(expectedOutput, actual.Trim());
        }

        [Theory]
        [InlineData("a b c", ArrayFormat.Char, "'a','b','c'")]
        [InlineData("a b c", ArrayFormat.Integer, "a,b,c")]
        [InlineData("a b c", ArrayFormat.String, "\"a\",\"b\",\"c\"")]
        public async Task ArrayFormatShouldFormatToArray(string givenInput, ArrayFormat format , string expectedOutput)
        {
            string actual = await StringHelper.ArrayStructureAsync(givenInput, format);
            Assert.Equal(expectedOutput, actual);
        }
    }
}
