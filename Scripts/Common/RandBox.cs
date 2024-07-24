using System;
using System.Collections.Generic;
using System.Linq;

namespace XCardGame.Common;

public class RandBox<TContent>
{
    public List<(TContent, int)> ContentWeights;

    public int RandBoxWeight;
    
    public RandBox(int randBoxWeight)
    {
        RandBoxWeight = randBoxWeight;
        ContentWeights = new List<(TContent, int)>();
    }

    public TContent Rand(Random rand)
    {
        var weights = ContentWeights.Select(x => x.Item2).ToList();
        int i = Utils.RandOnWeight(weights, rand);
        if (i >= 0)
        {
            return ContentWeights[i].Item1;
        }
        return default;
    }
}