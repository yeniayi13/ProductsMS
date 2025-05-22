using MongoDB.Driver;
using Moq;
using ProductsMS.Infrastructure.Database.Context.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMs.Core.Database;
using Xunit;

namespace ProductsMS.Test.Infrastructure.DataBase
{
    public class ApplicationDbContextMongoTests
    {
        private readonly Mock<IMongoClient> _mongoClientMock;
        private readonly Mock<IMongoDatabase> _mongoDatabaseMock;
        private readonly Mock<IClientSessionHandle> _sessionMock;
        private ApplicationDbContextMongo _context;

        public ApplicationDbContextMongoTests()
        {
            _mongoClientMock = new Mock<IMongoClient>();
            _mongoDatabaseMock = new Mock<IMongoDatabase>();
            _sessionMock = new Mock<IClientSessionHandle>();

            _mongoClientMock.Setup(client => client.GetDatabase(It.IsAny<string>(), null))
                .Returns(_mongoDatabaseMock.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDatabaseContext()
        {
            // Act
            _context = new ApplicationDbContextMongo("mongodb+srv://yadefreitas19:08092001@cluster0.owy2d.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0", "TestDatabase");

            // Assert
            Assert.NotNull(_context);
            Assert.NotNull(_context.Database);
        }

        [Fact]
        public void Constructor_ShouldHandleException_Gracefully()
        {
            // Arrange
            var invalidConnectionString = "invalid_connection_string";

            // Act & Assert
            var exception = Record.Exception(() => new ApplicationDbContextMongo(invalidConnectionString, "TestDatabase"));
            Assert.Null(exception); // No debe lanzar excepción
        }

        [Fact]
        public void Database_ShouldReturnMongoDatabase()
        {
            // Arrange
            _context = new ApplicationDbContextMongo("mongodb+srv://yadefreitas19:08092001@cluster0.owy2d.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0", "TestDatabase");

            // Act
            var database = _context.Database;

            // Assert
            Assert.NotNull(database);
        }

       

       
    }
}
