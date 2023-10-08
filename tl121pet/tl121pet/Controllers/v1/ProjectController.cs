using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProjectController : ApiController
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
        [HttpGet("personProjects/{personId}")]
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
        public async Task<ActionResult> DeleteTeamFromPerson([FromBody] ChangePersonMembershipRequestDTO request)
        {
            await _projectService.DeletePersonMembershipAsync(request.PersonId, request.ProjectId);
            return Ok();
        }
        #endregion PersonProjects

        #region UserProjects
        [HttpGet("userProjects/{userId}")]
        public async Task<ActionResult<List<ProjectTeam>>> GetProjectsByUserId(long userId)
        {
            return await _projectService.GetUserMembershipAsync(userId); ;
        }

        [HttpPost("userProjects/{id}")]
        public async Task<ActionResult> AddTeamToUser([FromBody] ChangeUserMembershipRequestDTO request)
        {
            await _projectService.AddUserMembershipAsync(request.UserId, request.ProjectId);
            return Ok();
        }

        [HttpDelete("userProjects/{id}")]
        public async Task<ActionResult> DeleteTeamFromUser([FromBody] ChangeUserMembershipRequestDTO request)
        {
            await _projectService.DeleteUserMembershipAsync(request.UserId, request.ProjectId);
            return Ok();
        }
        #endregion UserProjects
    }
}
