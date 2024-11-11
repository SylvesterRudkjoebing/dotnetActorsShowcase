using Microsoft.EntityFrameworkCore;
using Moq;

public static class DbSetMockingExtensions
{
    public static Mock<DbSet<T>> ReturnsDbSet<T>(this Mock<DbSet<T>> mockSet, List<T> data) where T : class
    {

        // Tilføjer LINQ operationer til MockDB
        var queryableData = data.AsQueryable();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

        // Mock Add and Remove methods
        mockSet.Setup(m => m.Add(It.IsAny<T>()))
               .Callback<T>(data.Add);

        mockSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<T>>()))
               .Callback<IEnumerable<T>>(items => data.AddRange(items));

        mockSet.Setup(m => m.Remove(It.IsAny<T>()))
               .Callback<T>(item => data.Remove(item));

        mockSet.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<T>>()))
               .Callback<IEnumerable<T>>(items =>
               {
                   foreach (var item in items)
                       data.Remove(item);
               });

        return mockSet;
    }
}
