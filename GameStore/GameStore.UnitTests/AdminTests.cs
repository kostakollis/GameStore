using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GameStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Games()
        {
         
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Gra1"},
                new Game { GameId = 2, Name = "Gra2"},
                new Game { GameId = 3, Name = "Gra3"},
                new Game { GameId = 4, Name = "Gra4"},
                new Game { GameId = 5, Name = "Gra5"}
            });

      
            AdminController controller = new AdminController(mock.Object);

 
            List<Game> result = ((IEnumerable<Game>)controller.Index().
                ViewData.Model).ToList();

            Assert.AreEqual(result.Count(), 5);
            Assert.AreEqual("Gra1", result[0].Name);
            Assert.AreEqual("Gra2", result[1].Name);
            Assert.AreEqual("Gra3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Game()
        {
  
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Gra1"},
                new Game { GameId = 2, Name = "Gra2"},
                new Game { GameId = 3, Name = "Gra3"},
                new Game { GameId = 4, Name = "Gra4"},
                new Game { GameId = 5, Name = "Gra5"}
            });

  
            AdminController controller = new AdminController(mock.Object);

   
            Game game1 = controller.Edit(1).ViewData.Model as Game;
            Game game2 = controller.Edit(2).ViewData.Model as Game;
            Game game3 = controller.Edit(3).ViewData.Model as Game;

 
            Assert.AreEqual(1, game1.GameId);
            Assert.AreEqual(2, game2.GameId);
            Assert.AreEqual(3, game3.GameId);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Game()
        {
           
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Gra1"},
                new Game { GameId = 2, Name = "Gra2"},
                new Game { GameId = 3, Name = "Gra3"},
                new Game { GameId = 4, Name = "Gra4"},
                new Game { GameId = 5, Name = "Gra5"}
            });

            AdminController controller = new AdminController(mock.Object);

            Game result = controller.Edit(6).ViewData.Model as Game;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();

            AdminController controller = new AdminController(mock.Object);

            Game game = new Game { Name = "Test" };

            ActionResult result = controller.Edit(game, null);

            mock.Verify(m => m.SaveGame(game));

            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();

            AdminController controller = new AdminController(mock.Object);

            Game game = new Game { Name = "Test" };

            controller.ModelState.AddModelError("error", "error");

            ActionResult result = controller.Edit(game, null);

            mock.Verify(m => m.SaveGame(It.IsAny<Game>()), Times.Never());

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Games()
        {
            Game game = new Game { GameId = 2, Name = "Gra2" };

            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Gra1"},
                new Game { GameId = 2, Name = "Gra2"},
                new Game { GameId = 3, Name = "Gra3"},
                new Game { GameId = 4, Name = "Gra4"},
                new Game { GameId = 5, Name = "Gra5"}
            });

            AdminController controller = new AdminController(mock.Object);

            controller.Delete(game.GameId);

            mock.Verify(m => m.DeleteGame(game.GameId));
        }
    }
}
