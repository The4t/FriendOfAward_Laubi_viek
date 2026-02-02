using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VotingController : ControllerBase
{
    [HttpPost("submit")]
    public IActionResult SubmitVote([FromBody] VoteRequest vote)
    {
        var db = DbWrapperMySqlV2.Wrapper;

        try
        {
            // Favoriten +1
            foreach (var fav in vote.Favoriten)
            {
                db.RunNonQuery($"UPDATE diplomarbeiten SET stimmen = stimmen + 1 WHERE id = {fav}");
            }

            // Topfavorit +2
            if (vote.Topfavorit != null)
            {
                db.RunNonQuery($"UPDATE diplomarbeiten SET stimmen = stimmen + 2 WHERE id = {vote.Topfavorit}");
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}

public class VoteRequest
{
    public List<int> Favoriten { get; set; } = new();
    public int? Topfavorit { get; set; }
}
