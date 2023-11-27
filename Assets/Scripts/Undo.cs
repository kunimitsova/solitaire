using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undo : MonoBehaviour {

    // get the last Move from the CommandList and see if it's valid:
    //      if not then the move should be FROM STACK. if it' s not  , return error.
    //          unflip the last card in the stack that was originally moved From (this will the the ROW in the CardAction item)
    //      then check again if the move is valid.
    //          if not exit sub
}
