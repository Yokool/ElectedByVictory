public class CallingTestClassNameProvider : IUnitTestNameProvider
{

    private string callingClassName = null;

    public CallingTestClassNameProvider(string callingClassName)
    {
        SetCallingClassName(callingClassName);
    }

    public void SetCallingClassName(string callingClassName)
    {
        this.callingClassName = callingClassName;
    }

    public string GetCallingClassName()
    {
        return this.callingClassName;
    }

    public string GetUnitTestName()
    {
        return $" CCN: {callingClassName} ";
    }
}
