using UnityEngine;

public class TestedClassNameProvider : IUnitTestNameProvider
{

    private string className = null;
    
    public TestedClassNameProvider(string className)
    {
        SetClassName(className);
    }

    public string GetClassName()
    {
        return this.className;
    }

    public void SetClassName(string className)
    {
        this.className = className;
    }

    public virtual string GetUnitTestName()
    {
        return "CE:" + GetClassName() + " ";
    }
}
