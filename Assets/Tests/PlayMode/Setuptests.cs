using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Setuptests {
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
    public void CardActionToString_Test() {
        CommandListFunctions.CardAction ca;
        ca.CardName = "HA";
        ca.cardMovedFrom = CommandListFunctions.CardMovement.DF;
        ca.Row = null;

        Assert.AreEqual("Card : HA move : DF row : null", ca.ToString());
    }

    [Test]
    public void TestingEnumAndEtcStrings()
    {
        List<CommandListFunctions.CardAction> caList = makeList();


        //Assert.AreEqual("Card : CA move : DF row : null", CommandListFunctions.ShowListInConsole());
        
    }

    List<CommandListFunctions.CardAction> makeList() {

        List<CommandListFunctions.CardAction> calist = new List<CommandListFunctions.CardAction>();
        CommandListFunctions.CardAction ca;
        CommandListFunctions.CardAction flip;

        ca.CardName = "H3";
        ca.cardMovedFrom = CommandListFunctions.CardMovement.SS;
        ca.Row = 5;
        calist.Add(ca);

        flip.CardName = null;
        flip.cardMovedFrom = CommandListFunctions.CardMovement.flip;
        flip.Row = ca.Row;
        calist.Add(flip);

        ca.cardMovedFrom = CommandListFunctions.CardMovement.DD;
        ca.CardName = null;
        ca.Row = null;
        calist.Add(ca);

        ca.cardMovedFrom = CommandListFunctions.CardMovement.SS;
        ca.CardName = "D8";
        ca.Row = 2;
        calist.Add(ca);

        flip.Row = ca.Row;
        calist.Add(flip);

        ca.cardMovedFrom = CommandListFunctions.CardMovement.DD;
        ca.CardName = null;
        ca.Row = null;
        calist.Add(ca);

        ca.cardMovedFrom = CommandListFunctions.CardMovement.DS;
        ca.CardName = "SQ";
        ca.Row = null;
        calist.Add(ca);

        ca.cardMovedFrom = CommandListFunctions.CardMovement.DF;
        ca.CardName = "CA";
        ca.Row = null;
        calist.Add(ca);

        return calist;
    }
}
