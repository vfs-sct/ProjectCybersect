using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Selection
{
    public delegate float Input();
    public delegate void Callback();

    public Input[] inputs;
    public float cumulativeValue;
    public Callback callback;
}

public class UtilitySelector 
{
    private const uint MAX_SELECTIONS = 16;

    private Selection[] selections = new Selection[MAX_SELECTIONS];
    private uint selectionCount = 0;

    public void AddSelection(Selection.Input[] inputs, Selection.Callback callback)
    {
        selections[selectionCount].callback = callback;
        selections[selectionCount].inputs = inputs;
        selections[selectionCount].cumulativeValue = 0f;

        ++selectionCount;
    }

    public void Run()
    {
        float max = float.MinValue;
        uint maxIndex = 0;
        for (uint i = 0; i < selectionCount; ++i)
        {
            selections[i].cumulativeValue = 0f;
            for (uint j = 0; j < selections[i].inputs.Length; ++j)
                selections[i].cumulativeValue += selections[i].inputs[j]();

            if (selections[i].cumulativeValue > max)
            {
                max = selections[i].cumulativeValue;
                maxIndex = i;
            }
        }

        selections[maxIndex].callback();
    }
}
