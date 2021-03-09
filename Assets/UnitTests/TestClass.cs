using ElectedByVictory.WorldCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClass : MonoBehaviour
{

    

    private UnitTestWrapper[] wrappers;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        IUnitTestBundle[] unitTests = GetComponents<IUnitTestBundle>();
        WrapUnitTests(unitTests);

        LaunchTests();
    }

    private void WrapUnitTests(IUnitTestBundle[] unitTestsToWrap)
    {
        wrappers = new UnitTestWrapper[unitTestsToWrap.Length];
        for(int i = 0; i < wrappers.Length; ++i)
        {
            IUnitTestBundle unitTest = unitTestsToWrap[i];
            wrappers[i] = new UnitTestWrapper(unitTest);
        }
    }

    private void LaunchTests()
    {
        for(int i = 0; i < wrappers.Length; ++i)
        {
            UnitTestWrapper unitTestWrapper = wrappers[i];
            unitTestWrapper.LaunchWrappedTest();
        }
    }

}
