using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Extensions;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;

namespace Dfe.Complete.Tests.Extensions;

public class IQueryableProjectsExtensionTests
{
    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenNoOrderByProvided_OrderBySignificantDate(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);
        var expectedOrder = projects.OrderBy(project => project.SignificantDate).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy().ToList();

        // Assert
        for (var i = 0; i < projects.Count(); i++)
        {
            Assert.Equal(expectedOrder[i].Id, result[i].Id);
            Assert.Equal(expectedOrder[i].Urn, result[i].Urn);
        }
    }

    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenOrderByProvided_OrderBySignificantDateDescending(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);
        var expectedOrder = projects.OrderByDescending(project => project.SignificantDate).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy(
            new(OrderProjectByField.SignificantDate, OrderByDirection.Descending)
        ).ToList();

        // Assert
        for (var i = 0; i < projects.Count(); i++)
        {
            Assert.Equal(expectedOrder[i].Id, result[i].Id);
            Assert.Equal(expectedOrder[i].Urn, result[i].Urn);
        }
    }

    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenOrderByProvided_OrderByCreatedAtDescending(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);
        var expectedOrder = projects.OrderByDescending(project => project.CreatedAt).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy(
            new(OrderProjectByField.CreatedAt, OrderByDirection.Descending)
        ).ToList();

        // Assert
        for (var i = 0; i < projects.Count(); i++)
        {
            Assert.Equal(expectedOrder[i].Id, result[i].Id);
            Assert.Equal(expectedOrder[i].Urn, result[i].Urn);
        }
    }

    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenOrderByProvided_OrderByCreatedAtAscending(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);
        var expectedOrder = projects.OrderBy(project => project.CreatedAt).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy(
            new(OrderProjectByField.CreatedAt, OrderByDirection.Ascending)
        ).ToList();

        // Assert
        for (var i = 0; i < projects.Count(); i++)
        {
            Assert.Equal(expectedOrder[i].Id, result[i].Id);
            Assert.Equal(expectedOrder[i].Urn, result[i].Urn);
        }
    }

    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenOrderByProvided_OrderByCompletedAtDescending(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);
        var expectedOrder = projects.OrderByDescending(project => project.CompletedAt).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy(
            new(OrderProjectByField.CompletedAt, OrderByDirection.Descending)
        ).ToList();

        // Assert
        for (var i = 0; i < projects.Count(); i++)
        {
            Assert.Equal(expectedOrder[i].Id, result[i].Id);
            Assert.Equal(expectedOrder[i].Urn, result[i].Urn);
        }
    }

    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenOrderByProvided_OrderByCompletedAtAscending(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);
        var expectedOrder = projects.OrderBy(project => project.CompletedAt).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy(
            new(OrderProjectByField.CompletedAt, OrderByDirection.Ascending)
        ).ToList();

        // Assert
        for (var i = 0; i < projects.Count(); i++)
        {
            Assert.Equal(expectedOrder[i].Id, result[i].Id);
            Assert.Equal(expectedOrder[i].Urn, result[i].Urn);
        }
    }
}