using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestInhabitants.Generators
{
    public interface IForestCommandGenerator
    {
        Coordinates GenerateCommand();
        void GenerateCommands(Forest forest);
    }
}
