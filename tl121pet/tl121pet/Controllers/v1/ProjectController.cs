using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
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

        #region ProjectsReference
        [HttpGet]
        public async Task<ActionResult<List<ProjectTeam>>> GetProjectList()
        {
            return await _projectService.GetAllTeamsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTeam>> GetProjectById(long id)
        {
            return await _projectService.GetProjectTeamByIdAsync(id) ?? new ProjectTeam();
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTeam>> CreateProject([FromBody] ProjectTeam newProject)
        {
            return await _projectService.CreateProjectTeamAsync(newProject);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectTeam>> UpdateProject([FromBody] ProjectTeam project)
        {
            return await _projectService.UpdateProjectTeamAsync(project);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            await _projectService.DeleteProjectTeamAsync(id);
            return Ok();
        }
        #endregion ProjectsReference


        #region PersonProjects
        [HttpGet("personProjects/{id}")]
        public async Task<ActionResult<List<ProjectTeam>>> GetProjectsByPersonId(long personId)
        {
            return await _projectService.GetPersonMembershipAsync(personId) ?? new List<ProjectTeam>();
        }

        [HttpPost("personProjects/{id}")]
        public async Task<ActionResult> AddTeamToPerson([FromBody] ChangePersonMembershipRequestDTO request)
        {
            await _projectService.AddPersonMembershipAsync(request.PersonId, request.ProjectId);
            return Ok();
        }

        [HttpDelete("personProjects/{id}")]
        public async Task<ActionResult> DeleteTeamToPerson([FromBody] ChangePersonMembershipRequestDTO request)
        {
            await _projectService.DeletePersonMembershipAsync(request.PersonId, request.ProjectId);
            return Ok();
        }
        #endregion PersonProjects

        #region UserProjects
        [HttpGet("userProjects/{id}")]
        public async Task<ActionResult<List<ProjectTeam>>> GetProjectsByUserId(long userId)
        {
            throw new NotImplementedException();
        }
        #endregion UserProjects
    }
}
