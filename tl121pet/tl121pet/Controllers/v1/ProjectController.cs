using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ProjectTeam>>> Get()
        {
            return await _projectService.GetAllTeamsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTeam>> Get(long id)
        {
            return await _projectService.GetProjectTeamByIdAsync(id) ?? new ProjectTeam();
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTeam>> Create([FromBody] ProjectTeam newProject)
        {
            return await _projectService.CreateProjectTeamAsync(newProject);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectTeam>> Update([FromBody] ProjectTeam project)
        {
            return await _projectService.UpdateProjectTeamAsync(project);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _projectService.DeleteProjectTeamAsync(id);
            return Ok();
        }
    }
}
