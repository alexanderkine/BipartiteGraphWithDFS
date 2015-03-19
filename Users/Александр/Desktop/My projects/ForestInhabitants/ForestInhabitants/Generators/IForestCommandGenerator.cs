namespace ForestInhabitants.Generators
{
    public interface IForestCommandGenerator
    {
        Coordinates GenerateCommand();
        void GenerateCommands(Forest forest);
    }
}
