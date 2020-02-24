using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace GameStore.UnitTests
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void Can_Retrieve_Image_Data()
        {
            Game game = new Game
            {
                GameId = 2,
                Name = "Gra2",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };

            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game {GameId = 1, Name = "Gra1"},
                game,
                new Game {GameId = 3, Name = "Gra3"}
            }.AsQueryable());

            GameController controller = new GameController(mock.Object);

            ActionResult result = controller.GetImage(2);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(game.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Cannot_Retrieve_Image_Data_For_Invalid_ID()
        {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game {GameId = 1, Name = "Gra1"},
                new Game {GameId = 2, Name = "Gra2"}
            }.AsQueryable());

            GameController controller = new GameController(mock.Object);

            ActionResult result = controller.GetImage(10);

            Assert.IsNull(result);
        }
    }
}
