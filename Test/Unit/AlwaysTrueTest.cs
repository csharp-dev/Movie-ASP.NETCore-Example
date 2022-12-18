using BusinessLogic;

namespace Unit;

public class AlwaysTrueTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ShouldBeAlwaysTrue()
    {   
        var obj = new AlwaysTrue();
        Assert.IsTrue(obj.alwaysTrue());
    }
}
