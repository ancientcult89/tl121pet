﻿using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IProjectTeamRepository
    {
        public Task<List<ProjectTeam>> GetAllTeamsAsync();
        public Task<ProjectTeam> GetProjectTeamByIdAsync(long id);
        public Task CreateProjectTeamAsync(ProjectTeam pt);
        public Task UpdateProjectTeamAsync(ProjectTeam pt);
        public Task DeleteProjectTeamAsync(long id);
        public Task<string> GetPersonsProjectsAsync(long id);
        public Task<string> GetUserProjectsAsync(long userId);
        public Task<List<ProjectTeam>> GetPersonMembershipAsync(long id);
        public Task<List<ProjectTeam>> GetUserMembershipAsync(long id);
        public Task DeletePersonMembershipAsync(long userId, long projectTeamId);
        public Task AddPersonMembershipAsync(long userId, long projectTeamId);
        public Task DeleteUserMembershipAsync(long userId, long projectTeamId);
        public Task AddUserMembershipAsync(long userId, long projectTeamId);
    }
}
