using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
  public record UserTreeNodeVm(long Id, string Name);
  public record UserTreeVm(long Id, string Name, UserTreeNodeVm[] Children);

  [ApiController]
  [Route("api.user.tree")]
  [Produces("application/json")]
  public class TreeController : ControllerBase
  {
    [HttpPost("get")]
    public Task<UserTreeVm> Get(string treeName)
    {
      throw new NotImplementedException();
    }

    [HttpPost("node.create")]
    public Task NodeCreate(string treeName, long parentNodeId, string nodeName)
    {
      throw new NotImplementedException();
    }

    [HttpPost("node.delete")]
    public Task NodeDelete(string treeName, long nodeId)
    {
      throw new NotImplementedException();
    }

    [HttpPost("node.rename")]
    public Task NodeRename(string treeName, long nodeId, string newNodeName)
    {
      throw new NotImplementedException();
    }

  }
}
