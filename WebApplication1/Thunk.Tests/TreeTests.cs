using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Thunk.Services.Tree;

namespace Thunk.Tests
{
  public class TreeTests
  {
    [Fact]
    public async Task Test1()
    {
      await using var ctx = new TreeFacade();
      var treeService = new TreeService(ctx);

      var random = new Random();
      var treeName = random.Next().ToString();
      
      var (tree, nodes) = await treeService.GetOrCreateTree(treeName, CancellationToken.None);
      tree.TreeName.Should().Be(treeName);
      nodes.Should().HaveCount(1, because: "Root node created by default");

      const int deepness = 10;
      long currentNodeId = nodes[0].Id;
      string currentNodeName = nodes[0].Name;

      var beforeNodesCount = await ctx.Nodes.CountAsync(CancellationToken.None);

      for (int i = 0; i < deepness; ++i)
      {
        currentNodeName = $"{currentNodeName}/{random.Next()}";

        currentNodeId = await treeService.NodeCreate(treeName, currentNodeId,
          currentNodeName,
          CancellationToken.None);
      }

      (tree, nodes) = await treeService.GetOrCreateTree(treeName, CancellationToken.None);
      
      (await ctx.Nodes.CountAsync(CancellationToken.None) - beforeNodesCount)
        .Should().Be(deepness, because: "We have created such amount of nodes");
      
      // trying with clean context
      await using var ctx2 = new TreeFacade();
      treeService = new TreeService(ctx2);

      (tree, nodes) = await treeService.GetOrCreateTree(treeName, CancellationToken.None);
      var currentNode = nodes[0];
      for (int i = 0; i < deepness; ++i)
      {
        currentNode.Children.Should().NotBeNullOrEmpty();
        currentNode = currentNode.Children[0];
      }

    }

  }
}