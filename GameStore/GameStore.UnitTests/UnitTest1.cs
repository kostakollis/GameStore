using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.Models;
using GameStore.WebUI.HtmlHelpers;

namespace GameStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
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
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            GamesListViewModel result = (GamesListViewModel)controller.List(null, 2).Model;

            List<Game> games = result.Games.ToList();
            Assert.IsTrue(games.Count == 2);
            Assert.AreEqual(games[0].Name, "Gra4");
            Assert.AreEqual(games[1].Name, "Gra5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
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
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            GamesListViewModel result
                = (GamesListViewModel)controller.List(null, 2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Games()
        {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Gra1", Category="Cat1"},
                new Game { GameId = 2, Name = "Gra2", Category="Cat2"},
                new Game { GameId = 3, Name = "Gra3", Category="Cat1"},
                new Game { GameId = 4, Name = "Gra4", Category="Cat2"},
                new Game { GameId = 5, Name = "Gra5", Category="Cat3"}
            });
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            List<Game> result = ((GamesListViewModel)controller.List("Cat2", 1).Model)
                .Games.ToList();

            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result[0].Name == "Gra2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "Gra4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1, Name = "Gra1", Category="Simulator"},
                new Game { GameId = 2, Name = "Gra2", Category="Simulator"},
                new Game { GameId = 3, Name = "Gra3", Category="Shooter"},
                new Game { GameId = 4, Name = "Gra4", Category="RPG"},
            });

            NavController target = new NavController(mock.Object);

            List<string> results = ((IEnumerable<string>)target.Menu().Model).ToList();

            Assert.AreEqual(results.Count(), 3);
            Assert.AreEqual(results[0], "RPG");
            Assert.AreEqual(results[1], "Simulator");
            Assert.AreEqual(results[2], "Shooter");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new Game[] {
                new Game { GameId = 1, Name = "Gra1", Category="Simulator"},
                new Game { GameId = 2, Name = "Gra2", Category="Shooter"}
            });

            NavController target = new NavController(mock.Object);

            string categoryToSelect = "Shooter";

            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Game_Count()
        {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Gra1", Category="Cat1"},
                new Game { GameId = 2, Name = "Gra2", Category="Cat2"},
                new Game { GameId = 3, Name = "Gra3", Category="Cat1"},
                new Game { GameId = 4, Name = "Gra4", Category="Cat2"},
                new Game { GameId = 5, Name = "Gra5", Category="Cat3"}
            });
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;
            int res1 = ((GamesListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((GamesListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((GamesListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((GamesListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
