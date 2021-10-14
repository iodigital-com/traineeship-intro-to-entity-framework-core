namespace IntroToEntityFrameworkCore.Tests;

[TestFixture]
public class CommuteEntityTests
{
    [Test]
    public void CommutePropertiesTest()
    {
        var entity = new Commute();
        Assert.AreEqual(typeof(BaseEntity), entity.GetType().BaseType);
        System.Reflection.PropertyInfo[] properties = entity.GetType().GetProperties();
        Assert.IsTrue(properties.Any(x => x.Name == "Id" && x.PropertyType == typeof(Guid)));
        Assert.IsTrue(properties.Any(x => x.Name == "CreatedAt" && x.PropertyType == typeof(DateTime)));
        Assert.IsTrue(properties.Any(x => x.Name == "UpdatedAt" && x.PropertyType == typeof(DateTime)));
        Assert.IsTrue(properties.Any(x => x.Name == "EmployeeId" && x.PropertyType == typeof(Guid)));
        Assert.IsTrue(properties.Any(x => x.Name == "Employee" && x.PropertyType == typeof(Employee)));
        Assert.IsTrue(properties.Any(x => x.Name == "Type" && x.PropertyType == typeof(CommuteType)));
    }
}