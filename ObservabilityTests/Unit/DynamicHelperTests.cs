using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Observability.LibraryBase.Utilities;

namespace ObservabilityTests.Unit
{
    public class DynamicHelperTests
    {
        #region Tests

        [Fact]
        public void IsPropertyExistTest_Success()
        {
            // Arrange
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            dynamic dynamicObject = Newtonsoft.Json.JsonConvert.DeserializeObject(/*lang=json,strict*/ "{\"Property1\":\"Value1\", \"Property2\":\"Value2\"}");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            // Act
            bool isPropertyExist = DynamicHelper.IsPropertyExist(dynamicObject, "Property1");

            // Assert
            Assert.True(isPropertyExist);
        }
        #endregion
    }
}
