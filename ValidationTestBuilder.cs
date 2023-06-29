using System.Collections.Generic;
using System.Linq;
using bimeh_back.Components.Response;

namespace bimeh_back.Components.Tools
{
    public class ValidationTestBuilder
    {
        private readonly ValidationTest _validationTest;

        public ValidationTestBuilder()
        {
            _validationTest = new ValidationTest();
        }

        public ValidationTestBuilder SetData(StdResponse response)
        {
            _validationTest.Response = response;
            return this;
        }

        public StdResponse GetData()
        {
            return _validationTest.Response;
        }

        public List<ValidationFailure> GetAsserts()
        {
            return _validationTest.Failures;
        }

        public ValidationTest Build()
        {
            return _validationTest;
        }

        public ValidationTestBuilder Clear()
        {
            _validationTest.Failures.Clear();
            return this;
        }

        public ValidationTestBuilder AddTest(string propertyName, string errorMessage)
        {
            _validationTest.Failures.Add(new ValidationFailure {
                PropertyName = propertyName,
                ErrorMessage = errorMessage,
            });
            return this;
        }

        public ValidationTestBuilder AddTest(ValidationFailure failure)
        {
            AddTest(failure.PropertyName, failure.ErrorMessage);
            return this;
        }

        public ValidationTestBuilder RemoveTest(string propertyName, string errorMessage)
        {
            var failure = GetFailure(propertyName, errorMessage);

            if (failure != null) {
                _validationTest.Failures.Remove(failure);
            }

            return this;
        }

        public ValidationTestBuilder RemoveTest(ValidationFailure failure)
        {
            RemoveTest(failure.PropertyName, failure.ErrorMessage);

            return this;
        }

        private ValidationFailure GetFailure(string propertyName, string errorMessage)
        {
            return _validationTest.Failures
                .FirstOrDefault(failure =>
                    failure.PropertyName == propertyName &&
                    failure.ErrorMessage == errorMessage
                );
        }
    }
}