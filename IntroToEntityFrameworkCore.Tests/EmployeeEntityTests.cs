namespace IntroToEntityFrameworkCore.Tests;

[TestFixture]
public class EmployeeEntityTests
{
    [Test]
    public void EmployeePropertiesTest()
    {
        var entity = new Employee();
        Assert.AreEqual(typeof(BaseEntity), entity.GetType().BaseType);
        System.Reflection.PropertyInfo[] properties = entity.GetType().GetProperties();
        Assert.IsTrue(properties.Any(x => x.Name == "Id" && x.PropertyType == typeof(Guid)));
        Assert.IsTrue(properties.Any(x => x.Name == "CreatedAt" && x.PropertyType == typeof(DateTime)));
        Assert.IsTrue(properties.Any(x => x.Name == "UpdatedAt" && x.PropertyType == typeof(DateTime)));
        Assert.IsTrue(properties.Any(x => x.Name == "Name" && x.PropertyType == typeof(string)));
        Assert.IsTrue(properties.Any(x => x.Name == "Email" && x.PropertyType == typeof(string)));
        Assert.IsTrue(properties.Any(x => x.Name == "HomeLocation" && x.PropertyType == typeof(Point)));
        Assert.IsTrue(properties.Any(x => x.Name == "DefaultWorkLocation" && x.PropertyType == typeof(Point)));
        Assert.IsTrue(properties.Any(x => x.Name == "DefaultCommuteType" && x.PropertyType == typeof(CommuteType)));
        
    }
}