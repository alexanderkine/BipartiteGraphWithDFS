using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForestInhabitants.ForestObjects;
using NUnit.Framework;

namespace ForestInhabitants
{
    class Forest_should
    {
        [TestCase]
        public void do_not_create_inhabitant_on_the_forbidden_place()
        {
            var testForest = new ForestLoader(new StreamReader("TestMap.txt")).Load();
            var testInhabitant = new Inhabitant("Alex", 10);
            testForest.CreateInhabitant(testInhabitant, new Coordinates(0, 0));
            Assert.True(testForest.Map[0][0] is Bush);
        }
        
        [TestCase]
        public void do_not_move_inhabitant_on_the_forbidden_place()
        {
            var testForest = new ForestLoader(new StreamReader("TestMap.txt")).Load();
            var testInhabitant = new Inhabitant("Alex", 10);
            testForest.CreateInhabitant(testInhabitant, new Coordinates(2, 1));
            testForest.Move(testInhabitant, MoveCommand.Up);
            Assert.True(testForest.Map[1][2] is Inhabitant && testForest.Map[2][0] is Bush);
        }

        [TestCase]
        public void move_on_the_trap()
        {
            var testForest = new ForestLoader(new StreamReader("TestMap.txt")).Load();
            var testInhabitant = new Inhabitant("Alex", 10);
            var lifeAfterTrapCell = testInhabitant.Life - 1;
            testForest.CreateInhabitant(testInhabitant, new Coordinates(7, 2));
            testForest.Move(testInhabitant, MoveCommand.Up);
            testForest.Move(testInhabitant, MoveCommand.Down);
            Assert.True(testForest.Map[1][7] is Trap && testForest.Map[2][7] is Inhabitant && (testForest.Map[2][7] as Inhabitant).Life == lifeAfterTrapCell);
        }

        [TestCase]
        public void move_on_the_life()
        {
            var testForest = new ForestLoader(new StreamReader("TestMap.txt")).Load();
            var testInhabitant = new Inhabitant("Alex", 10);
            var lifeAfterLifeCell = testInhabitant.Life + 1;
            testForest.CreateInhabitant(testInhabitant, new Coordinates(7, 5));
            testForest.Move(testInhabitant, MoveCommand.Down);
            testForest.Move(testInhabitant, MoveCommand.Up);
            Assert.True(testForest.Map[6][7] is Footpath && testForest.Map[5][7] is Inhabitant && (testForest.Map[5][7] as Inhabitant).Life == lifeAfterLifeCell);
        }

        [TestCase]
        public void do_not_move_inhabitant_out_of_borders()
        {
            var testForest = new ForestLoader(new StreamReader("TestMap.txt")).Load();
            var testInhabitant = new Inhabitant("Alex", 10);
            testForest.CreateInhabitant(testInhabitant, new Coordinates(1, 1));
            testForest.Move(testInhabitant, MoveCommand.Up);
            Assert.True(!testForest.Move(testInhabitant, MoveCommand.Up) && testForest.Map[0][1] is Inhabitant);
        }
    }
}
