
/// <summary>
/// CC - Calling class name
/// TC - Tested class name
/// TM - Tested method name
/// </summary>
public static class TestNameFactory
{

    public static IUnitTestNameProvider GetName_CC(string callingClassName)
    {
        NameProviderCollection nameCollection = new NameProviderCollection();
        nameCollection.AddProvider(new CallingTestClassNameProvider(callingClassName));
        return nameCollection;
    }

    public static IUnitTestNameProvider GetName_CC_TC(string callingClassName, string testedClassName)
    {
        NameProviderCollection nameCollection = (NameProviderCollection)GetName_CC(callingClassName);
        nameCollection.AddProvider(new TestedClassNameProvider(testedClassName));
        return nameCollection;
    }

    public static IUnitTestNameProvider GetName_CC_TC_TM(string callingClassName, string testedClassName, string testedMethodName)
    {
        NameProviderCollection nameCollection = (NameProviderCollection)GetName_CC_TC(callingClassName, testedClassName);
        nameCollection.AddProvider(new TestedClassMethodNameProvider(testedMethodName));
        return nameCollection;
    }

}
