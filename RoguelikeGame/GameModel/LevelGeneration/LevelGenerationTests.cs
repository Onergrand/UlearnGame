using NUnit.Framework;
using RoguelikeGame.GameView;

namespace RoguelikeGame.GameModel.LevelGeneration;

[TestFixture]
public class LevelGenerationTests
{
    [Test]
    public void LevelGeneratesAskedRoomAmount()
    {
        for (var i = 3; i < 15; i++)
        for (var j = 0; j < 5000; j++)
        {
            var level = new Level(i);
            Assert.AreEqual(i, level.Rooms.Count);
        }
    }

    [Test]
    public void LevelGeneratesAskedMonstersAmount()
    {
        for (var i = 3; i < 15; i++)
        for (var j = 0; j < 5000; j++)
        {
            var level = new Level(i);
            var monstersAmount = level.MonsterAmountToCreate;
        
            Assert.AreEqual(monstersAmount, level.MonstersCreated);
        }
    }
    
    [Test]
    public void LevelGeneratesAskedRoomsAmountByLevelNumber()
    {
        for (var i = 0; i < 15; i++)
        for (var j = 0; j < 5000; j++)
        {
            var roomsGenerated = CreateLevel(i);
            var x = i / 5;
            
            Assert.AreEqual(7 + x, roomsGenerated);
        }
    }
    
    private int CreateLevel(int currentLevelNumber)
    {
        var roomsAmountToAdd = currentLevelNumber / 5;
        var level = new Level(7 + roomsAmountToAdd);
        
        return level.Rooms.Count;
    }
}