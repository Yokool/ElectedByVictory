
using UnityEngine;

public class TestedClassMethodNameProvider : IUnitTestNameProvider
{
    private string methodName;

    public TestedClassMethodNameProvider(string methodName)
    {
        SetMethodName(methodName);
    }

    private void SetMethodName(string methodName)
    {
        this.methodName = methodName;
    }

    public string GetMethodName()
    {
        return this.methodName;
    }

    public string GetUnitTestName()
    {
        return $" M:{GetMethodName()} ";
    }
}