using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ForestInhabitants.Generators;

namespace ForestInhabitants
{
    public interface IForestVisualisator
    {
        void DrawForest(Forest forest);
    }
}
