using _2DRPG_Object_Oriented_Map_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moq;

namespace _2DRPG_UnitTests
{
    /// <summary>
    /// Represents a player in the game world.
    /// </summary>
    public class PlayerTests
    {
        private Mock<Tilemap> mockTilemap;
        private Mock<Transform> mockTransform;
        private GameObject playerObject;
        private Mock<MapManager> mapManager;
        private PlayerController playerController;
        //TODO Get a graphics device somehow.
    

        [SetUp]
        public void Setup()
        {

            mockTilemap = new Mock<Tilemap>();
            mockTransform = new Mock<Transform>();
            mapManager = new Mock<MapManager>();
            playerObject = new GameObject("player");
            playerObject.AddComponent(mockTransform.Object);


            playerController = new PlayerController("player");
        }

        [Test]
        public void Player_MoveUp()
        {
            mockTransform.Setup(t => t.Position).Returns(new Vector2(0, 0));
            playerController.Update();
            playerController.TryMove(Vector2.UnitX);
            mockTransform.Verify(t => t.Translate(new Vector2(32, 0)), Times.Once);
        }
    }
}