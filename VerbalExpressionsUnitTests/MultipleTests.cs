using CSharpVerbalExpressions;
using NUnit.Framework;
using System;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class MultipleTests
    {
        [Test]
        public void Multiple_WhenNullArgumentPassed_ThrowsArgumentNullException()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string value = null;

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => verbEx.Multiple(value));
        }

        [Test]
        public void Multiple_WhenParamIsGiven_ShouldMatchOneOrMultipleValuesGiven()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string text = "testesting 123 yahoahoahou another test";
            string expectedExpression = "y(aho)+u";
            //Act
            verbEx.Add("y")
                .Multiple("aho")
                .Add("u");

            //Assert
            Assert.IsTrue(verbEx.Test(text));
            Assert.AreEqual(expectedExpression, verbEx.ToString());
        }

        [Test]
        public void Multiple_WhenEmptyStringPassedWithNothingProceeding_ThrowsArgumentException()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string argument = string.Empty;

            //Act
            //verbEx.Add(".", false);
            verbEx.Multiple(argument);
            //Assert
            Assert.Throws<ArgumentException>(() => verbEx.ToRegex());
        }

        [Test]
        public void Multiple_WhenEmptyStringPassed_AddPlus()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string argument = string.Empty;

            //Act
            verbEx.Add(".", false);
            verbEx.Multiple(argument);
            //Assert
            Assert.AreEqual(verbEx.ToString(), ".+");
        }
    }
}
