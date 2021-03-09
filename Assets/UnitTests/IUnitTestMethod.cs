
public class SingleUnitTest
{

    private UnitTestMethod testMethod;
    private IUnitTestNameProvider testInformation;

    public SingleUnitTest(UnitTestMethod unitTest, IUnitTestNameProvider unitTestInformation)
    {
        SetTestMethod(unitTest);
        SetTestInformation(unitTestInformation);
    }

    public bool InvokeTestMethod()
    {
        return testMethod();
    }

    public void SetTestMethod(UnitTestMethod unitTest)
    {
        this.testMethod = unitTest;
    }

    public UnitTestMethod GetTestMethod()
    {
        return this.testMethod;
    }

    public void SetTestInformation(IUnitTestNameProvider unitTestInformation)
    {
        this.testInformation = unitTestInformation;
    }

    public IUnitTestNameProvider GetTestInformation()
    {
        return this.testInformation;
    }

}
