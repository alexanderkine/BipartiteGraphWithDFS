using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ForestInhabitants.ForestObjects;
using ForestInhabitants.Generators;

namespace ForestInhabitants
{
    public interface IForestVisualisator
    {
        void DrawForest(Forest forest);
        object VisualisateForestObject(ForestObject forestObject);
    }
}
