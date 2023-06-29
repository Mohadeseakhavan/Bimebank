using System.Collections.Generic;
using System.Linq;
using bimeh_back.Components.Extensions;
using bimeh_back.Components.Response;
using Xunit;

namespace bimeh_back.Components.Tools
{
    public class ValidationTest
    {
        public StdResponse Response { get; set; }
        protected internal List<ValidationFailure> Failures { get; }

        public ValidationTest(StdResponse response)
        {
            Response = response;
            Failures = new List<ValidationFailure>();
        }

        public ValidationTest()
        {
            Failures = new List<ValidationFailure>();
        }

        public void Test()
        {
            TestStatus();
            TestMessage();
            TestData();
        }

        public void TestStatus()
        {
            Assert.Equal("validation-error", Response.Status);
        }

        public void TestMessage()
        {
            Assert.Null(Response.Message);
        }

        public void TestData()
        {
            var data = TestExtension.ToExpandoList(Response.Data);

            Assert.IsType<List<object>>(data);

            Assert.Equal(Failures.Count, data.Count);

            for (var i = 0; i < Failures.Count; i++) {
                TestDataItem(Failures[i], data[i]);
            }
        }

        public void TestDataItem(ValidationFailure expected, dynamic actual)
        {
            Assert.NotNull(actual);

            Assert.Equal(expected.PropertyName, actual.PropertyName);
            Assert.Equal(expected.ErrorMessage, actual.ErrorMessage);
        }
    }
}