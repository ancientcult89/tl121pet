using Microsoft.EntityFrameworkCore;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Data
{
    public static class SeedData
    {
        public static void SeedDatabase(DataContext dataContext)
        {
            dataContext.Database.Migrate();
            if (dataContext.People.Count() == 0 && dataContext.Grades.Count() == 0 &&
            dataContext.ProjectTeams.Count() == 0 && dataContext.SkillGroups.Count() == 0 &&
            dataContext.SkillTypes.Count() == 0 && dataContext.Skills.Count() == 0)
            {
                ProjectTeam pt1 = new ProjectTeam() { ProjectTeamName = "САБПЭК" };
                ProjectTeam pt2 = new ProjectTeam() { ProjectTeamName = "УРВП" };
                ProjectTeam pt3 = new ProjectTeam() { ProjectTeamName = "ЦИАС" };
                ProjectTeam pt4 = new ProjectTeam() { ProjectTeamName = "СИДЭ" };
                ProjectTeam pt5 = new ProjectTeam() { ProjectTeamName = "СТЕК" };
                dataContext.ProjectTeams.AddRange(pt1, pt2, pt3, pt4, pt5);
                dataContext.SaveChanges();

                SkillType st1 = new SkillType() { SkillTypeName = "Soft" };
                SkillType st2 = new SkillType() { SkillTypeName = "Hard" };
                dataContext.SkillTypes.AddRange(st1, st2);
                dataContext.SaveChanges();

                SkillGroup sg1 = new SkillGroup() { SkillGroupName = "SQL" };
                SkillGroup sg2 = new SkillGroup() { SkillGroupName = "WCF" };
                SkillGroup sg3 = new SkillGroup() { SkillGroupName = "C#" };
                SkillGroup sg4 = new SkillGroup() { SkillGroupName = "WPF" };
                dataContext.SkillGroups.AddRange(sg1, sg2, sg3, sg4);
                dataContext.SaveChanges();

                Grade g1 = new Grade() { GradeName = "Junior entry" };
                Grade g2 = new Grade() { GradeName = "Junior intermediate" };
                Grade g3 = new Grade() { GradeName = "Junior plus" };
                Grade g4 = new Grade() { GradeName = "Trainee" };
                dataContext.Grades.AddRange(g1, g2, g3, g4);
                dataContext.SaveChanges();

                Skill s1 = new Skill() { SkillGroup = sg1, SkillType = st2, SkillsDescription = "Индексы" };
                dataContext.Skills.Add(s1);
                dataContext.SaveChanges();

                Person p1 = new Person() { FirstName = "John", LastName = "Smith", Grade = g4 };
                dataContext.People.Add(p1);
                dataContext.SaveChanges();

            }

        }
    }
}
