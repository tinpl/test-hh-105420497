#nullable enable
using Microsoft.EntityFrameworkCore;

namespace Thunk.Services.Tree
{
  public class TreeService
  {
    private readonly TreeFacade _treeFacade;

    public TreeService(TreeFacade treeFacade)
    {
      _treeFacade = treeFacade;
    }

    async Task CreateTree(string treeName, CancellationToken cancellationToken)
    {
      var userTreeEntry = await _treeFacade.UserTrees.AddAsync(new UserTree { TreeName = treeName }, cancellationToken);

      // create default node
      await _treeFacade.Nodes.AddAsync(new TreeNode
      {
        Name = "Root",
        Parent = null,
        UserTree = userTreeEntry.Entity
      }, cancellationToken);

      await _treeFacade.SaveChangesAsync(cancellationToken);
    }

    public async Task<(UserTree, List<TreeNode>)> GetOrCreateTree(string treeName, CancellationToken cancellationToken)
    {
      if (null == await _treeFacade.UserTrees.SingleOrDefaultAsync(x => x.TreeName == treeName, cancellationToken))
        await CreateTree(treeName, cancellationToken);

      var userTree = await _treeFacade.UserTrees
        .Where(x => x.TreeName == treeName)
        .SingleAsync(cancellationToken);

      // populate local context
      var nodesList = await _treeFacade.Nodes
        .Where(x => x.UserTree.TreeName == treeName)
        .ToListAsync(cancellationToken);

      return (userTree, nodesList.Where(x => x.Parent == null).ToList());
    }

    public async Task<long> NodeCreate(string treeName, long parentNodeId, string nodeName,
      CancellationToken cancellationToken)
    {
      var parentNode = await _treeFacade.Nodes
        .Where(x => x.Id == parentNodeId)
        .Where(x => x.UserTree.TreeName == treeName)
        .Include(x => x.UserTree)
        .SingleAsync(cancellationToken);

      var tn = await _treeFacade.Nodes.AddAsync(
        new TreeNode { Name = nodeName, UserTree = parentNode.UserTree, Parent = parentNode }, cancellationToken);

      await _treeFacade.SaveChangesAsync(cancellationToken);

      return tn.Entity.Id;
    }

    public async Task NodeDelete(string treeName, long nodeId, CancellationToken cancellationToken)
    {
      var node = await _treeFacade.Nodes
        .SingleAsync(x => x.Id == nodeId && x.UserTree.TreeName == treeName, cancellationToken);
      _treeFacade.Nodes.Remove(node);

      await _treeFacade.SaveChangesAsync(cancellationToken);
    }

    public async Task NodeRename(string treeName, long nodeId, string newNodeName, CancellationToken cancellationToken)
    {
      var node = await _treeFacade.Nodes
        .Where(x => x.Id == nodeId)
        .Where(x => x.UserTree.TreeName == treeName)
        .SingleAsync(cancellationToken);
      node.Name = newNodeName;

      await _treeFacade.SaveChangesAsync(cancellationToken);
    }

  }
}