using UnityEngine;

public class UnitTestWrapper
{

    private IUnitTestBundle wrappedTest;
    
    public UnitTestWrapper(IUnitTestBundle wrappedTest)
    {
        SetWrappedTest(wrappedTest);
    }

    public void LaunchWrappedTest()
    {

        SingleUnitTest[] unitTests = wrappedTest.GetUnitTests();

        for(int i = 0; i < unitTests.Length; ++i)
        {
            SingleUnitTest unitTest = unitTests[i];

            bool success = unitTest.InvokeTestMethod();

            if (!success)
            {
                // TN - Test number
                Debug.LogError($"{unitTest.GetTestInformation().GetUnitTestName()} TN: [{i + 1}] failed.");
            }
        }

    }

    private void SetWrappedTest(IUnitTestBundle wrappedTest)
    {
        this.wrappedTest = wrappedTest;
    }

}