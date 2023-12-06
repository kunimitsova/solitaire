using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Setuptests
{
    [Test]
    public void TestTheTestCalc_Test() {
        int i = 0;
        float xDeckOffset = .2f;
        float initXDeckOffset = -1.5f;
        int dealAmt = 4;
        float finalX = Utilities.TestingCalculations(initXDeckOffset, xDeckOffset, dealAmt, i); // at 0, equals FURTHEST difference, so that's the TOP card
        Assert.AreEqual(-0.9f, finalX);
    }

    [Test]
    public void TestingEnumAndEtcStrings()
    {
        


        //Assert.AreEqual("Card : CA move : DF row : null", CommandListFunctions.ShowListInConsole());
        
    }


}
