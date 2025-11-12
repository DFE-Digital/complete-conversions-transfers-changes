using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Extensions;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;

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

        // Shuffle the projects
        var random = new Random();
        projects = projects.OrderBy(_ => random.Next()).ToList();

        var expectedOrder = projects.OrderBy(project => project.SignificantDate).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy().ToList();

        // Assert
        Assert.Equal(expectedOrder.Select(p => p.Id), result.Select(p => p.Id));
    }

    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenOrderByProvided_OrderBySignificantDateDescending(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);

        // Shuffle the projects
        var random = new Random();
        projects = projects.OrderBy(_ => random.Next()).ToList();

        var expectedOrder = projects.OrderByDescending(project => project.SignificantDate).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy(
            new(OrderProjectByField.SignificantDate, OrderByDirection.Descending)
        ).ToList();

        // Assert
        Assert.Equal(expectedOrder.Select(p => p.Id), result.Select(p => p.Id));
    }

    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenOrderByProvided_OrderByCreatedAtDescending(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);

        // Shuffle the projects
        var random = new Random();
        projects = projects.OrderBy(_ => random.Next()).ToList();

        var expectedOrder = projects.OrderByDescending(project => project.CreatedAt).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy(
            new(OrderProjectByField.CreatedAt, OrderByDirection.Descending)
        ).ToList();

        // Assert
        Assert.Equal(expectedOrder.Select(p => p.Id), result.Select(p => p.Id));
    }

    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenOrderByProvided_OrderByCreatedAtAscending(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);

        // Shuffle the projects
        var random = new Random();
        projects = projects.OrderBy(_ => random.Next()).ToList();

        var expectedOrder = projects.OrderBy(project => project.CreatedAt).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy(
            new(OrderProjectByField.CreatedAt, OrderByDirection.Ascending)
        ).ToList();

        // Assert
        Assert.Equal(expectedOrder.Select(p => p.Id), result.Select(p => p.Id));
    }

    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenOrderByProvided_OrderByCompletedAtDescending(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);

        // Shuffle the projects
        var random = new Random();
        projects = projects.OrderBy(_ => random.Next()).ToList();

        var expectedOrder = projects.OrderByDescending(project => project.CompletedAt).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy(
            new(OrderProjectByField.CompletedAt, OrderByDirection.Descending)
        ).ToList();

        // Assert
        Assert.Equal(expectedOrder.Select(p => p.Id), result.Select(p => p.Id));
    }

    [Theory]
    [CustomAutoData(
        typeof(ProjectCustomization))]
    public void OrderProjectBy_WhenOrderByProvided_OrderByCompletedAtAscending(IFixture fixture)
    {
        //Arrange
        var projects = fixture.CreateMany<Project>(50);

        // Shuffle the projects
        var random = new Random();
        projects = [.. projects.OrderBy(_ => random.Next())];

        var expectedOrder = projects.OrderBy(project => project.CompletedAt).ToList();

        // Act
        var result = projects.AsQueryable().OrderProjectBy(
            new(OrderProjectByField.CompletedAt, OrderByDirection.Ascending)
        ).ToList();

        // Assert
        Assert.Equal(expectedOrder.Select(p => p.Id), result.Select(p => p.Id));
    }
}