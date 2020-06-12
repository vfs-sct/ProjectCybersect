using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Selection
{
    public delegate float Input();
    public delegate void Callback();

    public Input input;
    public float cumulativeValue;
    public Callback callback;
}

public class UtilitySelector
{
    private const uint MAX_SELECTIONS = 16;

    private Selection[] selections = new Selection[MAX_SELECTIONS];
    private Selection.Callback _default;
    private uint selectionCount = 0;

    public void AddSelection(Selection.Input input, Selection.Callback callback)
    {
        selections[selectionCount].callback = callback;
        selections[selectionCount].input = input;
        selections[selectionCount].cumulativeValue = 0f;

        ++selectionCount;
    }

    public void SetDefault(Selection.Callback default_)
    {
        _default = default_;
    }

    public void Run()
    {
        float max = float.MinValue;
        uint maxIndex = 0;
        for (uint i = 0; i < selectionCount; ++i)
        {
            selections[i].cumulativeValue = 0f;
            selections[i].cumulativeValue += selections[i].input();

            if (selections[i].cumulativeValue > max)
            {
                max = selections[i].cumulativeValue;
                maxIndex = i;
            }
        }

        if (max > 0f)
            selections[maxIndex].callback();
        else if (_default != null)
            _default();
    }
}
