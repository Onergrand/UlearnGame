using NUnit.Framework;

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
}