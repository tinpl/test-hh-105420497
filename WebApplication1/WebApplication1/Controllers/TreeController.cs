using Microsoft.AspNetCore.Mvc;
using Thunk.Services.Tree;

namespace WebApi.Controllers
{
  public record UserTreeNodeVm(long Id, string Name, UserTreeNodeVm[] Children);
  public record UserTreeVm(long Id, string Name, UserTreeNodeVm[] Children);

  [ApiController]
  [Route("api.user.tree")]
  [Produces("application/json")]
  public class TreeController : ControllerBase
  {
    private readonly TreeService _treeService;

    public TreeController(TreeService treeService)
    {
      _treeService = treeService;
    }

    [HttpPost("get")]
    public async Task<UserTreeVm> Get(string treeName, CancellationToken cancellationToken)
    {
      var (tree, rootNodes) = await _treeService.GetOrCreateTree(treeName, cancellationToken);

      UserTreeNodeVm[] ParseChildren(IEnumerable<TreeNode> children)
      {
        if (children == null) 
          return Array.Empty<UserTreeNodeVm>();

        return children.Select(childNode => new UserTreeNodeVm(
            childNode.Id,
            childNode.Name,
            ParseChildren(childNode.Children)))
          .ToArray();
      }
      
      return new UserTreeVm(tree.Id, tree.TreeName, ParseChildren(rootNodes));
    }

    [HttpPost("node.create")]
    public Task NodeCreate(string treeName, long parentNodeId, string nodeName, CancellationToken cancellationToken)
    {
      return _treeService.NodeCreate(treeName, parentNodeId, nodeName, cancellationToken);
    }

    [HttpPost("node.delete")]
    public Task NodeDelete(string treeName, long nodeId, CancellationToken cancellationToken)
    {
      return _treeService.NodeDelete(treeName, nodeId, cancellationToken);
    }

    [HttpPost("node.rename")]
    public Task NodeRename(string treeName, long nodeId, string newNodeName, CancellationToken cancellationToken)
    {
      return _treeService.NodeRename(treeName, nodeId, newNodeName, cancellationToken);
    }

  }
}
